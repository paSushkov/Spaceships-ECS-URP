using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


// System to "respawn" grid content elements in case their position.Value.z is less than it`s spawner
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class GridContentRespawn : SystemBase
{
    EndSimulationEntityCommandBufferSystem mCommandBufferSystem;

    protected override void OnCreate()
    {
        mCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = mCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .ForEach((int entityInQueryIndex, ref DynamicBuffer <GridBufferContent> gridBufferContents, in LocalToWorld l2W, in GridComp gridOptions) =>
            {

                float3 spawnerPos = l2W.Position;
                float3 cellOffset = new float3(gridOptions.widthSize * gridOptions.widhtCellCenterOffset, gridOptions.heightSize * gridOptions.heightCellCenterOffset, gridOptions.lenghtSize * gridOptions.lenghtCellCenterOffset);
                for (int i = 0, k = 0; i < gridBufferContents.Length - k; i++)
                {
                    Entity content = gridBufferContents[i].Content;
                    float3 currentPos = GetComponent<Translation>(content).Value;
                    if (spawnerPos.z - currentPos.z > 0)
                    {
                        var newPos = new Translation
                        {
                            Value =
                            // Position of the last element in Dynamic buffer of spawner
                            GetComponent<Translation>(gridBufferContents[gridBufferContents.Length - 1].Content).Value
                            // + Z value of cell offset
                            + new float3(0, 0, gridOptions.lenghtSize * (1 + cellOffset.z))
                        };
                        commandBuffer.SetComponent<Translation>(entityInQueryIndex, content, newPos);
                        gridBufferContents.RemoveAt(i);
                        gridBufferContents.Add(new GridBufferContent { Content = content });
                        k++;
                    }
                }
            }).ScheduleParallel();

        mCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
