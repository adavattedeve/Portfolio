using UnityEngine;
using System.Collections;
using System;
public class UnitPathFinding : MonoBehaviour {
	private Node[] path;
	private PathFinding pathFinding;
	private UnitController unitController;
	private Animator animator;
	public float waypointErrorMarginal=0.15f;
	void Awake(){
		pathFinding = GameManager.instance.GetComponent<PathFinding> ();
		unitController = GetComponent<UnitController> ();
		animator = unitController.Anim;
	}
	public IEnumerator MoveTo(Vector3 target, Action callBack){
		bool pathFound = false;
		for (int i=0; i< unitController.Unit.TriggableAbilities.Count; ++i){
			if (unitController.Unit.TriggableAbilities[i] is IOverrideMovement){
				path =  ((IOverrideMovement)unitController.Unit.TriggableAbilities[i]).FindPath(transform.position , target, pathFinding);
				pathFound=true;
			}
		}
		if (!pathFound) {
			path = pathFinding.FindPath (transform.position, target);
		}
		unitController.currentMovementTarget = path[path.Length-1];
		int currentIndex = 0;
		Node currentNode = GameManager.instance.GetComponent<Grid> ().NodeFromWorldPoint(transform.position);
		yield return new WaitForEndOfFrame();
		unitController.StartCoroutine(unitController.RotateTo(transform.rotation * Quaternion.FromToRotation (transform.forward, target-transform.position)));
		while (true) {
			if (unitController.Unit.state==UnitState.DEFAULT){
				unitController.Unit.state=UnitState.MOVING;
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		animator.SetFloat("h", path[currentIndex].gridX-currentNode.gridX);
		animator.SetFloat("v", path[currentIndex].gridY-currentNode.gridY);
		animator.SetBool ("Move", true);

		Vector3 currentPosition;
		while (true) {
			currentPosition= transform.position;
			currentPosition.y=0;
			if ((currentPosition-path[currentIndex].worldPosition).sqrMagnitude<waypointErrorMarginal){
				currentNode = path[currentIndex];
				++currentIndex;
				if (currentIndex >= path.Length){
					Debug.Log ("movementready");
					break;
				}
			}
			animator.SetFloat("h", path[currentIndex].worldPosition.x-currentPosition.x);
			animator.SetFloat("v", path[currentIndex].worldPosition.z-currentPosition.z);
			yield return new WaitForEndOfFrame();

		}
		animator.SetBool ("Move", false);
		animator.SetFloat("h", 0);
		animator.SetFloat("v", 0);
		target.y = transform.position.y;
		transform.position = target;
		unitController.Unit.RotateTowards ();
		while (true) {
			if (unitController.Unit.state==UnitState.DEFAULT){
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		callBack();
		//PathRequestManager.RequestPath (transform.position, target, true, OnPathFound);
	}
	
}
