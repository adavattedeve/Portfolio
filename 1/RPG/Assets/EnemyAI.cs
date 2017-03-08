using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {
	enum RangeFromPlayer{ATTACKRANGE, AGRORANGE, OUTOFRANGE};

	RangeFromPlayer rangeState;
	private UnitPathFinding unitPathFinding;
	public bool agro;
	public float stoppingRange;
	public float agroTime;
	public float agroRange;
	private float distanceFromPlayer;
	float timer=0;
	
	private Transform player;
	private AbilityManager aManager;
	private ActionBuffer buffer;

	private Ray ray;
	private RaycastHit hit;
	private Vector3 tempVector;

	public LayerMask UnWalkable;
	void Awake(){
		unitPathFinding = GetComponent<UnitPathFinding> ();
		buffer = GetComponent<ActionBuffer> ();
		aManager = GetComponent<AbilityManager> ();
		rangeState = RangeFromPlayer.OUTOFRANGE;
	}
	void Start(){
		player = PlayerManager.instance.Player.transform;
	}

	void Update(){
		if (player != null) {
			distanceFromPlayer = transform.position.CalculateDistance (player.position);
			timer += Time.deltaTime;


			switch (rangeState) {
			case RangeFromPlayer.OUTOFRANGE:
				if (agro && timer > 1f) {
					unitPathFinding.MoveTo (player.position);
					timer = 0;
				}
				if (distanceFromPlayer <= agroRange) {
					rangeState = RangeFromPlayer.AGRORANGE;
				}
				break;

			case RangeFromPlayer.AGRORANGE:
				if (distanceFromPlayer <= stoppingRange) {
					unitPathFinding.CancelPath ();
					rangeState = RangeFromPlayer.ATTACKRANGE;
				} else if (distanceFromPlayer > agroRange) {
					AgroTimer (agroTime);
					rangeState = RangeFromPlayer.OUTOFRANGE;
				}
				tempVector = player.position - transform.position;
				tempVector.y = 0.2f;
				ray.direction = tempVector;
				tempVector = transform.position;
				tempVector.y = 0.2f;
				ray.origin = tempVector;
				if (Physics.Raycast (ray, out hit, (player.position - ray.origin).magnitude, UnWalkable)) {
					StartCoroutine (AgroTimer (agroTime));
				} else if (agro) {
					StopCoroutine (AgroTimer (agroTime));
				} else {
					agro = true;
				}
				if (agro && timer > 1f) {
					unitPathFinding.MoveTo (player.position);
					timer = 0;
				}
				break;
			case RangeFromPlayer.ATTACKRANGE:
				if (distanceFromPlayer > stoppingRange) {
					rangeState = RangeFromPlayer.AGRORANGE;
				}
				buffer.AddToBuffer (aManager.EquippedAbilities [0].ActionID, (player.position - transform.position));
				break;
			}
		}
	}
	IEnumerator AgroTimer(float t){
		yield return new WaitForSeconds(t);
		agro = false;
	}
}
