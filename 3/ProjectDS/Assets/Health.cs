using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour, IDamageable {
	public float blockingAngle = 45;
	public float maxHealth=100;
	public bool invulnerable=false;
	private float currentHealth;


	//Refs
	private CharacterControl characterControl;

	void Awake () {
		currentHealth = maxHealth;
		characterControl = (CharacterControl)GetComponent(typeof(CharacterControl));
	}
	public void TakeDamage(float damage, Vector3 sourcePos){
		float angle = Vector3.Angle (-transform.forward, transform.position - sourcePos);
		if (angle < blockingAngle && characterControl.Blocking) {
			characterControl.Blocked();
			return;
		}
		currentHealth -= damage;

		if (currentHealth <= 0) {
			characterControl.Die(angle);
		}
		characterControl.GetHit(angle);
	}
}
