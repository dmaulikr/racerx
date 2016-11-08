using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameViewController: MonoBehaviour {

    private static GameViewController instance;

    public static GameViewController Instance { get { return instance; } }

    public bool playing = false;
    public float timeMillis = 0f;
    public TrackController trackController;

    public CarController Car;
    public float difficulty = 0f;
    public int seed = 0;

    void Awake() {
        if(instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
        trackController = FindObjectOfType<TrackController>();
        SceneManager.activeSceneChanged += StartGame;
        DontDestroyOnLoad(this);
    }

    public void Update() {
        if (playing) {
            timeMillis += Time.deltaTime * 1000;
        }
    }

    public void SetTrackParams(float difficulty, int seed) {
        this.difficulty = difficulty;
        this.seed = seed;
    }

    public void StartGame(Scene old, Scene current) {
        if (current.name == "Main") {
            if (trackController == null) {
                trackController = FindObjectOfType<TrackController>();
            }
            trackController.difficulty = difficulty;
            trackController.seed = seed != 0 ? seed : UnityEngine.Random.Range(1, 100000);
            trackController.GenerateTrack();
            Instantiate(Car);
        }
    }
    
    public void FinishGame() {

    }


}
