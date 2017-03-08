using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour {
	struct PathRequest {
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public bool noticeOccupied;
		public Action<Vector3[], bool> callback;
		
		public PathRequest(Vector3 _start, Vector3 _end, bool _noticeOccupied, Action<Vector3[], bool> _callback) {
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
			noticeOccupied = _noticeOccupied;
		}
		
	}
	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	PathRequest currentPathRequest;

	static PathRequestManager instance;
	PathFinding pathfinding;

	bool isProcessingPath;

	void Awake() {
		if (!instance){
		instance = this;
		pathfinding = GetComponent<PathFinding>();
		}
	}

	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, bool noticeOccupied, Action<Vector3[], bool> callback) {
		PathRequest newRequest = new PathRequest(pathStart,pathEnd, noticeOccupied, callback);
		instance.pathRequestQueue.Enqueue(newRequest);
		instance.TryProcessNext();
	}

	void TryProcessNext() {
		if (!isProcessingPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.noticeOccupied);
		}
	}

	public void FinishedProcessingPath(Vector3[] path, bool success) {
		currentPathRequest.callback(path,success);
		isProcessingPath = false;
		TryProcessNext();
	}


}
