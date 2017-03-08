using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeFont : MonoBehaviour {
	public Text text;
	void Awake(){
		text.font = DataBase.instance.gameData.font;
	}
	void OnEnable(){
		text.color = DataBase.instance.gameData.textColorInActive;
	}
}
