using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class CreateNewGameUI : MonoBehaviour {
	public Button createNewButton;
	public Button nextButton;
	public Button prevButton;
	public Image heroIcon;
	public Text inputFieldText;
	public Transform heroPos;
	private List<GameObject> heroModels;

	private int currentHeroIndex;
	void Awake(){
		currentHeroIndex = 0;

		RefreshText ("");
		createNewButton.onClick.AddListener (delegate{ChooseHero();});
		//createNewButton.onClick.AddListener (delegate{GameManager.instance.CreateNewSave(hero);});
		nextButton.onClick.AddListener (delegate{NextHero();});
		prevButton.onClick.AddListener (delegate{PrevHero();});
		heroModels = new List<GameObject> ();
		for (int i=0; i<DataBase.instance.startingTroops.Length; ++i) {
			heroModels.Add((GameObject)Resources.Load<GameObject>(DataBase.instance.startingTroops[i].hero.prefabPath));
			if (heroModels[i]!=null){
				heroModels[i] = Instantiate(heroModels[i], heroPos.position, heroPos.rotation) as GameObject;
				heroModels[i].transform.parent = heroPos;
				heroModels[i].SetActive(false);
			}
		}
		Refresh ();
	}
	public void NextHero(){
		++currentHeroIndex;
		Refresh ();

	}
	public void PrevHero(){
		--currentHeroIndex;
		Refresh ();
	}

	public void Refresh(){
		if (currentHeroIndex <= 0) {
			prevButton.interactable = false;
		} else {
			prevButton.interactable = true;
		}
		if (currentHeroIndex >= DataBase.instance.startingTroops.Length-1) {
			nextButton.interactable=false;
		}else{
			nextButton.interactable=true;
		}
		heroIcon.sprite = DataBase.instance.startingTroops[currentHeroIndex].hero.Icon;
		ActiveHeroModel ();
	}
	public void RefreshText(string text){
		if (text.Length >= GameManager.HEROES_NAME_MIN_CHAR) {
			createNewButton.interactable=true;
		}else{
			createNewButton.interactable=false;
		}
	}
	private void ActiveHeroModel(){
		for (int i=0; i<DataBase.instance.startingTroops.Length; ++i) {
			if (heroModels[i]!=null ){
				if (currentHeroIndex!=i){
					heroModels[i].SetActive(false);
				}else {
					heroModels[i].SetActive(true);
				}
			}
		}
	}
	public void ChooseHero(){
		Troop troop = DataBase.instance.startingTroops[currentHeroIndex];
		troop.hero.name = inputFieldText.text;
		GameManager.instance.CreateNewSave(troop);
	}
}
