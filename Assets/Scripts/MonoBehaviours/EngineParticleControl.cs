using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class EngineParticleControl : MonoBehaviour
{
    EntityManager entityManager;
    private Entity moveOptionsEnt;
    private ParticleSystem flame;
    [SerializeField]
    private float ExtraSpeed = -2f;
    [SerializeField]
    private float MinSpeed = -1f;
    private float maxMoveSpeed;

    ParticleSystem.MainModule mainModule;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.MinMaxCurve tempCurve;

    void Start()
    {
        flame = GetComponent<ParticleSystem>();
        emissionModule = flame.emission;
        tempCurve = emissionModule.rateOverTime;
        mainModule = flame.main;

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        moveOptionsEnt = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ChangeSpeedSystem>().OptionsHolder;
        maxMoveSpeed = entityManager.GetComponentData<MoveEnviromentOptions>(moveOptionsEnt).MinSpeed + entityManager.GetComponentData<MoveEnviromentOptions>(moveOptionsEnt).MaxExtraSpeed;
    }

    void Update()
    {
        float currentMoveSpeed = entityManager.GetComponentData<MoveEnviromentOptions>(moveOptionsEnt).CurrentSpeed;
        mainModule.startSpeed = (currentMoveSpeed / (maxMoveSpeed) * ExtraSpeed) * 0.5f + MinSpeed;

        // Changing rate
        tempCurve.constant = (currentMoveSpeed / (maxMoveSpeed) * 35) * 0.5f + 10;
        emissionModule.rateOverTime = tempCurve;
    }
}
