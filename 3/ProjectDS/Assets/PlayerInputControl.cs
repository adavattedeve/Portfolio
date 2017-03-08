using UnityEngine;
using System.Collections;

public class PlayerInputControl : MonoBehaviour, ICharacterInput {
	private PlayerControl playerControl;

	private bool ingoreInput;
	// Use this for initialization
	void Awake () {
		playerControl = GetComponent<PlayerControl> ();
		ingoreInput = false;
	}
	
	public void RefreshInput(){
		if (ingoreInput)
			return;

		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		Vector3 dir = new Vector3 (h, 0, v);
		playerControl.SetDirection (dir);
		if (Input.GetAxisRaw ("Mouse ScrollWheel")!=0) {
			playerControl.ChangeTarget((int)Input.GetAxisRaw ("Mouse ScrollWheel"));
		}
		if (Input.GetButtonDown ("Attack")) {
			playerControl.Attack();
		}
		if (Input.GetButtonDown ("JumpAttack")) {
			playerControl.JumpAttack();
		}
		playerControl.Sprint (Input.GetButton ("Sprint"));

		if (Input.GetButtonDown ("Roll")) {
			playerControl.Roll();
		}
		
		if (Input.GetMouseButtonDown (2)) {
			playerControl.ToLockOn();
		}
		playerControl.Block (Input.GetButton ("Block"));
	}
	public void IngoreInput(float t){
		StartCoroutine (DisableInput(t));
	}

	private IEnumerator DisableInput(float t){
		ingoreInput = true;
		yield return new WaitForSeconds (t);
		ingoreInput = false;
	}
}
