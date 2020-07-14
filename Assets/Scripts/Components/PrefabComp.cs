using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PrefabComp : IComponentData
{
    public Entity Prefab;
}
