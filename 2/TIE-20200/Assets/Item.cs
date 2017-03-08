using UnityEngine;
using System.Collections;
public enum ItemType{WEAPON, ARMOR, ARTIFACT, NULL}
[System.Serializable]
public class Item: Entity {
	public int id ;
	public string name;
	public string description;
	public string iconPath;
	[System.NonSerialized]private Sprite icon;
	public Sprite Icon {get{
			if (icon != null) {
				return icon;
			}else { 
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
	public ItemType type ;
	public int goldValue;

	public virtual void Use(Hero hero){}
}
