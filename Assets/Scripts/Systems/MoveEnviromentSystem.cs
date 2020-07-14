using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

[UpdateAfter(typeof(ChangeSpeedSystem))]
public class MoveEnviromentSystem : SystemBase
{
    protected override void OnCreate()
    {
        this.RequireSingletonForUpdate<MoveEnviromentOptions>();
    }

    protected override void OnUpdate()
    {
        float speed = GetSingleton<MoveEnviromentOptions>().CurrentSpeed;
        float dTime = Time.DeltaTime;

        Entities.ForEach((ref Translation position, in MoveEnviromentComp moveTag) =>
        {
            position.Value.z -= speed* dTime;

        }).ScheduleParallel();
    }
}