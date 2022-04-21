using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BulletData : IComponentData
{
    public int waypoint;
    public Entity explosionPrefab;
}
