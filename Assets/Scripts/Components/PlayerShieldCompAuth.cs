using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class PlayerShieldCompAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public float MaxPlayerShield;
    public float CurrentPlayerShield;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var playerShieldComp = new PlayerShieldComp
        {
            MaxPlayerShield = this.MaxPlayerShield,
            CurrentPlayerShield = this.CurrentPlayerShield < this.MaxPlayerShield ? this.CurrentPlayerShield : this.MaxPlayerShield
        };

        dstManager.AddComponentData(entity, playerShieldComp);
    }
}
