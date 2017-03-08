using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterStats : MonoBehaviour {
	public GameObject mainHand;
	public float width;
	private Transform[] weaponRays;
	public Transform[] WeaponRays { get { return weaponRays; } }
	private float[] weaponRayDistances;
	public float[] WeaponRayDistances { get { return weaponRayDistances; } }
	public bool immortal;

	public float baseArmor;
	public float baseMaxHealth;
	public float baseMaxStability;
	public float baseMinDamage;
	public float baseMaxDamage;
	public float baseImpact;

	public float armorPerLevel;
	public float healthPerLevel;
	public float minDamagePerLevel;
	public float maxDamagePerLevel;

	protected Stat[] baseStats;
	public Stat[] stats;

	protected List<Stat> buffs;

	public LayerMask damageLayer;
	public int characterLevel=1;

	void Awake(){
		baseStats = new Stat[6];
		baseStats [0] = new Stat (StatType.ARMOR, baseArmor,armorPerLevel);
		baseStats [1] = new Stat (StatType.MAXHEALTH, baseMaxHealth,healthPerLevel);
		baseStats [2] = new Stat (StatType.STABILITY, baseMaxStability,0);
		baseStats [3] = new Stat (StatType.IMPACT, baseImpact,0);
		baseStats [4] = new Stat (StatType.DAMAGEMAX, baseMaxDamage,maxDamagePerLevel);
		baseStats [5] = new Stat (StatType.DAMAGEMIN, baseMinDamage,minDamagePerLevel);
		stats = new Stat[baseStats.Length];
		for (int i=0; i<stats.Length; ++i) {
			stats[i] = new Stat (baseStats[i].type, baseStats[i].amount, baseStats[i].perLevel);
		}

		buffs = new List<Stat>();
	}
	void Start(){
		RefreshWeaponInfo ();
		AdditionalStartInit ();
	}
	protected virtual void AdditionalStartInit (){}
	public void RefreshWeaponInfo(){
		WeaponModelInfo weapon = mainHand.GetComponentInChildren<WeaponModelInfo> ();
		if (weapon != null) {
			weaponRays = weapon.Rays;
			weaponRayDistances = weapon.rayRanges;
		} else {
			weaponRays = null;
			weaponRayDistances = null;
		} 
	}

	public void Buff(Stat buff, float time){
		buffs.Add(buff);
		StartCoroutine (buffTimer(buff, time));
	}
	private IEnumerator buffTimer (Stat buff, float time){
		GlobalEvents.instance.LaunchOnStatChange ();
		yield return new WaitForSeconds(time);
		buffs.Remove(buff);
		GlobalEvents.instance.LaunchOnStatChange ();
	}

	public float GetStat(StatType type){
		for (int i=0; i<stats.Length; ++i) {
			if (type == stats[i].type){
				return stats[i].amount+stats[i].perLevel*characterLevel;
			}
		}
		return 0;
	}
}
