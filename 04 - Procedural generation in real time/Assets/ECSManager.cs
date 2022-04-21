using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ECSManager : MonoBehaviour
{
    BlobAssetStore store;

    EntityManager manager;

    public GameObject player;

    public GameObject sandPrefab;
    public GameObject dirtPrefab;
    public GameObject grassPrefab;
    public GameObject rockPrefab;
    public GameObject snowPrefab;

    const int worldHalfSize = 75;

    [Header("Perlin 1")]
    public bool active1 = true;
    [Range(0.1f, 10f)]
    public float strenght1 = 1f;
    [Range(0.1f, 1f)]
    public float scale1 = 0.1f;

    [Header("Perlin 2")]
    public bool active2 = true;
    [Range(0.1f, 10f)]
    public float strenght2 = 1f;
    [Range(0.1f, 1f)]
    public float scale2 = 0.1f;

    [Header("Perlin 3")]
    public bool active3 = true;
    [Range(0.1f, 10f)]
    public float strenght3 = 1f;
    [Range(0.1f, 1f)]
    public float scale3 = 0.1f;

    [Header("Landscape Settings")]
    [Range(0f, 100f)]
    public float sandLevel = 2f;
    [Range(0f, 100f)]
    public float dirtLevel = 4f;
    [Range(0f, 100f)]
    public float grassLevel = 6f;
    [Range(0f, 100f)]
    public float rockLevel = 8f;
    [Range(0f, 100f)]
    public float snowLevel = 10f;

    private void Start()
    {
        store = new BlobAssetStore();

        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, store);

        GameDataManager.sand = GameObjectConversionUtility.ConvertGameObjectHierarchy(sandPrefab, settings);
        GameDataManager.dirt = GameObjectConversionUtility.ConvertGameObjectHierarchy(dirtPrefab, settings);
        GameDataManager.grass = GameObjectConversionUtility.ConvertGameObjectHierarchy(grassPrefab, settings);
        GameDataManager.rock = GameObjectConversionUtility.ConvertGameObjectHierarchy(rockPrefab, settings);
        GameDataManager.snow = GameObjectConversionUtility.ConvertGameObjectHierarchy(snowPrefab, settings);


        for (int z = -worldHalfSize; z <= worldHalfSize; z++)
        {
            for (int x = -worldHalfSize; x < worldHalfSize; x++)
            {
                var position = new Vector3(x, 0, z);
                Entity instance = manager.Instantiate(GameDataManager.sand);

                manager.SetComponentData(instance, new Translation { Value = position });
                manager.SetComponentData(instance, new BlockData { initialPosition = position });
            }
        }

    }

    private void Update()
    {
        if (GameDataManager.strenght1 != strenght1) GameDataManager.changedData = true;
        else if (GameDataManager.scale1 != scale1) GameDataManager.changedData = true;
        else if (GameDataManager.strenght2 != strenght2) GameDataManager.changedData = true;
        else if (GameDataManager.scale2 != scale2) GameDataManager.changedData = true;
        else if (GameDataManager.strenght3 != strenght3) GameDataManager.changedData = true;
        else if (GameDataManager.scale3 != scale3) GameDataManager.changedData = true;
        else if (GameDataManager.sandLevel != sandLevel) GameDataManager.changedData = true;
        else if (GameDataManager.dirtLevel != dirtLevel) GameDataManager.changedData = true;
        else if (GameDataManager.grassLevel != grassLevel) GameDataManager.changedData = true;
        else if (GameDataManager.rockLevel != rockLevel) GameDataManager.changedData = true;
        else if (GameDataManager.snowLevel != snowLevel) GameDataManager.changedData = true;
        else if (GameDataManager.playerPosition != player.transform.position) GameDataManager.changedData = true;
        else if (GameDataManager.active1 != active1) GameDataManager.changedData = true;
        else if (GameDataManager.active2 != active2) GameDataManager.changedData = true;
        else if (GameDataManager.active3 != active3) GameDataManager.changedData = true;


        GameDataManager.playerPosition = player.transform.position;


        GameDataManager.active1 = active1;
        GameDataManager.strenght1 = strenght1;
        GameDataManager.scale1 = scale1;

        GameDataManager.active2 = active2;
        GameDataManager.strenght2 = strenght2;
        GameDataManager.scale2 = scale2;

        GameDataManager.active3 = active3;
        GameDataManager.strenght3 = strenght3;
        GameDataManager.scale3 = scale3;

        GameDataManager.sandLevel = sandLevel;
        GameDataManager.dirtLevel = dirtLevel;
        GameDataManager.grassLevel = grassLevel;
        GameDataManager.rockLevel = rockLevel;
        GameDataManager.snowLevel = snowLevel;
    }

    private void OnDestroy()
    {
        store.Dispose();
    }
}
