using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class DestructionByTagSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem myCommandBufferSystem;

    protected override void OnCreate()
    {
        myCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }
    protected override void OnUpdate()
    {
        var commandBuffer = myCommandBufferSystem.CreateCommandBuffer();

        Entities
            .WithNone<DestructionEffectComp>()
            .ForEach((Entity entity, in DestroyMeTagComp destructionTag) => {
                commandBuffer.DestroyEntity(entity);
            })
            .WithoutBurst()
            .Run();
        Entities
            .ForEach((Entity entity, in Translation myTranslation, in DestroyMeTagComp destructionTag, in DestructionEffectComp destructionComponent) => {
                var GO = UnityEngine.MonoBehaviour.Instantiate(destructionComponent.effectPrefab);
                GO.transform.position = myTranslation.Value;
                commandBuffer.DestroyEntity(entity);
        })
            .WithoutBurst()
            .Run();
        myCommandBufferSystem.AddJobHandleForProducer(Dependency);

    }
}
