using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    [SerializeField]
    private Transform spawnPointParent;
    [SerializeField]
    private ObjectToBePooled enemyPoolData;
    [SerializeField]
    private int enemyCount = 50;
    public AnimationCurve curve;

    private Transform[] spawnPoints;
    //public float SpawnRate = 0.25f;
    void Awake() {
        spawnPoints = new Transform[spawnPointParent.childCount];
        for (int i = 0; i < spawnPointParent.childCount; ++i) {
            spawnPoints[i] = spawnPointParent.GetChild(i);
        }
    }
	void Start () {
        ObjectPool.instance.AddNewObjectToBePooled(enemyPoolData);
        for (int i = 0; i < enemyCount; ++i) {
            SpawnEnemy();
        }
        
    }
    private Transform getSpawn() {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
    private void SpawnEnemy() {
        GameObject enemy = ObjectPool.instance.GetObjectFromPool(enemyPoolData);
        Transform currentSpawn = getSpawn();
        enemy.transform.position = currentSpawn.position;
        enemy.transform.rotation = currentSpawn.rotation;
    }
}
