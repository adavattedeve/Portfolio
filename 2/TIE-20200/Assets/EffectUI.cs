using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EffectUI : MonoBehaviour {
	public Text durationText;
	private Animator anim;
	private Image image;
	private RectTransform rect;
	private Effect effect;
	void Awake(){
		anim = GetComponentInChildren<Animator> ();
		image = GetComponentInChildren<Image> ();
		rect = GetComponent<RectTransform> ();
	}
	public void EffectAdded(Effect _effect){
		effect = _effect;
		RefreshDurationText (effect.duration);
		effect.OnDurationChange += RefreshDurationText;
		image.sprite = DataBase.instance.GetSprite (effect.name);
		anim.SetTrigger ("Enter");
	}
	public void EffectRemoved(){
		effect.OnDurationChange -= RefreshDurationText;
		anim.SetTrigger ("Exit");
	}
	public void MoveOneRigth(){
		rect.anchoredPosition = new Vector2 (rect.anchoredPosition.x +1, rect.anchoredPosition.y);
		anim.SetTrigger ("Move");
	}
	public void RefreshDurationText(int newDurationText){
		durationText.text = newDurationText.ToString ();
	}
}
