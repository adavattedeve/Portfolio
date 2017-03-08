using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class HeroController : MonoBehaviour {
	public Hero hero;
	public GameObject projectileSlot;
	public void SpellCastTo(Spell spell, Node target){
		if (spell is SpellMultipleSingleVisuals) {
			SpellMultipleSingleVisuals temp = (SpellMultipleSingleVisuals)(spell);
			List<Node> targets = temp.targets;
			List<VisualEffectLauncher> visuals = temp.Visuals;
			if (targets != null && visuals != null) {
				for (int i=0; i<targets.Count; ++i){
					GameObject go = visuals[i].gameObject;
					go.SetActive (true);
					Debug.Log (targets[i].worldPosition.ToString());
					go.transform.position = projectileSlot.transform.position;
					go.transform.rotation = Quaternion.identity;
					Vector3 targetPos = targets[i].worldPosition;
					targetPos.y = 1.5f;
					visuals[i].Launch (targetPos, () => {
						hero.state = HeroState.DEFAULT;});
				}
			}
		}
		else {
			VisualEffectLauncher visualEffect = spell.VisualEffect;
			if (visualEffect != null) {
				GameObject go = visualEffect.gameObject;
				go.SetActive (true);
				go.transform.position = projectileSlot.transform.position;
				go.transform.rotation = Quaternion.identity;
				Vector3 targetPos = target.worldPosition;
				targetPos.y = 1.5f;

				visualEffect.Launch (targetPos, () => {
					hero.state = HeroState.DEFAULT;});
			} else {
				//Normally launch from animation event
				StartCoroutine (SpellCastForTest ());
			}
		}
	}
	private IEnumerator SpellCastForTest(){
		yield return new WaitForSeconds(1f);
		hero.state = HeroState.DEFAULT;
		Debug.Log ("hero is ready");
	}
}
