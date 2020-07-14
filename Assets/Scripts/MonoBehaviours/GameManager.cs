using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IPlayerStatsReciever
{
    private float camShakeTimeOnDamage = 0.25f;
    private float camShakeIntensityModyfier = 0.5f;

    public static GameManager instance;
    
    EntityManager entityManager;
    #region Enviroment Settings
    [Header("Enviroment settings")]
    [SerializeField]
    private float MaxSpeed = 10f;
    [SerializeField]
    private float MinSpeed = 1f;
    [SerializeField]
    private float Acceleration = 3f;
    [SerializeField]
    [Range(0, 10)]
    private float Damping = 5f;
    private Entity envMoveOptionsEntity;
    private MoveEnviromentOptions optionsComp;

    private float currentEnviromentSpeed;
    public float CurrentEnviromentSpeed { get => currentEnviromentSpeed; private set => currentEnviromentSpeed = value; }

    #endregion
    #region PlayerStats

    private float currentPlayerHealth;
    private float maxPlayerHealth;
    public float CurrentPlayerHealth
    {
        get => currentPlayerHealth;
        private set
        {
            if (value < currentPlayerHealth && CinemachineCameraShaker.Instance != null)
            {
                ShakeCamera(currentPlayerHealth, value, maxPlayerHealth, camShakeTimeOnDamage);
            }
            currentPlayerHealth = value;
            if (NotifyHealthChanges != null) NotifyHealthChanges(MaxPlayerHealth, CurrentPlayerHealth);
        }
    }
    public float MaxPlayerHealth
    {
        get => maxPlayerHealth;
        private set
        {
            maxPlayerHealth = value;
            if (NotifyHealthChanges != null) NotifyHealthChanges(MaxPlayerHealth, CurrentPlayerHealth);
        }
    }

    private float currentPlayerShield;
    private float maxPlayerShield;
    public float CurrentPlayerShield
    {
        get => currentPlayerShield;
        private set
        {
            if (value < currentPlayerShield && CinemachineCameraShaker.Instance != null)
            {
                ShakeCamera(currentPlayerShield, value, maxPlayerShield, camShakeTimeOnDamage);
            }
            currentPlayerShield = value;
            if (NotifyShieldChanges != null) NotifyShieldChanges(MaxPlayerShield, CurrentPlayerShield);
        }
    }
    public float MaxPlayerShield
    {
        get => maxPlayerShield;
        private set
        {
            maxPlayerShield = value;
            if (NotifyShieldChanges != null) NotifyShieldChanges(MaxPlayerShield, CurrentPlayerShield);

        }
    }


    public event PlayerStatsChange NotifyHealthChanges;
    public event PlayerStatsChange NotifyShieldChanges;
    #endregion

    private void ShakeCamera(float currentStatValue, float newStatValue, float maxStatValue, float time)
    {
        float intensity = (currentStatValue - newStatValue) / maxStatValue *100f * camShakeIntensityModyfier;
        CinemachineCameraShaker.Instance.ShakeCamera(intensity, time);

    }

    private void Start()
    {
        instance = this;
        UpdateEnviromentMoveOptions();
        SendEnviromentMoveOptionsToSystem();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    private void Update()
    {
        CurrentEnviromentSpeed = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ChangeSpeedSystem>().CurrentSpeed;
    }

    /// <summary>
    /// Updates variable type of IComponentData stored in this manager, which holds parameters of enviroment movement
    /// </summary>
    private void UpdateEnviromentMoveOptions()
    {
        optionsComp = new MoveEnviromentOptions { MaxExtraSpeed = this.MaxSpeed, MinSpeed = this.MinSpeed, Acceleration = this.Acceleration, Damping = this.Damping };
    }

    public MoveEnviromentOptions GetMoveEnviromentOptions()
    {
        return World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ChangeSpeedSystem>().GetSingleton<MoveEnviromentOptions>();
    }

    /// <summary>
    /// Calls method from system to set up initial enviroment moving options
    /// </summary>
    private void SendEnviromentMoveOptionsToSystem()
    {
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ChangeSpeedSystem>().SetNewOptions(optionsComp);
    }
    /// <summary>
    /// Called from NotifyMonobehavioursSystems. Whenever gets called sets internal variables and triggers notification events.
    /// </summary>
    public void GetPlayerStats(PlayerHealthComp PlayerHealthComp, PlayerShieldComp PlayerShieldComp)
    {
        MaxPlayerHealth = PlayerHealthComp.MaxPlayerHealth;
        CurrentPlayerHealth = PlayerHealthComp.CurrentPlayerHealth;
     
        MaxPlayerShield = PlayerShieldComp.MaxPlayerShield;
        CurrentPlayerShield = PlayerShieldComp.CurrentPlayerShield;
    }
}
public delegate void PlayerStatsChange(float maxAmount, float currentAmount);

