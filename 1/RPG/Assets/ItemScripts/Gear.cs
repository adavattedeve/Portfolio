using UnityEngine;
using System.Collections;

public enum GearType{HEAD, TORSO, HAND, LEGS, FOOT, WEAPON, BACK};
public class Gear : MonoBehaviour {
	private ItemSlot[] gear;
	public ItemSlot[] GearSlots{get{return gear;}}
	private ModelSticher sticher;
	private Inventory inventory;

	public GameObject[] defaultAnimatedHead;
	public GameObject[] defaultAnimatedChest;
	public GameObject[] defaultAnimatedLegs;
	public GameObject[] defaultAnimatedFeet;
	public GameObject[] defaultAnimatedHands;

	private GameObject mainHand;
	private CharacterStats stats;
	//public GameObject offHand;
	

	struct AnimatedModel{
		public GameObject[] models;
		public ItemType type;
		public AnimatedModel(GameObject[] _models, ItemType _type){
			models = _models;
			type=_type;
		}
	}
	AnimatedModel[] animatedModels;
	AnimatedModel[] defaultAnimatedModels;

	void Awake(){
		stats = GetComponent<CharacterStats> ();
		mainHand = stats.mainHand;
		sticher = GetComponentInChildren<ModelSticher> ();
		inventory = GetComponent<Inventory> ();
		gear = new ItemSlot[10];
		gear [0] = new ItemSlot (0, ItemType.HELMET);
		gear [1] = new ItemSlot (1, ItemType.AMULET);
		gear [2] = new ItemSlot (2, ItemType.RING);
		gear [3] = new ItemSlot (3, ItemType.RING);
		gear [4] = new ItemSlot (4, ItemType.CHEST);
		gear [5] = new ItemSlot (5, ItemType.GLOVES);
		gear [6] = new ItemSlot (6, ItemType.WEAPON);
		gear [7] = new ItemSlot (7, ItemType.OFFHAND);
		gear [8] = new ItemSlot (8, ItemType.LEGS);
		gear [9] = new ItemSlot (9, ItemType.BOOTS);
		defaultAnimatedModels = new AnimatedModel[5];
		defaultAnimatedModels [0] = new AnimatedModel (defaultAnimatedHead, ItemType.HELMET);
		defaultAnimatedModels [1] = new AnimatedModel (defaultAnimatedChest, ItemType.CHEST);
		defaultAnimatedModels [2] = new AnimatedModel (defaultAnimatedLegs, ItemType.LEGS);
		defaultAnimatedModels [3] = new AnimatedModel (defaultAnimatedFeet, ItemType.BOOTS);
		defaultAnimatedModels [4] = new AnimatedModel (defaultAnimatedHands, ItemType.GLOVES);
		animatedModels = new AnimatedModel[5];
		animatedModels [0] = new AnimatedModel (new GameObject[defaultAnimatedModels [0].models.Length], ItemType.HELMET);
		animatedModels [1] = new AnimatedModel (new GameObject[defaultAnimatedModels [1].models.Length], ItemType.CHEST);
		animatedModels [2] = new AnimatedModel (new GameObject[defaultAnimatedModels [2].models.Length], ItemType.LEGS);
		animatedModels [3] = new AnimatedModel (new GameObject[defaultAnimatedModels [3].models.Length], ItemType.BOOTS);
		animatedModels [4] = new AnimatedModel (new GameObject[defaultAnimatedModels [4].models.Length], ItemType.GLOVES);
	}
	public IItem Equip (Equipment equipment){
		IItem returnObject=null;
		int equipIndex = -1;
		for (int i=0; i<gear.Length; ++i) {
			if (gear[i].typeAllowed ==equipment.Type && gear[i].Item==null){
				equipIndex=i;
				break;
			}
		}
		if (equipIndex < 0) {
			for (int i=0; i<gear.Length; ++i) {
				if (gear[i].typeAllowed ==equipment.Type){
					equipIndex=i;
					break;
				}
			}
		}
		returnObject = gear [equipIndex].Item;
		gear [equipIndex].Item = (IItem)equipment;
		//animated Item
		if (equipment.StichableObjects != null && equipment.StichableObjects.Length > 0) {
			EquipAnimatedItem(equipment, (Equipment)returnObject);
		}
		// static object item
		else if (equipment.Type == ItemType.WEAPON || equipment.Type == ItemType.OFFHAND) {
			Weapon temp = (Weapon)equipment as Weapon;
			if (returnObject != null) {
				Weapon returnTemp = (Weapon)returnObject as Weapon;
				returnTemp.WeaponModel.transform.parent = returnTemp.GO.transform;
				returnTemp.WeaponModel.SetActive (false);
			}
			temp.WeaponModel.SetActive (true);
			temp.WeaponModel.transform.parent = mainHand.transform;
			temp.WeaponModel.transform.localPosition = Vector3.zero;
			temp.WeaponModel.transform.localRotation = temp.LocalRot;
		} else if (equipment.Type == ItemType.RING) {
			for (int i=0; i<gear.Length; ++i){
				if (gear[i].typeAllowed == ItemType.RING && gear[i].Item==null){
					gear[i].Item=returnObject;
					returnObject=null;
				}
			}
		}

		GlobalEvents.instance.LaunchOnGearChange();
		return returnObject;
	}


	public IItem Equip (IItem equipment, int index){
		IItem returnObject=null;
		Equipment temp = (Equipment)equipment as Equipment;
		if (gear [index].typeAllowed == equipment.Type) {
			returnObject = gear [index].Item;
			gear [index].Item = equipment;
		

			if (temp.StichableObjects != null && temp.StichableObjects.Length > 0) {
				EquipAnimatedItem (temp, (Equipment)returnObject);
			} else if (equipment.Type == ItemType.WEAPON || equipment.Type == ItemType.OFFHAND) {
				EquipWeapon ((Weapon)equipment, (Weapon)returnObject);
			}
			GlobalEvents.instance.LaunchOnGearChange ();
		} else {
			returnObject = equipment;
		}

		return returnObject;
	}

	public void UnEquip (int index){

		//Unequipping current Item
		if (gear [index].Item!=null){
			inventory.AddItem(gear [index].Item);
			Equipment temp = (Equipment) gear [index].Item as Equipment; 
			if (temp.StichableObjects != null && temp.StichableObjects.Length>0) {
				int animatedModelIndex=0;
				for (int i=0; i<animatedModels.Length; ++i){
					if (temp.Type == animatedModels[i].type){
						animatedModelIndex = i;
						break;
					}
				}

				for (int i2=0; i2<temp.StichableObjects.Length; ++i2){
					Destroy(animatedModels[animatedModelIndex].models[i2]);
				}
	
				//"equipping" default models
				for (int i2=0; i2<defaultAnimatedModels[animatedModelIndex].models.Length; ++i2){
					defaultAnimatedModels[animatedModelIndex].models[i2].SetActive(true);
					//sticher.AddLimb(defaultAnimatedModels[i].models[i2]);
				}

			}else if(gear [index].Item is Weapon){
				Weapon temp2 = (Weapon)gear [index].Item;
				temp2.WeaponModel.transform.parent = temp2.GO.transform;
				temp2.WeaponModel.SetActive (false);
			}
			gear [index].Item = null;
			GlobalEvents.instance.LaunchOnGearChange();
		}

	}
	public IItem GetEquipment (ItemType type){
		for (int i=0; i<gear.Length; ++i){
			if (gear[i].Item==null){
				continue;
			}
			if (type ==gear[i].Item.Type){
				return gear[i].Item;
			}
		}
		return null;
	}
	private void EquipAnimatedItem(Equipment newItem, Equipment oldItem){
		int animatedModelIndex = 0;
		for (int i=0; i<animatedModels.Length; ++i) {
			if (newItem.Type == animatedModels [i].type) {
				animatedModelIndex = i;
				break;
			}
		}
		if (oldItem != null) {
			
			Equipment temp = (Equipment)oldItem as Equipment;
			for (int i2=0; i2<temp.StichableObjects.Length; ++i2) {
				Destroy (animatedModels [animatedModelIndex].models [i2]);
			}
		} else {
			for (int i2=0; i2<defaultAnimatedModels[animatedModelIndex].models.Length; ++i2) {
				defaultAnimatedModels [animatedModelIndex].models [i2].SetActive (false);
			}
		}
		for (int i=0; i<newItem.StichableObjects.Length; ++i) {
			newItem.StichableObjects [i].SetActive (true);
			animatedModels [animatedModelIndex].models [i] = sticher.AddLimb (newItem.StichableObjects [i]);
		}
	}

	private void EquipWeapon(Weapon newItem, Weapon oldItem){
		if (oldItem != null) {
			oldItem.WeaponModel.transform.parent = oldItem.GO.transform;
			oldItem.WeaponModel.SetActive (false);
		}
		newItem.WeaponModel.SetActive (true);
		newItem.WeaponModel.transform.parent = mainHand.transform;
		newItem.WeaponModel.transform.localPosition = Vector3.zero;
		newItem.WeaponModel.transform.localRotation = newItem.LocalRot;
	}
}
