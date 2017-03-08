using UnityEngine;
using System.Collections;

public class EnemyWaveManager : MonoBehaviour {

    [SerializeField]
    private EnemyWave[] enemyWaves;

    private float timeSinceWaveStarted = 0;
    private int currentWave = -1;
    private static int enemiesLeft = 0;
    private bool allEnemiesFromWaveSpawned = false;

	void Awake () {
        currentWave = -1;
        StartNextWave();
	}

	void Update () {
        if (!allEnemiesFromWaveSpawned) {
            enemyWaves[currentWave].UpdateWave(timeSinceWaveStarted);
            allEnemiesFromWaveSpawned = enemyWaves[currentWave].AllEnemiesSpawned;
        } else if (enemiesLeft == 0) {
            StartNextWave();
        }
        timeSinceWaveStarted += Time.deltaTime;
    }

    public static void EnemySpawned () {
        enemiesLeft++;
    }

    public static void EnemyKilled() {
        enemiesLeft--;
    }

    private void StartNextWave () {
        if (currentWave + 1 < enemyWaves.Length) {
            currentWave++;
            enemiesLeft = 0;
            timeSinceWaveStarted = 0;
            allEnemiesFromWaveSpawned = false;
            enemyWaves[currentWave].StartWave();
            Debug.Log("Wave started! current wave is: " + currentWave);

        } else {
            AllWavesFinished();
        }
    }

    private void AllWavesFinished () {

    }
}