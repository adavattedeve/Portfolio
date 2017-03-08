using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PriestSpawner : MonoBehaviour {
    public GameObject priestPrefab;
    public float timeBetweenSpawns = 8f;
    public float timeBeforeFirstSpawn = 2f;
    public float timeBetweenSpawnsWhenNoPriestsLeft = 1f;
    [Range(0,1)]public float chanceForAdditionalPriestSpawn= 0.25f;
    public float chanceIncreasePerDifficultyLvl;
    private List<Transform> priestSpawnPoints;
    private int lastSpawnPoint=0;
    private int difficulty=0;
    public float spawnTimeDecreasePerDifficultyLevel = 0.05f;

    private int priestsSpawned;
	// Use this for initialization
	void Awake () {
        priestsSpawned = 0;
        Transform[] childrenAndSelf = GetComponentsInChildren<Transform>();
        for (int i = 0; i < childrenAndSelf.Length; ++i)
        {
            if (childrenAndSelf[i] == transform)
            {
                childrenAndSelf[i] = null;
                break;
            }
        }
        priestSpawnPoints = new List<Transform>();
        for (int i = 0; i < childrenAndSelf.Length; ++i)
        {
            if (childrenAndSelf[i] != null)
            {
                priestSpawnPoints.Add(childrenAndSelf[i]);
            }
        }

        StartCoroutine(SpawnPriests(timeBeforeFirstSpawn));
        Debug.Log("Adding priest spawner events");



    }
    void Start()
    {
        GameManager.OnGameOver += StopSpawning;
        GameManager.OnPriestDefeated += AddDifficulty;
        GameManager.OnPriestDefeated += ResetPriestSpawnTime;
    }
        void Disable()
    {
        StopAllCoroutines();
       
  
    }

    private IEnumerator SpawnPriests(float initialWaitTime)
    {
        yield return new WaitForSeconds(initialWaitTime);
        while (true)
        {
            SpawnPriest();
            yield return new WaitForSeconds(timeBetweenSpawns-difficulty* spawnTimeDecreasePerDifficultyLevel);
        }
        
    }
    private void SpawnPriest()
    {
        int spawnIndex = Random.Range(0, priestSpawnPoints.Count);
        Instantiate(priestPrefab, priestSpawnPoints[spawnIndex].position, Quaternion.identity);
        ++priestsSpawned;
        lastSpawnPoint = spawnIndex;
        float random = Random.Range(0f,1f);

        if (random < chanceForAdditionalPriestSpawn + difficulty * chanceIncreasePerDifficultyLvl)
        {
            spawnIndex = Random.Range(0, priestSpawnPoints.Count);
            if (spawnIndex == lastSpawnPoint)
            {
                if (spawnIndex < priestSpawnPoints.Count - 1)
                {
                    ++spawnIndex;
                }
                else
                {
                    --spawnIndex;
                }
            }
            Instantiate(priestPrefab, priestSpawnPoints[spawnIndex].position, Quaternion.identity);
            ++priestsSpawned;
            lastSpawnPoint = spawnIndex;
        }
    }
    public void StopSpawning()
    {
        Debug.Log("Disconnecting priest spawner events");
        GameManager.OnGameOver -= StopSpawning;
        GameManager.OnPriestDefeated -= AddDifficulty;
        GameManager.OnPriestDefeated -= ResetPriestSpawnTime;
        Destroy(gameObject);
    }
    public void AddDifficulty(int priestsDefeated)
    {
        difficulty = priestsDefeated;
    }
    public void ResetPriestSpawnTime(int priestsDefeated)
    {
        if (priestsDefeated == priestsSpawned)
        {
            StopAllCoroutines();
            StartCoroutine(SpawnPriests(timeBetweenSpawnsWhenNoPriestsLeft));
        }

    }
}
