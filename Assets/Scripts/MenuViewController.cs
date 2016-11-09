using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuViewController : MonoBehaviour {

    private static MenuViewController instance;

    public static MenuViewController Instance {
        get {
            return instance;
        }
    }

    public GameObject MainPanel;
    public GameObject DifficultyPanel;
    public GameObject StartPanel;
    public GameObject Logo;

    public InputField seedInput;

    public float difficulty = 0.2f;

    void Awake() {
        if(instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
            SceneManager.activeSceneChanged += StartMenu;
        }
    }

    public void StartMenu(Scene old, Scene current) {
        if(current.name == "Menu") {
            ShowMainPanel();
        }
    }

    public void ShowMainPanel() {
        Logo.SetActive(true);
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
        difficulty = 0.1f;
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
        MainPanel.SetActive(false);
        DifficultyPanel.SetActive(false);
        StartPanel.SetActive(false);
        Logo.SetActive(false);
        int seed = 0;
        if (!String.IsNullOrEmpty(seedInput.text)) {
            seed = int.Parse(seedInput.text);
        }
        GameViewController.Instance.SetTrackParams(difficulty, seed);
        SceneManager.LoadScene("Main");
    }

    public void QuitApplication() {
        Application.Quit();
    }
}
