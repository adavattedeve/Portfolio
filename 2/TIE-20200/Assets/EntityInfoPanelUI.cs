using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EntityInfoPanelUI : MonoBehaviour {
	public float widthAnchorDelta;
	public float heightAnchorDelta;
	public Image icon;
	public Text nameText;
	private StatsPanelUI statsPanelUI;
	private AbilityPanelUI abilityPanelUI;
	public Text descriptionText;
	
	private RectTransform rect;
	void Awake () {
		rect = GetComponent<RectTransform> ();
		statsPanelUI = GetComponentInChildren<StatsPanelUI> ();
		abilityPanelUI = GetComponentInChildren<AbilityPanelUI> ();
		abilityPanelUI.gameObject.SetActive (false);
		statsPanelUI.gameObject.SetActive (false);
		descriptionText.gameObject.SetActive (false);
	}
	void OnDisable(){
		abilityPanelUI.gameObject.SetActive (false);
		statsPanelUI.gameObject.SetActive (false);
		descriptionText.gameObject.SetActive (false);
	}
	public void DisplayEntityInfo(Entity entity, Vector2 position){
		if (entity is Hero) {
			Hero hero = (Hero)entity;
			statsPanelUI.gameObject.SetActive (true);
			nameText.text = hero.name + ", Lvl: " + hero.level;
			icon.sprite = hero.Icon;
			statsPanelUI.CreateStats (hero.stats);
		} else if (entity is Unit) {
			Unit unit = (Unit)entity;
			statsPanelUI.gameObject.SetActive (true);
			abilityPanelUI.gameObject.SetActive (true);
			abilityPanelUI.Refresh(unit.abilities);
			nameText.text = unit.name;
			icon.sprite = unit.Icon;
			statsPanelUI.CreateStats (unit.stats);
		}
		else if (entity is Item) {
			Item item = (Item)entity;
			descriptionText.gameObject.SetActive(true);
			nameText.text = item.name;
			descriptionText.text = item.description;
			icon.sprite = item.Icon;
			if (item is Equipment){
				statsPanelUI.gameObject.SetActive (true);
				statsPanelUI.CreateStats(((Equipment)item).stats);
			}
		}
		else if (entity is Ability) {
			Ability ability = (Ability)entity;
			descriptionText.gameObject.SetActive(true);
			nameText.text = ability.name;
			descriptionText.text = ability.description;
			icon.sprite = ability.Icon;
			if (ability is Spell){
				statsPanelUI.gameObject.SetActive (true);
				statsPanelUI.CreateStats(((Spell)ability).stats);
			}
		}
		Vector2 offset = Vector2.zero;
		if (position.y+heightAnchorDelta*Screen.height > Screen.height){
			offset.y +=Screen.height-(position.y+heightAnchorDelta*Screen.height);
		}
		if (position.x + widthAnchorDelta*Screen.width  > Screen.width) {
			offset.x +=Screen.width-(position.x+widthAnchorDelta*Screen.width);
		}
		offset.x += position.x;
		offset.y += position.y;
		rect.anchorMin = new Vector2( offset.x / Screen.width, offset.y / Screen.height);
		rect.anchorMax = new Vector2( rect.anchorMin.x+widthAnchorDelta, rect.anchorMin.y +heightAnchorDelta);
		rect.offsetMax = Vector2.zero;
		rect.offsetMin = Vector2.zero;
		
	}
}