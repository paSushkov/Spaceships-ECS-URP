using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class PrefabListComponentAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public List<GameObject> PrefabsList;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<PrefabListContentBuffer>(entity);
        var buffer = dstManager.GetBuffer<PrefabListContentBuffer>(entity);

        foreach (GameObject newPrefab in PrefabsList)
        {
            Entity newPrefabContent = conversionSystem.GetPrimaryEntity(newPrefab);

            PrefabListContentBuffer newElem = new PrefabListContentBuffer
            {
                Content = newPrefabContent,
            };
            
            buffer.Add(newElem);
        }
    }
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (GameObject myPrefab in PrefabsList)
        {
            referencedPrefabs.Add(myPrefab);
        }
    }
}
