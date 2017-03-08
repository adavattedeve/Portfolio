using UnityEngine;
using System.Collections;

public class BurningTileEffect : Effect {
	Node tile;
	public void Initialize(Node _tile, int _duration){
		tile = _tile;
		duration = _duration;
		//tile.temporalEffects.Add (this);
		OnEffectApplied ();
	}
	public override void OnEffectApplied ()
	{
		base.OnEffectApplied ();
	}
	public override void OnEffectEnd ()
	{
		base.OnEffectEnd ();
	}
}
