using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RewardTableData : ScriptableObject {
	[SerializeField]private List<RewardTable> rewardTables;
	public RewardTable[] GetTables(){
		return rewardTables.ToArray ();
	}
}
