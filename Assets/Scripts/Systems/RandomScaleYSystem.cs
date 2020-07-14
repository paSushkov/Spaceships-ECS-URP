using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class RandomScaleYSystem : SystemBase
{
        EndSimulationEntityCommandBufferSystem myCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        myCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

    }
    protected override void OnUpdate()
    {
        var commandBuffer = myCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        var randomArray = World.GetExistingSystem<RandomSystem>().RandomArray;
        float dTime = Time.DeltaTime;

        Entities
            .WithNone<NonUniformScale>()
            .WithAll<RandomScaleYComp>()
            .ForEach((Entity entity, int entityInQueryIndex) =>
            {
                commandBuffer.AddComponent<NonUniformScale>(entityInQueryIndex, entity);
                commandBuffer.SetComponent(entityInQueryIndex, entity, new NonUniformScale { Value = new float3(1, 1, 1) });
            }).ScheduleParallel();



        Entities
            .WithNativeDisableParallelForRestriction(randomArray)
            .ForEach((int nativeThreadIndex, ref NonUniformScale currentScale, ref RandomScaleYComp scaleOptions) =>
            {
                var random = randomArray[nativeThreadIndex];

                if (math.abs(scaleOptions.DesiredYScale - currentScale.Value.y) < 0.1f)
                {
                    scaleOptions.DesiredYScale = random.NextFloat(1f, 5f);
                }
                else
                {
                    currentScale.Value.y = math.lerp(currentScale.Value.y, scaleOptions.DesiredYScale, scaleOptions.LerpSpeed * dTime);
                }
                randomArray[nativeThreadIndex] = random;
            }).ScheduleParallel();

        myCommandBufferSystem.AddJobHandleForProducer(Dependency);

    }
}
