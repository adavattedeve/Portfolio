using UnityEngine;
using System.Collections;

public class ScrollPanels : MonoBehaviour {
	enum ScrollingState {SCROLLING, BREAKING, DEFAULT}
	private ScrollingState state;
	public GameObject[] panelObjects;
	public GameObject[] buttonObjects;
	private RectTransform[] panelRect;
	public float maxSpeed=8000;

	public float currentSpeed;
	public float defaultAccerelation=20000;
	public float acceleration;
	public float errorMargin=150;
	public float errorMarginBreaking=10;
	public float breakingSpeedMPL=0.02f;
	public float breakingAccerelationMPL=0.05f;

	private Vector2 newPos;
	private float currentX;
	private float direction;
	private int target = 0;
	//private int current = 0;
	private float defaultPosX;
	void Awake(){
		state = ScrollingState.DEFAULT;
		currentSpeed =0;
		panelRect = new RectTransform[panelObjects.Length];
		for (int i=0; i<panelObjects.Length; ++i) {
			panelRect[i] = panelObjects[i].GetComponent<RectTransform>();
		}
		defaultPosX = panelRect [0].anchoredPosition.x;
		for (int i=0; i<panelObjects.Length; ++i) {
			panelObjects[i].SetActive(true);
			newPos =panelRect[i].anchoredPosition;
			newPos.x=defaultPosX+Screen.width*i;
			panelRect[i].anchoredPosition = newPos;
		}
		buttonObjects [target].GetComponent<MouseOnButtonAnimation> ().Selected = true;
	}
	public void ScrollTo(int index){
		acceleration = defaultAccerelation;
		buttonObjects [target].GetComponent<MouseOnButtonAnimation> ().Selected = false;
		buttonObjects [index].GetComponent<MouseOnButtonAnimation> ().Selected = true;
		target = index;

		direction = (-1) * (panelRect [target].anchoredPosition.x - defaultPosX);
		state = ScrollingState.SCROLLING;
	}
//	public void ClosePanels(){
//		for (int i=0; i<panelObjects.Length; ++i) {
//			panelObjects[i].SetActive(false);
//		}
//	}
	void Update(){
		switch(state){

		case ScrollingState.SCROLLING:
			currentX = panelRect [target].anchoredPosition.x - defaultPosX;
			if (currentX > 0) {
				if (-currentSpeed<maxSpeed){
					currentSpeed -= Time.deltaTime * acceleration;
				}

			} 
			else if (currentX < 0) {
				if (currentSpeed<maxSpeed){
					currentSpeed += Time.deltaTime * acceleration;
				}
			} 

			if (Mathf.Abs(currentX) <= errorMargin || (direction>0 && currentX>0) || (direction<0 && currentX<0)) {
				Debug.Log ("BREAKING");
				direction=currentSpeed;
				currentSpeed*=breakingSpeedMPL;
				acceleration*=breakingAccerelationMPL;
				state = ScrollingState.BREAKING;
				break;
			}
			for (int i=0; i<panelRect.Length; ++i) {
				newPos = panelRect [i].anchoredPosition;
				newPos.x += Time.deltaTime * currentSpeed;
				panelRect [i].anchoredPosition = newPos;
			}
			break;

		case ScrollingState.BREAKING:
			currentX = panelRect [target].anchoredPosition.x - defaultPosX;


			if ((direction>0 && currentSpeed<0) || (direction<0 && currentSpeed>0)){
				if ((Mathf.Abs(currentX) <= errorMarginBreaking || (direction>0 && currentX<0) || (direction<0 && currentX>0))) {
					state = ScrollingState.DEFAULT;
					for (int i=0; i<panelObjects.Length; ++i) {
						newPos =panelRect[i].anchoredPosition;
						newPos.x-=currentX;
						panelRect[i].anchoredPosition = newPos;
					}
					currentSpeed=0;
					break;
				}
			}
			if (currentX > 0) {
				if (-currentSpeed<maxSpeed){
					currentSpeed -= Time.deltaTime * acceleration;
				}else {
					currentSpeed = -maxSpeed;
				}
				
			} 
			else if (currentX < 0) {
				if (currentSpeed<maxSpeed){
					currentSpeed += Time.deltaTime * acceleration;
				}else {
					currentSpeed = maxSpeed;
				}
			} 
			for (int i=0; i<panelRect.Length; ++i) {
				newPos = panelRect [i].anchoredPosition;
				newPos.x += Time.deltaTime * currentSpeed;
				panelRect [i].anchoredPosition = newPos;
			}

			break;
		}

	}
}
