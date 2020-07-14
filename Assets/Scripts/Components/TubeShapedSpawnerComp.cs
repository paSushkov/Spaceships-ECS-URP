using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TubeShapedSpawnerComp : IComponentData
{
    public Entity Prefab;
    public int MaxAmount;
    public float MaxRadius;
    public float MinRadius;
    public float MaxDistanceFromSpawner;

    public bool RotateAroundZ;
    public float MinRotSpeed;
    public float MaxRotSpeed;


}