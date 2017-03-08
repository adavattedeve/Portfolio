using UnityEngine;
using System.Collections;

public class AreaGenerators : MonoBehaviour {
	public AreaType type;
	[SerializeField]private DefaultAreaGeneration[] defaultAreaGenerators;

	public IAreaDataGeneration[] GetAreaDataGenerators(){
		int generatorsAmount = defaultAreaGenerators.Length;
		IAreaDataGeneration[] returnObject = new IAreaDataGeneration[generatorsAmount];
		int generatorsIndex = 0;
		for (int i=0; i<defaultAreaGenerators.Length; ++i){
			returnObject[generatorsIndex] = defaultAreaGenerators[i];
		}

		return returnObject;
	}
}
