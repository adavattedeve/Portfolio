using UnityEngine;
using System.Collections;

public class PanelControl : MonoBehaviour {

	public GameObject[] panelObjects;
	public GameObject[] additionalTurnOffObjects;
	public void OpenPanel(int index){
		for (int i=0; i<panelObjects.Length; ++i) {
			if (i!=index)panelObjects[i].SetActive(false);
			else panelObjects[i].SetActive(true);
		}
		if (additionalTurnOffObjects == null) {
			return;
		}
			for (int i=0; i<additionalTurnOffObjects.Length; ++i) {
				additionalTurnOffObjects [i].SetActive (false);
			}
	}
	public void ClosePanels(){
		for (int i=0; i<panelObjects.Length; ++i) {
			panelObjects[i].SetActive(false);
		}
		if (additionalTurnOffObjects == null) {
			return;
		}
		for (int i=0; i<additionalTurnOffObjects.Length; ++i) {
			additionalTurnOffObjects[i].SetActive(true);
		}
	}
}
