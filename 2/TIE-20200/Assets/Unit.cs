using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum UnitState{DEFAULT, ATTACKING, TAKINGDAMAGE, MOVING , ROTATING}
[System.Serializable]
public class Unit: Entity{

	public int id;
	public int amount;
	public float basePopulateValue;
	public string iconPath;
	[System.NonSerialized]private Sprite icon;
	public Sprite Icon{get{
			if (icon!=null){return icon;}
			else{ 
				icon = Resources.Load<Sprite>(iconPath) as Sprite;
				if (icon!=null){return icon;}
				else{Debug.Log ("cant find unit sprite from" + iconPath);
					return null;}
			}
		}
	}
	public int goldValue;
	public float experienceValue;
	public string name="";
	public string prefabPath;
	public Stats stats;
	public bool ranged;
	public string attackProjectileName;
	[System.NonSerialized]public List<Ability> abilities;
	private List<Effect> temporalEffects;
	public List<Effect> Effects{get{return temporalEffects;}}
	public delegate void EffectsChangeAction (Effect effect, bool Added);
	[field: System.NonSerialized]public event EffectsChangeAction OnEffectsChange;

	public List<Ability> TriggableAbilities{get{
			List<Ability> triggableAbilities = new List<Ability>();
			for (int i= 0; i< abilities.Count; ++i){
				triggableAbilities.Add(abilities[i]);
			}
			for (int i= 0; i< temporalEffects.Count; ++i){
				triggableAbilities.Add((Ability)temporalEffects[i]);
			}
			return triggableAbilities;
		}}
	public List<AbilityIdentifier> abilityIdentifiers;

	[System.NonSerialized]public UnitController unitController;
	[System.NonSerialized]public UnitState state;
	[System.NonSerialized]private Quaternion defaultRotation;
	[System.NonSerialized]public PlayerID owner;
	[System.NonSerialized]public int actionPoints;
	[System.NonSerialized]public bool ableToMove;
	[System.NonSerialized]public bool ableToAttack;
	[System.NonSerialized]public bool ableToRetaliate;
	private bool visible;
	public bool Visible{get{return visible;}set{
			unitController.ChangeVisibility(value, CombatManager.instance.GetPlayer (owner).control == Control.AI);
			visible =value;
		}}

	[System.NonSerialized]public bool doubleStriked = false;


	public Unit(){
		stats = new Stats(StatOwnerType.UNIT);
		abilityIdentifiers =new List<AbilityIdentifier>();
		abilities = new List<Ability>();
		state = UnitState.DEFAULT;
		visible=true;
	}

	public void InstantiateTo(Vector3 position, Quaternion rotation){
		OnEffectsChange = null;
		temporalEffects = new List<Effect> ();
		GameObject temp = Resources.Load<GameObject>(prefabPath) as GameObject;
		defaultRotation = rotation;
		temp = MonoBehaviour.Instantiate (temp, position, defaultRotation) as GameObject;
		unitController = temp.GetComponent<UnitController> ();
		unitController.Unit = this;

	}
	public int TakeDamage(int damage){
		Stat health = stats.GetStat (StatType.HEALTH);
		int deathCount=0;
		int currentDamage = damage;
		if (currentDamage<health.AdditionalValue){
			health.AdditionalValue -= currentDamage;
			currentDamage=0;
		}
		else{
			deathCount++;
			currentDamage-=health.AdditionalValue;
			health.AdditionalValue = health.Value;
			int additionalDeadCount = currentDamage / health.Value;
			if (additionalDeadCount>0){
				deathCount+=additionalDeadCount;
				currentDamage -= additionalDeadCount*health.Value;
			}
			if (deathCount >= amount) {
				deathCount=amount;
			}
			amount -= deathCount;
		}

		CombatManager.instance.GetPlayer (owner).losses.AddUnits (this, deathCount);
		if (amount > 0) {
			health.AdditionalValue -= currentDamage;
		} else {
			OnEffectsChange=null;
		}
		return deathCount;
	}
	public void VisualizeAttack(Node to){
		unitController.AttackTo (to);
	}
	public void VisualizeTakeDamage(int damage, int deathCount, bool death=false){
		if (death) {
			unitController.OnDeath ( damage, deathCount);
		} else {
			unitController.TakeDamage ( damage, deathCount);
		}
	}
	public void RotateTowards(Unit unit=null){
		Quaternion targetRotation;
		if (unit == null) {
			targetRotation = defaultRotation;
		} else {
			Vector3 from = unitController.transform.forward;
			from.y=0;
			Vector3 to = unit.unitController.transform.position-unitController.transform.position;
			to.y=0;
			targetRotation = unitController.transform.rotation*Quaternion.FromToRotation(from, to);
		}
		unitController.StartCoroutine (unitController.RotateTo (targetRotation));
	}
	public void Reset(){
		int effectCount = temporalEffects.Count;
		while (effectCount>0) {
			Debug.Log (temporalEffects[0].name + " from "+ name);
			temporalEffects[0].EndEffect();
			--effectCount;
		}
		Stat temp = stats.GetStat (StatType.HEALTH);
		temp.AdditionalValue = temp.Value;
	}
	public void AddEffect(Effect effect){
		for (int i=0; i< temporalEffects.Count; ++i) {
			if (temporalEffects[i].GetType().IsAssignableFrom(effect.GetType())){
				if (OnEffectsChange!=null){
					OnEffectsChange(temporalEffects[i], false);
				}
				temporalEffects.RemoveAt(i);
			}
		}
		if (OnEffectsChange != null) {
			OnEffectsChange (effect, true);
		}
		temporalEffects.Add (effect);
	}
	public void RemoveEffect(Effect effect){
		for (int i=0; i< temporalEffects.Count; ++i) {
			if (temporalEffects[i]==effect){
				if (OnEffectsChange!=null){
					OnEffectsChange(temporalEffects[i], false);
				}
				temporalEffects.RemoveAt(i);
			}
		}
	}
	public void PrepareForSaving(){
		if (abilities != null) {
			abilityIdentifiers = new List<AbilityIdentifier>();
			for (int i=0; i< abilities.Count; ++i) {
				if (abilities [i] != null) {
					abilityIdentifiers.Add (abilities [i].id);
					} else {
					abilityIdentifiers.Add (AbilityIdentifier.NULL);
					}
				}
			}
		}
	public void FinishLoading(){
		if (DataBase.instance != null && abilityIdentifiers != null && abilityIdentifiers != null) {
			abilities = new List<Ability> ();
			for (int i=0; i< abilityIdentifiers.Count; ++i) {
				if (abilityIdentifiers [i] != AbilityIdentifier.NULL) {
					abilities.Add (DataBase.instance.GetAbility (abilityIdentifiers [i]));
				} else {
					abilities.Add (null);
				}
			}
		}
		visible = true;
		temporalEffects = new List<Effect> ();
	}
	public Unit GetDublicate (){
		Unit returnObject = new Unit ();
		returnObject.id = id;
		returnObject.iconPath = iconPath;
		returnObject.ranged = ranged;
		returnObject.name = name;
		returnObject.prefabPath = prefabPath;
		returnObject.basePopulateValue = basePopulateValue;
		returnObject.goldValue = goldValue;
		returnObject.experienceValue = experienceValue;
		returnObject.stats = stats.GetDublicate ();
		returnObject.attackProjectileName = attackProjectileName;
		for (int i=0; i<abilityIdentifiers.Count; ++i) {
			returnObject.abilities.Add (DataBase.instance.GetAbility(abilityIdentifiers[i]));
			returnObject.abilityIdentifiers.Add (abilityIdentifiers[i]);
		}
		return returnObject;
	}
}

