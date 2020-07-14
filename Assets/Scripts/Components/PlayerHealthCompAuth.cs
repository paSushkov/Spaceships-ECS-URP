using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class PlayerHealthCompAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public float MaxPlayerHealth;
    public float CurrentPlayerHealth;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var playerHealthComp = new PlayerHealthComp
        {
            MaxPlayerHealth = this.MaxPlayerHealth,
            CurrentPlayerHealth = this.CurrentPlayerHealth < this.MaxPlayerHealth ? this.CurrentPlayerHealth : this.MaxPlayerHealth
        };

        dstManager.AddComponentData(entity, playerHealthComp);
    }
}
