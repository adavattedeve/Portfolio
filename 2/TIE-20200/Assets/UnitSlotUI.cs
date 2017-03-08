using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public enum TroopLocation{PLAYER, QUEST, RECRUITABLE, BARRACKS, CURRENTQUEST, NULL}
public class UnitSlotUI :MonoBehaviour, IIndexable, IUnitUI, IPointerDownHandler , IPointerUpHandler{
	

	private int index;
	public int Index{get{return index;}set{index=value;}}
	private TroopLocation troopLocation = TroopLocation.NULL;
	public TroopLocation TroopLocation{ set{
			troopLocation=value; 
			Troop troop = GetTroop ();
			if (troopLocation== TroopLocation.QUEST){
				GameManager.OnQuestSelected += Refresh;
			}
			else if (troopLocation!=TroopLocation.CURRENTQUEST) {
				troop.OnTroopsChange +=Refresh;
			}
			Refresh ();
		}
	}

	private bool empty;
	public bool Empty{set{
			if (value){
				childObjectsRoot.SetActive(false);
			}else{

				childObjectsRoot.SetActive(true);
			}empty=value;}}
	public GameObject childObjectsRoot;
	public Image unitIcon;
	public Text amountText;
	private Button button;
	private Troop troop;
	private MouseOnButtonAnimation buttonAnim;
	void Awake(){
		button = GetComponent<Button> ();
		buttonAnim = GetComponent<MouseOnButtonAnimation> ();
	}
	public void OnPointerDown(PointerEventData eventData)
	{

			if (eventData.button == PointerEventData.InputButton.Right && !empty) {
				Troop troop = GetTroop ();
				if (index < troop.units.Count && troop.units [index] != null) {
					GuiManager.instance.ShowEntityInfo ((Entity)troop.units [index], eventData.pressPosition);
				}
			}
	}
	public void OnPointerUp(PointerEventData eventData){
		if (eventData.button == PointerEventData.InputButton.Left) {
			if (troopLocation==TroopLocation.BARRACKS || troopLocation==TroopLocation.PLAYER){
				GuiManager.instance.SelectUnit(index, troopLocation, troop.units.Count<=index || troop.units[index]==null);
				buttonAnim.Selected=CheckIfSelected();
			}
		}
	}
	void OnEnable(){
		GuiManager.OnSelectedUnitChange +=Refresh;
		if (troopLocation != TroopLocation.NULL) {
			troop = GetTroop ();
			if (troopLocation== TroopLocation.QUEST){
				GameManager.OnQuestSelected += Refresh;
			}
			else if (troopLocation!=TroopLocation.CURRENTQUEST) {
				troop.OnTroopsChange +=Refresh;
			}
			else{

				Refresh();
			}
		}
	}
	void OnDisable(){
		GuiManager.OnSelectedUnitChange -=Refresh;
		if (troopLocation== TroopLocation.QUEST){
			GameManager.OnQuestSelected -= Refresh;
		}
		else if (troopLocation!=TroopLocation.CURRENTQUEST) {
					troop.OnTroopsChange -=Refresh;
		}
	}
	private bool CheckIfSelected(){
		if (GuiManager.instance.selectedUnit.index == index && GuiManager.instance.selectedUnit.location == troopLocation) {
			return true;
		}
		return false;
	}
	private void Refresh(){
		SetButtonInteractable(true);
		buttonAnim.Selected=CheckIfSelected();
		troop = GetTroop ();
		if (troop != null && index < troop.units.Count && troop.units[index]!=null) {
			Empty=false;
			amountText.text = troop.units[index].amount.ToString();
			unitIcon.sprite = troop.units[index].Icon;
			return;
						
		}
		Empty = true;
	}
	private void SetButtonInteractable(bool interactable){
		if (troopLocation == TroopLocation.QUEST) {
			button.interactable = false;
		} else if (troopLocation == TroopLocation.CURRENTQUEST) {
			button.interactable = false;
		} else if (troopLocation == TroopLocation.RECRUITABLE) {
			button.interactable = false;
		} else {
			button.interactable=interactable;
		}
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
