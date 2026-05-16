using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    private enum TeleportState
    {
        Idle,
        SelectingDestination,
        SelectingOrientation
    }

    [Header("References")]
    public Transform cameraRig;          // OVRCameraRig
    public Transform controllerAnchor;   // RightControllerAnchor
    public Transform centerEyeAnchor;    // CenterEyeAnchor
    public LineRenderer teleportRay;     // Ray visualizer
    public GameObject destinationMarker; // Destination visualizer
    public GameObject orientationArrow;  // Orientation visualizer

    [Header("Teleport Settings")]
    public float maxRayDistance = 20.0f;
    public float arrowDistance = 0.5f;
    public float arrowHeight = 0.08f;
    public float rotationSpeed = 120.0f;
    public LayerMask teleportLayerMask = ~0;

    private TeleportState state = TeleportState.Idle;

    private bool hasValidDestination = false;
    private Vector3 currentHitPoint;
    private Vector3 selectedDestination;
    private float selectedYaw;

    void Start()
    {
        if (teleportRay != null)
        {
            teleportRay.positionCount = 2;
            teleportRay.enabled = false;
        }

        if (destinationMarker != null)
        {
            destinationMarker.SetActive(false);
        }

        if (orientationArrow != null)
        {
            orientationArrow.SetActive(false);
        }
    }

    void Update()
    {
        switch (state)
        {
            case TeleportState.Idle:
                HandleIdle();
                break;

            case TeleportState.SelectingDestination:
                HandleSelectingDestination();
                break;

            case TeleportState.SelectingOrientation:
                HandleSelectingOrientation();
                break;
        }
    }

    private void HandleIdle()
    {
        // Start destination selection while holding the right index trigger
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            state = TeleportState.SelectingDestination;

            if (teleportRay != null)
            {
                teleportRay.enabled = true;
            }
        }
    }

    private void HandleSelectingDestination()
    {
        CastTeleportRay();

        // Confirm destination when the right index trigger is released
        if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
        {
            if (hasValidDestination)
            {
                selectedDestination = currentHitPoint;

                // Start orientation from the user's current viewing direction
                Vector3 flatForward = centerEyeAnchor.forward;
                flatForward.y = 0f;

                if (flatForward.sqrMagnitude < 0.001f)
                {
                    flatForward = cameraRig.forward;
                    flatForward.y = 0f;
                }

                flatForward.Normalize();
                selectedYaw = Quaternion.LookRotation(flatForward, Vector3.up).eulerAngles.y;

                state = TeleportState.SelectingOrientation;

                if (teleportRay != null)
                {
                    teleportRay.enabled = false;
                }

                if (destinationMarker != null)
                {
                    destinationMarker.SetActive(true);
                    destinationMarker.transform.position = selectedDestination + Vector3.up * 0.02f;
                }

                if (orientationArrow != null)
                {
                    orientationArrow.SetActive(true);
                }

                UpdateOrientationArrow();
            }
            else
            {
                ResetTeleport();
            }
        }
    }

    private void CastTeleportRay()
    {
        hasValidDestination = false;

        Vector3 rayOrigin = controllerAnchor.position;
        Vector3 rayDirection = controllerAnchor.forward;

        RaycastHit hit;

        Vector3 rayEnd = rayOrigin + rayDirection * maxRayDistance;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxRayDistance, teleportLayerMask))
        {
            hasValidDestination = true;
            currentHitPoint = hit.point;
            rayEnd = hit.point;

            if (destinationMarker != null)
            {
                destinationMarker.SetActive(true);
                destinationMarker.transform.position = hit.point + Vector3.up * 0.02f;
            }
        }
        else
        {
            if (destinationMarker != null)
            {
                destinationMarker.SetActive(false);
            }
        }

        if (teleportRay != null)
        {
            teleportRay.SetPosition(0, rayOrigin);
            teleportRay.SetPosition(1, rayEnd);
        }
    }

    private void HandleSelectingOrientation()
    {
        // Use right thumbstick left/right to rotate selected orientation
        Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        selectedYaw += thumbstick.x * rotationSpeed * Time.deltaTime;

        UpdateOrientationArrow();

        // Press A button to execute teleport
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            ExecuteTeleport();
            ResetTeleport();
        }

        // Optional: press B button to cancel
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            ResetTeleport();
        }
    }

    private void UpdateOrientationArrow()
    {
        if (orientationArrow == null)
        {
            return;
        }

        Vector3 direction = Quaternion.Euler(0f, selectedYaw, 0f) * Vector3.forward;

        Vector3 arrowPosition = selectedDestination
                                + direction * arrowDistance
                                + Vector3.up * arrowHeight;

        orientationArrow.transform.position = arrowPosition;
        orientationArrow.transform.rotation = Quaternion.Euler(0f, selectedYaw, 0f);
    }

    private void ExecuteTeleport()
    {
        // Move the rig to the selected ground position while preserving current rig height
        Vector3 newRigPosition = cameraRig.position;
        newRigPosition.x = selectedDestination.x;
        newRigPosition.z = selectedDestination.z;
        cameraRig.position = newRigPosition;

        // Rotate only around Y axis
        cameraRig.rotation = Quaternion.Euler(0f, selectedYaw, 0f);
    }

    private void ResetTeleport()
    {
        state = TeleportState.Idle;
        hasValidDestination = false;

        if (teleportRay != null)
        {
            teleportRay.enabled = false;
        }

        if (destinationMarker != null)
        {
            destinationMarker.SetActive(false);
        }

        if (orientationArrow != null)
        {
            orientationArrow.SetActive(false);
        }
    }
}