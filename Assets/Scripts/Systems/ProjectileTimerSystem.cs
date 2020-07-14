using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

[UpdateAfter(typeof(PlayerShootSystem))]
public class ProjectileTimerSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        EntityCommandBuffer.Concurrent commandBuffer = commandBufferSystem.CreateCommandBuffer().ToConcurrent();
        float dTime = Time.DeltaTime;

        Entities.ForEach((Entity ent, int entityInQueryIndex, ref ProjectileComp options) =>
        {
            options.lifeTime -= dTime;
            if (options.lifeTime <= 0)
            {
                commandBuffer.DestroyEntity(entityInQueryIndex, ent);
            }
        }).ScheduleParallel();
        commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}