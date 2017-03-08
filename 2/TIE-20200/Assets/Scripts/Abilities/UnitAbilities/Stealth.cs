using UnityEngine;
using System.Collections;
[System.Serializable]
public class Stealth : Ability, IOnStartMovement{
	[System.NonSerialized] private float damageMpl=1.5f;
	public void OnStartMovement(Unit unit){
		bool applyStealth = true;
		for (int i=0; i<unit.Effects.Count; ++i) {
			if (unit.Effects[i] is StealthEffect){
				unit.Effects[i].EndEffect();
				applyStealth=false;
			}
		}
		if (applyStealth) {
			StealthEffect effect = new StealthEffect();
			effect.Initialize(unit, 3);
		}
	}
}
