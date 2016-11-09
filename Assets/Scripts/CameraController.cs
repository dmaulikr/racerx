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
        if(Input.GetKeyDown(KeyCode.Z)) {
            if(camera1 == null || camera2 == null) {
                Camera[] cameras = FindObjectsOfType<Camera>();
                if (cameras.Length > 0) {
                    if (cameras[0].name == "Camera1") {
                        camera1 = cameras[0];
                        camera2 = cameras[1];
                    } else {
                        camera2 = cameras[0];
                        camera1 = cameras[1];
                    }
                }
            }
            if (camera2.enabled) {
                camera1.enabled = true;
                camera2.enabled = false;
            } else {
                camera1.enabled = false;
                camera2.enabled = true;
            }
        }
    }
}
