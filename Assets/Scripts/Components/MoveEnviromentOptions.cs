using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct MoveEnviromentOptions : IComponentData
{
    public float CurrentSpeed;

    public float MinSpeed;

    public float MaxExtraSpeed;
    public float CurrentExtraSpeed;

    public float Acceleration;
    public float Damping;
}
