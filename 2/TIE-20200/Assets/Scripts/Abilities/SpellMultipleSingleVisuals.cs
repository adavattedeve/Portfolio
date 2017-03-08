using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class SpellMultipleSingleVisuals: Spell {
	[System.NonSerialized]public List<Node> targets;
	[System.NonSerialized]protected List<VisualEffectLauncher> visuals;
	public virtual List<VisualEffectLauncher> Visuals{ get{
			if (visuals == null) {
				visuals = new List<VisualEffectLauncher>();
			}
			GameObject prefab;
			if (visualEffectName==""){
				prefab = DataBase.instance.GetVisualEffect (name);
			}else {
				prefab = DataBase.instance.GetVisualEffect (visualEffectName);
			}
			
			if (prefab == null) {
				return null;
			}
			int visualCount = visuals.Count-1;
			while (visualCount>=0) {
				if (visuals [visualCount] == null || visuals[visualCount].gameObject==null){
					visuals.RemoveAt(visualCount);

				}
				--visualCount;
			}
			Debug.Log (visuals.Count);
			for (int i= visuals.Count; i<targets.Count; ++i){
			//if (visuals.Count < targets.Count) {
				GameObject go = (MonoBehaviour.Instantiate (prefab) as GameObject);
				Debug.Log (i + "  " + go.name);
				visuals.Add((VisualEffectLauncher)(go.GetComponent(typeof(VisualEffectLauncher))));
				visuals[i].gameObject.SetActive(false);
			}
			return visuals;
		}
	}
}
