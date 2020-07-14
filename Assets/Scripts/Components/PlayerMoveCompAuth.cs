using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class PlayerMoveCompAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public float maxStrafeSpeed;
    public float strafeAcceleration;
    public float drag;

    private quaternion originalRotation;
    public float strafeRotationAngle;
    public float rotationSpeed;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var moveComp = new PlayerMoveComp
        {
            maxStrafeSpeed = this.maxStrafeSpeed,
            strafeAcceleration = this.strafeAcceleration,
            drag = this.drag,
            originalRotation = transform.rotation,
            strafeRotationAngle = math.radians (this.strafeRotationAngle),
            rotationSpeed = this.rotationSpeed
        };
        dstManager.AddComponentData(entity, moveComp);
        
        
    }
}
