using UnityEngine;
using System.Collections;

[System.Serializable]
public class EnemySubWave : ScriptableObject {

    [Tooltip("Maximum amount enemies can be spawned by this spawner. Negative number means no cap.")]
    [SerializeField]
    private int baseMaxEnemies = -1;

    [Tooltip("When the subwave starts")]
    [SerializeField]
    private float subWaveStartTime = 0;

    [Tooltip("Base spawn rate in enemies per second.")]
    [SerializeField]
    private float baseSpawnRate = 1;

    [Tooltip("How much time one unit of x corresponds in time in spawnRate curve.")]
    [SerializeField]
    private float waveLengthInTime = 20;

    [Tooltip("Pool data for enemy that this spawner is spawning.")]
    [SerializeField]
    private ObjectToBePooled enemyPoolData;

    [Tooltip("Enemy spawnPoints")]
    [SerializeField]
    private LocationsData[] enemySpawnPointDatas;

    [Tooltip("X*baseSpawnRate = enemies/second = , Y*waveLengthInTime = waveLength")]
    [SerializeField]
    private AnimationCurve spawnRate;

    //public EnemySpawnBurst[] burstSpawns;

    private Vector3[] spawnPoints;
    private float lastUpdateTime = 0;
    private float enemySpawnProgress = 0; //When reaches 1 -> spawn enemy    
    private float enemyCountMpl = 1;
    private int enemyMaxCount = -1;
    private int enemiesSpawned = 0;
    private bool waveIsReady = false;

    public bool WaveIsReady { get{ return waveIsReady; } }
    public float SubWaveStartTime { get{ return subWaveStartTime; } }

    public void StartWave(float _enemyCountMpl) {
        lastUpdateTime = 0;
        enemySpawnProgress = 0;
        enemyCountMpl = _enemyCountMpl;
        enemyMaxCount = Mathf.FloorToInt(baseMaxEnemies * enemyCountMpl);
        enemiesSpawned = 0;
        waveIsReady = false;

        int spawnPointCount = 0;
        for (int i =0; i < enemySpawnPointDatas.Length; ++i)
        {
            spawnPointCount += enemySpawnPointDatas[i].locations.Length;
        }
        spawnPoints = new Vector3[spawnPointCount];
        int spawnPointIndex = 0;
        for (int i = 0; i < enemySpawnPointDatas.Length; ++i)
        {
            for (int j = 0; j < enemySpawnPointDatas[i].locations.Length; ++j)
            {
                spawnPoints[spawnPointIndex] = enemySpawnPointDatas[i].locations[j];
                ++spawnPointIndex;
            }
        }
    }

    public void UpdateSubwave(float timeFromWaveStart) {
        
        float timeFromSubWaveStart = timeFromWaveStart - subWaveStartTime;
        if (timeFromSubWaveStart >= waveLengthInTime || (enemyMaxCount >= 0 && enemiesSpawned >= enemyMaxCount)) {
            waveIsReady = true;
            Debug.Log("EnemiesSpawned: "  + enemiesSpawned);
        }
        enemySpawnProgress += baseSpawnRate * spawnRate.Evaluate(timeFromSubWaveStart / waveLengthInTime) * (timeFromSubWaveStart - lastUpdateTime) * enemyCountMpl;
        if (enemySpawnProgress >= 1f) {
            SpawnEnemy();
            enemySpawnProgress -= 1;
        }
        lastUpdateTime = timeFromSubWaveStart;
    }

    private void SpawnEnemy() {
        Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = ObjectPool.instance.GetObjectFromPool(enemyPoolData, spawnPoint, Quaternion.identity);
        ++enemiesSpawned;
        EnemyWaveManager.EnemySpawned();
        enemy.GetComponent<Health>().OnDeathEvent += EnemyWaveManager.EnemyKilled;
    }

}
