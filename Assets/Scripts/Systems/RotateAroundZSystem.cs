using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class RotateAroundZSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dTime = Time.DeltaTime;


        Entities.ForEach((ref PhysicsVelocity velocity, ref RotateAroundZComp moveOptions, in Translation translation) => {

            float3 oldVelocityDir = math.normalizesafe(velocity.Linear);
            float3 vectorToCenter = moveOptions.CenterPoint - translation.Value;

            float3 rotationAxis = new float3(moveOptions.CenterPoint.x, moveOptions.CenterPoint.y, moveOptions.CenterPoint.z - 1f) - moveOptions.CenterPoint;
            float3 rotationForceVector = math.normalize(math.cross(rotationAxis, vectorToCenter));
            if (!moveOptions.Clockwise)
            {
                rotationForceVector *= -1f;
            }

            float3 newVelocity = rotationForceVector + oldVelocityDir;
            
            if (math.distance(moveOptions.CenterPoint, translation.Value) < moveOptions.MinDistance)
            {
                moveOptions.MoveOut = true;
            }
            else if (math.distance(moveOptions.CenterPoint, translation.Value) > moveOptions.MaxDistance)
            {
                moveOptions.MoveOut = false;
            }

            if (moveOptions.MoveOut)
            {
                newVelocity += math.normalize(-vectorToCenter);
            }
            else
            { 
                newVelocity += math.normalize(vectorToCenter);
            }
            
            newVelocity = math.normalize(newVelocity);

            newVelocity *= moveOptions.Speed;

            velocity.Linear = newVelocity;

        }).ScheduleParallel();
    }
}
