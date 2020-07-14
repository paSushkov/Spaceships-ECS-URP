using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class UpdateListCompAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject[] Recievers;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (!dstManager.HasComponent<UpdateListComp>(entity))
        {
            UpdateListComp newUpdateList = new UpdateListComp
            {
                recievers = new List<IPlayerStatsReciever>()
            };

            dstManager.AddComponent<UpdateListComp>(entity);
            dstManager.SetComponentData(entity, newUpdateList);
        }

        // Try every GO in public array (assigned manually)
        foreach (GameObject DataRecieverGO in Recievers)
        {
            var potentialReceivers = DataRecieverGO.GetComponents<MonoBehaviour>();
            // Try every component on current GameObject...
            foreach (var potentialReceiver in potentialReceivers)
            {
                // Does it implement necessary interface?
                if (potentialReceiver is IPlayerStatsReciever reciever)
                {
                    // Does it already included in update list?
                    if (!dstManager.GetComponentData<UpdateListComp>(entity).recievers.Contains(reciever))
                    {
                        // if not - let`s include that MonoBehavior with Interface in list
                        dstManager.GetComponentData<UpdateListComp>(entity).recievers.Add((IPlayerStatsReciever)potentialReceiver);
                    }
                    // And send a call to update initial values
                    if (dstManager.HasComponent<PlayerHealthComp>(entity) && dstManager.HasComponent<PlayerShieldComp>(entity))
                    {
                        reciever.GetPlayerStats(dstManager.GetComponentData<PlayerHealthComp>(entity), dstManager.GetComponentData<PlayerShieldComp>(entity));
                    }

                }

            }
        }
    }
}