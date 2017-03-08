using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Teleport : SpellMultipleSingleVisuals, ISpellEffectComponent, ISpellMultiplePhasesComponent {
	private int phase=0;
	private Node[] phaseTargets;
	public override List<VisualEffectLauncher> Visuals {
		get {
			if (visuals == null) {
				visuals = new List<VisualEffectLauncher>();
			}
			GameObject prefabFrom;
			GameObject prefabTo;
			if (visualEffectName==""){
				prefabFrom = DataBase.instance.GetVisualEffect (name+"From");
				prefabTo = DataBase.instance.GetVisualEffect (name+"To");
			}else {
				prefabFrom = DataBase.instance.GetVisualEffect (name+"From");
				prefabTo = DataBase.instance.GetVisualEffect (name+"To");
			}
			
			if (prefabFrom == null || prefabTo==null) {
				return null;
			}
			if (visuals.Count>0) {
				if (visuals [0] == null || visuals[0].gameObject==null){
					visuals=new List<VisualEffectLauncher>();
					
				}
			}
			if (visuals.Count!=2){
				GameObject goFrom = (MonoBehaviour.Instantiate (prefabFrom) as GameObject);
				GameObject goTo = (MonoBehaviour.Instantiate (prefabTo) as GameObject);
				visuals.Add((VisualEffectLauncher)(goFrom.GetComponent(typeof(VisualEffectLauncher))));
				visuals.Add((VisualEffectLauncher)(goTo.GetComponent(typeof(VisualEffectLauncher))));
				visuals[0].gameObject.SetActive(false);
				visuals[1].gameObject.SetActive(false);
			}
			return visuals;
		}
	}
	public override TargetingType targetinType {
		get {
			return targetingTypes [phase];
		}
	}
	public int Phase{ get { return phase; } set{
			phase = value;
			Debug.Log ("phase" + phase);
		}}
	public Node[] PhaseTargets { get{
			if (phaseTargets == null){
				phaseTargets = new Node[targetingTypes.Length];
			}
			return phaseTargets;
		}}
	public void ApplyEffects(List<Node> _targets, int intelligence){
		phaseTargets [1] = _targets [0];
		if (phaseTargets[1].Unit != null) {
			Debug.Log ("Couldnt teleport to that tile");
			return;
		}
		targets = new List<Node> ();
		targets.Add (phaseTargets[0]);
		targets.Add (phaseTargets[1]);
		phaseTargets[1].Unit = phaseTargets[0].Unit;
		phaseTargets[0].Unit = null;
		phaseTargets [1].Unit.unitController.currentMovementTarget = phaseTargets [1];
	}
	public void EffectVisualization (Node target){
		phaseTargets [1].Unit.unitController.transform.position = phaseTargets [1].worldPosition;
	}
}
