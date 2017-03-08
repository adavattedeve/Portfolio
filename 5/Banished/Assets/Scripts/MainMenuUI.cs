using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUI : MonoBehaviour {
    public Button startGameButton;
    public Button exitGameButton;
	// Use this for initialization
	void Start () {
        startGameButton.onClick.AddListener(delegate { GameManager.instance.StartGame(); });
        exitGameButton.onClick.AddListener(delegate { GameManager.instance.ExitGame(); });
    }
	
}
