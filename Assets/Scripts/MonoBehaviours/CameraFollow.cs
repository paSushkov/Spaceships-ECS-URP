using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.Mathematics;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private EntityTracker tracker;

    [SerializeField]
    private CameraMode cameraMode = CameraMode.ThirdPersonCamera;

    [SerializeField]
    private Vector3 TopDownPositionOffset = Vector3.zero;
    [SerializeField]
    private Vector3 ThirdPersonPositionOffset = Vector3.zero;
    [SerializeField]
    private Vector3 ThirdPersonLookAtOffset = Vector3.forward;

    private void Start()
    {
        if (tracker == null)
        { TryGetComponent<EntityTracker>(out tracker); }

       // SwitchCameraMode();
        UpdateCameraPosition();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchCameraMode();
        }
    }
    void LateUpdate()
    {
        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    private void UpdateCameraPosition()
    {
        if (tracker != null)
        {
            switch (cameraMode)
            {
                case (CameraMode.TopDownCamera):
                    {
                        transform.position = tracker.TargetPosition + TopDownPositionOffset;
                        break;
                    }
                case (CameraMode.ThirdPersonCamera):
                    {

                        transform.position = tracker.TargetPosition + ThirdPersonPositionOffset;
                        break;
                    }
            }
        }
        else
        {
            Debug.LogWarning(this.name + " in "+ this.gameObject.name + " have EntityTracker. It should be assogned manually or exist on the same GameObject!");
        }
    }

    private void SwitchCameraMode()
    {
        cameraMode = (CameraMode)((int)++cameraMode % (Enum.GetNames(typeof(CameraMode)).Length));
    }

    private void UpdateCameraRotation()
    {
        switch (cameraMode)
        {
            case (CameraMode.TopDownCamera):
                {
                    transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
                    break;
                }
            case (CameraMode.ThirdPersonCamera):
                {
                    transform.rotation = Quaternion.LookRotation(ThirdPersonLookAtOffset, Vector3.up);
                    break;
                }

        }
    }
}
public enum CameraMode { TopDownCamera = 0, ThirdPersonCamera = 1 };
