using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interact : MonoBehaviour {
	string interactTag="Interactable";
	List<IInteractable> interactablesInRange;
	void Awake () {
		interactablesInRange = new List<IInteractable> ();
	}


	void OnTriggerEnter(Collider other){
			interactablesInRange.Add((IInteractable)other.GetComponent(typeof(IInteractable)));
		SetNextInteractableReady ();
	}
	void OnTriggerExit(Collider other){
		IInteractable temp = (IInteractable)other.GetComponent (typeof(IInteractable));
		temp.ReadyForInteract = false;
		interactablesInRange.Remove(temp);
	}
	public void InteractFirst(){
		for (int i=0; i< interactablesInRange.Count; ++i) {
			if (interactablesInRange [i] != null) {
				interactablesInRange [i].ReadyForInteract = false;
				interactablesInRange [i].Interact ();
				interactablesInRange [i] = null;
				SetNextInteractableReady();
				return;
			}
		}
	}
	private void SetNextInteractableReady(){
		for (int i=0; i< interactablesInRange.Count; ++i) {
			if (interactablesInRange [i] != null) {
				interactablesInRange [i].ReadyForInteract=true;
				break;
			}
		}
	}

}
