using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int baseEnemyCount = 5;
    [SerializeField] private float spawnInterval = 0.5f;
    [SerializeField] private float waveCooldown = 3f;

    private int currentWave = 0;
    private int aliveEnemyCount = 0;

    public int CurrentWave => currentWave;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        while (true)
        {
            currentWave++;
            int count = baseEnemyCount + (currentWave - 1) * 2;

            GameManager.Instance?.uiManager?.UpdateWaveUI(currentWave);

            for (int i = 0; i < count; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }

            yield return new WaitUntil(() => aliveEnemyCount <= 0);
            yield return new WaitForSeconds(waveCooldown);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        aliveEnemyCount++;
    }

    public void OnEnemyDied()
    {
        aliveEnemyCount = Mathf.Max(0, aliveEnemyCount - 1);
    }
}