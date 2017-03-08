using UnityEngine;
using System.Collections;

public interface IItem {
	int ID{ get; set;}
	GameObject GO {
		get;
	}
	ItemType Type {
		get;
		set;
	}
	GameObject ObjectPrefab{ get; set;}
	string Name {
		get;
		set;
	}
	Sprite Icon {
		get;
		set;
	}
	string Description {
		get;
		set;
	}
	void Use(int inventoryIndex);
	void SetOwnerAndObjectReferences(GameObject owner, GameObject itemObject);
	IItem GetDublicate(int level);
	string GetAdditionalInfoAsString();
}
