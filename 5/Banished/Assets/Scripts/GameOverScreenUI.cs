using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverScreenUI : MonoBehaviour {
    public Text sessionInfo;
    public Button playAgainButton;
    public Button exitToMainMenuButton;

    void Start()
    {
        playAgainButton.onClick.AddListener(delegate { GameManager.instance.StartGame(); });
        exitToMainMenuButton.onClick.AddListener(delegate { GameManager.instance.ToMainMenu(); });
        gameObject.SetActive(false);
        GameManager.OnGameOver += DisplayGameover;
    }
    
    public void DisplayGameover()
    {
        gameObject.SetActive(true);
        int oldHighScore = PlayerPrefs.GetInt("HighScore");
        int currentScore = GameManager.instance.score;
        sessionInfo.text = "Total score: " + (GameManager.instance.score) + "\n" + "Bigges combo: " + GameManager.instance.longestCombo + "\n" + "HighScore: " + oldHighScore;
        if (oldHighScore < currentScore)
        {
            sessionInfo.text += "\n" + "New HighScore!!  " + currentScore;
        }
    }
}
