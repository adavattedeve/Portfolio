using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleBranchData : ScriptableObject {
	public List<int> battleHeroIds;
	public List<int> battleHeroLvls;
	public List<SerializableIntList> battleUnitIds;
	public List<int> battleUnitAmounts;

}
