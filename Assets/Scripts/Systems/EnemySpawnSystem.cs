using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class EnemySpawnSystem : SystemBase
{
    private float distanceTraveled = 0f;

    private EntityManager entityManager;

    protected override void OnCreate()
    {
        entityManager = World.EntityManager;
        RequireSingletonForUpdate<MoveEnviromentOptions>();
    }

    protected override void OnUpdate()
    {
        distanceTraveled += GetSingleton<MoveEnviromentOptions>().CurrentSpeed * Time.DeltaTime;
        Entities
            .WithStructuralChanges()
            .WithName("EnemySpawnSystem")
            .ForEach((in EnemySpawnOptions spawnOptions, in DynamicBuffer<PrefabListContentBuffer> contentBuffers, in Translation spawnerPos) =>
            {
                if (distanceTraveled > spawnOptions.SpawnDistance)
                {   
                    // Randomly spwaning prefab from prefabListBuffer
                    int spawnID = UnityEngine.Random.Range(0, contentBuffers.Length);
                    Entity newEnt = entityManager.Instantiate(contentBuffers[spawnID]);

                    // Calculating possible position options. We want to set position in bounds range randomly, but spawned collider should not overlap with bounds
                    PhysicsCollider colliderComp = GetComponent<PhysicsCollider>(newEnt);
                    Aabb myAabb = colliderComp.Value.Value.CalculateAabb();

                    float minXpos = -spawnOptions.SpawnBordersDistance + myAabb.Extents.x / 2 - myAabb.Center.x;
                    float maxXpos = spawnOptions.SpawnBordersDistance - myAabb.Extents.x / 2 - myAabb.Center.x;

                    // Creating new Translation component with calculated options
                    float3 newPos = new float3();
                    newPos.x = UnityEngine.Random.Range(minXpos, maxXpos);
                    newPos.y = spawnerPos.Value.y;
                    newPos.z = spawnerPos.Value.z;
                    Translation newTranslation = new Translation
                    {
                        Value = newPos
                    };

                    entityManager.SetComponentData<Translation>(newEnt, newTranslation);
                    entityManager.AddComponent<MoveEnviromentComp>(newEnt);
                    
                    distanceTraveled = 0f;
                }
            }).Run();
    }
}
