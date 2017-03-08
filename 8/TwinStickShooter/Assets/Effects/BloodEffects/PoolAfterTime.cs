using UnityEngine;
using System.Collections;

public class PoolAfterTime : MonoBehaviour {
    public float poolAfterTime = 1f;
    private float timer = 0;
	void Update () {
        timer += Time.deltaTime;
        if (timer >= poolAfterTime)
            Pool();
	}   

    private void Pool() {
        timer = 0;
        ObjectPool.instance.PoolObject(gameObject);
    }
}
