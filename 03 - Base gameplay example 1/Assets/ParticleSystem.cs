using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ParticleSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        var jobHandle = Entities
            .WithName("ParticleSystem")
            .ForEach((ref NonUniformScale scale, ref ParticleData particelData) =>
            {
                particelData.timeAlive += deltaTime;
                scale.Value += particelData.timeAlive * 0.8f;
            }).Schedule(inputDeps);

        jobHandle.Complete();

        

        return jobHandle;
    }
}
