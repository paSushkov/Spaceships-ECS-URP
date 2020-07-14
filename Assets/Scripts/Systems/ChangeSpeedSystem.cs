using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateAfter(typeof(PlayerInputSystem))]
public class ChangeSpeedSystem : ComponentSystem
{
    public Entity OptionsHolder;
    public float CurrentSpeed;
    protected override void OnCreate()
    {
        OptionsHolder = EntityManager.CreateEntity(typeof(MoveEnviromentOptions));
        SetSingleton(new MoveEnviromentOptions());

        this.RequireSingletonForUpdate<PlayerInput>();
    }

    public void SetNewOptions(MoveEnviromentOptions NewOptions)
    { 
        SetSingleton(NewOptions);
    }


    protected override void OnUpdate()
    {
        float dTime = Time.DeltaTime;

        var playerInput = GetSingleton<PlayerInput>();
        var moveOptions = GetSingleton<MoveEnviromentOptions>();

        float newSpeed = moveOptions.MinSpeed;

        if (playerInput.Vertical > 0)
        {
            moveOptions.CurrentExtraSpeed = math.lerp(moveOptions.CurrentExtraSpeed, moveOptions.MaxExtraSpeed, moveOptions.Acceleration * dTime);
        }
        else
        {
            moveOptions.CurrentExtraSpeed = moveOptions.CurrentExtraSpeed * (1 - dTime * moveOptions.Damping);
        }

        newSpeed += moveOptions.CurrentExtraSpeed;
        moveOptions.CurrentSpeed = newSpeed;
        CurrentSpeed = newSpeed;
        SetSingleton(moveOptions);
    }
}