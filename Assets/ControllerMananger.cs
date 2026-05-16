using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{

    public GameObject selectedObject = null;
    public GameObject hand;
    public GameObject grabbedObject;

    // extra credit controller viewer
    public GameObject controllerVisualizer;

    Vector3 GetPointingDir() {
        return hand.transform.forward;
    }

    Vector3 GetPosition() {
        return hand.transform.position;
    }

    public bool GetButtonPress() {
        if (OVRInput.GetUp(OVRInput.Button.One)) {
            return CastRay();
        }
        return false;
    }

    bool CastRay() {
        RaycastHit hit;

        if (Physics.Raycast(GetPosition(), GetPointingDir(), out hit, 100.0f)) {
            selectedObject = hit.transform.gameObject;
            return true;
        } else {
            selectedObject = null;
            return false;
        }
    }

    public bool GetTriggerPress() {
        // if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > .1) {
        if (OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger, OVRInput.Controller.Touch) > .1) {
                print("trigger down");
            return GrabObject();
        } else {
            grabbedObject = null;
            return false;
        }
    }

    bool GrabObject() {
        if (grabbedObject == null) {
            Collider[] grabbables = Physics.OverlapSphere(GetPosition(), 100);
            if (grabbables.Length > 0) {
                print("choosing object");
                float minDistance = float.MaxValue;
                var closestObj = grabbables[0];
                foreach (var hitCollider in grabbables) {
                    if (hitCollider.gameObject.GetComponent<GrabbableObject>() != null) {
                        Vector3 direction;

                        // float distance;
                        // Physics.ComputePenetration(hand.GetComponent<Collider>(), GetPosition(), hand.transform.rotation, hitCollider, hitCollider.transform.position, hitCollider.transform.rotation, out direction, out distance);

                        float distance = Vector3.Distance(GetPosition(), hitCollider.transform.position);

                        if (distance < minDistance) {
                            minDistance = distance;
                            closestObj = hitCollider;
                        }
                    }
                }
                grabbedObject = closestObj.gameObject;
                print("object assigned "+grabbedObject.ToString());
                return true;
            } else {
                print("nothing..");
                return false;
            }
        }
        return true;
    }

    // extra credit controller viewer
    void Update() {
        if (controllerVisualizer != null && hand != null) {
            controllerVisualizer.transform.position = hand.transform.position;
            controllerVisualizer.transform.rotation = hand.transform.rotation;
        }
    }

}