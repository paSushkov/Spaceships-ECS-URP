using System.Runtime.Remoting;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.PlayerLoop;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class DamageToPlayerSystem : JobComponentSystem
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

    struct DealDamageOnCollision : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<DamageComp> allDamages;
        [ReadOnly] public ComponentDataFromEntity<UpdateListNowTag> updateTags;
        public ComponentDataFromEntity<PlayerHealthComp> allPlayersHealth;
        public ComponentDataFromEntity<PlayerShieldComp> allPlayersShields;
        public EntityCommandBuffer commandBuffer;


        public void Execute(TriggerEvent triggerEvent)
        {
            #region Checking up who is player & who is damage source
            Entity player;
            Entity damageSource;

            if ((allDamages.Exists(triggerEvent.EntityA)) && allPlayersHealth.Exists(triggerEvent.EntityB))
            {
                player = triggerEvent.EntityB;
                damageSource = triggerEvent.EntityA;
            }
            else if (allDamages.Exists(triggerEvent.EntityB) && allPlayersHealth.Exists(triggerEvent.EntityA))
            {
                player = triggerEvent.EntityA;
                damageSource = triggerEvent.EntityB;
            }
            else
            {
                return;
            }
            #endregion
            
            #region Splitting damage between health and shiled
            float damageToHealth = allDamages[damageSource].damage;
            if (allPlayersShields.Exists(player))
            {
                float damageToShield;
                float playerShieldValue = allPlayersShields[player].CurrentPlayerShield;
                if (playerShieldValue > 0)
                {
                    damageToShield = (playerShieldValue > damageToHealth) ? damageToHealth : playerShieldValue;
                    damageToHealth -= damageToShield;
                    var newShield = allPlayersShields[player];
                    newShield.CurrentPlayerShield -= damageToShield;
                    allPlayersShields[player] = newShield;
                }
            }
            if (damageToHealth > 0)
            {
                var newHealth = allPlayersHealth[player];
                newHealth.CurrentPlayerHealth = math.clamp(newHealth.CurrentPlayerHealth - damageToHealth,
                                                           0f,
                                                           newHealth.MaxPlayerHealth);
                allPlayersHealth[player] = newHealth;
            }
            #endregion

            #region Marking entity with tag to send message to the subscribers
            if (!updateTags.Exists(player))
            {
                commandBuffer.AddComponent<UpdateListNowTag>(player);
            }
            #endregion

            commandBuffer.AddComponent<DestroyMeTagComp>(damageSource);
            commandBuffer.RemoveComponent<DamageComp>(damageSource);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DealDamageOnCollision();
        job.allDamages = GetComponentDataFromEntity<DamageComp>(true);
        job.allPlayersHealth = GetComponentDataFromEntity<PlayerHealthComp>(false);
        job.allPlayersShields = GetComponentDataFromEntity<PlayerShieldComp>(false);
        job.updateTags = GetComponentDataFromEntity<UpdateListNowTag>(true);
        job.commandBuffer = commandBufferSystem.CreateCommandBuffer();


        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        commandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }

}
