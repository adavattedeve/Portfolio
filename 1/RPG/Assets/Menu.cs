using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public void StartLevel(){
		GameManager.instance.LoadLevel ();
	}
	public void ToMainMenu(){
		GameManager.instance.MainMenu ();
	}
	public void QuitGame(){
		GameManager.instance.QuitGame ();
	}
}
