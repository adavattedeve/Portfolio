using UnityEngine;
using System.Collections;

public class InstantiateUIObjects : MonoBehaviour {


	public GameObject uiPrefab;

	public int amountX, amountY;
	public float width, heigth;
	[Header("Ingore if not instatiating unit slots")]
	public TroopLocation unitLocation;
	[Header("Ingore if not instatiating item slots")]
	public ItemLocation itemLocation;
	// Use this for initialization
	void Awake () {
		RectTransform tempRect;
		float offsetX = (1f - amountX * width) / (amountX + 1);
		float offsetY = (1f - amountY * heigth) / (amountY + 1);
		for (int y=0; y<amountY; ++y) {
			for (int x=0; x<amountX; ++x) {
				tempRect = Instantiate(uiPrefab).GetComponent<RectTransform>();
				tempRect.SetParent(transform, false);
				tempRect.anchorMin = new Vector2(offsetX+x*width+x*offsetX,
				                                 offsetY+y*heigth+y*offsetY);
				tempRect.anchorMax = new Vector2(tempRect.anchorMin.x+width,
				                                 tempRect.anchorMin.y+heigth);
				tempRect.anchoredPosition = Vector2.zero;
				IIndexable indexable = tempRect.GetComponent(typeof(IIndexable)) as IIndexable;
				if (indexable!=null){
					indexable.Index = x+(amountY-1-y)*(amountX); 
				}
				if (indexable is IUnitUI){
					IUnitUI temp = (IUnitUI) indexable;
					temp.TroopLocation = unitLocation;
				}
				if (indexable is IItemUI){
					IItemUI temp = (IItemUI) indexable;
					temp.ItemLocation = itemLocation;
				}

			}
		}
	}


}
