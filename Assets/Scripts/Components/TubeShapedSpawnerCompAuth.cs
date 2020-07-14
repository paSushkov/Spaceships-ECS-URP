using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TubeShapedSpawnerCompAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject Prefab;
    public float MaxRadius;
    public float MinRadius;
    public float MaxDistanceFromSpawner;
    public int MaxAmount;

    public bool RotateAroundZ;
    public float MinRotationSpeed;
    public float MaxRotationSpeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new TubeShapedSpawnerComp
        {
            Prefab = conversionSystem.GetPrimaryEntity(this.Prefab),
            MaxRadius = this.MaxRadius,
            MinRadius = this.MinRadius,
            MaxDistanceFromSpawner = this.MaxDistanceFromSpawner,
            MaxAmount = this.MaxAmount,
            RotateAroundZ = this.RotateAroundZ,
            MinRotSpeed = this.MinRotationSpeed,
            MaxRotSpeed = this.MaxRotationSpeed
        };

        dstManager.AddComponentData(entity, spawnerData);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Prefab);
    }
}
