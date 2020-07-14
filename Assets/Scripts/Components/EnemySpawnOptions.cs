using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct EnemySpawnOptions : IComponentData
{
    // Some threat would be spawned every ... distance traveled by player 
    public float SpawnDistance;

    // Spawned threats colliders won`t extend over this borders on World X-axis
    public float SpawnBordersDistance;
}
