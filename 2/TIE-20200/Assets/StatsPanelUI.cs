using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class StatsPanelUI : MonoBehaviour {
	public GameObject statUIPrefab;
	public float anchorYPerStat=0.46f/8;
	private RectTransform rect;
	List<StatUI> statUIs;
	void Awake(){
		statUIs = new List<StatUI> ();
		rect = GetComponent<RectTransform> ();
	}
	public void CreateStats(Stats stats){
		RectTransform tempRect;

		while (statUIs.Count<stats.stats.Count) {
			statUIs.Add (Instantiate (statUIPrefab).GetComponent<StatUI> ());
		}
		for (int i=0; i< statUIs.Count; ++i) {
			statUIs[i].gameObject.SetActive(false);
		}
		float offsetY = 1f/stats.stats.Count;
		for (int y=stats.stats.Count-1; y>=0; --y) {
			statUIs[stats.stats.Count-1-y].gameObject.SetActive(true);
			statUIs[stats.stats.Count-1-y].Refresh(stats.stats[stats.stats.Count-1-y]);
			tempRect = statUIs[stats.stats.Count-1-y].GetComponent<RectTransform>();
			tempRect.SetParent(transform, false);
			tempRect.anchorMin = new Vector2(0,y*offsetY);
			tempRect.anchorMax = new Vector2(1,y*offsetY+offsetY);
			tempRect.offsetMax = Vector2.zero;
			tempRect.offsetMin = Vector2.zero;

		}
		rect.anchorMin = new Vector2 (rect.anchorMin.x, rect.anchorMax.y-stats.stats.Count*anchorYPerStat);
		rect.offsetMin =  Vector2.zero;
	}
}
