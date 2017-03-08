using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitRecruimentSelectionPanelUI : MonoBehaviour, IIndexable {
	private int index;
	public int Index {
		get{return index;}
		set{index=value;
			Refresh();}
	}
	public Text goldCostText;
	public InputField inputField;
	public void OnEnable(){
		GameManager.OnUnitsBought += Refresh;
		GameManager.instance.CurrentGame.recruitableUnits.OnTroopsChange += Refresh;
	}
	public void OnDisable(){
		GameManager.OnUnitsBought -= Refresh;
		GameManager.instance.CurrentGame.recruitableUnits.OnTroopsChange -= Refresh;
	}
	public void Refresh(){
		if (GameManager.instance.CurrentGame.recruitableUnits.units.Count > index &&
			GameManager.instance.CurrentGame.recruitableUnits.units [index] != null) {
			goldCostText.text = GameManager.instance.CurrentGame.recruitableUnits.units [index].goldValue.ToString ();
			inputField.interactable=true;
		} else {
			inputField.interactable=false;
			goldCostText.text="0";
		}
		inputField.text = "0";
	}
	public void OnInputFieldChange(string str){
		int result = 0;

		if (int.TryParse (str, out result)) {
			if (index<GameManager.instance.CurrentGame.recruitableUnits.units.Count){
				Unit unit = GameManager.instance.CurrentGame.recruitableUnits.units[index];
				int maxUnitsCanBeBought = GameManager.instance.CurrentGame.gold/unit.goldValue;
				if (maxUnitsCanBeBought>unit.amount){
					maxUnitsCanBeBought=unit.amount;
				}
				result = Mathf.Clamp(result, 0, maxUnitsCanBeBought);
				inputField.text = result.ToString();
				GameManager.instance.SelectToRecruit (index, result);
			}
		}
	}
}
