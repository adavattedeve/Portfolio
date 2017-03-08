using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad {
	
	public static List<GameState> savedGames = new List<GameState>();

	public delegate void SavedGamesChangeAction ();
	public static event SavedGamesChangeAction OnSavedGamesChange;


	public static void DeleteGame(int index){
		if (index == GameManager.instance.currenGameIndex) {
			GameManager.instance.CurrentGame=null;
			GameManager.instance.currenGameIndex = -1;
		}
		SaveLoad.savedGames.RemoveAt (index);
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/savedGames.gd"); 
		bf.Serialize(file, SaveLoad.savedGames);
		file.Close();

		if (OnSavedGamesChange!=null) {
			OnSavedGamesChange ();
		}
	}
	//it's static so we can call it from anywhere
	public static void Save(int saveSlotIndex=-1) {
		Debug.Log ("Saving current game");
		GameManager.instance.CurrentGame.PrepareForSaving();
		if (SaveLoad.savedGames.Count > saveSlotIndex && saveSlotIndex>=0) {
			SaveLoad.savedGames [saveSlotIndex] = GameManager.instance.CurrentGame;
		} else {
			SaveLoad.savedGames.Add(GameManager.instance.CurrentGame);
			GameManager.instance.currenGameIndex =SaveLoad.savedGames.Count-1;
		}
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/savedGames.gd"); 
		bf.Serialize(file, SaveLoad.savedGames);
		file.Close();
		if (OnSavedGamesChange!=null) {
			OnSavedGamesChange ();
		}
	}   
	
	public static void LoadSaveGames() {
		Debug.Log ("Loading saved games");
		if (File.Exists (Application.persistentDataPath + "/savedGames.gd")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
			SaveLoad.savedGames = (List<GameState>)bf.Deserialize (file);
			file.Close ();
		}
	}
	public static void LoadGame(int saveSlotIndex){
		if (SaveLoad.savedGames.Count > saveSlotIndex) {
			GameManager.instance.CurrentGame = SaveLoad.savedGames [saveSlotIndex];
			GameManager.instance.currenGameIndex = saveSlotIndex;
			GameManager.instance.CurrentGame.FinishLoading();
		} else {
			Debug.Log ("Save not found from index: "+saveSlotIndex);
		}
	}
}