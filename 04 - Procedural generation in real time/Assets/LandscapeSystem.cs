using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Collections;

public class LandscapeSystem : JobComponentSystem
{
    EntityQuery blockQuery;

    protected override void OnCreate()
    {
        blockQuery = GetEntityQuery(typeof(BlockData));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        bool active1 = GameDataManager.active1;
        int _perlin1;
        if (active1)
        {
            _perlin1 = 1;
        }
        else _perlin1 = 0;
        float strenght1 = GameDataManager.strenght1;
        float scale1 = GameDataManager.scale1;

        bool active2 = GameDataManager.active2;
        int _perlin2;
        if (active2)
        {
            _perlin2 = 1;
        }
        else _perlin2 = 0;
        float strenght2 = GameDataManager.strenght2;
        float scale2 = GameDataManager.scale2;

        bool active3 = GameDataManager.active3;
        int _perlin3;
        if (active3)
        {
            _perlin3 = 1;
        }
        else _perlin3 = 0;
        float strenght3 = GameDataManager.strenght3;
        float scale3 = GameDataManager.scale3;

        float3 offset = GameDataManager.playerPosition;

        var jobHandle = Entities
            .WithName("LandscapeSystem")
            .ForEach((ref Translation position, ref BlockData blockData) =>
            {
                var vertex = blockData.initialPosition + offset;                     
                var perlin1 = (Mathf.PerlinNoise(vertex.x * scale1, vertex.z * scale1) * strenght1)*_perlin1;                                
                var perlin2 = (Mathf.PerlinNoise(vertex.x * scale2, vertex.z * scale2) * strenght2)*_perlin2;
                var perlin3 = (Mathf.PerlinNoise(vertex.x * scale3, vertex.z * scale3) * strenght3)*_perlin3;
                var height = perlin1 + perlin2 + perlin3;
                position.Value = new Vector3(vertex.x, height, vertex.z);
            }).Schedule(inputDeps);
        jobHandle.Complete();

        if (GameDataManager.changedData)
        {
            using (var blockEntities = blockQuery.ToEntityArray(Allocator.TempJob))
            {
                foreach (var entity in blockEntities)
                {
                    float height = EntityManager.GetComponentData<Translation>(entity).Value.y;

                    Entity block;
                    if (height <= GameDataManager.sandLevel)
                        block = GameDataManager.sand;
                    else if (height <= GameDataManager.dirtLevel)
                        block = GameDataManager.dirt;
                    else if (height <= GameDataManager.grassLevel)
                        block = GameDataManager.grass;
                    else if (height <= GameDataManager.rockLevel)
                        block = GameDataManager.rock;
                    else
                        block = GameDataManager.snow;

                    RenderMesh colourRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(block);
                    var entityRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);

                    entityRenderMesh.material = colourRenderMesh.material;
                    EntityManager.SetSharedComponentData(entity, entityRenderMesh);
                }
            }
            GameDataManager.changedData = false;
        }


        return inputDeps;
    }
}
