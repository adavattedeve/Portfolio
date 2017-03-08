using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Ability {
	private string name;
	private string description;
	private float damageMultiplier;
	private float impactMultiplier;
	private Image icon;
	private int actionID;

	public string Name {get{return name;}}
	public string Description{get{return description;}}
	public float DamageMultiplier{get{return damageMultiplier;}}
	public float ImpactMultiplier{get{return impactMultiplier;}}
	public Image Icon{get{return icon;}}
	public int ActionID{get{return actionID;}}
//	public List<IAbilityComponent> effects;

	public Ability(string name, float damage, float impact, int actionID, string description, Image icon){
		this.name = name;
		damageMultiplier = damage;
		impactMultiplier = impact;
		this.description = description;
		this.icon = icon;
		this.actionID = actionID;
	}
	public Ability(string name, float damage, float impact, int actionID){
		this.name = name;
		damageMultiplier = damage;
		impactMultiplier = impact;
		this.description = "default";
		this.icon = null;
		this.actionID = actionID;
	}
}
