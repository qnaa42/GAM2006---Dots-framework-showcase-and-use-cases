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
using Unity.Transforms;

public class PointDistanceHitJob : MonoBehaviour
{
    public float distance = 10f;
    public bool CollectAllHits = false;
    public bool DrawSurfaceNormals = true;
    NativeList<DistanceHit> DistanceHits;
    PointDistanceInput PointDistanceInput;

    BuildPhysicsWorld buildPhysicsWorld;
    StepPhysicsWorld stepWorld;

    Entity closestEntity;
    Vector3 lockedOn;

    public AudioSource splat;
    public AudioSource fire;

    public ParticleSystem shoot;

    public struct PointDistanceJob : IJob
    {
        public PointDistanceInput PointDistanceInput;
        public NativeList<DistanceHit> DistanceHits;
        public bool CollectAllhits;
        [ReadOnly] public PhysicsWorld world;

        public void Execute()
        {
            if(CollectAllhits)
            {
                world.CalculateDistance(PointDistanceInput, ref DistanceHits);
            }
            else if(world.CalculateDistance(PointDistanceInput, out DistanceHit hit))
            {
                DistanceHits.Add(hit);
            }
        }

    }

    void Start()
    {
        DistanceHits = new NativeList<DistanceHit>(Allocator.Persistent);
        buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        stepWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<StepPhysicsWorld>();
        shoot.Stop();
    }

    private void OnDestroy()
    {
        if(DistanceHits.IsCreated)
        {
            DistanceHits.Dispose();
        }
    }

    void LateUpdate()
    {
        stepWorld.FinalSimulationJobHandle.Complete();
        float3 origin = transform.position;

        DistanceHits.Clear();

        PointDistanceInput = new PointDistanceInput
        {
            Position = origin,
            MaxDistance = distance,
            Filter = CollisionFilter.Default,
        };

        JobHandle pdjHandle = new PointDistanceJob
        {
            PointDistanceInput = PointDistanceInput,
            DistanceHits = DistanceHits,
            CollectAllhits = CollectAllHits,
            world = buildPhysicsWorld.PhysicsWorld,
        }.Schedule();

        pdjHandle.Complete();

        if(!GameDataManager.instance.manager.Exists(closestEntity))
        {
            float closestDistance = Mathf.Infinity;
            lockedOn = Vector3.zero;

            foreach (DistanceHit hit in DistanceHits.ToArray())
            {
                Assert.IsTrue(hit.RigidBodyIndex >= 0 && hit.RigidBodyIndex < buildPhysicsWorld.PhysicsWorld.NumBodies);
                Assert.IsTrue(math.abs(math.lengthsq(hit.SurfaceNormal) - 1.0f) < 0.01f);

                var entity = buildPhysicsWorld.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                bool hasComponent = GameDataManager.instance.manager.HasComponent<ZombieData>(entity);
                if(closestDistance > hit.Distance && hasComponent)
                {
                    closestDistance = hit.Distance;
                    closestEntity = entity;
                    lockedOn = GameDataManager.instance.manager.GetComponentData<Translation>(entity).Value;
                    Invoke("DestroyClosest", 2);
                    fire.Play();
                    shoot.Play();
                }
            }
        }
        else
            lockedOn = GameDataManager.instance.manager.GetComponentData<Translation>(closestEntity).Value;

        this.transform.LookAt(lockedOn);

        
    }

    void DestroyClosest()
    {
        GameDataManager.instance.manager.DestroyEntity(closestEntity);
        splat.Play();
        fire.Stop();
        shoot.Stop();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;        

        if (DistanceHits.IsCreated)
        {
            foreach (DistanceHit hit in DistanceHits.ToArray())
            {
                Assert.IsTrue(hit.RigidBodyIndex >= 0 && hit.RigidBodyIndex < buildPhysicsWorld.PhysicsWorld.NumBodies);
                Assert.IsTrue(math.abs(math.lengthsq(hit.SurfaceNormal) - 1.0f) < 0.01f);

                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(this.transform.position, hit.Position - (float3) this.transform.position);
                Gizmos.DrawSphere(hit.Position, 0.02f);

                if (DrawSurfaceNormals)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(hit.Position, hit.SurfaceNormal);
                }
            }
        }
    }
}
