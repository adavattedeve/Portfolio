using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	void Awake () {
		if (!instance) {
			instance = this;
		} else {
			Destroy(gameObject);
		}
		DontDestroyOnLoad (gameObject);
	}

	public void LoadLevel(){
		Application.LoadLevel ("TestLevel");
	}
	public void MainMenu(){
		Application.LoadLevel ("MainMenu");
	}
	public void QuitGame(){
		Application.Quit ();
	}
}
