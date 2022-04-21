using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct ZombieData : IComponentData
{
    public float speed;
    public float rotationSpeed;
    public int currentWP;
}
