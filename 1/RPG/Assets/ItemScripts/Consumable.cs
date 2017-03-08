using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Consumable : IItem, IStackable {
	protected GameObject owner;
	protected GameObject gO;
	[SerializeField]protected int itemID;
	public int ID{ get { return itemID; } set { itemID = value; } }
	public GameObject GO{
		get { return gO;}
		set{ gO = value;}
	}
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
	public List<Restoration> restoration;
	public List<Stat> buff;
	public float buffTime;

	public int level;

	private CharacterStats characterStats;
	private Health characterHealth;
	private Inventory inventory;
	private int stack;
	public void Use(int index){
		ConsumeStack (index);
		if (restoration != null && restoration.Count>0) {
			for (int i=0; i<restoration.Count; ++i){
				characterHealth.Restore(restoration[i]);
			}
		}
		if (buff != null && buff.Count > 0) {
			for (int i=0; i<buff.Count; ++i){
				characterStats.Buff(buff[i], buffTime);
			}
		}
		GlobalEvents.instance.LaunchOnInventoryChange ();
	}

	public void SetOwnerAndObjectReferences(GameObject owner, GameObject _gO){
		this.owner = owner;
		this.gO = _gO;
		characterStats = owner.GetComponent<CharacterStats> ();
		characterHealth = owner .GetComponent<Health> ();
		inventory = owner.GetComponent<Inventory> ();
	}
	public IItem GetDublicate (int level){
		Consumable returnItem = new Consumable ();
		returnItem.type = type;
		returnItem.Name = name;
		returnItem.ObjectPrefab = objectPrefab;
		returnItem.Icon = icon;
		returnItem.description = description;
		returnItem.restoration = restoration;
		returnItem.buff = buff;
		returnItem.buffTime = buffTime;
		returnItem.stack = 1;
		return (IItem)returnItem as IItem;
	}
	public string GetAdditionalInfoAsString(){
		return "";
	}

	public void AddToStack(){
		++stack;
	}
	private void ConsumeStack(int indexInInventory){
		--stack;
		if (stack <= 0) {
			if (inventory.Consumable.Item==this){
				inventory.Consumable.Item=null;
			}
			inventory.SetItem(null, indexInInventory);
			MonoBehaviour.Destroy(gO);

		}
	}
}
