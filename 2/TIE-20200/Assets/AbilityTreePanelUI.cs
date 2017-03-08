using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class AbilityTreePanelUI : MonoBehaviour {
	public Button[] branchButtons;
	public Text abilityPointsLeftText;
	private int selectedBranch;
	public int SelectedBranch{set{
			if (selectedBranch>=0){
				branchButtons[selectedBranch].GetComponent<MouseOnButtonAnimation>().Selected=false;
			}
			Debug.Log (value + " selected");
			branchButtons[value].GetComponent<MouseOnButtonAnimation>().Selected=true;
			selectedBranch=value;
			Refresh();}}


	public GameObject tierPrefab;
	private List<AbilityPanelUI> tierUIs;
	void Awake(){
		for (int i=0; i<branchButtons.Length; ++i) {
			int temp=i;
			branchButtons[i].onClick.AddListener(delegate {
				SelectedBranch=temp;
		});
		}
		tierUIs = new List<AbilityPanelUI> ();
		Debug.Log ("settinh Selected Branch");
		SelectedBranch = 0;
	}
	void OnEnable(){
		GameManager.instance.CurrentGame.playerTroop.hero.OnSpellLearned += Refresh;
	}
	void OnDisable(){
		GameManager.instance.CurrentGame.playerTroop.hero.OnSpellLearned -= Refresh;
	}
	public void Refresh(){
		RectTransform tempRect;
		abilityPointsLeftText.text = GameManager.instance.CurrentGame.playerTroop.hero.abilityPoints.ToString ();
		Branch currentBranch = GameManager.instance.CurrentGame.playerTroop.hero.abilityTree.tree [selectedBranch];
		while (tierUIs.Count<=currentBranch.maxTier) {
			tierUIs.Add (Instantiate (tierPrefab).GetComponent<AbilityPanelUI> ());
		}
		for (int i=0; i< tierUIs.Count; ++i) {
			tierUIs[i].gameObject.SetActive(false);
		}
		float offsetY = 1f/(currentBranch.maxTier+1);
		for (int i=0; i<=currentBranch.maxTier; ++i) {
			tierUIs[i].gameObject.SetActive(true);
			tierUIs[i].Refresh(currentBranch.GetAbilitiesFromBranchAtTier(i));
			tempRect = tierUIs[i].GetComponent<RectTransform>();
			tempRect.SetParent(transform, false);
			tempRect.anchorMin = new Vector2(0, offsetY*i);
			tempRect.anchorMax = new Vector2(1, offsetY*i+offsetY);
			tempRect.offsetMax = Vector2.zero;
			tempRect.offsetMin = Vector2.zero;
			
		}
	}
}
