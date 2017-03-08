using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public enum StatType{DAMAGE, HEALTH, OFFENCE, DEFENCE, MOVEMENT, LUCK, INTELLIGENCE, KNOWLEDGE, MANA, MANAREGEN, DAMAGESCALING, DURATION,DURATIONSCALING}
[System.Serializable]
public class Stat {
	public StatType type;
	public bool useAdditionalValue;
	public delegate void StatChangeAction (Stat stat);
	[field: System.NonSerialized]public event StatChangeAction OnStatChange;
	[SerializeField]private int amount;
	public int Value{get{return amount;}
		set{
//			if (type!=StatType.DAMAGE){
//				additionalValue+=value-amount;
//			}

			amount = value;
			if (additionalValue>amount){
				additionalValue=amount;
			}
			if (OnStatChange!=null){
				OnStatChange(this);
			}
		}}
	[SerializeField]private int additionalValue;
	public int AdditionalValue{get{return additionalValue;}
		set{

			if (value>amount){
				additionalValue=amount;
			}else{additionalValue = value;}
			if (OnStatChange!=null){
				OnStatChange(this);
			}
		}}
	public Stat(StatType _type){
		type = _type;
		amount = 0;
		if (type == StatType.DAMAGE || type == StatType.HEALTH || type == StatType.MANA) {
			useAdditionalValue = true;
		} else {
			useAdditionalValue = false;
		}
	}
	public Stat(){}
}
public enum StatOwnerType{HERO, UNIT}
[System.Serializable]
public class Stats{
	public List<Stat> stats;
	public Stat GetStat(StatType type){
		for (int i=0; i<stats.Count; ++i) {
			if (stats[i].type == type){
				return stats[i];
			}
		}
		return null;
	}
//	public void AddToStat(StatType type, int value){
//		for (int i=0; i<stats.Count; ++i) {
//			if (stats[i].type ==type){
//				stats[i].Value+=value;
//			}
//		}
//	}
	public Stats(){
		stats = new List<Stat>(); 
	}
	public Stats(StatOwnerType owner){
		stats = new List<Stat>(); 
		switch (owner) {
		case StatOwnerType.HERO:
			stats.Add(new Stat (StatType.INTELLIGENCE));
			stats.Add(new Stat (StatType.KNOWLEDGE));
			stats.Add(new Stat (StatType.MANA));
			stats.Add(new Stat (StatType.OFFENCE));
			stats.Add(new Stat (StatType.DEFENCE));
			stats.Add(new Stat (StatType.MOVEMENT));
			stats.Add(new Stat (StatType.LUCK));
			stats.Add(new Stat (StatType.MANAREGEN));
			break;
		case StatOwnerType.UNIT:
			stats.Add(new Stat (StatType.DAMAGE));
			stats.Add(new Stat (StatType.HEALTH));
			stats.Add(new Stat (StatType.OFFENCE));
			stats.Add(new Stat (StatType.DEFENCE));
			stats.Add(new Stat (StatType.MOVEMENT));
			stats.Add(new Stat (StatType.LUCK));
			break;
		}
	}
	public Stats GetDublicate(){
		Stats returnObject = new Stats ();
		for (int i=0; i<stats.Count; ++i) {

			returnObject.stats.Add(new Stat(stats[i].type));

			returnObject.stats [i].Value = stats [i].Value;
			returnObject.stats [i].AdditionalValue = stats [i].AdditionalValue;
			returnObject.stats [i].useAdditionalValue = stats [i].useAdditionalValue;
		}
		return returnObject;
	}

}
