using UnityEngine;
using System.Collections;

public class GlobalEvents : MonoBehaviour {
	public static GlobalEvents instance;

	public delegate void GearChangeAction();
	public event GearChangeAction OnGearChange;

	public delegate void InventoryChangeAction();
	public event GearChangeAction OnInventoryChange;

	public delegate void StatChangeAction();
	public event StatChangeAction OnStatChange;

	void Awake(){
		instance = this;
	}
	void OnLevelWasLoaded(){
		OnGearChange = null;
		OnInventoryChange = null;
		OnStatChange = null;
}
	public void LaunchOnGearChange(){
		OnGearChange ();
		OnStatChange ();
	}
	public void LaunchOnInventoryChange(){
		OnInventoryChange ();
	}
	public void LaunchOnStatChange(){
		OnStatChange ();
	}
}
