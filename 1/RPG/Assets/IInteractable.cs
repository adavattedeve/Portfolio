using UnityEngine;
using System.Collections;

public interface IInteractable {
	bool ReadyForInteract{ set;}
	void Interact();
}
