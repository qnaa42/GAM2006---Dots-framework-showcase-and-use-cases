using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

public class MoveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        int nextWp = UnityEngine.Random.Range(0, GameDataManager.instance.wps.Length);

        NativeArray<float3> waypointPosition = new NativeArray<float3>(GameDataManager.instance.wps, Allocator.TempJob);
        var jobHandle = Entities
            .WithNativeDisableParallelForRestriction(waypointPosition)
            .WithName("MoveSystem")
            .ForEach((ref PhysicsVelocity physics, ref PhysicsMass mass, ref Translation position, ref Rotation rotation, ref ZombieData zombieData) =>
            {
                float distance = math.distance(position.Value, waypointPosition[zombieData.currentWP]);
                if (distance < 15)
                {
                    //random wp
                    zombieData.currentWP = nextWp;
                    

                    //Wp in order
                    //zombieData.currentWP++;
                    //if (zombieData.currentWP >= waypointPosition.Length)
                    //    zombieData.currentWP = 0;
                }

                float3 heading;
                heading = waypointPosition[zombieData.currentWP] - position.Value;

                quaternion targetDirection = quaternion.LookRotation(heading, math.up());
                rotation.Value = math.slerp(rotation.Value, targetDirection, deltaTime * zombieData.rotationSpeed);
                physics.Linear = deltaTime * zombieData.speed * math.forward(rotation.Value);

                mass.InverseInertia[0] = 0;
                mass.InverseInertia[1] = 0;
                mass.InverseInertia[2] = 0;
            })
            .WithDisposeOnCompletion(waypointPosition)
            .Schedule(inputDeps);

        jobHandle.Complete();
        return jobHandle;
    }
}
