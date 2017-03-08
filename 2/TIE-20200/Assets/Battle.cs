using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public enum QuestReward{NORMAL, NEWUNIT,UNITPOPULATION , NEWTOWN}
[System.Serializable]
public class Battle {
	public string name;
	public QuestReward rewardType;
	public Troop troop;
	public string descriptionText;
	public List<int> possibleUnitIds;
	[SerializeField]protected  float minValue, maxValue; //troop strength where 1 is basic strength in current town
	[SerializeField][Range(0f,1f)]protected float chancesForAdditionalReward;
	[SerializeField]protected int rewardTableID;
	private int minStacks = 2;
	private int maxStacks=6;

//	private int goldReward;
//	public int GoldReward{get{
//			//calculat3e gold reward
//			return goldReward;
//		}}
//	private float experienceReward;
//	public float ExperienceReward{get{
//			//calculat3e expierenceReward reward
//			return experienceReward;
//		}}

	public void CalculateTroops(int baseDifficultyValue){
		troop = new Troop ();
		List<float> distributionValues = new List<float> ();
		float sum = 0;
		int stackCount = Random.Range (minStacks, maxStacks+1);
		int totalTroopValue =(int) (baseDifficultyValue*Random.Range(minValue, maxValue));
		//unit distibution in troop
		if (possibleUnitIds.Count > 1) {
			for (int i=0; i<possibleUnitIds.Count; ++i) {
				float random = Random.Range (0f, 1f);
				if (random < 0.15f) {
					distributionValues.Add (0);
					continue;
				}
				distributionValues.Add (random);
				sum += random;
			}
		}

		if (sum == 0) {
			int random = Random.Range (0, possibleUnitIds.Count);
			for (int i=0; i<stackCount;++i){
				Unit unit = DataBase.instance.GetUnit (possibleUnitIds [random]);
				unit.amount = (int)((totalTroopValue / unit.goldValue)/stackCount);
				troop.units.Add (unit);
			}
			Debug.Log(troop.units.Count +" stack count is : " + stackCount);
		} else {
			for (int i=0; i<possibleUnitIds.Count; ++i) {

				if (distributionValues [i] > 0) {
					int stacks=1;

					while ((distributionValues [i] / sum)>sum/stackCount){
						distributionValues[i] /=2;
						++stacks;
					}
					for (int ii=0; ii<stacks;++ii){
						Unit unit = DataBase.instance.GetUnit (possibleUnitIds [i]);
						unit.amount = Mathf.CeilToInt((distributionValues [i] / sum) * (totalTroopValue / unit.goldValue));
						troop.units.Add (unit);
					}
					
				}
				
			}
		}
	}
	public void UnitGrowth(float mpl){
		for (int i=0; i<troop.units.Count; ++i) {
			if (troop.units[i]!=null){
				troop.units[i].amount =Mathf.CeilToInt((troop.units[i].amount*mpl)); 
			}
		}
	}
	public List<Reward> GetAdditionalRewards(int baseDifficultyValue){
		RewardTable table = DataBase.instance.GetRewardTable(rewardTableID);
		List<Reward> additionalRewards =new List<Reward> ();
		float random = Random.Range (0,1f);

		if (random < chancesForAdditionalReward) {
			additionalRewards.Add(table.GetReward(baseDifficultyValue));
			if (random*3<chancesForAdditionalReward){
				additionalRewards.Add(table.GetReward(baseDifficultyValue));
			}
		}
		return additionalRewards;
	}
}
