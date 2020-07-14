using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(DamageToPlayerSystem))]
public class NotifyMonobehavioursSystems : SystemBase
{
    EntityManager entityManager;
    protected override void OnCreate()
    {
        entityManager = World.EntityManager;
    }
    protected override void OnUpdate()
    {
        
        Entities.ForEach((UpdateListComp subscribers, Entity entity, ref UpdateListNowTag tagUpdate, in PlayerHealthComp healthComp, in PlayerShieldComp shieldComp) => {
            foreach (IPlayerStatsReciever subscriber in  subscribers.recievers)
            {
                if (subscriber is null)
                {
                    subscribers.recievers.Remove(subscriber);
                }
                else
                {
                    subscriber.GetPlayerStats(healthComp, shieldComp);
                }
            }
           entityManager.RemoveComponent<UpdateListNowTag>(entity);
        }).WithStructuralChanges().WithoutBurst().Run();


    }
}
