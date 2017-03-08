using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TrapSpawner : MonoBehaviour {
    public GameObject trapPrefab;
    private List<Transform> trapSpawnPoints;
    void Awake()
    {
        Transform[] childrenAndSelf = GetComponentsInChildren<Transform>();
        for (int i = 0; i < childrenAndSelf.Length; ++i)
        {
            if (childrenAndSelf[i] == transform)
            {
                childrenAndSelf[i] = null;
                break;
            }
        }
        trapSpawnPoints = new List<Transform>();
        for (int i = 0; i < childrenAndSelf.Length; ++i)
        {
            if (childrenAndSelf[i] != null)
            {
                trapSpawnPoints.Add(childrenAndSelf[i]);
                Instantiate(trapPrefab, childrenAndSelf[i].position, childrenAndSelf[i].rotation);
            }
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
