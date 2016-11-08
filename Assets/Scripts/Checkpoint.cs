using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    public int checkpointIndex = 0;

    void OnTriggerEnter(Collider other) {
        CarController carController = other.transform.parent.gameObject.GetComponent<CarController>();
        if(carController != null && (Mathf.Abs(carController.lastCheckpoint - checkpointIndex) == 1 || carController.lastCheckpoint == -1)) {
            carController.lastCheckpoint = checkpointIndex;
            this.enabled = false;
        }
    }
}
