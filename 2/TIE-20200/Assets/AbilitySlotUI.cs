using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
public enum AbilityLocation{NULL, SPELLBOOK, SPELLTREE, INFOPANEL}
public class AbilitySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {
	private AbilityLocation location = AbilityLocation.NULL;
	public AbilityLocation Location{ set{
			location=value; 
			Refresh (ability);
		}
	}

	private bool empty;
	public bool Empty{set{
				button.interactable=!value;
				childObjectsRoot.SetActive(!value);
				empty=value;}}
	public GameObject childObjectsRoot;
	public Image abilityIcon;
	private Button button;
	private bool available;
	private Ability ability;
	private AbilityInBranch abilityInBranch;
	public AbilityInBranch AbilityInBranch{set{abilityInBranch=value;
			ability = DataBase.instance.GetAbility(abilityInBranch.ID);
			Refresh(ability);}}
	public StatUI manaUI;
	void Awake(){
		button = GetComponent<Button> ();
		available = false;
		manaUI = GetComponentInChildren<StatUI> ();
	}
	public void Refresh(Ability _ability){
		ability = _ability;
		Empty = ability == null;
		manaUI.gameObject.SetActive (false);
		if (empty) {return;}
		if (location == AbilityLocation.SPELLBOOK) {
			manaUI.gameObject.SetActive (true);
			Spell temp = (Spell)ability;
			manaUI.Refresh (temp.stats.GetStat(StatType.MANA));
		}

		abilityIcon.sprite= ability.Icon;
		RefreshButton ();
		RefreshInterActibilityInBattle();
	}
	void OnEnable(){
		if (abilityInBranch != null) {
			GameManager.instance.CurrentGame.playerTroop.hero.OnSpellLearned += RefreshButton;
		}
		RefreshInterActibilityInBattle();
	}
	private void RefreshInterActibilityInBattle(){
	if (location == AbilityLocation.SPELLBOOK && GameManager.instance.state == GameStatus.BATTLE) {
		Spell temp = (Spell)(ability);
		button.interactable = GameManager.instance.CurrentGame.playerTroop.hero.stats.GetStat (StatType.MANA).AdditionalValue >= 
			temp.stats.GetStat (StatType.MANA).Value;
	}
	}
	void OnDisable(){
			if (abilityInBranch != null) {
				GameManager.instance.CurrentGame.playerTroop.hero.OnSpellLearned -= RefreshButton;
			}
	}
	public void RefreshButton(){
		if (abilityInBranch != null) {
			available = GameManager.instance.CurrentGame.playerTroop.hero.IsSpellLearnable (ability);
			button.interactable = available;
			if (abilityInBranch.learned) {
				button.interactable=true;
				button.transform.localScale = Vector3.one*0.8f;
			} else {
				if (available) {
					button.transform.localScale = Vector3.one;
				}else {
					button.transform.localScale = Vector3.one*0.8f;
				}
			}
		}
	}
	public void OnPointerDown(PointerEventData eventData){
		if (empty) {return;}
		if (eventData.button == PointerEventData.InputButton.Left) {
			if (GameManager.instance.state == GameStatus.BATTLE && button.interactable){
				Debug.Log ("Seleceted spell from book");
				CombatManager.instance.SelectedSpell = (Spell)ability;
				GuiManager.instance.CloseEntityInfo();
				transform.parent.parent.gameObject.SetActive(false);
			}
			else if (available && abilityInBranch != null){
				GameManager.instance.CurrentGame.playerTroop.hero.LearnSpell(ability);
			}
		}

	}
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (empty) {return;}
			GuiManager.instance.ShowEntityInfo ((Entity)ability, eventData.position);
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		if (empty) {return;}
		GuiManager.instance.CloseEntityInfo();
	}
}
