using UnityEngine;
using System.Collections;

public class BloodDecal : MonoBehaviour {

    [Range(0.1f, 5)]public float scaleMin = 0.5f;
    [Range(0.1f, 5)]public float scaleMax = 2f;
    // Use this for initialization

    void Awake () {
        transform.localScale = new Vector3(Random.Range(scaleMin, scaleMax), 1, Random.Range(scaleMin, scaleMax));
        transform.Rotate(Vector3.up, Random.Range(0, 360));

    }
}
