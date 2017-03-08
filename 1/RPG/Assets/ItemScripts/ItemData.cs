using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemType{CONSUMABLE, KEYITEM, WEAPON, OFFHAND, RING, AMULET, HELMET, CHEST, GLOVES, LEGS, BOOTS, OTHER, ANY};
public enum Resource {HEALTH};
public enum StatType {ARMOR, MAXHEALTH, DAMAGEMIN, DAMAGEMAX, STABILITY, IMPACT};
[System.Serializable]
public class Restoration{
	public Resource type;
	public float amount;
	public Restoration (Resource _type, float _amount){
		type = _type;
		amount = _amount;
	}
	public void SetValues(Resource _type, float _amount){
		type = _type;
		amount = _amount;
	}
}
[System.Serializable]
public class Stat{
	public StatType type;
	public float amount;
	public float perLevel;
	public Stat (StatType _type, float _amount, float _perLevel){
		type = _type;
		amount = _amount;
		perLevel = _perLevel;
	}
	public void SetValues(StatType _type, float _amount, float _perLevel){
		type = _type;
		amount = _amount;
		perLevel = _perLevel;
	}
	public override string ToString ()
	{
		switch (type) {
		case StatType.ARMOR:
			return "Armor: " + amount;
		case StatType.DAMAGEMAX:
			return "Max damage: " + amount;
		case StatType.DAMAGEMIN:
			return "Min Damage: " + amount;
		case StatType.MAXHEALTH:
			return "Health: " + amount;
		case StatType.IMPACT:
			return "Impact: " + amount;
		case StatType.STABILITY:
			return "stability: " + amount;
		}
		return null;
	}
}
[System.Serializable]
public class ItemStatRange{
	public int ID;
	public string name;
	public int levelRange;

	public Stat[] min;
	public Stat[] max;
	public ItemStatRange(int id){
		ID = id;
		StatType[] statTypes = System.Enum.GetValues (typeof(StatType)) as StatType[];
		min = new Stat[statTypes.Length];
		max = new Stat[statTypes.Length];
		for (int i=0; i<statTypes.Length; ++i) {
			min[i] = new Stat(statTypes[i],0,0);
			max[i] = new Stat(statTypes[i],0,0);
		}
	}
}
public class ItemData : ScriptableObject {
	private List<int> usedIDs;
	public List<Equipment> equipments;
	public List<Weapon> weapons;
	public List<Consumable> consumables;

	public List<ItemStatRange> statRanges;

	public int GetNewItemStatRangeID(){
		int[] usedIDs = new int[statRanges.Count];
		int tryID = 0;

		for (int i=0; i<usedIDs.Length; ++i) {
			usedIDs[i] = statRanges[i].ID;
		}
		for (int i=0; i<usedIDs.Length; ++i) {
			if (usedIDs[i]==tryID){
				i=-1;
				++tryID;
			}
		}
		return tryID;
	}
	public Stat GetItemStat(StatType type, int id, int level){
		ItemStatRange statRange=null;
		for (int i=0; i<statRanges.Count; ++i) {
			if (statRanges[i].ID == id){
				statRange = statRanges[i];
			}
		}
		if (statRange==null){return null;}
		for (int i=0; i<statRange.max.Length; ++i) {
			if (statRange.max[i].type == type){
				return new Stat(type, Random.Range (statRange.min[i].amount, statRange.max[i].amount), Random.Range (statRange.min[i].perLevel, statRange.max[i].perLevel));
			}
		}
		return null;

	}
	public int GetNewItemID(){
		int[] usedIDs = new int[equipments.Count+weapons.Count+consumables.Count];
		for (int i=0; i<usedIDs.Length; ++i) {
			for (int i2=0; i2<equipments.Count; ++i2) {
				usedIDs[i] = equipments[i2].ID;
				++i;
			}
			for (int i2=0; i2<weapons.Count; ++i2) {
				usedIDs[i] = weapons[i2].ID;
				++i;
			}
			for (int i2=0; i2<consumables.Count; ++i2) {
				usedIDs[i] = consumables[i2].ID;
				++i;
			}
		}
		int tryID = 0;
		for (int i=0; i<usedIDs.Length; ++i) {
			if (usedIDs[i]==tryID){
				i=-1;
				++tryID;
			}
		}
		return tryID;
	}

	public IItem[] GetItems(){
		IItem[] items = new IItem[equipments.Count+weapons.Count+consumables.Count];
		for (int i=0; i<items.Length; ++i) {
			for (int i2=0; i2<equipments.Count; ++i2) {
				items[i] = (IItem)equipments[i2];
				++i;
			}
			for (int i2=0; i2<weapons.Count; ++i2) {
				items[i] = (IItem)weapons[i2];
				++i;
			}
			for (int i2=0; i2<consumables.Count; ++i2) {
				items[i] = (IItem)consumables[i2];
				++i;
			}
		}
		return items;
	}
}
