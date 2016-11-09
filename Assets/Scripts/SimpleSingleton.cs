using System;
using UnityEngine;

public class SimpleSingleton : MonoBehaviour {

    private static SimpleSingleton instance;

    public static SimpleSingleton Instance {
        get {
            return instance;
        }
    }

    void Awake() {
        if(instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }
}
