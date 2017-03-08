using UnityEngine;
using System.Collections;
[System.Serializable]
public class MagicBlast : Spell, ISpellDamageComponent {
	public int CalculateDamage(int intelligence){
		int damage = stats.GetStat (StatType.DAMAGE).Value + intelligence*stats.GetStat (StatType.DAMAGESCALING).Value;
		return damage;
	}
}
