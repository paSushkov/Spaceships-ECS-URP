using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class DamageToEnemiesSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;


    struct DealDamageToEnemy : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<DamageComp> allDamages;
        public ComponentDataFromEntity<EnemyHealthComponent> allEnemyHealth;
        public EntityCommandBuffer commandBuffer;


        public void Execute(TriggerEvent triggerEvent)
        {
            Entity enemyEntity;
            Entity damageSourceEntity;

            #region Checking up who is enemy & who is damage source
            if ((allDamages.Exists(triggerEvent.EntityA)) && allEnemyHealth.Exists(triggerEvent.EntityB))
            {
                enemyEntity = triggerEvent.EntityB;
                damageSourceEntity = triggerEvent.EntityA;
            }
            else if (allDamages.Exists(triggerEvent.EntityB) && allEnemyHealth.Exists(triggerEvent.EntityA))
            {
                enemyEntity = triggerEvent.EntityA;
                damageSourceEntity = triggerEvent.EntityB;
            }
            else {return;}
            #endregion

            if (allEnemyHealth[enemyEntity].health - allDamages[damageSourceEntity].damage > 0)
            {
                var newEnemyHealth = allEnemyHealth[enemyEntity];
                newEnemyHealth.health -= allDamages[damageSourceEntity].damage;
                commandBuffer.SetComponent<EnemyHealthComponent>(enemyEntity, newEnemyHealth);
            }
            else
            {
                commandBuffer.AddComponent<DestroyMeTagComp>(enemyEntity);
            }
            commandBuffer.AddComponent<DestroyMeTagComp>(damageSourceEntity);
            commandBuffer.RemoveComponent<DamageComp>(damageSourceEntity);
        }
    }


    protected override void OnCreate()
    {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DealDamageToEnemy();
        job.commandBuffer = commandBufferSystem.CreateCommandBuffer();
        job.allDamages = GetComponentDataFromEntity<DamageComp>(true);
        job.allEnemyHealth = GetComponentDataFromEntity<EnemyHealthComponent>(false);

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        commandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;



    }
}
