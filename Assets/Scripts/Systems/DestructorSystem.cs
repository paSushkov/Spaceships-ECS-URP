using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class DestructorSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    struct DestroyEntityOnTrigger : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<DestroyerComp> allDestroyers;

        public EntityCommandBuffer commandBuffer;
        public void Execute(TriggerEvent triggerEvent)
        {
            // In case of incorrect "Collide with" options
            if (allDestroyers.Exists(triggerEvent.EntityA) && allDestroyers.Exists(triggerEvent.EntityB))
            {
                return;
            }

            Entity entityToDestroy;

            if (allDestroyers.Exists(triggerEvent.EntityA))
            {
                entityToDestroy = triggerEvent.EntityB;
            }
            else if (allDestroyers.Exists(triggerEvent.EntityB))
            {
                entityToDestroy = triggerEvent.EntityA;
            }
            else
            {
                return;
            }

            commandBuffer.DestroyEntity(entityToDestroy);
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DestroyEntityOnTrigger();
        job.allDestroyers = GetComponentDataFromEntity<DestroyerComp>(true);

        job.commandBuffer = commandBufferSystem.CreateCommandBuffer();

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        commandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;

    }
}
