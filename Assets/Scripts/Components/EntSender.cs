using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public interface IReceiveEntity
{
    void SetReceivedEntity(Entity entity);
}
public struct SentEntity : IComponentData { }

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class EntSender : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject[] EntityReceivers;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SentEntity() { });

        foreach (GameObject EntityReciever in EntityReceivers)
        { 
        var potentialReceivers = EntityReciever.GetComponents<MonoBehaviour>();
            foreach (var potentialReceiver in potentialReceivers)
            {
                if (potentialReceiver is IReceiveEntity reciever)
                {
                    reciever.SetReceivedEntity(entity);
                }
            }
        }
    }
}
