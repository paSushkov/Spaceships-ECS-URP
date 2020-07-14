using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PlayerShieldComp : IComponentData
{
    public float MaxPlayerShield;
    public float CurrentPlayerShield;
}
