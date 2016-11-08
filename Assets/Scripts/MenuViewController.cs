using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuViewController : MonoBehaviour {

    public GameObject MainPanel;
    public GameObject DifficultyPanel;
    public GameObject StartPanel;

    public InputField seedInput;

    public float difficulty = 0.2f;

    private GameViewController GameViewController;

    public void Awake() {
        GameViewController = FindObjectOfType<GameViewController>();
    }

    public void ShowMainPanel() {
        MainPanel.SetActive(true);
        DifficultyPanel.SetActive(false);
        StartPanel.SetActive(false);
    }

    public void ShowDifficultyPanel() {
        MainPanel.SetActive(false);
        DifficultyPanel.SetActive(true);
        StartPanel.SetActive(false);
    }

    public void ShowStartPanel() {
        MainPanel.SetActive(false);
        DifficultyPanel.SetActive(false);
        StartPanel.SetActive(true);
    }

    public void SetEasyDifficulty() {
        difficulty = 0.2f;
        ShowStartPanel();
    }

    public void SetMediumDifficulty() {
        difficulty = 0.4f;
        ShowStartPanel();
    }

    public void SetHardDifficulty() {
        difficulty = 0.7f;
        ShowStartPanel();
    }

    public void StartGame() {
        int seed = 0;
        if (!String.IsNullOrEmpty(seedInput.text)) {
            seed = int.Parse(seedInput.text);
        }
        GameViewController.SetTrackParams(difficulty, seed);
        SceneManager.LoadScene("Main");
    }

    public void QuitApplication() {
        Application.Quit();
    }
}
