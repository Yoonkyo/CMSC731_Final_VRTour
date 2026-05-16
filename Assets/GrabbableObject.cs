using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{

    public GameObject controller;
    bool triggerPress;
    bool objectMatch;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    void LateUpdate() {
        triggerPress = controller.GetComponent<ControllerManager>().GetTriggerPress();
        objectMatch = controller.GetComponent<ControllerManager>().grabbedObject == gameObject;

        if (triggerPress && objectMatch) 
        {
            Grab();
        } 
        else 
        {
            transform.parent = null;
            rb.isKinematic = true;
            rb.useGravity = false;
        } 
    }

    void Grab() {
        transform.parent = controller.transform;
        rb.isKinematic = true;
        rb.MovePosition(controller.transform.position);
        rb.MoveRotation(controller.transform.rotation);
    }
}