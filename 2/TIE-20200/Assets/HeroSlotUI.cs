using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class HeroSlotUI : MonoBehaviour, IPointerDownHandler  {

	public TroopLocation troopLocation;
	private bool empty;
	public bool Empty{set{
			if (value){
				button.interactable=false;
				childObjectsRoot.SetActive(false);
			}else{
				button.interactable=true;
				childObjectsRoot.SetActive(true);
			}empty=value;}}
	public GameObject childObjectsRoot;
	public Image heroIcon;
	private Button button;
	private Troop troop;
	void Awake(){
		button = GetComponent<Button> ();
		
	}
	void Start(){
		Refresh ();
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{
		if (!empty) {
			if (eventData.button == PointerEventData.InputButton.Left) {
			
			} else if (eventData.button == PointerEventData.InputButton.Right) {
				Troop troop = GetTroop ();
				if (troop.hero != null) {
					GuiManager.instance.ShowEntityInfo ((Entity)troop.hero, eventData.pressPosition);
				}
			}
		}
	}
	
	void OnEnable(){
		if (troopLocation != TroopLocation.NULL) {
			troop = GetTroop ();
			if (troopLocation== TroopLocation.QUEST){
				GameManager.OnQuestSelected += Refresh;
			}
			else if (troopLocation!=TroopLocation.CURRENTQUEST) {
				troop.OnTroopsChange +=Refresh;
			}else{
				Refresh();
			}
		}
	}
	void OnDisable(){
		if (troopLocation== TroopLocation.QUEST){
			GameManager.OnQuestSelected -= Refresh;
		}
		else if (troopLocation!=TroopLocation.CURRENTQUEST) {
			troop.OnTroopsChange -=Refresh;
		}
	}
	private void Refresh(){
		Troop troop = GetTroop ();
		if (troop != null) {
			if (troop.hero!=null) {
				Empty=false;
				heroIcon.sprite = troop.hero.Icon;
				return;	
			}
		}
		Empty = true;
	}
	
	private Troop GetTroop(){
		switch(troopLocation){
		case TroopLocation.BARRACKS:
			return GameManager.instance.CurrentGame.barracksUnits;
		case TroopLocation.QUEST:
			if (GameManager.instance.SelectedQuest >= 0 && GameManager.instance.CurrentGame.CurrentTown.currentQuests[GameManager.instance.SelectedQuest]!=null){
				return GameManager.instance.CurrentGame.CurrentTown.currentQuests[GameManager.instance.SelectedQuest].troop;
			}
			break;
		case TroopLocation.CURRENTQUEST:
			if (GameManager.instance.currentRandomEvent!=null){		
				return GameManager.instance.currentRandomEvent.troop;
			}else{
				return GameManager.instance.currentQuest.troop;
			}
		case TroopLocation.RECRUITABLE:
			return GameManager.instance.CurrentGame.recruitableUnits;
		case TroopLocation.PLAYER:
			return GameManager.instance.CurrentGame.playerTroop;
		}
		return null;
	}
}
