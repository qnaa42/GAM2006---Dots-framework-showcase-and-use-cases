using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateAfter(typeof(MoveBulletSystem))]
public class TimedDestroySystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem buffer;

    protected override void OnCreate()
    {
        buffer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        EntityCommandBuffer.ParallelWriter ecb = buffer.CreateCommandBuffer().AsParallelWriter();

        float deltaTime = Time.DeltaTime;

        Entities
            .WithName("TimedDestroySystem")
            .ForEach((Entity entity, int entityInQueryIndex, ref LifetimeData lifeTimeData) =>
            {
                lifeTimeData.lifeLeft -= deltaTime;
                if (lifeTimeData.lifeLeft <= 0)
                {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        buffer.AddJobHandleForProducer(Dependency);
    }


}
