using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Mathematics;
using Unity.Entities;
using UnityEngine.Assertions;
using Unity.Jobs;
using Unity.Collections;

public class RaycastHitJob : MonoBehaviour
{
    public float distance = 10;
    public Vector3 direction = new Vector3(0, 0, 1);

    public bool CollectAllHits = false;
    public bool DrawSurfaceNormals = true;

    RaycastInput raycastInput;

    NativeList<Unity.Physics.RaycastHit> RaycastHits;    

    BuildPhysicsWorld buildPhysicsWorld;
    StepPhysicsWorld stepWorld;

    public struct RaycastJob : IJob
    {
        public RaycastInput rayInput;
        public NativeList<Unity.Physics.RaycastHit> raycastHits;
        public bool CollectAllHits;
        [ReadOnly] public PhysicsWorld world;

        public void Execute()
        {
            if(CollectAllHits)
            {
                world.CastRay(rayInput, ref raycastHits);
            }
            else if(world.CastRay(rayInput, out Unity.Physics.RaycastHit hit))
            {
                raycastHits.Add(hit);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        RaycastHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Persistent);
        

        buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        stepWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<StepPhysicsWorld>();
    }

    private void OnDestroy()
    {
        if(RaycastHits.IsCreated)
        {
            RaycastHits.Dispose();
        }       
    }

    // Update is called once per frame
    void LateUpdate()
    {
        stepWorld.FinalSimulationJobHandle.Complete();

        float3 origin = this.transform.position;
        float3 rayDirection = (transform.rotation * direction) * distance;

        RaycastHits.Clear();        

        raycastInput = new RaycastInput
        {
            Start = origin,
            End = origin + rayDirection,
            Filter = CollisionFilter.Default
        };

        JobHandle rayCastJobHandle = new RaycastJob
        {
            rayInput = raycastInput,
            raycastHits = RaycastHits,
            CollectAllHits = CollectAllHits,
            world = buildPhysicsWorld.PhysicsWorld
        }.Schedule();

        rayCastJobHandle.Complete();

        foreach (Unity.Physics.RaycastHit hit in RaycastHits.ToArray())
        {
            var entity = buildPhysicsWorld.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
            GameDataManager.instance.manager.DestroyEntity(entity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(raycastInput.Start, raycastInput.End - raycastInput.Start);

        if(RaycastHits.IsCreated)
        {
            foreach(Unity.Physics.RaycastHit hit in RaycastHits.ToArray())
            {
                Assert.IsTrue(hit.RigidBodyIndex >= 0 && hit.RigidBodyIndex < buildPhysicsWorld.PhysicsWorld.NumBodies);
                Assert.IsTrue(math.abs(math.lengthsq(hit.SurfaceNormal) - 1.0f) < 0.01f);

                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(raycastInput.Start, hit.Position - raycastInput.Start);
                Gizmos.DrawSphere(hit.Position, 0.02f);

                if(DrawSurfaceNormals)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(hit.Position, hit.SurfaceNormal);
                }
            }
        }
    }
}
