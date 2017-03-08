using UnityEngine;
using System.Collections;

[System.Serializable]
public class RewardTable {
	public int id;
	public float itemDistribution=0.2f;
	public float goldDistribution=0.4f;
	public float expDistribution=0.4f;
	public float rewardVariationFloor=0.75f;
	public float rewardVariationCeiling=1.25f;

	public Reward GetReward(int baseGoldValue){
		Reward reward = new Reward ();
		float totalDistributionValue = itemDistribution + goldDistribution + expDistribution;
		float random = Random.Range (0, totalDistributionValue);
		int rewardValue = (int)(Random.Range (rewardVariationFloor,rewardVariationCeiling) * baseGoldValue);
		if (random <= itemDistribution) {
			reward.type = RewardType.ITEM;
			int[] itemDistributionValues = DataBase.instance.itemDistributionValues;
			totalDistributionValue=0;
			for (int i=0; i<itemDistributionValues.Length; ++i){
				totalDistributionValue+=itemDistributionValues[i];
			}
			random = Random.Range(0f, totalDistributionValue);
			for (int i=0; i<itemDistributionValues.Length; ++i){
				if (random<=itemDistributionValues[i]){
					reward.rewardValue=i;
					return reward;
				}
				random+=itemDistributionValues[i];
			}

		} else if ((random + itemDistribution) <= goldDistribution) {
			reward.type = RewardType.GOLD;
			reward.rewardValue= rewardValue;

		} else {
			reward.type = RewardType.EXP;
			reward.rewardValue= (int)(rewardValue* DataBase.instance.gameData.goldToExperienceMpl);
		}
		return reward;
	}
}
