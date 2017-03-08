using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SavedGamesUI : MonoBehaviour {
//	private SavedGameUI[] savedGameUIs;
	private Dictionary<int, SavedGameUI> savedGameUIs;
	private int selectedSavedGame;
	//[System.NonSerialized]public int selectedSavedGame;
	public Button deleteButton;
	public Button loadButton;
	void Awake () {
		selectedSavedGame = -1;
		loadButton.interactable = false;
		deleteButton.interactable = false;
	}
	void Start(){
		SavedGameUI[] temp = GetComponentsInChildren<SavedGameUI> ();
		savedGameUIs = new Dictionary<int, SavedGameUI> ();
		Debug.Log (temp.Length);
		for (int i=0; i<temp.Length; ++i) {

			savedGameUIs[temp[i].Index] = temp[i];
		}
	}
	void OnEnable(){
		SaveLoad.OnSavedGamesChange += RefreshButtons;
		
	}
	void OnDisable(){
		SaveLoad.OnSavedGamesChange -= RefreshButtons;
	}
	public void RefreshButtons(){
		if (SaveLoad.savedGames.Count<=selectedSavedGame ||  selectedSavedGame<0 || SaveLoad.savedGames[selectedSavedGame]==null){
			selectedSavedGame=-1;
			loadButton.interactable = false;
			deleteButton.interactable = false;
		}
	}
	public void SelectSavedGame(int index){
		loadButton.interactable = true;
		deleteButton.interactable = true;
		if (selectedSavedGame>=0){
			savedGameUIs[selectedSavedGame].GetComponent<MouseOnButtonAnimation>().Selected=false;
		}
		savedGameUIs[index].GetComponent<MouseOnButtonAnimation>().Selected=true;
		selectedSavedGame = index;
		Debug.Log (savedGameUIs[selectedSavedGame].Index.ToString() + " selected");
	}
	public void DeleteSelected(){
		SaveLoad.DeleteGame (selectedSavedGame);
	}
	public void LoadSelectedGame(){
		GameManager.instance.LoadGame (selectedSavedGame);
	}
}
