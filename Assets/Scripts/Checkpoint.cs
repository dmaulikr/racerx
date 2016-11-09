using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    public int checkpointIndex = 0;
    public bool passed = false;

    void OnTriggerEnter(Collider other) {
        CarController carController = other.transform.parent.gameObject.GetComponent<CarController>();
        if(carController != null && !passed && (carController.lastCheckpoint == null || Mathf.Abs(carController.lastCheckpoint.checkpointIndex - checkpointIndex) == 1)) {
            carController.lastCheckpoint = this;
            passed = true;
            GameViewController gameViewController = FindObjectOfType<GameViewController>();
            if (gameViewController != null) {
                gameViewController.SetCheckpoint(checkpointIndex);
            }
        }
    }
}
