using System;
using UnityEngine;

public class Finish : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        CarController carController = other.transform.parent.gameObject.GetComponent<CarController>();
        if(carController != null && carController.lastCheckpoint == 0) {
            //Finish routine
        }
    }
}
