using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PlayerInput : IComponentData
{
    public float Vertical;
    public float Horizontal;

    public bool Shoot;
}
