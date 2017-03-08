using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class MouseOnButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
	public string buttonName="Button";
	public bool rotate=true;
	public float mouseOverRotationSpeed=100;
	public float mouseOffRotationSpeed=20;
	public float mouseOverScaleSpeed=10;
	public float mouseOffScaleSpeed=4;
	public float maxErrorAngle=0.1f;
	public float maxErrorDistance=0.001f;
	private Quaternion defaultRotation;
	private Vector3 defaultScale;
	private RectTransform rect;
	private bool mouseOnButton;
	public bool MouseOnButton{ set{mouseOnButton=value;}}

	private Button button;
	private Image image;

	private Sprite normal;
	private Sprite pressed;
	private bool selected=false;
	public bool Selected{set{
			if (value && pressed != null) {
				image.sprite = pressed;
			} else if (normal!=null){
				image.sprite = normal;
			}
			selected=value;
		}
	}
	void Awake () {
		mouseOnButton = false;
		rect = GetComponent<RectTransform> ();
		defaultRotation = rect.rotation;
		defaultScale = rect.localScale;
		button = GetComponent<Button> ();
		image = GetComponent<Image> ();
	}
	void Start (){
		normal = DataBase.instance.GetSprite (buttonName);
		pressed = DataBase.instance.GetSprite (buttonName+"Pressed");
		Selected = selected;
	}
	void OnEnable(){
		mouseOnButton = false;
	}
	void Update(){
		if (rotate){
			Rotate(mouseOnButton || selected);
		}
		Scale (mouseOnButton || selected);
	}
	private void Rotate(bool straight){
		if (straight) {
			if (Quaternion.Angle (rect.rotation, Quaternion.identity) > maxErrorAngle) {
				rect.rotation = Quaternion.RotateTowards (rect.rotation, Quaternion.identity, mouseOverRotationSpeed * Time.deltaTime);
			}
		} 
		else {
			if (Quaternion.Angle (rect.rotation, defaultRotation) > maxErrorAngle) {
				rect.rotation = Quaternion.RotateTowards (rect.rotation, defaultRotation, mouseOffRotationSpeed * Time.deltaTime);
			}
		}
	}
	private void Scale(bool up){
		if (up) {
			if (Vector3.Distance(rect.localScale, Vector3.one)>maxErrorDistance){
				rect.localScale=Vector3.MoveTowards(rect.localScale, Vector3.one, mouseOverScaleSpeed* Time.deltaTime );
			}
			
		}
		else{
			if (Vector3.Distance(rect.localScale, defaultScale)>maxErrorDistance){
				rect.localScale=Vector3.MoveTowards(rect.localScale, defaultScale, mouseOffScaleSpeed* Time.deltaTime );
			}
		}
	}
	public void OnPointerEnter(PointerEventData e){
		if (button.interactable) {
			mouseOnButton = true;
			if (pressed != null) {
				image.sprite = pressed;
			}
		}
	}
	public void OnPointerExit(PointerEventData e){
		mouseOnButton = false;
		if (!selected) {
			image.sprite = normal;
		}
	}
	public void OnPointerDown(PointerEventData e){
		mouseOnButton = false;
		rect.localScale = defaultScale;

	}
	public void OnPointerUp(PointerEventData e){
		OnPointerEnter (null);
		//image.sprite = normal;
	}

}
