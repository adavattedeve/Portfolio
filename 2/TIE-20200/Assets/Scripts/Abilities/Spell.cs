using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public enum TargetingType{ALLFRIENDLY, ALLENEMY, UNIT, UNOCCUPIEDNODE ,TARGETEDAOE}
[System.Serializable]
public class Spell : Ability {
	public string visualEffectName="";
	protected VisualEffectLauncher visualEffect;
	public VisualEffectLauncher VisualEffect{get{
			if (visualEffect == null) {
				GameObject prefab;
				if (visualEffectName==""){
					prefab = DataBase.instance.GetVisualEffect (name);
				}else {
					prefab = DataBase.instance.GetVisualEffect (visualEffectName);
				}
				if (prefab != null) {
					prefab = (MonoBehaviour.Instantiate (prefab) as GameObject);
					visualEffect = (VisualEffectLauncher)(prefab.GetComponent(typeof(VisualEffectLauncher)));
					visualEffect.gameObject.SetActive(false);
				}
			}
			return visualEffect;
		}}
	//different types for different phases for example, teleport UNIT, UNOCCUPIEDNODE
	public TargetingType[] targetingTypes;
	public virtual TargetingType targetinType{get{return targetingTypes[0];}}
	public Stats stats;
}
