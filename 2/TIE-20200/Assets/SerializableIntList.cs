using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class SerializableIntList  {
	public List<int> intList;
	public SerializableIntList(){
		intList = new List<int> ();
	}
}
