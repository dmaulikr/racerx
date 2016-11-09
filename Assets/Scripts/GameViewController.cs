using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameViewController: MonoBehaviour {

    private static GameViewController instance;

    public static GameViewController Instance { get { return instance; } }

    public bool playing = false;
    public float timeMillis = 0f;
    public TrackController trackController;

    public GameObject InGamePanel;
    public Text time;
    public Text checkpoints;

    public CarController Car;
    public float difficulty = 0f;
    public int seed = 0;

    public int lastCheckpoint = 0;

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
            updateTimeDisplay();
            if(Input.GetKeyDown(KeyCode.R)) {
                RestartCarPosition();
            }
        }
    }

    private void updateTimeDisplay() {
        string minutes = Mathf.FloorToInt((timeMillis / 1000) / 60).ToString();
        string seconds = Mathf.FloorToInt((timeMillis / 1000) % 60).ToString();
        string millis = Mathf.FloorToInt(timeMillis % 60).ToString();
        time.text = minutes + ":" + ((seconds.Length == 1) ? "0" + seconds : seconds) + ":" + ((millis.Length == 1) ? "0" + millis : millis);
    }

    public void SetCheckpoint(int current) {
        checkpoints.text = (lastCheckpoint - current + 1).ToString() + "/" + (lastCheckpoint+2).ToString();
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
            lastCheckpoint = trackController.lastCheckpoint;
            Instantiate(Car);
            playing = true;
            InGamePanel.SetActive(true);
            SetCheckpoint(lastCheckpoint+1);
        }
    }
    
    public void FinishGame() {

    }

    public void RestartCarPosition() {
        CarController oldCar = FindObjectOfType<CarController>();
        if (oldCar != null) {
            //TODO Fix Car direction
            CarController newCar;
            if(oldCar.lastCheckpoint != null) {
                newCar = Instantiate(Car, oldCar.lastCheckpoint.transform.position, Quaternion.identity) as CarController;
                newCar.gameObject.transform.rotation = oldCar.lastCheckpoint.transform.parent.localRotation;
            } else {
                newCar = Instantiate(Car, Vector3.zero, Quaternion.identity) as CarController;
            }
            newCar.lastCheckpoint = oldCar.lastCheckpoint;
            Destroy(oldCar.gameObject);
        }
    }

}
