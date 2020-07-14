using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

//[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PlayerInputSystem))]
public class PlayerMoveSystem : SystemBase
{
    PlayerInput playerInput;


    protected override void OnCreate()
    {
        this.RequireSingletonForUpdate<PlayerInput>();
    }

    protected override void OnUpdate()
    {
        playerInput = GetSingleton<PlayerInput>();

        float horizontalInput = playerInput.Horizontal;

        float3 right = new float3(1f, 0f, 0f);

        float dTime = Time.DeltaTime;

        Entities.ForEach((ref PhysicsVelocity velocity, ref Rotation rotation, in PlayerMoveComp moveOptions) =>
        {
            float StrafeRightSpeed = math.dot(velocity.Linear, right);

            float speedChangeRequest = horizontalInput * moveOptions.strafeAcceleration * dTime;

            if ((horizontalInput != 0))
            {
                //Rotate to strafing rotation
                rotation.Value = math.normalize(math.lerp(
                    rotation.Value.value,
                    quaternion.RotateZ(moveOptions.strafeRotationAngle * -horizontalInput).value,
                    moveOptions.rotationSpeed * dTime));


                // Managing strafe speed
                if (math.abs(velocity.Linear.x + speedChangeRequest) < moveOptions.maxStrafeSpeed)
                {
                    velocity.Linear.x += speedChangeRequest;
                }
                else
                {
                    velocity.Linear.x = moveOptions.maxStrafeSpeed * math.sign(horizontalInput);
                }

            }
            else if (horizontalInput == 0)
            {
                // Rotate to normal rotation
                rotation.Value = math.normalize(math.lerp(
                    rotation.Value.value,
                    moveOptions.originalRotation.value,
                    moveOptions.rotationSpeed * dTime));

                velocity.Linear.x -= StrafeRightSpeed * moveOptions.drag * dTime;
            }

        }).Run();
    }
}
