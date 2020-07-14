using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class EngineAuthComp : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject EngineTrail;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (EngineTrail != null)
        {
            GameObject trail = Instantiate(EngineTrail);
            var potentialReceivers = trail.GetComponents<MonoBehaviour>();
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
