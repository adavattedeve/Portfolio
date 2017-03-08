using UnityEngine;
using System.Collections;

public class PlayerStats : CharacterStats {
	private Gear gear;

	protected override void AdditionalStartInit ()
	{
		gear = GetComponent<Gear> ();
		GlobalEvents.instance.OnGearChange += CalculateStats;
		GlobalEvents.instance.OnGearChange += RefreshWeaponInfo;
	}
	public void CalculateStats(){
		
		for (int i=0; i<stats.Length; ++i) {
			stats[i].amount = baseStats[i].amount + baseStats[i].perLevel*characterLevel;
		}
		for (int i=0; i<buffs.Count; ++i) {
			stats[i].amount += buffs[i].amount + buffs[i].perLevel*characterLevel;
		}
		for (int i=0; i<gear.GearSlots.Length; ++i) {
			if (gear.GearSlots [i].Item==null){
				continue;
			}
			Equipment temp = (Equipment)gear.GearSlots [i].Item as Equipment;
			if (temp.stats==null || temp.stats.Count<=0){
				continue;}
			for (int i2=0; i2<temp.stats.Count; ++i2) {
				for (int i3=0; i3<stats.Length; ++i3) {
					if (stats[i3].type ==temp.stats[i2].type){
						stats[i3].amount +=temp.stats[i2].amount;
					}
				}
			}
		}
	}
}
