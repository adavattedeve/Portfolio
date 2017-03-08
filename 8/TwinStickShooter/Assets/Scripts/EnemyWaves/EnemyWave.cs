using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyWave : ScriptableObject
{
    [SerializeField]
    private EnemySubWave[] subWaves;
    [Tooltip("Enemy count multiplier for the wave.")]
    [SerializeField]
    private float baseEnemyCountMpl = 1;

    private float realEnemyCountMpl = 1;
    private List<EnemySubWave> subWavesInProgress;
    private List<EnemySubWave> finishedSubWaves;

    public bool AllEnemiesSpawned { get { return subWaves.Length == finishedSubWaves.Count; } }

    public void StartWave(float _enemyCountMpl = 1) {
        subWavesInProgress = new List<EnemySubWave>();
        finishedSubWaves = new List<EnemySubWave>();
        realEnemyCountMpl = baseEnemyCountMpl * _enemyCountMpl;
    }


    public void UpdateWave(float timeFromWaveStart) {
        //Check if new sub waves has to be started
        for (int i = 0; i < subWaves.Length; ++i) {
            if (!finishedSubWaves.Contains(subWaves[i]) && 
                !subWavesInProgress.Contains(subWaves[i]) && 
                timeFromWaveStart >= subWaves[i].SubWaveStartTime) {

                subWaves[i].StartWave(realEnemyCountMpl);
                subWavesInProgress.Add(subWaves[i]);
            }
        }

        //Update subwaves currently in progress
        for (int i = 0; i < subWavesInProgress.Count; ++i) {
            if (!subWavesInProgress[i].WaveIsReady) {
                subWavesInProgress[i].UpdateSubwave(timeFromWaveStart);
            } else {
                finishedSubWaves.Add(subWavesInProgress[i]);
                subWavesInProgress.RemoveAt(i);
            }
        }
    }
}
