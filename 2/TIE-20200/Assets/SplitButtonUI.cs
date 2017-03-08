using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SplitButtonUI : MonoBehaviour {
	public GameObject splitPanel;
	private Button button;
	void Awake(){
		button = GetComponent<Button> ();
		button.onClick.AddListener (delegate {
			bool splitActive = !splitPanel.activeSelf;
			splitPanel.SetActive(splitActive);
			if (splitActive){
				if (GuiManager.instance.selectedUnit.location == TroopLocation.BARRACKS){
					splitPanel.GetComponent<UnitSplitUI>().Initialize(GameManager.instance.CurrentGame.barracksUnits.units [GuiManager.instance.selectedUnit.index], GuiManager.instance.selectedUnit);

				}else if (GuiManager.instance.selectedUnit.location == TroopLocation.PLAYER){
					splitPanel.GetComponent<UnitSplitUI>().Initialize(GameManager.instance.CurrentGame.playerTroop.units [GuiManager.instance.selectedUnit.index], GuiManager.instance.selectedUnit);
				}
				//
			}

	});
	}
	void OnEnable(){
		GuiManager.OnSelectedUnitChange += RefreshInteractibility;
		RefreshInteractibility ();
	}
	void OnDisable(){
		GuiManager.OnSelectedUnitChange -= RefreshInteractibility;
	}
	public void RefreshInteractibility(){
		button.interactable = GuiManager.instance.selectedUnit.location != TroopLocation.NULL;
	}
}
