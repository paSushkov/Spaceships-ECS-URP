using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[ConverterVersion("macton", 4)]
public class PrefabCompAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject Prefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var prefabData = new PrefabComp
        {
            Prefab = conversionSystem.GetPrimaryEntity(this.Prefab),
        };

        dstManager.AddComponentData(entity, prefabData);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Prefab);
    }
}