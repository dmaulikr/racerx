using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameViewController: MonoBehaviour {

    private static GameViewController instance;

    public static GameViewController Instance { get {
            return instance;
    } }

    public bool playing = false;
    public float timeMillis = 0f;
    public TrackController trackController;

    public GameObject InGamePanel;
    public GameObject WinPanel;
    public Text time;
    public Text checkpoints;
    public Text bestScore;

    public CarController Car;
    public float difficulty = 0f;
    public int seed = 0;

    public int lastCheckpoint = 0;

    private bool win = false;
    private bool inGame = false;
    private int currentCheckpoint = 0;
    private RacingTrack currentTrack;

    void Awake() {
        if(instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
            trackController = FindObjectOfType<TrackController>();
            SceneManager.activeSceneChanged += StartGame;
            DontDestroyOnLoad(this);
        }
    }

    public void Update() {
        if (playing) {
            timeMillis += Time.deltaTime * 1000;
            updateTimeDisplay();
            if (Input.GetKeyDown(KeyCode.R)) {
                RestartCarPosition();
            }
            if (Input.GetKeyDown(KeyCode.Q)) {
                FinishGame();
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
        currentCheckpoint = current;
        checkpoints.text = (lastCheckpoint - current + 1).ToString() + "/" + (lastCheckpoint+2).ToString();
    }

    public void SetTrackParams(float difficulty, int seed) {
        this.difficulty = difficulty;
        this.seed = seed;
    }

    public void StartGame(Scene old, Scene current) {
        if (current.name == "Main" && !inGame) {
            inGame = true;
            if (trackController == null) {
                trackController = FindObjectOfType<TrackController>();
            }
            trackController.difficulty = difficulty;
            trackController.seed = seed != 0 ? seed : UnityEngine.Random.Range(1, 100000);
            currentTrack = trackController.GenerateTrack();
            lastCheckpoint = trackController.lastCheckpoint;
            Instantiate(Car);
            playing = true;
            InGamePanel.SetActive(true);
            SetCheckpoint(lastCheckpoint+1);
            SetScore();
        }
    }

    public void SetScore() {
        Score score = ScoreController.Instance.GetScore(seed, difficulty);
        if (score == null) {
            bestScore.text = "None";
        } else {
            string checkpointScore = score.Checkpoints + "/" + (lastCheckpoint + 2).ToString();
            string minutes = Mathf.FloorToInt((score.Time / 1000) / 60).ToString();
            string seconds = Mathf.FloorToInt((score.Time / 1000) % 60).ToString();
            string millis = Mathf.FloorToInt(score.Time % 60).ToString();
            string timeScore = minutes + ":" + ((seconds.Length == 1) ? "0" + seconds : seconds) + ":" + ((millis.Length == 1) ? "0" + millis : millis);
            bestScore.text = checkpointScore + " " + timeScore;
        }
    }

    public void WinGame() {
        win = true;
        FinishGame();
    }
    
    public void FinishGame() {
        playing = false;
        InGamePanel.SetActive(false);
        if (win) {
            ScoreController.Instance.SetScore(seed, trackController.difficulty, (lastCheckpoint - currentCheckpoint + 2), timeMillis);
            win = false;
            WinPanel.SetActive(true);
        } else {
            ScoreController.Instance.SetScore(seed, trackController.difficulty, (lastCheckpoint - currentCheckpoint + 1), timeMillis);
            BackToMenu();
        }
    }

    public void BackToMenu() {
        WinPanel.SetActive(false);
        inGame = false;
        SceneManager.LoadScene("Menu");
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
