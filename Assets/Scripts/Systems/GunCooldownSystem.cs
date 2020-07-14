using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateBefore(typeof(PlayerShootSystem))]
public class GunCooldownSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dTime = Time.DeltaTime;
        Entities
            .WithName("RechargingGuns")
            .ForEach((ref GunSettings gunSettings) => {

                if (gunSettings.rechargeActive)
                {
                    gunSettings.currentCooldownTime += dTime;
                    if (gunSettings.currentCooldownTime >= gunSettings.shootCooldownTime)
                    {
                        gunSettings.currentCooldownTime = 0f;
                        gunSettings.rechargeActive = false;
                    }
                }
        }).ScheduleParallel();
    }
}
