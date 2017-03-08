using UnityEngine;
using System.Collections;

public class TileMouseInput : MonoBehaviour {
	Grid grid;
	public LayerMask tileLayer;
	private Ray ray;
	private RaycastHit hit;
	private float rayLength=100;

	public delegate void MouseOnTileAction(Vector3 pos);
	public  event MouseOnTileAction OnMouseOnTile;

	public delegate void MouseStayOnTileAction(Vector3 pos);
	public  event MouseStayOnTileAction OnMouseStayOnTile;

	public delegate void MouseExitTileAction(Node node);
	public  event MouseExitTileAction OnMouseExitTile;

	public delegate void MouseLeftClickTileAction(Vector3 pos);
	public  event MouseLeftClickTileAction OnMouseLeftClickTile;

	public delegate void MouseRigthClickTileAction(Vector3 pos);
	public  event MouseRigthClickTileAction OnMouseRigthClickTile;

	public delegate void MouseLeftClickUpAction();
	public  event MouseLeftClickUpAction OnMouseRigthButtonkUp;

	private Node newNode;
	private Node currentNode;
	private Node CurrentNode{set{
			if (currentNode!=value && currentNode!=null){
				if (OnMouseExitTile!=null){
				OnMouseExitTile(currentNode);
				}
			}
			currentNode = value;
		}
		get{return currentNode;}}
	void Awake () {
		grid = GetComponent<Grid> ();
	}
	void OnDisable(){
		currentNode = null;
	}
	public void Reset(){
		OnMouseOnTile = null;
		OnMouseStayOnTile = null;
		OnMouseExitTile = null;
		OnMouseLeftClickTile = null;
		OnMouseRigthClickTile = null;
		OnMouseRigthButtonkUp = null;
	}
	void Update () {
		if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (-1)) {
			if (Input.GetMouseButtonUp (1)) {
				if (OnMouseRigthButtonkUp != null) {
					OnMouseRigthButtonkUp ();
				}
			}
			ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, rayLength, tileLayer)) {
				newNode = grid.NodeFromWorldPoint (hit.point);
				if (currentNode != newNode && OnMouseOnTile != null) {
					OnMouseStayOnTile = null;
					OnMouseOnTile (hit.point);
				} else if (currentNode == newNode && OnMouseStayOnTile != null) {
					OnMouseStayOnTile (hit.point);
				}
				CurrentNode = newNode;
				if (Input.GetMouseButtonDown (0)) {
					if (OnMouseLeftClickTile != null) {
						OnMouseLeftClickTile (hit.point);
					}
				} else if (Input.GetMouseButtonDown (1)) {
					Debug.Log ("mouse, on rigth click");
					if (OnMouseRigthClickTile != null) {
						OnMouseRigthClickTile (hit.point);
					}
				}
			} else {
				CurrentNode = null;
			}
		}
	}
}
