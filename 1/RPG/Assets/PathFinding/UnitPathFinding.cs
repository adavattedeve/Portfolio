using UnityEngine;
using System.Collections;

public class UnitPathFinding : MonoBehaviour {
	public bool drawPath;
	public bool drawOccupied;
	public bool raysForSimplifyingPath;
	public LayerMask UnWalkable;
	public float stoppingRangeWayPoint;
	private float stoppingRange;
	
	
	private Vector3[] path;
	private int targetIndex;
	private Animator anim;
	private CharacterStats stats;
	private CharacterEvents events;

	private Vector3 target;
	
	private Ray ray;
	private RaycastHit hit;
	private Vector3 tempVector;

	private Node currentNode;
	private Node newNode;
	private bool nextNodeOccupied;
	private Grid grid;
	private bool joku;
	// Use this for initialization
	void Awake () {
		anim = GetComponent<Animator> ();
		stats = GetComponent<CharacterStats> ();
		events = GetComponent<CharacterEvents> ();
		joku = false;
	}
	
	void Start(){
		grid = PlayerManager.instance.GetComponent<Grid> ();
		currentNode = grid.NodeFromWorldPoint (transform.position);
		currentNode.occupied = true;
		events.Death += OccupiedToFalse;
		events.Death += CancelPath;
	}

	public void MoveTo(Vector3 _target){
		target = _target;
		PathRequestManager.RequestPath (transform.position, target, true, OnPathFound);
	}
	public void CancelPath(){
		StopCoroutine("FollowPath");
		anim.SetFloat ("h", 0);
		anim.SetFloat ("v", 0);

	}
	private bool ObstaclesBetween(Vector3 target){
		if (!raysForSimplifyingPath) {
			return true;
		}
			target.y = 0.2f;
			ray.direction = target - ray.origin;
			if (Vector3.Cross (transform.forward, ray.direction).y > 0) {
				tempVector = transform.TransformPoint (Vector3.right * stats.width);
			} else {
				tempVector = transform.TransformPoint (Vector3.left * stats.width);
			}
			tempVector.y = 0.2f;
			ray.origin = tempVector;
			if (Physics.Raycast (ray, out hit, (target - ray.origin).magnitude, UnWalkable)) {
				return true;
			}
		
		return false;
	}
	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
		if (pathSuccessful && newPath.Length>0) {

			joku=false;
			path = newPath;
			StopCoroutine ("FollowPath");
			targetIndex = 0;
			StartCoroutine ("FollowPath");

		} else if (!joku) {
			joku = true;
			PathRequestManager.RequestPath (transform.position, target, false, OnPathFound);}
		else {
			joku = false;
			path=null;
			CancelPath();
		}
	}
	
	private IEnumerator FollowPath() {
		Vector3 currentWaypoint = path[0];
		Vector3 direction=Vector3.zero;
		while (true) {

			if (targetIndex+1 < path.Length) {
				nextNodeOccupied=grid.NodeFromWorldPoint(path[targetIndex+1]).occupied;
				if (nextNodeOccupied) {
					CancelPath();
					yield break;
				}
				if (!ObstaclesBetween(path[targetIndex+1])){
					targetIndex ++;
					if (targetIndex >= path.Length) {
						CancelPath();
						yield break;
					}
					currentWaypoint = path[targetIndex];
				}
			}
			if (transform.position.CalculateDistance(currentWaypoint) <=stoppingRangeWayPoint ) {
				targetIndex ++;
				if (targetIndex >= path.Length) {
					CancelPath();
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}
			direction = currentWaypoint-transform.position;
			anim.SetFloat("h", direction.x);
			anim.SetFloat("v", direction.z);
			yield return null;
			
		}
	}
	void Update (){
		newNode = grid.NodeFromWorldPoint (transform.position);
		if (newNode!=currentNode) {
			currentNode.occupied=false;
			currentNode=newNode;
			currentNode.occupied = true;
		}

		//		switch (rangeState) {
		//		case RangeFromPlayer.OUTOFRANGE:
		//			break;
		//		case RangeFromPlayer.AGRORANGE:
		//			if (distanceFromPlayer >= agroRange){
		//				rangeState = RangeFromPlayer.OUTOFRANGE;
		//				StopCoroutine("LoseAgro");
		//				StartCoroutine("LoseAgro");
		//				break;
		//			}
		//			if (ObstaclesBetween(player.transform.position)){
		//				StopCoroutine("LoseAgro");
		//				StartCoroutine("LoseAgro");
		//			}else{
		//				StopCoroutine("LoseAgro");
		//				agro = true;
		//			}
		//			break;
		//		case RangeFromPlayer.ATTACKRANGE:
		//			//AbilityManageAttack
		//			break;
		//		}
		//		if (agro && distanceFromPlayer >= stoppingRangePlayer){
		//				PathRequestManager.RequestPath (transform.position, player.transform.position, OnPathFound);
		//			}else{
		//				StopCoroutine("FollowPath");
		//				anim.SetFloat("h", 0);
		//				anim.SetFloat("v", 0);}
	}
	public void OccupiedToFalse(){
		currentNode.occupied = false;
	}
	public void OnDrawGizmos() {
		if (drawPath) {
			if (path != null) {
				for (int i = targetIndex; i < path.Length; i ++) {
					Gizmos.color = Color.black;
					Gizmos.DrawCube (path [i], Vector3.one*grid.nodeRadius*2);
					
					if (i == targetIndex) {
						Gizmos.DrawLine (transform.position, path [i]);
					} else {
						Gizmos.DrawLine (path [i - 1], path [i]);
					}
				}
			}
		}
		if (drawOccupied && currentNode!=null){
			Gizmos.color = Color.cyan;
			Gizmos.DrawCube(currentNode.worldPosition, Vector3.one*grid.nodeRadius*2);
		}
	}
}
