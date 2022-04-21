using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct LifetimeData : IComponentData
{
    public float lifeLeft;
}
