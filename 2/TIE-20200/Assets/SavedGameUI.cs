using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SavedGameUI : MonoBehaviour, IIndexable {
	private int index;
	public int Index{
		get{return index;}
		set{
			index=value;
			Initialize();
			}
	}
	public Text name;
	public Text date;
	public Image heroIcon;
	
	void OnEnable(){
		SaveLoad.OnSavedGamesChange += Initialize ;
	}
	void OnDisable(){
		SaveLoad.OnSavedGamesChange -= Initialize ;
	}
	private void Initialize(){
		Button button = GetComponent<Button> ();
		button.interactable=true;
		button.onClick.AddListener (delegate{GetComponentInParent<SavedGamesUI>().SelectSavedGame(index);});
		if (index < SaveLoad.savedGames.Count) {
			name.gameObject.SetActive(true);
			date.gameObject.SetActive(true);
			heroIcon.gameObject.SetActive(true);
			if (SaveLoad.savedGames [index].playerTroop.hero==null){
				Debug.Log ("hero Is null");
			}
			name.text = SaveLoad.savedGames [index].playerTroop.hero.name+ "\nlvl. " + SaveLoad.savedGames [index].playerTroop.hero.level;
			date.text = "month " + SaveLoad.savedGames [index].month + "  " + "week " + SaveLoad.savedGames [index].week + "  " + "day " + SaveLoad.savedGames [index].day;
			heroIcon.sprite = SaveLoad.savedGames [index].playerTroop.hero.Icon;
		} else {
			name.gameObject.SetActive(false);
			date.gameObject.SetActive(false);
			heroIcon.gameObject.SetActive(false);
			button.interactable=false;
		}

	}
}
