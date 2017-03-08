using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class UnitCountUI : MonoBehaviour {
	public Text unitCountText;
	public Image unitCountImage;
	private int unitCount;
	private List<Effect> effects;
	private List<EffectUI> effectUIs;
	private RectTransform rectTransform;
	public GameObject effectSlotPrefab;
	public float effectSlotWidth=1f;
	void Awake(){
		effects = new List<Effect> ();
		effectUIs = new List<EffectUI> ();
		rectTransform = GetComponent<RectTransform> ();

	}
	public void Initialize(Unit unit){
		unitCount = unit.amount;
		unitCountText.text = unit.amount.ToString ();
		Sprite sprite;
		if (CombatManager.instance.GetPlayer (unit.owner).control == Control.INPUT) {
			sprite = DataBase.instance.GetSprite ("UnitCountAlly");
		} else {
			sprite = DataBase.instance.GetSprite ("UnitCountEnemy");
		}
		if (sprite != null) {
			unitCountImage.sprite = sprite;
		}
	}
	public void Refresh(int change){
		unitCount += change;
		unitCountText.text = unitCount.ToString ();
	}
	public void AddEffect(Effect effect){
		if (rectTransform == null) {
			rectTransform=GetComponent<RectTransform>();
		}
		rectTransform.sizeDelta = new Vector2 (effectSlotWidth+rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
		RectTransform temp = (Instantiate (effectSlotPrefab) as GameObject).GetComponent<RectTransform> ();
		temp.SetParent (rectTransform, false);
		temp.sizeDelta = new Vector2 (effectSlotWidth, temp.sizeDelta.y);
		temp.anchoredPosition = new Vector2 ((-1)*effects.Count*effectSlotWidth - effectSlotWidth, 0);
		EffectUI effectUI = temp.GetComponent<EffectUI> ();
		effectUI.EffectAdded(effect);
		effectUIs.Add (effectUI);
		effects.Add (effect);

	}
	public void RemoveEffect(Effect effect){
		bool foundEffect = false;
		for (int i=0; i<effects.Count; ++i) {
			if (effects[i] == effect){
				foundEffect = true;
				effectUIs[i].EffectRemoved();
				effectUIs.RemoveAt(i);
				effects.RemoveAt(i);
			}
			if (foundEffect && i<effects.Count){
				effectUIs[i].MoveOneRigth();
			}
		}
		rectTransform.sizeDelta = new Vector2 (rectTransform.sizeDelta.x-effectSlotWidth, rectTransform.sizeDelta.y);
	}
}
