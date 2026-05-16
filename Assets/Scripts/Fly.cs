using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    [Header("References")]
    public Transform cameraRig;       // OVRCameraRig
    public Transform centerEyeAnchor; // CenterEyeAnchor
    public CharacterController characterController; // Character Controller on OVRCameraRig

    [Header("Flying Settings")]
    public float maxSpeed = 3.0f;
    public float deadZone = 0.15f;

    void Update()
    {
        // Read right controller thumbstick
        Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // Ignore tiny accidental input
        if (thumbstick.magnitude < deadZone)
        {
            return;
        }

        // Get the direction the user is currently looking
        Vector3 forward = centerEyeAnchor.forward;
        Vector3 right = centerEyeAnchor.right;

        // Keep movement horizontal only
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Thumbstick y = forward/backward
        // Thumbstick x = left/right
        Vector3 moveDirection = forward * thumbstick.y + right * thumbstick.x;

        // Clamp so diagonal movement is not too fast
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Speed proportional to thumbstick tilt amount
        float speed = maxSpeed * thumbstick.magnitude;
        Vector3 movement = moveDirection * speed * Time.deltaTime;

        // Move with collision if CharacterController is assigned
        if (characterController != null)
        {
            characterController.Move(movement);
        }
        else
        {
            // Fallback: no collision
            cameraRig.position += movement;
        }
    }
}