using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        float speed = 10.0f;
        float3 targetLocation = new float3(0, 0, 0);
        var jobHandle = Entities
            .WithName("MoveSystem")
            .ForEach((ref Translation position, ref Rotation rotation) =>
            {
                float3 pivot = targetLocation;
                float rotationSpeed = deltaTime * speed * 1/math.distance(position.Value, targetLocation);
                position.Value = math.mul(quaternion.AxisAngle(new float3(0,1,0), rotationSpeed), position.Value - pivot)+pivot;
            }).Schedule(inputDeps);
        return jobHandle;
    }
}
