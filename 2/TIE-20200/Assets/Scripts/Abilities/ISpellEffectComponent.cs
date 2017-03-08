using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public interface ISpellEffectComponent {
	void ApplyEffects(List<Node> targets, int intelligence);
	void EffectVisualization (Node node);
}
