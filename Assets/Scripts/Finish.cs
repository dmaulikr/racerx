using System;
using UnityEngine;

public class Finish : MonoBehaviour {

    public bool passed = false;
    void OnTriggerEnter(Collider other) {
        CarController carController = other.transform.parent.gameObject.GetComponent<CarController>();
        if(carController != null && carController.lastCheckpoint.checkpointIndex == 0 && !passed) {
            this.passed = true;
            GameViewController gameViewController = GameViewController.Instance;
            if (gameViewController != null) {
                gameViewController.FinishGame();
            }
        }
    }
}
