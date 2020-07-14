using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct GunSettings : IComponentData
{
    public bool rechargeActive;
    public float shootCooldownTime;
    public float currentCooldownTime;
}
