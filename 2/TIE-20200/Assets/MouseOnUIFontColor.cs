using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
public class MouseOnUIFontColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public Text text;
	private Button button;
	void Awake(){
		button = GetComponent<Button> ();
	}
	public void OnPointerEnter(PointerEventData e){ if (button.interactable)text.color = DataBase.instance.gameData.textColorActive;}
	public void OnPointerExit(PointerEventData e){text.color = DataBase.instance.gameData.textColorInActive;}
}
