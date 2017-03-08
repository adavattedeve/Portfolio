using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityData : ScriptableObject {
	public CantRetalitiate cantRetalitiate;
	public DoubleStrike doubleStrike;
	public Fear fear;
	public Flying flying;
	public HitAndRun hitAndRun;
	public MasterArchery masterArchery;
	public MeleePenalty meleePenalty;
	public Stealth stealth;
	public Taunt taunt;
	public UnlimitedRetalitions unlimitedRetalitions;
	public ExtendedAttack extendedAttack;

	//SPELLS
	public Blessing blessing;
	public MassBlessing massBlessing;
	public Earthquake earthquake;
	public FireExplosion fireExplosion;
	public Haste haste;
	public MassHaste massHaste;
	public MindlesAgression mindlesAgression;
	public Plague plague;
	public Slow slow;
	public Teleport teleport;
	public Confusion confusion;
	public MagicBlast magicBlast;

	public Ability[] GetAbilities(){
		List<Ability> abilities = new List<Ability>();
		abilities.Add (cantRetalitiate);
		abilities.Add (doubleStrike);
		abilities.Add (fear);
		abilities.Add (flying);
		abilities.Add (hitAndRun);
		abilities.Add (masterArchery);
		abilities.Add (meleePenalty);
		abilities.Add (stealth);
		abilities.Add (taunt);
		abilities.Add (unlimitedRetalitions);
		abilities.Add (extendedAttack);

		//Spells
		abilities.Add (blessing);
		abilities.Add (massBlessing);
		abilities.Add (earthquake);
		abilities.Add (fireExplosion);
		abilities.Add (haste);
		abilities.Add (massHaste);
		abilities.Add (mindlesAgression);
		abilities.Add (plague);
		abilities.Add (slow);
		abilities.Add (teleport);
		abilities.Add (magicBlast);
		abilities.Add (confusion);

		return abilities.ToArray ();
	}
}
