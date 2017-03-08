using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExitbuttonUI : MonoBehaviour {
    public Button exitGameButton;

    private bool isPaused;
    // Use this for initialization
    void Start()
    {
        exitGameButton.onClick.AddListener(delegate { GameManager.instance.ToMainMenu(); });
    }

}
