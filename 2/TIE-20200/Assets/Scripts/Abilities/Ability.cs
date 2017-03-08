using UnityEngine;
using System.Collections;
[System.Serializable]
public enum AbilityIdentifier{NULL, FLYING, DOUBLESTRIKE, NORETALITION, MASTERARCHERY, TAUNT, STEALTH, FEAR, HITANDRUN, UNLIMITEDRETALITIONS, 
MELEEPENALTY, CANTRETALITIATE, EXTENDEDATTACK, EARTHQUAKE, BLESSING, FIREEXPLOSION, HASTE, MAGICBLAST, MASSBLESSING, MASSHASTE, MINDLESAGRESSION, 
PLAGUE, SLOW, TELEPORT, CONFUSION};
[System.Serializable]
public class Ability: Entity {
	public AbilityIdentifier id;
	public string name;
	[System.NonSerialized]private Sprite icon;
	public Sprite Icon {get{
			if (icon != null) {
				return icon;
			} else { 
				icon = DataBase.instance.GetSprite(name);
				if (icon != null) {
					return icon;
				} else {
					Debug.Log ("cant find item sprite with name: " + name);
					return null;
				}
			} 
		}
	}
	public string iconPath;
	public string description;

}
