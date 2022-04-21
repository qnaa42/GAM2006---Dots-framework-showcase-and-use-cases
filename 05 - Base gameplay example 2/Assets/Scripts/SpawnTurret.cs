using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTurret : MonoBehaviour
{
    public GameObject TurretPrefab;
    public GameObject TurretSpawn;
    public Vector3 mousePos;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                TurretSpawn = Instantiate(TurretPrefab, new Vector3(hit.point.x, 1.5f, hit.point.z), Quaternion.identity) as GameObject;
            }


            
        }
    }
}
