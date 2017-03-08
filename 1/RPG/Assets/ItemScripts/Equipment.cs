using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Equipment : IItem {
	protected GameObject owner;
	//Reference to instance of item gO
	protected GameObject gO;
	[SerializeField]protected int itemID;
	public int ID{ get { return itemID; } set { itemID = value; } }
	public GameObject GO{
		get { return gO;}
		set{ gO = value;}
	}
	protected MeshRenderer meshRenderer;
	protected MeshFilter meshFilter;
	protected SkinnedMeshRenderer skinnedMeshRenderer;
	[SerializeField] protected string name= "new item";
	public string Name{
		get {return name;}
		set{name=value;}
	}
	[SerializeField] protected string description= "new item";
	public string Description{
		get {return description;}
		set{description=value;}
	}
	[SerializeField] protected ItemType type;
	public ItemType Type{
		get {return type;}
		set{type=value;}
	}
	[SerializeField] protected GameObject objectPrefab;
	public GameObject ObjectPrefab{
		get { return objectPrefab;}
		set{ objectPrefab = value;}
	}
	[SerializeField] protected Sprite icon;
	public Sprite Icon{
		get {return icon;}
		set{icon=value;}
	}

	public int itemLevel;
	public int statRangeID;
	public List<Stat> stats;
	public List<StatType> randomStatTypes;
	public int randomStatAmount;

	private GameObject[] stichableObjects;
	public GameObject[] StichableObjects{get{ return stichableObjects; }}

//	public Equipment(string _name, string description, ItemType type, GameObject objectPrefab, Sprite icon){
//
//	}

	protected Gear gear;
	protected Inventory inventory;

	public virtual void Use(int inventoryIndex){
		inventory.SetItem(gear.Equip(this), inventoryIndex);
	}
	public virtual void Use(int inventoryIndex, int gearIndex){
		inventory.SetItem(gear.Equip(this, gearIndex), inventoryIndex);
	}
	public virtual void SetOwnerAndObjectReferences(GameObject owner, GameObject _gO){
		this.owner = owner;
		this.gO = _gO;
		SkinnedMeshRenderer[] temp = gO.GetComponentsInChildren<SkinnedMeshRenderer> (true);
		if (temp!=null){
			stichableObjects = new GameObject[temp.Length];
			for (int i =0; i<temp.Length; ++i) {
				stichableObjects[i] = temp[i].gameObject;
			}
		}
		gear = owner.GetComponent<Gear> ();
		inventory = owner.GetComponent<Inventory> ();
	}

	public virtual IItem GetDublicate (int level){
		Equipment returnItem = new Equipment ();
		returnItem.itemLevel = level;
		returnItem.type = type;
		returnItem.Name = name;
		returnItem.ObjectPrefab = objectPrefab;
		returnItem.Icon = icon;
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
		returnItem.description = description;
		return (IItem)returnItem as IItem;
	}
	public string GetAdditionalInfoAsString(){
		string temp = "";
		for (int i =0; i<stats.Count; ++i) {
			temp += stats[i].ToString() + "\n";
		}
		return temp;
	}
}
