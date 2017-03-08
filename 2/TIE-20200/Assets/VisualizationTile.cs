using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class VisualizationTile : MonoBehaviour {
	public float maxEmission=1f;
	public float minEmission=0.3f;
	[Header("Scaling parameters")]
	public float highestScale = 2f;
	public float lowestScale=1.5f;
	public float scaleSpeed=100;
	public float maxErrorDistance=0.01f;
	private Node node;
	public Node Node{set{node =value;
			node.OnUnitOnNodeChange+= OnUnitOnNodeChange;
		}}

	private bool possible;
	private bool selected;

	public MeshRenderer transparentMesh;
	public MeshRenderer solidMesh;
	private Material transparentMat;
	private Material solidMat;
	private Transform child;

	private Color red;
	private Color yellow;
	private Color green;

	private Color currentColor;
	private bool emissing;

	private float currentTargetScale;
	void Awake(){
		solidMat = solidMesh.material;
//		solidMat.EnableKeyword ("_EMISSION");
		transparentMat = transparentMesh.material;
		transparentMat.EnableKeyword ("_EMISSION");
		child = solidMesh.transform;
		child.localScale = lowestScale * Vector3.one;
		red = DataBase.instance.gameData.visualizationTileRed;
		yellow = DataBase.instance.gameData.visualizationTileYellow;
		green = DataBase.instance.gameData.visualizationTileGreen;
		SetGreen ();
		emissing = true;
		SetEmission (false);
	}
	void OnDisable(){
		node.OnUnitOnNodeChange-= OnUnitOnNodeChange;
		StopAllCoroutines ();
	}
	public void SetGreen(){
		//solidMat.color = green;
		transparentMat.color = green;
		currentColor = green;
		if (emissing) {
			transparentMat.SetColor ("_EmissionColor", currentColor * Mathf.LinearToGammaSpace (maxEmission));
			return;
		}
		transparentMat.SetColor ("_EmissionColor", currentColor * Mathf.LinearToGammaSpace (minEmission));

	}
	public void SetRed(){
		Debug.Log ("Visualization tile setting RED  " + "  to " + node.gridX + "  " + node.gridY);
		//solidMat.color = red;
		transparentMat.color = red;
		currentColor = red;
		if (emissing) {
			transparentMat.SetColor ("_EmissionColor", currentColor * Mathf.LinearToGammaSpace (maxEmission));
			return;
		}
		transparentMat.SetColor ("_EmissionColor", currentColor * Mathf.LinearToGammaSpace (minEmission));

	}
	public void SetYellow(){
		//solidMat.color = yellow;
		transparentMat.color = yellow;
		currentColor = yellow;
		if (emissing) {
			transparentMat.SetColor ("_EmissionColor", currentColor * Mathf.LinearToGammaSpace (maxEmission));
			return;
		}
		transparentMat.SetColor ("_EmissionColor", currentColor * Mathf.LinearToGammaSpace (minEmission));

	}

	public void SetEmission(bool emission){
		if (emission && !emissing) {
			transparentMat.SetColor ("_EmissionColor", currentColor * Mathf.LinearToGammaSpace (maxEmission));
		} else if (!emission && emissing){
			transparentMat.SetColor ("_EmissionColor", currentColor * Mathf.LinearToGammaSpace (minEmission));
		}
		emissing = emission;
		if (!emissing) {
			ScaleToZero ();
		} else {
			ScaleDown();
		}

	}
	public void ScaleUp(){
		if (currentTargetScale != highestScale) {
			StopAllCoroutines ();
			currentTargetScale = highestScale;
			StartCoroutine (ScaleToTarget (highestScale * Vector3.one));
		}
	}
	public void ScaleDown(){
		if (!emissing) {
			ScaleToZero ();
		}
		else if (currentTargetScale != lowestScale) {
			StopAllCoroutines ();
			currentTargetScale = lowestScale;
			StartCoroutine (ScaleToTarget (lowestScale * Vector3.one));
		}
	}
	public void ScaleToZero(){
		StopAllCoroutines ();
		currentTargetScale = 0;
		StartCoroutine (ScaleToTarget (Vector3.zero));
	}
	private IEnumerator ScaleToTarget(Vector3 target){

		while (true) {
			yield return new WaitForEndOfFrame();
			if (Vector3.Distance(child.localScale, target)>maxErrorDistance){
				child.localScale=Vector3.MoveTowards(child.localScale, target, scaleSpeed* Time.deltaTime );
			}else {
				break;
			}
		}
	}
	public void OnUnitOnNodeChange(Unit newUnit){
		string name ="null";
		if (newUnit!=null){
			name = newUnit.name;
		}
		Debug.Log ("Visualization tile on units node change!!  " + name + "  to " + node.gridX + "  " + node.gridY);
		if (newUnit == null) {
			SetGreen();
			return;
		}
		if (CombatManager.instance.IsThisUnitsTurn (newUnit)) {
			SetYellow();
			return;
		}
		else if(!newUnit.Visible){
			SetGreen();
			return;
		}
		SetRed ();
		return;
	}
}
