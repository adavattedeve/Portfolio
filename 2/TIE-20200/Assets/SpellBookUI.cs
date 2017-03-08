using UnityEngine;
using  UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class SpellBookUI : MonoBehaviour {
	private Button spellBookButton;
	private AbilityPanelUI abilityPanelUI;
	private StatUI statUI;
	private CombatManager.Player currentPlayer;
	void Awake () {
		spellBookButton = GetComponent<Button> ();
		spellBookButton.onClick.AddListener (delegate {
			GuiManager.instance.SpellBookObject.SetActive(true);
			RefreshSpellBookSpells();
			if (CombatManager.instance!=null && GameManager.instance.state == GameStatus.BATTLE){
				CombatManager.instance.SelectedSpell=null;
			}
	});
		GuiManager.instance.SpellBookObject.SetActive (true);
		abilityPanelUI = GuiManager.instance.SpellBookObject.GetComponentInChildren<AbilityPanelUI> ();
		statUI = GuiManager.instance.SpellBookObject.GetComponentInChildren<StatUI> ();
		statUI.Refresh (GameManager.instance.CurrentGame.playerTroop.hero.stats.GetStat(StatType.MANA));
		GuiManager.instance.SpellBookObject.SetActive (false);
	}
	void OnEnable(){

		if (GameManager.instance.state == GameStatus.BATTLE) {
			//Awake ();
			Debug.Log ("Surscibing events");
			CombatManager.instance.OnTurnChange+=RefreshInteracability;
			CombatManager.instance.OnSelectedUnitChange+=RefreshInteracability;
		}
	}
	void OnDisable(){
		if (GameManager.instance.state == GameStatus.BATTLE) {
			Debug.Log ("UNSurscibing events");
			CombatManager.instance.OnTurnChange-=RefreshInteracability;
			CombatManager.instance.OnSelectedUnitChange-=RefreshInteracability;
		}
	}
	void RefreshInteracability(CombatManager.Player player){
		spellBookButton.interactable = player.control == Control.INPUT;
		currentPlayer = player;
	}
	void RefreshInteracability(Node node){
		bool interactable = (currentPlayer.troop.hero != null && currentPlayer.troop.hero.actionPoints > 0);
		spellBookButton.interactable=interactable;
	}
	void RefreshSpellBookSpells(){
		List<Ability> abilities = new List<Ability>();
		for (int i=0; i<GameManager.instance.CurrentGame.playerTroop.hero.spells.Count; ++i) {
			abilities.Add ((Ability)GameManager.instance.CurrentGame.playerTroop.hero.spells[i]);
		}
		abilityPanelUI.Refresh (abilities);
	}
}
