using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HitInfoUI : MonoBehaviour {
	public Text damageText;
	public Text deathCountText;
	public float defaultYPos=3;
	public float speed=10;
	public float lifeTime = 2;
	private float currentTime=0;
	private Vector3 pos;
	void Update () {
		pos = transform.position;
		pos.y += speed * Time.deltaTime;
		transform.position = pos;
	}
	public void ShowHitInfo(Vector3 position, int damage, int deathCount){
		position.y = defaultYPos;
		transform.position = position;
		damageText.text = damage.ToString ();
		deathCountText.text = deathCount.ToString ();
		currentTime = 0;
		Destroy (gameObject, lifeTime);
	}
}
