using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class GridFillSystem : ComponentSystem
{
    EntityManager entityManager;
    EndInitializationEntityCommandBufferSystem mCommandBufferSystem;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        entityManager = World.EntityManager;
        mCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        EntityCommandBuffer commandBuffer = mCommandBufferSystem.CreateCommandBuffer();

        Entities
            .WithAll<GridBufferContent>()
            .ForEach((Entity entity, DynamicBuffer<GridBufferContent> gridBufferContents, ref LocalToWorld l2W, ref PrefabComp prefab, ref GridComp grid) =>
            {
                float3 cellOffset = new float3(grid.widthSize * grid.widhtCellCenterOffset, grid.heightSize * grid.heightCellCenterOffset, grid.lenghtSize * grid.lenghtCellCenterOffset);


                for (int widhtCell = 0; widhtCell < grid.width; widhtCell++)
                {
                    for (int lenghtCell = 0; lenghtCell < grid.lenght; lenghtCell++)
                    {
                        for (int heightCell = 0; heightCell < grid.height; heightCell++)
                        {
                            Entity gridElement = entityManager.Instantiate(prefab.Prefab);
                            
                            var position = new Translation
                            {
                                Value = l2W.Position
                                        + new float3(widhtCell * grid.widthSize, heightCell * grid.heightSize, lenghtCell * grid.lenghtSize)
                                        + cellOffset
                            };

                            entityManager.SetComponentData<Translation>(gridElement, position);
                            entityManager.SetComponentData<Rotation>(gridElement, new Rotation { Value = l2W.Rotation });
                            commandBuffer.AppendToBuffer<GridBufferContent>(entity, new GridBufferContent { Content = gridElement });
                        }
                    }
                }
            }
            );
    }
    protected override void OnUpdate()
    {
    }
}
