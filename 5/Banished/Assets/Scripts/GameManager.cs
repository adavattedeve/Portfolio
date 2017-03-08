using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public int priestsKilled;

    private GameObject player;
    public GameObject Player {
        get {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            return player;
        }
    }
    private Banishment playerBanishment;
    public Banishment PlayerBanishment
    {
        get
        {
            if (playerBanishment == null )
            {
                if (Player != null)
                {
                    playerBanishment = Player.GetComponent<Banishment>();
                }
                else
                {
                    Debug.Log("Error: Player is null");
                    return null;
                }
            }
            return playerBanishment;
        }
    }
    public int combo;
    public int score;
    public int longestCombo;
    public delegate void GameOverAction();
    public static event GameOverAction OnGameOver;

    public delegate void PriestDefeatedAction(int priestsDefeated);
    public static event PriestDefeatedAction OnPriestDefeated;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(gameObject);
        }
    }

    public void PriestDeath(int priestKillReward) {
        ++combo;
        if (combo > longestCombo)
        {
            longestCombo = combo;
        }
        ++score;
        ++priestsKilled;
        if (combo % 5 == 0)
        {
            score += combo;
        }
        if (PlayerBanishment != null)
        {
            PlayerBanishment.RecoverBanishmentValue(priestKillReward);
        }

        if (OnPriestDefeated != null)
        {
            OnPriestDefeated(priestsKilled);
        }
    }

    public void StartGame()
    {
        score = 0;
        longestCombo = 0;
        OnPriestDefeated = null;
        OnGameOver = null;
        SceneManager.LoadScene(1);
        priestsKilled = 0;
    }

    public void GameOver()
    {
        //save total score
        if (OnGameOver != null)
        {
            Debug.Log("gameover event");
            OnGameOver();
            OnGameOver = null;
        }
        if (PlayerPrefs.GetInt("HighScore", 0) < score)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        if (Player != null)
        {
            Destroy(Player);
        }
        Debug.Log("GameOver");
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void ExitGame()
    {

        Application.Quit();
    }
    public void PlayerTookDamage()
    {
        combo = 0;
    }
}
