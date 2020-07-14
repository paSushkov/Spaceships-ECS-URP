using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct RandomScaleYComp : IComponentData
{
    public float MaxScaleY;
    public float MinScaleY;
    public float DesiredYScale;
    public float LerpSpeed;
}
