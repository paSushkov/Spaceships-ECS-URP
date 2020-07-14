using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct RotateAroundZComp : IComponentData
{
    public float Speed;
    // To rotate around
    public float3 CenterPoint;
    public float MaxDistance;
    public float MinDistance;
    // if true to move out of center untill bounce and move in
    public bool MoveOut;
    public bool Clockwise;
}
