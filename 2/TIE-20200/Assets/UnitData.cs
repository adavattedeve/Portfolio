using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitData : ScriptableObject {
	public List<Unit> units;

	public Unit[] GetUnits(){
		return units.ToArray();
	}
}
