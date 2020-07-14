using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineFlameMover : MonoBehaviour
{
    [SerializeField]
    private EntityTracker tracker;
    
    [SerializeField]
    [Range (-5f, 5f)]
    private float positionOffset = -1f;

    void Start()
    {
        if (tracker == null)
        {
            if (TryGetComponent<EntityTracker>(out tracker) == false)
            {
                Debug.LogError($"No tracker assigned or found on the GameObject {gameObject.name}");
            }
        }
    }

    void LateUpdate()
    {
        transform.position = tracker.TargetPosition - tracker.TargetForward * positionOffset;
        transform.rotation = tracker.TargetRotation;
    }
}
