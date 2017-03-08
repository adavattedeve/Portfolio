using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemInfoField : MonoBehaviour {
	public float defaultHeigth;
	public float perStatHeigth;
	public Image itemImage;
	public Text nameText;
	public Text statText;
	public Text descriptionText;

	[System.NonSerialized]public RectTransform rect;
	void Awake () {
		rect = GetComponent<RectTransform> ();
	}
	public float DescriptionOn(Vector3 screenSpacePosition, IItem item){
		if (item is Equipment) {
			Equipment temp = (Equipment)item;
			rect.sizeDelta = new Vector2(rect.sizeDelta.x, defaultHeigth+temp.stats.Count * perStatHeigth);
		} else {
			rect.sizeDelta = new Vector2(rect.sizeDelta.x, defaultHeigth);
		}
		nameText.text = item.Name;
		statText.text = item.GetAdditionalInfoAsString ();
		descriptionText.text = item.Description;
		itemImage.sprite = item.Icon;
		Vector2 offset = Vector2.zero;
		if (screenSpacePosition.y+rect.sizeDelta.y > Screen.height){
			offset.y +=Screen.height-(screenSpacePosition.y+rect.sizeDelta.y);
		}
		if (screenSpacePosition.x + rect.sizeDelta.x > Screen.width) {
			offset.x +=Screen.width-(screenSpacePosition.x+rect.sizeDelta.x);
		}
		offset.x += screenSpacePosition.x;
		offset.y += screenSpacePosition.y;
		rect.anchoredPosition = offset;
		return offset.x-rect.sizeDelta.x;
	}
}
