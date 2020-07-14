using System.Diagnostics;
using System.Runtime.Remoting;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(SimulationSystemGroup))]

public class BoundsColliderSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;

    protected override void OnCreate()
    {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    struct BounceOnCollideBounds : ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<BoundColliderComp> allBounds;
        [ReadOnly] public ComponentDataFromEntity<PlayerMoveComp> allPlayers;
        public ComponentDataFromEntity<Translation> allTranslations;
        public ComponentDataFromEntity<PhysicsVelocity> allVelocity;

        public void Execute(CollisionEvent collisionEvent)
        {

            // In case of incorrect "Collide with" options
            if (allBounds.Exists(collisionEvent.EntityA) && allBounds.Exists(collisionEvent.EntityB))
            {
                return;
            }

            Entity player;
            Entity border;

            if (allBounds.Exists(collisionEvent.EntityA) && allPlayers.Exists(collisionEvent.EntityB))
            {
                player = collisionEvent.EntityB;
                border = collisionEvent.EntityA;
            }
            else if (allBounds.Exists(collisionEvent.EntityB) && allPlayers.Exists(collisionEvent.EntityA))
            {
                player = collisionEvent.EntityA;
                border = collisionEvent.EntityB;

            }
            else
            {
                return;
            }

            // To manually new position of player
            // prevent player glithing through the border in case he pushes enough long and hard
            float3 bounce = new float3(0.2f, 0, 0);
            float3 newPosition = allTranslations[player].Value;

            if (allVelocity.Exists(player))
            {
                PhysicsVelocity newVelocity = allVelocity[player];

                // "Magic number" to change .X value of linear velocity on collision. It works and looks not bad. 
                float bouncyFactorVelocity = 0.75f;

                if (allTranslations[player].Value.x < allTranslations[border].Value.x)
                {
                    newPosition -= bounce;
                }
                else
                {
                    newPosition += bounce;
                }
                
                // Change Velocity on opposite X * bouncyFactor
                newVelocity.Linear.x = -newVelocity.Linear.x * bouncyFactorVelocity;

                allTranslations[player] = new Translation { Value = newPosition };
                allVelocity[player] = newVelocity;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new BounceOnCollideBounds();
        job.allBounds = GetComponentDataFromEntity<BoundColliderComp>(true);
        job.allPlayers = GetComponentDataFromEntity<PlayerMoveComp>(true);
        job.allTranslations = GetComponentDataFromEntity<Translation>(false);
        job.allVelocity = GetComponentDataFromEntity<PhysicsVelocity>(false);


        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDependencies);
        jobHandle.Complete();
        return jobHandle;
    }
}