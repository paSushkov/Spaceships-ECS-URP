using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class EntityTracker : MonoBehaviour, IReceiveEntity
{
    #region Variables for isnpector
    [SerializeField] private bool UseUpdate = false;
    [SerializeField] private bool UseFixedUpdate = false;
    [SerializeField] private bool UseLateUpdate = false;
    [SerializeField] private bool UsePreditionByVelocity = false;
    #endregion
    #region Properties
    public Entity Target { get => target; private set => target = value; }
    public Vector3 TargetPosition { get => targetPosition; private set => targetPosition = value; }
    public Quaternion TargetRotation { get => targetRotation; private set => targetRotation = value; }
    public Vector3 TargetForward { get => targetForward; private set => targetForward = value; }
    public Vector3 TargetRight { get => targetRight; private set => targetRight = value; }
    public Vector3 TargetUP { get => targetUP; private set => targetUP = value; }
    #endregion
    #region Variables for local usage
    EntityManager em;
    private Entity target;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 targetForward;
    private Vector3 targetRight;
    private Vector3 targetUP;
    #endregion

    // Realization of IReceiveEntity 
    public void SetReceivedEntity(Entity entity)
    {
        Target = entity;
    }

    private void Update()
    {
        if (UseUpdate)
        {
            UpdateAll();
        }
    }
    private void FixedUpdate()
    {
        if (UseFixedUpdate)
        {
            UpdateAll();
        }
    }
    private void LateUpdate()
    {
        if (UseLateUpdate)
        {
            UpdateAll();
        }
    }
    private void UpdateAll()
    {
        if (Target == null)
        {
            Debug.Log(this.name + "have no target to track!");
        }
        else if (em.HasComponent<LocalToWorld>(Target))
        {
            TargetPosition = em.GetComponentData<LocalToWorld>(Target).Position;
            TargetRotation = em.GetComponentData<LocalToWorld>(Target).Rotation;
            TargetForward = em.GetComponentData<LocalToWorld>(Target).Forward;
            TargetRight = em.GetComponentData<LocalToWorld>(Target).Right;
            TargetUP = em.GetComponentData<LocalToWorld>(Target).Up;

            if (UsePreditionByVelocity)
            {
                TargetPosition += allVelocitiesInHierarchy(Target)*Time.fixedDeltaTime;
            }
        }
        else 
        {
            Debug.Log("Target entity have no LocalToWorld component!");
        }
    }

    // Recursive search for Parents entities which have PhysicsVelocity component to predict and compensate difference
    private Vector3 allVelocitiesInHierarchy(Entity target)
    {
        Vector3 myOffset = Vector3.zero;
        if (em.HasComponent<PhysicsVelocity>(target))
        {
            myOffset += (Vector3)em.GetComponentData<PhysicsVelocity>(target).Linear;
        }
        if (em.HasComponent<Parent>(target))
        {
            myOffset += allVelocitiesInHierarchy(em.GetComponentData<Parent>(target).Value);
        }
        return myOffset;
    }

    void Awake()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
}


