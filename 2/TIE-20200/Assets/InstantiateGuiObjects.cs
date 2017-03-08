using UnityEngine;
using System.Collections;

public class InstantiateGuiObjects : MonoBehaviour {
	public GameObject prefab;
	public int amountX, amountY;
	public float width, heigth;
	// Use this for initialization
	void Awake () {
		RectTransform tempRect;
		float offsetX = (1f - amountX * width) / (amountX + 1);
		float offsetY = (1f - amountY * heigth) / (amountY + 1);
		for (int y=0; y<amountY; ++y) {
			for (int x=0; x<amountX; ++x) {
				tempRect = Instantiate(prefab).GetComponent<RectTransform>();
				tempRect.SetParent(transform, false);
				tempRect.anchorMin = new Vector2(offsetX+x*width+x*offsetX,
				                                 offsetY+y*heigth+y*offsetY);
				tempRect.anchorMax = new Vector2(tempRect.anchorMin.x+width,
				                                 tempRect.anchorMin.y+heigth);
				tempRect.anchoredPosition = Vector2.zero;
			}
		}
	}


}
