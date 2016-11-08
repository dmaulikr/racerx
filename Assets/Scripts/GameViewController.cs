using UnityEngine;
using System.Collections;

public class GameViewController: MonoBehaviour {

    private static GameViewController instance;

    public static GameViewController Instance { get { return instance; } }

    public bool playing = false;
    public float timeMillis = 0f;
    public TrackController trackController;

    void Awake() {
        if(instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
        trackController = FindObjectOfType<TrackController>();
        DontDestroyOnLoad(this);
    }

    public void Update() {
        if (playing) {
            timeMillis += Time.deltaTime * 1000;
        }
    }

    public void StartGame(float difficulty, int seed) {
        if (trackController == null) {
            trackController = FindObjectOfType<TrackController>();
        }
        trackController.difficulty = difficulty;
        trackController.seed = seed != 0 ? seed : UnityEngine.Random.Range(1, 100000);
        trackController.GenerateTrack();
    }

    public void FinishGame() {

    }


}
