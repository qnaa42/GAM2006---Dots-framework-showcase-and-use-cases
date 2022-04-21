using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveBulletSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        var jobHandle = Entities
            .WithName("MoveBulletSystem")
            .ForEach((ref Translation postion, ref Rotation rotation, ref BulletData bulletData) =>
            {
                postion.Value += deltaTime * 100f * math.forward(rotation.Value);
            }).Schedule(inputDeps);

        jobHandle.Complete();

        Entities.WithoutBurst().WithStructuralChanges()
            .ForEach((Entity entity, ref Translation postion, ref Rotation rotation, ref BulletData bulletData, ref LifetimeData lifeTimeData) =>
            {
                float difstanceToTarget = math.distance(GameDataManager.instance.wps[bulletData.waypoint], postion.Value);
                if( difstanceToTarget < 27f)
                {
                    lifeTimeData.lifeLeft = 0f;
                    if(UnityEngine.Random.Range(0,1000) <= 50)
                    {
                        var instance = EntityManager.Instantiate(bulletData.explosionPrefab);
                        EntityManager.SetComponentData(instance, new Translation { Value = postion.Value });
                        EntityManager.SetComponentData(instance, new Rotation { Value = rotation.Value });
                        EntityManager.SetComponentData(instance, new LifetimeData { lifeLeft = 0.5f });
                    }
                }
            }).Run();

        return jobHandle;
    }
}
