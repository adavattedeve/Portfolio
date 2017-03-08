using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitInfoPanelUI: MonoBehaviour {
	public float widthAnchorDelta;
	public float heightAnchorDelta;

	public Image unitIcon;
	public Text nameText;
	private StatsPanelUI statsPanelUI;
	
	private RectTransform rect;
	void Awake () {
		rect = GetComponent<RectTransform> ();
		statsPanelUI = GetComponentInChildren<StatsPanelUI> ();
	}
	public void Display(Unit unit, Vector2 position){
		nameText.text = unit.name;
		unitIcon.sprite = unit.Icon;
		statsPanelUI.CreateStats (unit.stats);
		Vector2 offset = Vector2.zero;
		if (position.y+heightAnchorDelta*Screen.height > Screen.height){
			offset.y +=Screen.height-(position.y+rect.sizeDelta.y);
		}
		if (position.x + widthAnchorDelta*Screen.width  > Screen.width) {
			offset.x +=Screen.width-(position.x+rect.sizeDelta.x);
		}
		offset.x += position.x;
		offset.y += position.y;
		rect.anchorMin = new Vector2( offset.x / Screen.width, offset.y / Screen.height);
		rect.anchorMax = new Vector2( rect.anchorMin.x+widthAnchorDelta, rect.anchorMin.y +heightAnchorDelta);
		rect.offsetMax = Vector2.zero;
		rect.offsetMin = Vector2.zero;

	}
}
