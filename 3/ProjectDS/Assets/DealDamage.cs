using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DealDamage : MonoBehaviour {

	public LayerMask enemyLayer;
	private Weapon damageDealingWeapon;
	private bool dealingDamage;

	private List<Collider> damaged;

	void Awake(){
		damaged = new List<Collider> ();
		dealingDamage = false;
	}
	void Update(){
		if (dealingDamage) {
			RaycastHit hit;
			for (int i=0; i<damageDealingWeapon.rayPoints.Length; ++i){
			//	Debug.DrawLine(damageDealingWeapon.rayPoints[i].transform.position,
				             //  damageDealingWeapon.rayPoints[i].transform.position+damageDealingWeapon.rayPoints[i].transform.forward*damageDealingWeapon.rayLengths[i],
				               //Color.green, 2, false);
				if (Physics.Raycast(damageDealingWeapon.rayPoints[i].transform.position, damageDealingWeapon.rayPoints[i].transform.forward, out hit ,damageDealingWeapon.rayLengths[i], enemyLayer)){
	
					if (!damaged.Contains(hit.collider)){
						Health health = hit.collider.GetComponent<Health>();
						if (!health.invulnerable){
							damaged.Add(hit.collider);
							health.TakeDamage(Random.Range(damageDealingWeapon.damage.x, damageDealingWeapon.damage.y), transform.position);
						}
					}
				}
			}
		}
	}
	public void DealDamageOn(Weapon weapon){
		damaged.Clear ();
		damageDealingWeapon = weapon;
		dealingDamage = true;
	}
	public void DealDamageOff(){
		dealingDamage = false;
	}
}
