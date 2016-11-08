using System;
using UnityEngine;

/*
 * Based on: 
 *      https://docs.unity3d.com/540/Documentation/Manual/WheelColliderTutorial.html 
 */
public class Checkpoint : MonoBehaviour {

    public int checkpointIndex = 0;

    void OnTriggerEnter(Collider other) {
        CarController carController = other.transform.parent.gameObject.GetComponent<CarController>();
        if(carController != null) {
            carController.lastCheckpoint = checkpointIndex;
            this.enabled = false;
        }
    }
}
