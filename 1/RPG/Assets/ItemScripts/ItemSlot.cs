using UnityEngine;
using System.Collections;

public class ItemSlot{
	public int index;
	public ItemType typeAllowed;
	private IItem item;
	public IItem Item {
		get{return item;}
		set{ if (value==null){
				item=null;
			}
			else if (typeAllowed==ItemType.ANY){
				item=value;
			}else if (typeAllowed==value.Type){
				item = value;
			}
		}
	}

	public ItemSlot(int _index, ItemType _type){
		index = _index;
		typeAllowed = _type;
	}

}
