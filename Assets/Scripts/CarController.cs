using System;
using UnityEngine;

/*
 * Based on: 
 *      https://docs.unity3d.com/540/Documentation/Manual/WheelColliderTutorial.html 
 */
public class CarController : MonoBehaviour {

    [Serializable]
    public class AxleInfo {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
    }

    public AxleInfo[] axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public Checkpoint lastCheckpoint = null;

    public void ApplyLocalPositionToVisuals(WheelCollider collider) {
        if (collider.transform.childCount == 0) {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void FixedUpdate() {
        if (transform.position.y < -100) {
            GameViewController gameViewController = GameViewController.Instance;
            gameViewController.RestartCarPosition();
        }
        float motor = maxMotorTorque * -Input.GetAxisRaw("Vertical");
        float steering = maxSteeringAngle * Input.GetAxisRaw("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

}
