using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Troop {
	[SerializeField]private Hero troopHero;
	public Hero hero{set{troopHero=value;}get{
			if (troopHero != null) {
				if (troopHero.name!=null && troopHero.name.Length > 2){
					return troopHero;
				}
			}
			return null;
		}}
	public List<Unit> units;
	public delegate void TroopsChangeAction ();
	[field: System.NonSerialized]public event TroopsChangeAction OnTroopsChange;
	public Troop(Hero _hero){
		hero = _hero;
		units = new List<Unit>(6);
	}
	public Troop(){
		hero = null;
		units = new List<Unit>(6);
	}

	public Unit AddUnits (Unit unit, int amount, int index=-1, bool destroyEmpty=true){
		if (amount == 0) {
			return null;
		}
		Unit returnUnit=null;
		if (index == -1) {
			for (int i=0; i<units.Count; ++i) {
				if (units [i]!=null && units [i].id == unit.id) {
					units [i].amount += amount;
					if (units [i].amount<=0 && destroyEmpty){
						units[i]=null;
					}
					if (OnTroopsChange!=null){
						OnTroopsChange();
					}
					return returnUnit;
				}
			}
			unit = unit.GetDublicate ();
			unit.amount = amount;
			for (int i=0; i<units.Count; ++i) {
				if (units[i]==null){
					units[i]=unit;
					if (OnTroopsChange!=null){
						OnTroopsChange();
					}
					return returnUnit;
				}
			}

			units.Add (unit);
			if (OnTroopsChange!=null){
				OnTroopsChange();
			}
			return returnUnit;
		}
		else {
			while (units.Count<=index) {
				units.Add(null);
			}
			if (unit==null){
				returnUnit = units[index];
				units[index]=unit;
				return returnUnit;
			}
			if (units[index] != null){

				if (units[index].id == unit.id){
					units [index].amount += amount;
					if (units [index].amount<=0){
						units[index]=null;
					}
					if (OnTroopsChange!=null){
						OnTroopsChange();
					}
					return returnUnit;
				}
				returnUnit = units[index];
			}
			unit = unit.GetDublicate ();
			unit.amount = amount;
			units[index] = unit;
			if (OnTroopsChange!=null){
				OnTroopsChange();
			}
			return returnUnit;
		}
	}
	public void PrepareForSaving(){
		if (hero != null) {
			hero.PrepareForSaving();
		}
		if (units != null) {
			for (int i=0; i<units.Count; ++i) {
				if (units [i] != null) {
					units [i].PrepareForSaving ();
				}
			}
		} else {
			units = new List<Unit>(6);
		}
	}
	public void FinishLoading(){
		Debug.Log ("loading troops");
		if (hero != null) {
			hero.FinishLoading ();
		}
		if (units != null) {
			for (int i=0; i<units.Count; ++i) {
				if (units [i] != null) {
					if (units[i].name==""){
						Debug.Log ("unit null");
						units[i]=null;
					}else{
						units [i].FinishLoading ();
					}
				}
			}
		}
		Debug.Log (units.Count);
		}
	
	}
