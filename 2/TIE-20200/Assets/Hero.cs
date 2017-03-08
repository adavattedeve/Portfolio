using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum HeroState{DEFAULT, SPELLCASTING}
[System.Serializable]
public class Hero: Entity {
	public int id;
	public string iconPath;
	[System.NonSerialized]private Sprite icon;
	public Sprite Icon{get{
			if (icon!=null){return icon;}
			else{ 
				icon = Resources.Load<Sprite>(iconPath) as Sprite;
				if (icon!=null){return icon;}
				else{Debug.Log ("cant find hero sprite from" + iconPath);
					return null;}
			}
		}
	}
	public string name="";
	public string prefabPath;
	public float Experience{get{return experience;}}
	private float experience;
	public int level=1;
	public Stats stats;
	[System.NonSerialized]public List<Ability> abilities;
	[System.NonSerialized]public List<Spell> spells;
	private  AbilityIdentifier[] abilityIds;
	private  AbilityIdentifier[] spellIds;
	public AbilityTree abilityTree;
	public int attributePoints;
	public int abilityPoints;

	public ItemList inventory;
	public ItemList equipments;
	public ItemType[] equipmentSlots; 

	private int manaPerKnowledge=10;
	private int manaRegenPerKnowledge = 5;

	public delegate void SpellLearnedAction ();
	[field: System.NonSerialized]public event SpellLearnedAction OnSpellLearned;

	[System.NonSerialized]public int actionPoints;
	[System.NonSerialized]public HeroState state=HeroState.DEFAULT;
	[System.NonSerialized]public HeroController heroController;

	public Hero(string _name){
		name = _name;
		experience = 0;
		level = 1;
		stats = new Stats(StatOwnerType.HERO);
		abilities = new List<Ability>();
		spells =  new List<Spell>();
		abilityTree =DataBase.instance.abilityTree;
		equipmentSlots = new ItemType[3];
		equipmentSlots [0] = ItemType.WEAPON;
		equipmentSlots [1] = ItemType.ARMOR;
		equipmentSlots [2] = ItemType.ARTIFACT;
		equipments = new ItemList ();
		while (equipmentSlots.Length>equipments.items.Count) {
			equipments.items.Add(null);
		}
		inventory = new ItemList ();

	}
	public Hero(){}
	public void RegenMana(){
		stats.GetStat (StatType.MANA).AdditionalValue += stats.GetStat (StatType.MANAREGEN).Value;
	}
	public void InstantiateTo(Vector3 position, Quaternion rotation){
		Debug.Log ("instantiating hero named: " + name);
		GameObject temp = Resources.Load<GameObject>(prefabPath) as GameObject;
		temp = MonoBehaviour.Instantiate (temp, position, rotation) as GameObject;
		heroController = temp.GetComponent<HeroController> ();
		heroController.hero = this;
		
	}
	public void AddExpierence(float _experience){
		experience += _experience;

		if (DataBase.instance.gameData.levelExpierenceRequirements.Length>=level && 
		    DataBase.instance.gameData.levelExpierenceRequirements [level - 1] <= experience) {
			experience -= DataBase.instance.gameData.levelExpierenceRequirements [level - 1];
			LevelUp();
		}
	}
	private void LevelUp(){
		level++;
		abilityPoints++;
		attributePoints++;
	}
	public void Equip(int index, ItemType type){
		Item item = inventory.GetItem (index, type);
		if (item != null && item is Equipment) {;
			inventory.DeleteItem (index, type);
			for (int i=0; i< equipmentSlots.Length; ++i) {
				if (equipmentSlots[i] == item.type){
					if (equipments.items [i] != null) {
						inventory.AddItem (equipments.items [i]);
					}
					equipments.AddItem(item, i);
				}
			}
			Equipment temp = (Equipment)item;
			for (int i=0; i<temp.stats.stats.Count; ++i){
				stats.GetStat(temp.stats.stats[i].type).Value +=temp.stats.stats[i].Value;
				if (temp.stats.stats[i].useAdditionalValue){
					stats.GetStat(temp.stats.stats[i].type).AdditionalValue +=temp.stats.stats[i].AdditionalValue;

				}
			}
		}
	}
	public void UnEquip(int index){
		Debug.Log ("unequipping: "+index);
		if (equipments.items[index] != null) {

			Equipment temp = (Equipment)equipments.items[index];
			for (int i=0; i<temp.stats.stats.Count; ++i){
				stats.GetStat(temp.stats.stats[i].type).Value -=temp.stats.stats[i].Value;
				if (temp.stats.stats[i].useAdditionalValue){
					stats.GetStat(temp.stats.stats[i].type).AdditionalValue -=temp.stats.stats[i].AdditionalValue;
					
				}
			}
			inventory.AddItem(equipments.items[index]);
			equipments.AddItem(null, index);
		}
	}
	public bool IsSpellLearnable(Ability ability){
		if (abilityPoints <= 0) {
			return false;}
		return abilityTree.IsAvailable (ability.id);
	}
	public void LearnSpell(Ability ability){
		abilityPoints--;
		abilityTree.Learn (ability.id);
		UpdateSpells ();
		if (OnSpellLearned != null) {
			OnSpellLearned();
		}
	}
	public void VisualizeSpellCast(Spell spell, Node target){
		state = HeroState.SPELLCASTING;
		heroController.SpellCastTo (spell, target);
	}

	public void LevelUpAttribute(StatType type){
		attributePoints--;
		stats.GetStat (type).Value++;
		if (type == StatType.KNOWLEDGE) {
			stats.GetStat (StatType.MANA).AdditionalValue += manaPerKnowledge;
		}
	}
	public void PrepareForSaving(){
		inventory.PrepareForSaving ();
		equipments.PrepareForSaving ();
			if (abilities != null) {
			abilityIds = new AbilityIdentifier[abilities.Count];
				for (int i=0; i<abilities.Count; ++i) {
					abilityIds [i] = abilities [i].id;
				}
			} else {
			abilityIds = new AbilityIdentifier [0];
			}
			if (spells != null) {
			spellIds = new AbilityIdentifier[spells.Count];
				for (int i=0; i<spells.Count; ++i) {
				spellIds [i] = spells [i].id;
				}
			}else {
			spellIds = new AbilityIdentifier [0];
			}
	}
	public void FinishLoading(){
		inventory.FinishLoading ();
		equipments.FinishLoading ();

		while (equipmentSlots.Length>equipments.items.Count) {
			equipments.items.Add(null);
		}

		if (abilityIds != null) {
			abilities = new List<Ability> (abilityIds.Length);
			for (int i=0; i<abilityIds.Length; ++i) {
				abilities [i] = DataBase.instance.GetAbility (abilityIds [i]);
			}
		}
		if (spellIds != null) {
			spells = new List<Spell> ();
			for (int i=0; i<spellIds.Length; ++i) {
				spells.Add((Spell)DataBase.instance.GetAbility (spellIds [i]));
			}
		}
		stats.GetStat (StatType.KNOWLEDGE).OnStatChange += UpdateManaAndManaRegen;
	}
	public void UpdateManaAndManaRegen(Stat stat){
		int knowledge = stats.GetStat (StatType.KNOWLEDGE).Value;
		stats.GetStat (StatType.MANA).Value = knowledge*manaPerKnowledge ;
		stats.GetStat (StatType.MANAREGEN).Value = knowledge*manaRegenPerKnowledge;
	}
	public void UpdateSpells(){
		spells = abilityTree.GetSpells ();
	}
	public Hero GetDublicate(){
		Hero returnObject = new Hero (name);
		returnObject.id = id;
		returnObject.iconPath = iconPath;
		returnObject.prefabPath = prefabPath;
		returnObject.experience = experience;
		returnObject.level = level;
		returnObject.abilityPoints = abilityPoints;
		returnObject.attributePoints = attributePoints;

		returnObject.stats = stats.GetDublicate ();
		if (abilityIds != null) {
			for (int i=0; i<abilityIds.Length; ++i) {
				returnObject.abilities.Add (DataBase.instance.GetAbility (abilityIds [i]));
			}
		} else {
			returnObject.abilities= new List<Ability>();
		}
		if (spellIds != null) {
			for (int i=0; i<spellIds.Length; ++i) {
				returnObject.spells.Add ((Spell)DataBase.instance.GetAbility (spellIds [i]));
			}
		} else {
			returnObject.spells= new List<Spell>();
		}
		Debug.Log ("GETTING HERO DUBLICATE!!!");
		returnObject.abilityTree = abilityTree.GetDublicate ();
		returnObject.stats.GetStat (StatType.KNOWLEDGE).OnStatChange += returnObject.UpdateManaAndManaRegen;
		returnObject.UpdateManaAndManaRegen (returnObject.stats.GetStat (StatType.KNOWLEDGE));
		returnObject.stats.GetStat (StatType.MANA).AdditionalValue = returnObject.stats.GetStat (StatType.MANA).Value;
		returnObject.UpdateSpells ();
		return returnObject;

	}
}
