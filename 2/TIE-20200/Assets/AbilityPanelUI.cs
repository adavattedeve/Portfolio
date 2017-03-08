using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AbilityPanelUI : MonoBehaviour {
	public AbilityLocation location;
	public GameObject abilitySlotPrefab;
	public float anchorSizeY=0.6f;
	public int rows;
	public int abilitiesInRow;
	private RectTransform rect;
	private List<AbilitySlotUI> abilityUIs;
	void Awake(){
		abilityUIs = new List<AbilitySlotUI> ();
		rect = GetComponent<RectTransform> ();
	}

	public void Refresh(List<Ability> abilities){
		RectTransform tempRect;
		while (abilityUIs.Count<abilities.Count) {
			abilityUIs.Add (Instantiate (abilitySlotPrefab).GetComponent<AbilitySlotUI> ());
		}
		for (int i=0; i< abilityUIs.Count; ++i) {
			abilityUIs[i].gameObject.SetActive(false);
		}
		float rowHeight = 1f / rows;
		float offsetX = 1f/abilitiesInRow;
		int currentRow;
		int currentAbilityInRow;
		for (int i=0; i<abilities.Count; ++i) {
			currentRow = i/abilitiesInRow;
			currentAbilityInRow = i%abilitiesInRow;
			abilityUIs[i].gameObject.SetActive(true);
			abilityUIs[i].Refresh(abilities[i]);
			tempRect = abilityUIs[i].GetComponent<RectTransform>();
			tempRect.SetParent(transform, false);
			tempRect.anchorMin = new Vector2(currentAbilityInRow*offsetX, (1-rowHeight*currentRow-rowHeight)+((rowHeight-anchorSizeY*rowHeight)/2));
			tempRect.anchorMax = new Vector2(currentAbilityInRow*offsetX+offsetX, (1-rowHeight*currentRow)-((rowHeight-anchorSizeY*rowHeight)/2));
			tempRect.offsetMax = Vector2.zero;
			tempRect.offsetMin = Vector2.zero;
			tempRect.anchoredPosition = Vector2.zero;
			abilityUIs[i].Location=location;
		}
	}
	public void Refresh(List<AbilityInBranch> abilitiesBranch){
		RectTransform tempRect;
		while (abilityUIs.Count<abilitiesBranch.Count) {
			abilityUIs.Add (Instantiate (abilitySlotPrefab).GetComponent<AbilitySlotUI> ());
		}
		for (int i=0; i< abilityUIs.Count; ++i) {
			abilityUIs[i].gameObject.SetActive(false);
		}
		float offsetX = 1f/abilitiesBranch.Count;
		for (int i=0; i<abilitiesBranch.Count; ++i) {
			abilityUIs[i].gameObject.SetActive(true);
			abilityUIs[i].AbilityInBranch = abilitiesBranch[i];
			tempRect = abilityUIs[i].GetComponent<RectTransform>();
			tempRect.SetParent(transform, false);
			tempRect.anchorMin = new Vector2(i*offsetX, 0.5f-anchorSizeY/2);
			tempRect.anchorMax = new Vector2(i*offsetX+offsetX, 0.5f+anchorSizeY/2);
			tempRect.offsetMax = Vector2.zero;
			tempRect.offsetMin = Vector2.zero;
			tempRect.anchoredPosition = Vector2.zero;
			
		}
	}
}
