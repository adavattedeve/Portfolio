using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IOverrideMovement {
	List<Node> OnCheckValidMovement(Node node);
	Node[] FindPath(Vector3 start, Vector3 target,PathFinding pathFinding);
}
