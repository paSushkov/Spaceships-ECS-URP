using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(PlayerMoveSystem))]
public class PlayerShootSystem : SystemBase
{
    EntityManager entityManager;

    protected override void OnCreate()
    {
        entityManager = World.EntityManager;
        RequireSingletonForUpdate<PlayerInput>();
    }
    protected override void OnUpdate()
    {
        float dTime = Time.DeltaTime;
        bool shooting = GetSingleton<PlayerInput>().Shoot;

        if (shooting)
        {

            Entities
                .WithStructuralChanges()
                .WithName("PlayerShootJob")
            .ForEach((int entityInQueryIndex, ref GunSettings gunSettings, in LocalToWorld L2W, in PrefabComp prefabComp) =>
            {
                if (gunSettings.rechargeActive)
                {
                    return;
                }
                Entity projectile = entityManager.Instantiate(prefabComp.Prefab);

                var projectileSettings = GetComponent<ProjectileComp>(prefabComp.Prefab);

                var newVelocity = new PhysicsVelocity();
                newVelocity.Linear.z = projectileSettings.speed;

                var newPos = new Translation { Value = L2W.Position };

                entityManager.SetComponentData(projectile, newPos);
                entityManager.SetComponentData(projectile, newVelocity);

                gunSettings.rechargeActive = true;
            }).Run();
        }
    }
}
