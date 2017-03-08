using UnityEngine;
using System.Collections;

public class Effect:Ability, IOnTurnChangeTrigger {
	public int duration;
	public delegate void DurationChangeAction (int newDuration);
	public event DurationChangeAction OnDurationChange;
	public Unit owner;
	public virtual void Initialize(Unit _owner, int _duration){
		owner = _owner;
		duration = _duration;
		for (int i=0; i< owner.Effects.Count; ++i) {
			if (owner.Effects[i].GetType() == this.GetType()){
				Debug.Log (owner.Effects[i].name + "  " + name);
				((Effect)owner.Effects[i]).EndEffect();
			}
		}
		owner.AddEffect (this);
		OnEffectApplied ();
	}
	public virtual void OnTurnChange(Node node){
		duration--;
		if (OnDurationChange != null) {
			OnDurationChange(duration);
		}
		Debug.Log (name + "  Onturn change, duration: "+duration);
		if (duration <= 0) {
			EndEffect();
			return;
		}
	}
	public void EndEffect(){
		OnEffectEnd ();
		OnDurationChange = null;
		owner.RemoveEffect (this);
		owner = null;
	}
	public virtual void OnEffectApplied(){

	}
	public virtual void OnEffectEnd(){

	}
}
