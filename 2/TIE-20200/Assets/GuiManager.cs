using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GuiManager : MonoBehaviour {
	public static GuiManager instance;


	public GameObject entityPanelPrefab;
	private EntityInfoPanelUI[] entityPanelUIs;

	public GameObject rewardPanelPrefab;
	private GameObject rewardPanelInstance;
	public GameObject randomEventPrefab;
	private GameObject randomEventInstance;

	public GameObject spellBookPrefab;
	private GameObject spellBookObject; 
	public GameObject SpellBookObject{ get {
			if (spellBookObject == null) {
				spellBookObject = Instantiate (spellBookPrefab);
				RectTransform rect = spellBookObject.GetComponent<RectTransform> ();
				rect.SetParent (CanvasUI.GetComponent<RectTransform> (), false);
			}
			return spellBookObject;
		} }
	public string statIconPath;

	private GameObject canvas;
	public GameObject CanvasUI{get{
			if (canvas==null){
				canvas= GameObject.Find("Canvas");
				return canvas;
			}
			return canvas;
		}
	}
	public struct SelectedUnit{
		public int index;
		public TroopLocation location;
	}
	public SelectedUnit selectedUnit;

	public delegate void SelectedUnitChange();
	public static event SelectedUnitChange OnSelectedUnitChange;

	void Awake(){
		if (instance == null) {
			instance = this;
		}
		entityPanelUIs = new EntityInfoPanelUI[2];
		selectedUnit = new SelectedUnit ();
		selectedUnit.location = TroopLocation.NULL;

	}
	public void ShowQuestReward(){
		if (rewardPanelInstance == null) {
			rewardPanelInstance = Instantiate (rewardPanelPrefab) as GameObject;
			RectTransform rect = rewardPanelInstance.GetComponent<RectTransform> ();
			rect.SetParent (CanvasUI.GetComponent<RectTransform> ());
			rect.offsetMax = Vector2.zero;
			rect.offsetMin = Vector2.zero;
			rect.localScale=Vector3.one;

		} else {
			rewardPanelInstance.SetActive(true);
		}
	}
	public void ShowRandomEvent(){
		if (randomEventInstance == null) {
			randomEventInstance = Instantiate (randomEventPrefab) as GameObject;
			RectTransform rect = randomEventInstance.GetComponent<RectTransform> ();
			rect.SetParent (CanvasUI.GetComponent<RectTransform> ());
			rect.offsetMax = Vector2.zero;
			rect.offsetMin = Vector2.zero;
			rect.localScale=Vector3.one;
		} else {
			randomEventInstance.SetActive(true);
		}
	}
	public void ShowEntityInfo(Entity entity, Vector2 position){
		Debug.Log ("guimanager show entity");
		if (entityPanelUIs[0]==null) {
			entityPanelUIs[0] = (Instantiate (entityPanelPrefab) as GameObject).GetComponent<EntityInfoPanelUI> ();
			entityPanelUIs[0].GetComponent<RectTransform>().SetParent(CanvasUI.GetComponent<RectTransform>());
			entityPanelUIs[0].gameObject.SetActive(false);

			entityPanelUIs[1] = (Instantiate (entityPanelPrefab) as GameObject).GetComponent<EntityInfoPanelUI> ();
			entityPanelUIs[1].GetComponent<RectTransform>().SetParent(CanvasUI.GetComponent<RectTransform>());
			entityPanelUIs[1].gameObject.SetActive(false);
		}
		if (!entityPanelUIs [0].gameObject.activeSelf) {
			entityPanelUIs [0].gameObject.SetActive (true);
			entityPanelUIs [0].DisplayEntityInfo (entity, position);
		} else {
			entityPanelUIs [1].gameObject.SetActive (true);
			entityPanelUIs [1].DisplayEntityInfo (entity, position);
		}

	}
	public void CloseEntityInfo(){
		if (entityPanelUIs[0]==null) {
			entityPanelUIs[0] = (Instantiate (entityPanelPrefab) as GameObject).GetComponent<EntityInfoPanelUI> ();
			entityPanelUIs[0].GetComponent<RectTransform>().SetParent(CanvasUI.GetComponent<RectTransform>());
			entityPanelUIs[0].gameObject.SetActive(false);
			
			entityPanelUIs[1] = (Instantiate (entityPanelPrefab) as GameObject).GetComponent<EntityInfoPanelUI> ();
			entityPanelUIs[1].GetComponent<RectTransform>().SetParent(CanvasUI.GetComponent<RectTransform>());
			entityPanelUIs[1].gameObject.SetActive(false);
		}
		if (entityPanelUIs [1].gameObject.activeSelf) {
			entityPanelUIs [1].gameObject.SetActive(false);
			return;
		}
		entityPanelUIs [0].gameObject.SetActive(false);
	}
	public void SelectUnit(int index, TroopLocation location, bool isNullUnit){
		if (selectedUnit.location == TroopLocation.NULL && !isNullUnit) {
			selectedUnit.index = index;
			selectedUnit.location = location;

		} else if (location != TroopLocation.NULL && selectedUnit.location != TroopLocation.NULL) {
			TroopLocation temp = selectedUnit.location;
			selectedUnit.location = TroopLocation.NULL;
			GameManager.instance.TransferUnits (temp, selectedUnit.index, location, index);
		} else {
			selectedUnit.location=location;
			selectedUnit.index=index;
		}
		if (OnSelectedUnitChange != null) {
			OnSelectedUnitChange();
		}

	}
	void Update(){
		if (Input.GetMouseButtonUp (1)) {
			if (entityPanelUIs[0]!=null){
				CloseEntityInfo();
				CloseEntityInfo();
			}
		}
	}
}
