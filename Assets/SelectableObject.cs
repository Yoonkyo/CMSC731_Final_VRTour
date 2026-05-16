// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class SelectableObject : MonoBehaviour
// {

//     public GameObject controller;
//     public GameObject obj;

//     void Update() {
//         if (controller.GetComponent<ControllerManager>().GetButtonPress() 
//         && controller.GetComponent<ControllerManager>().selectedObject == obj) {
//             Highlight();
//         }
//     }

//     void Highlight() {
//         var objRenderer = obj.GetComponent<Renderer>();

//         if (objRenderer.material.GetColor("_Color") == Color.white) {
//             objRenderer.material.SetColor("_Color", Color.red);
//         } else {
//             objRenderer.material.SetColor("_Color", Color.white);
//         }
//     }

// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public GameObject controller; // The controller to be assigned in the Inspector
    public GameObject obj; // The object to highlight

    private ControllerManager controllerManager; // Caching the ControllerManager component

    void Start() {
        // Cache the ControllerManager component on start to avoid repeated calls to GetComponent
        controllerManager = controller.GetComponent<ControllerManager>();
    }

    void Update() {
        // Check if the button is pressed and the selected object matches the current object
        if (controllerManager.GetButtonPress() && controllerManager.selectedObject == obj) {
            Highlight(); // Call the Highlight method if conditions are met
        }
    }

    void Highlight() {
        // Get the Renderer component of the object to change its material color
        var objRenderer = obj.GetComponent<Renderer>();

        // Check the current color of the object and toggle between white and red
        if (objRenderer.material.color == Color.white) { // Use material.color instead of GetColor("_Color")
            objRenderer.material.color = Color.red; // Change the color to red
        } else {
            objRenderer.material.color = Color.white; // Change the color back to white
        }
    }
}