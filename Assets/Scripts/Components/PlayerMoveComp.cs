using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PlayerMoveComp : IComponentData
{
    public float maxStrafeSpeed;
    public float strafeAcceleration;
    public float drag;

    public quaternion originalRotation;
    public float strafeRotationAngle;
    public float rotationSpeed;
}
