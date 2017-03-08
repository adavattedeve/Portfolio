using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetSprite : MonoBehaviour {
	public string spriteName;
	private Image image;
	void Awake(){
		image = GetComponent<Image> ();
	}
	void Start(){
		image.sprite = DataBase.instance.GetSprite (spriteName);
	}
}
