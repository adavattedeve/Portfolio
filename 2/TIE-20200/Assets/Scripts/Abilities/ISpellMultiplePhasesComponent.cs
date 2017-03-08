using UnityEngine;
using System.Collections;
public interface ISpellMultiplePhasesComponent {
	int Phase{ get; set;}
	Node[] PhaseTargets { get;}
}
