using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GunSettingsAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public float ShootRatePerSecond;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var newSettings = new GunSettings
        {
            shootCooldownTime = 1f / ShootRatePerSecond,
            currentCooldownTime = 0,
            rechargeActive = false
        };
        
        dstManager.AddComponentData(entity, newSettings);
    }
}
