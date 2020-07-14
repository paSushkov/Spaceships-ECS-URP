using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class TubeShapedSpawnSystem : SystemBase
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    public JobHandle finalDependency;
    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        var randomArray = World.GetExistingSystem<RandomSystem>().RandomArray;
        
        Entities
            .WithNativeDisableParallelForRestriction(randomArray)
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity spawnerEntity, int nativeThreadIndex, int entityInQueryIndex, in TubeShapedSpawnerComp spawnOptions, in LocalToWorld localToWorld) =>
            {
                var random = randomArray[nativeThreadIndex];
                float3 spawnerPosition = localToWorld.Position;
             
                for (int i = 0; i < spawnOptions.MaxAmount; i++)
                {
                    var instance = commandBuffer.Instantiate(entityInQueryIndex, spawnOptions.Prefab);

                    // Calculate position based on spawn options
                    float r = spawnOptions.MinRadius + random.NextFloat(0, spawnOptions.MaxRadius - spawnOptions.MinRadius);
                    float angle = random.NextFloat(0, math.PI * 2);

                    float3 newPos = new float3();
                    newPos.x = math.sin(angle) * r;
                    newPos.y = math.cos(angle) * r;
                    newPos.z = random.NextFloat(0, spawnOptions.MaxDistanceFromSpawner);
                    var position = math.transform(localToWorld.Value, newPos);

                    // Calculate initial random rotations
                    float4 initialRotation = new float4();
                    initialRotation.x = random.NextFloat();
                    initialRotation.y = random.NextFloat();
                    initialRotation.z = random.NextFloat();
                    initialRotation.w = random.NextFloat();

                    // Setting up position and rotation of spawned prefab instance
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new Translation { Value = position });
                    commandBuffer.SetComponent(entityInQueryIndex, instance, new Rotation { Value = math.quaternion(initialRotation) });

                    // Do we need to rotate instances around Z axis?  
                    if (spawnOptions.RotateAroundZ)
                    {
                        float3 centerPoint = spawnerPosition;
                        centerPoint.z = position.z;


                        // Calculating speed for rotation
                        float rotationSpeed = random.NextFloat(spawnOptions.MinRotSpeed, spawnOptions.MaxRotSpeed);
                        
                        bool moveOut = random.NextBool();
                        bool clockWise = random.NextBool();

                        commandBuffer.AddComponent(entityInQueryIndex,  instance,
                            new RotateAroundZComp { Speed = rotationSpeed, 
                                                    CenterPoint = centerPoint,
                                                    MaxDistance = spawnOptions.MaxRadius, 
                                                    MinDistance = spawnOptions.MinRadius, 
                                                    MoveOut = moveOut,
                                                    Clockwise = clockWise });
                    }
                }

                // Destoying spawner entity
                commandBuffer.DestroyEntity(entityInQueryIndex, spawnerEntity);

                randomArray[nativeThreadIndex] = random;
            })
            .WithName ("Tube_Shaped_spawn_Job")
            .ScheduleParallel();
        
        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);

    }
}
