using System;
using UnityEngine;

/*
 * Based on: 
 *      https://docs.unity3d.com/540/Documentation/Manual/WheelColliderTutorial.html 
 */
public class CameraController : MonoBehaviour {
    public Camera camera1;
    public Camera camera2;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            if (camera1.enabled) {
                camera1.enabled = false;
                camera2.enabled = true;
            } else {
                camera1.enabled = true;
                camera2.enabled = false;
            }
        }
    }
}
