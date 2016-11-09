using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * Based on: 
 *      https://docs.unity3d.com/540/Documentation/Manual/WheelColliderTutorial.html 
 */
public class ScoreController : MonoBehaviour {

    int scoresAmount = 0;
    private Dictionary<String, Score> scores;

    private static ScoreController instance;

    public static ScoreController Instance {
        get {
            return instance;
        }
    }

    void Awake() {
        if(instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
            init();
        }
    }

    private void init() {
        scores = new Dictionary<string, Score>();
        scoresAmount = PlayerPrefs.GetInt("scoresAmount");
        for(int i = 0; i < scoresAmount; i++) {
            Score score = new Score();
            score.LevelSeed = PlayerPrefs.GetInt("seed" + i);
            score.LevelDifficulty = PlayerPrefs.GetFloat("difficulty" + i);
            score.Checkpoints = PlayerPrefs.GetInt("checkpoints" + i);
            score.Time = PlayerPrefs.GetFloat("time" + i);
            scores[score.LevelSeed + "-" + score.LevelDifficulty] = score;
        }
    }

    public Score GetScore(int seed, float difficulty) {
        string key = seed + "-" + difficulty;
        int checkpoints = PlayerPrefs.GetInt(key + "checkpoints", -1);
        if (checkpoints != -1) {
            Score score = new Score();
            score.LevelSeed = seed;
            score.LevelDifficulty = difficulty;
            score.Checkpoints = checkpoints;
            score.Time = PlayerPrefs.GetFloat(key + "time", 0);
            return score;
        }
        return null;
    }

    public void SetScore(int seed, float difficulty, int checkpoints, float time) {
        Score score = GetScore(seed, difficulty);
        bool scoreExists = score != null;
        if (!scoreExists || checkpoints > score.Checkpoints || (checkpoints == score.Checkpoints && time < score.Time)) {
            string key = seed + "-" + difficulty;
            PlayerPrefs.SetFloat(key + "time", time);
            PlayerPrefs.SetInt(key + "checkpoints", checkpoints);
        }
    }
}
