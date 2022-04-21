using System.Diagnostics;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Burst;
using System.Collections;

public class ECSManager : MonoBehaviour
{
    public GameObject zombiePrefab;
    int numZombies = 500;
    BlobAssetStore store;

    public float secondBetweenSpawns = 1f;
    public float secondsToNextSpawn = 0f;
    // Start is called before the first frame update
    void Start()
    {
        store = new BlobAssetStore();
        GameDataManager.instance.manager = World.DefaultGameObjectInjectionWorld.EntityManager;                
    }

    public void SpawnZombies(int howMany)
    {
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, store);
        var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(zombiePrefab, settings);
        

        for (int i = 0; i < howMany; i++)
        {
            for (int _x = 0; _x < math.sqrt(numZombies); _x++)
            {
                for (int _z = 0; _z < math.sqrt(numZombies); _z++)
                {
                    var instance = GameDataManager.instance.manager.Instantiate(prefab);
                    float x = _x;
                    float y = 1.5f;
                    float z =_z ;

                    var position = transform.TransformPoint(new float3(x + (x *0.25f), y, z+(z*0.25f)));
                    GameDataManager.instance.manager.SetComponentData(instance, new Translation { Value = position });

                    int closestWP = 0;
                    float distance = Mathf.Infinity;
                    for (int j = 0; j < GameDataManager.instance.wps.Length; j++)
                    {
                        if (Vector3.Distance(GameDataManager.instance.wps[j], position) < distance)
                        {
                            closestWP = j;
                            distance = Vector3.Distance(GameDataManager.instance.wps[j], position);
                        }
                    }

                    GameDataManager.instance.manager.SetComponentData(instance, new ZombieData { speed = UnityEngine.Random.Range(150, 200), rotationSpeed = UnityEngine.Random.Range(1, 2), currentWP = closestWP });
                }
            }                                         
        }
    }

    private IEnumerator wait(float howLong)
    {
        
        yield return new WaitForSeconds(howLong);
    }

    private void OnDestroy()
    {
        store.Dispose();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {

            SpawnZombies(1);
            
            
        }
    }
    


}
