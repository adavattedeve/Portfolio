using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour {
	public GameObject unitCountCanvasPrefab;
	public GameObject hitInfoCanvasPrefab;

	public GameObject bloodParticlesSlot;
	private GameObject bloodParticles;

	public GameObject attackProjectileSlot;
	private VisualEffectLauncher attackProjectile;

	private SkinnedMeshRenderer[] renderers;
	private UnitCountUI unitCountUI;
	public float unitCountCanvasPosY=0.24f;
	public float unitCountCanvasPosZ=1f;
	private UnitPathFinding unitPathFinding;
	[System.NonSerialized]public Node currentMovementTarget;
	private Animator animator;
	public Animator Anim{get{return animator;}}
	private Unit unit;
	[Range(0f,1f)]public float rotationSpeed=0.35f;
	public float maxErrorAngle=1f;
	private Vector3 attackTargetPosition;
	public Unit Unit{
		get{return unit;}
		set{
			unit = value;
			GetComponentInChildren<UnitAnimationEvents>().unit = unit;
			unitCountUI.Initialize (unit);
			GameObject attackProjectileGO = DataBase.instance.GetVisualEffect (unit.attackProjectileName);
			if (attackProjectileGO != null) {
				attackProjectileGO = Instantiate (attackProjectileGO) as GameObject;
				attackProjectileGO.transform.parent = attackProjectileSlot.transform;
				attackProjectileGO.transform.position = Vector3.zero;
				attackProjectileGO.transform.rotation = Quaternion.identity;
				attackProjectile = (VisualEffectLauncher)attackProjectileGO.GetComponent (typeof(VisualEffectLauncher));
				attackProjectileGO.SetActive (false);
			}
			unit.OnEffectsChange +=OnEffectsChange;
		}
	}
	void Awake(){
		renderers = GetComponentsInChildren<SkinnedMeshRenderer> ();
		unitPathFinding = GetComponent<UnitPathFinding> ();
		unitCountUI = (Instantiate (unitCountCanvasPrefab) as GameObject).GetComponent<UnitCountUI>();
		unitCountUI.transform.SetParent (transform, true);
		unitCountUI.transform.localPosition = new Vector3 (0,unitCountCanvasPosY, unitCountCanvasPosZ);
		animator = GetComponentInChildren<Animator> ();

		bloodParticles = DataBase.instance.GetVisualEffect (DataBase.instance.gameData.bloodParticleName);
		bloodParticles = Instantiate (bloodParticles) as GameObject;
		bloodParticles.transform.parent = bloodParticlesSlot.transform;
		bloodParticles.transform.localPosition = Vector3.zero;
		bloodParticles.transform.localRotation = Quaternion.identity;
		bloodParticles.SetActive (false);

	}
	public void MoveTo (Vector3 position){
		if (unit.state == UnitState.DEFAULT && !CombatManager.instance.IsUnitInAttackQueue (unit)) {
			unitPathFinding.StartCoroutine (unitPathFinding.MoveTo (position, MovedToTarget));
			Debug.Log ("state TO moving");
			unit.state = UnitState.MOVING;
		}
		else {
			StartCoroutine(WaitForDefaultState(position));
		}
	}
	private IEnumerator WaitForDefaultState(Vector3 position){
		while (unit.state!= UnitState.DEFAULT || CombatManager.instance.IsUnitInAttackQueue (unit)) {
			yield return new WaitForSeconds(0.5f);
		}
		MoveTo(position);
	}
	public void MovedToTarget(){
		Debug.Log ("MOVED TO TARGET");
		unit.state = UnitState.DEFAULT;
	}
	public void AttackTo(Node to){
		unit.state = UnitState.ATTACKING;
		attackTargetPosition = to.worldPosition;
		attackTargetPosition.y = 1.25f;
		animator.SetTrigger ("Attack");
	}
	public void ChangeVisibility(bool visible, bool invisible=false){
		if (!visible) {
			if (invisible) {
				for (int i=0; i<renderers.Length; ++i) {
					renderers [i].enabled = false;
				}
			}
			else {
				for (int i=0; i<renderers.Length; ++i) {
					Material mat = renderers [i].material;
					mat.SetInt ("_SrcBlend", 1);
					mat.SetInt ("_DstBlend", 10);
					mat.SetInt ("_ZWrite", 0);
					mat.DisableKeyword ("_ALPHATEST_ON");
					mat.DisableKeyword ("_ALPHABLEND_ON");
					mat.EnableKeyword ("_ALPHAPREMULTIPLY_ON");
					mat.renderQueue = 3000;
				}
			}
		}
		else {
			Debug.Log ("TO VISIBLE");
			for (int i=0; i<renderers.Length; ++i) {
				renderers [i].enabled = true;
				Material mat = renderers [i].material;
				mat.SetInt ("_SrcBlend", 1);
				mat.SetInt ("_DstBlend", 0);
				mat.SetInt ("_ZWrite", 1);
				mat.DisableKeyword ("_ALPHATEST_ON");
				mat.DisableKeyword ("_ALPHABLEND_ON");
				mat.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
				mat.renderQueue = -1;
			}
		}
	}
	public bool AttackWithProjectile(){
		if (attackProjectile != null) {
			if (!attackProjectile.gameObject.activeSelf){
				SpawnAttackProjectile();
			}
			attackProjectile.Launch(attackTargetPosition, () => { unit.state=UnitState.DEFAULT; });
			return true;
		}
		return false;
	}
	public void OnEffectsChange(Effect effect, bool added){
		Debug.Log ("EFFECTS CHANGE");
		if (added) {
			unitCountUI.AddEffect (effect);
		} else {
			unitCountUI.RemoveEffect(effect);
		}
	}
	public void SpawnAttackProjectile(){
		attackProjectile.gameObject.SetActive (true);
		attackProjectile.transform.parent = attackProjectileSlot.transform;
		attackProjectile.transform.localPosition = Vector3.zero;
		attackProjectile.transform.localRotation = Quaternion.identity;
	}
	public void TakeDamage(int damage, int deathCount){
		GraphicalEffectsManager.instance.ShakeCamera (VisualEffectSize.SMALL);
		unit.state = UnitState.TAKINGDAMAGE;
		animator.SetTrigger ("TakeDamage");
		bloodParticles.SetActive (false);
		bloodParticles.SetActive (true);
		(Instantiate (hitInfoCanvasPrefab) as GameObject).GetComponent<HitInfoUI>().ShowHitInfo(transform.position, damage, deathCount);
		unitCountUI.Refresh (-deathCount);
	}

	public void OnDeath(int damage, int deathCount){
		StopAllCoroutines ();
		GraphicalEffectsManager.instance.ShakeCamera (VisualEffectSize.SMALL);
		unit.state = UnitState.DEFAULT;
		bloodParticles.SetActive (false);
		bloodParticles.SetActive (true);
		(Instantiate (hitInfoCanvasPrefab) as GameObject).GetComponent<HitInfoUI>().ShowHitInfo(transform.position, damage, deathCount);
		Destroy (unitCountUI.gameObject);
		animator.SetTrigger ("Death");
	}
	public IEnumerator RotateTo(Quaternion target){
		unit.state = UnitState.ROTATING;
		while (true) {
			yield return new WaitForEndOfFrame();
			transform.rotation = Quaternion.Lerp(transform.rotation, target, rotationSpeed);
			if (Quaternion.Angle(target, transform.rotation)<=maxErrorAngle){
				break;
			}
		}
		unit.state = UnitState.DEFAULT;
	}
}
