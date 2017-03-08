using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitSplitUI : MonoBehaviour {
	public Button splitButton;
	public Image unitLeftIcon;
	public Image unitRightIcon;

	public Text unitLeftAmount;
	public Text unitRightAmount;

	public Slider slider;
	private Unit unit;
	private GuiManager.SelectedUnit selectedUnit;
	void Awake(){
		splitButton.onClick.AddListener (delegate {

			Troop troop=null;
			if (selectedUnit.location==TroopLocation.BARRACKS){
				troop = GameManager.instance.CurrentGame.barracksUnits;
			}
			else if (selectedUnit.location==TroopLocation.PLAYER){
				troop = GameManager.instance.CurrentGame.playerTroop;
			}
			for (int i=0; i<troop.units.Count; ++i){
				if (troop.units[i]==null){
					troop.AddUnits(unit, (-1)*(int)slider.value, selectedUnit.index);
					Unit newUnit = unit.GetDublicate();
					newUnit.amount=(int)slider.value;
					troop.AddUnits(newUnit, newUnit.amount, i);
					break;
				}
			}
			GuiManager.instance.SelectUnit(-1, TroopLocation.NULL, true);
			gameObject.SetActive(false);
	});
	}
	public void Initialize(Unit _unit, GuiManager.SelectedUnit _selectedUnit){

		unit = _unit;
		selectedUnit = _selectedUnit;
		slider.maxValue = unit.amount;
		slider.value = 0;
		unitLeftIcon.sprite = unit.Icon;
		unitRightIcon.sprite = unit.Icon;

		unitLeftAmount.text = unit.amount.ToString();
		unitRightAmount.text = "0";
	}
	public void OnSliderValueChanged(float newValue){
		int value = (int)newValue;
		unitLeftAmount.text = (unit.amount-value).ToString();
		unitRightAmount.text = value.ToString();
	}
}
