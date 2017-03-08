using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Weapon : Equipment {
//	private GameObject owner;
//	[SerializeField] private string name= "new item";
//	public string Name{
//		get {return name;}
//		set{name=value;}
//	}
//	[SerializeField] private string description= "new item";
//	public string Description{
//		get {return description;}
//		set{description=value;}
//	}
//	[SerializeField] private ItemType type;
//	public ItemType Type{
//		get {return type;}
//		set{type=value;}
//	}
//	[SerializeField] private GameObject objectPrefab;
//	public GameObject ObjectPrefab{
//		get { return objectPrefab;}
//		set{ objectPrefab = value;}
//	}
//	[SerializeField] private Sprite icon;
//	public Sprite Icon{
//		get {return icon;}
//		set{icon=value;}
//	}
	private GameObject weaponModel;
	public GameObject WeaponModel{ get { return weaponModel; } set { weaponModel=value; } }
	private Quaternion localRot;
	public Quaternion LocalRot {get {return localRot;}}
	public override void SetOwnerAndObjectReferences (GameObject owner, GameObject _gO)
	{
		base.SetOwnerAndObjectReferences (owner, _gO);
		weaponModel = _gO.GetComponentsInChildren<WeaponModelInfo> (true)[0].gameObject;
		localRot = weaponModel.transform.localRotation;
	}
	public override IItem GetDublicate (int level){
		Weapon returnItem = new Weapon ();
		returnItem.itemLevel = level;
		returnItem.type = type;
		returnItem.Name = name;
		returnItem.ObjectPrefab = objectPrefab;
		returnItem.Icon = icon;
		returnItem.description = description;
		returnItem.stats = new List<Stat> ();
		for (int i=0; i<stats.Count; ++i) {
			Stat temp = ItemDB.instance.itemData.GetItemStat(stats[i].type, statRangeID, level);
			returnItem.stats.Add (new Stat(stats[i].type, temp.amount+temp.perLevel*level, 0));
		}
		for (int i=0; i<randomStatAmount; ++i) {
			StatType newStatType = randomStatTypes[Random.Range(0, randomStatTypes.Count)];
			Stat temp = ItemDB.instance.itemData.GetItemStat(newStatType, statRangeID, level);
			returnItem.stats.Add(new Stat(newStatType, temp.amount+temp.perLevel*level, 0));
		}
		return (IItem)returnItem as IItem;
	}
}
