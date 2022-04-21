using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class ShootSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        NativeArray<float3> gunPostions = new NativeArray<float3>(GameDataManager.instance.gunLocations, Allocator.TempJob);

        Entities.WithoutBurst().WithStructuralChanges().WithNativeDisableParallelForRestriction(gunPostions)
            .ForEach((Entity entity, ref Translation position, ref Rotation rotation, ref ShipData shipData) =>
            {
                float3 directionToTarget = GameDataManager.instance.wps[shipData.currentWP] - position.Value;
                float angleToTarget = math.acos(
                                        math.dot(math.forward(rotation.Value), directionToTarget) /
                                        (math.length(math.forward(rotation.Value)) * math.length(directionToTarget)));
                if (angleToTarget < math.radians(5) && math.length(directionToTarget) < 100)
                {
                    foreach (float3 gunPos in gunPostions)
                    {
                        var instance = EntityManager.Instantiate(shipData.bulletPrefab);
                        EntityManager.SetComponentData(instance, new Translation { Value = position.Value + math.mul(rotation.Value, gunPos) });
                        EntityManager.SetComponentData(instance, new Rotation { Value = rotation.Value });
                        EntityManager.SetComponentData(instance, new LifetimeData { lifeLeft = 1f });
                        EntityManager.SetComponentData(instance, new BulletData { waypoint = shipData.currentWP, explosionPrefab = shipData.explosionPrefab });
                    }
                }
            })
            .WithDisposeOnCompletion(gunPostions)
            .Run();

        return inputDeps;
    }
}
