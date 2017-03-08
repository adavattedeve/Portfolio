using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour {
	private List<Ability> abilities;
	private Ability[] equippedAbilities;
	public Ability[] EquippedAbilities{get{return equippedAbilities;}}
	private Ability currentlyCasting;
	public Ability CurrentlyCasting{get{ return currentlyCasting; }set{ currentlyCasting = value; }}
	// Use this for initialization


	private bool damaging;
	void Awake () {
		equippedAbilities= new Ability[1];
		currentlyCasting = null;
		damaging = false;
		//placeholöder
		equippedAbilities [0] = new Ability ("BasicAttack", 1, 1, 2);
	}

	public void SetCurrentlyCasting(int actionID){
		for (int i=0; i< equippedAbilities.Length; ++i) {
			if (equippedAbilities[i].ActionID == actionID){
				currentlyCasting=equippedAbilities[i];
			}
		}
	}
}
