using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct ProjectileComp : IComponentData
{
    public float speed;
    public float lifeTime;
}
