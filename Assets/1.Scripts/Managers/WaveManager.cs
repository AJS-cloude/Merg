using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 디펜스: 웨이브 진행, 적 스폰.
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [SerializeField] WaveData[] waves;
    [SerializeField] Transform spawnPoint;
    [Tooltip("적이 이동할 목표(기지). 도달 시 기지 HP 감소")]
    [SerializeField] Transform baseTarget;
    [SerializeField] float spawnInterval = 0.5f;
    [SerializeField] Enemy enemyPrefab;

    int _currentWaveIndex = -1;
    bool _spawning;

    public int CurrentWaveIndex => _currentWaveIndex;
    public int TotalWaves => waves != null ? waves.Length : 0;
    public bool IsSpawning => _spawning;
    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;
    public event Action<Enemy> OnEnemySpawned;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartNextWave()
    {
        if (waves == null || waves.Length == 0) return;
        _currentWaveIndex++;
        if (_currentWaveIndex >= waves.Length)
        {
            _currentWaveIndex = waves.Length - 1;
            OnWaveCompleted?.Invoke(_currentWaveIndex);
            return;
        }
        OnWaveStarted?.Invoke(_currentWaveIndex);
        StartCoroutine(SpawnWaveRoutine(waves[_currentWaveIndex]));
    }

    IEnumerator SpawnWaveRoutine(WaveData wave)
    {
        _spawning = true;
        if (wave?.entries == null) { _spawning = false; yield break; }

        foreach (var entry in wave.entries)
        {
            if (entry.enemy == null) continue;
            for (int i = 0; i < entry.count; i++)
            {
                SpawnEnemy(entry.enemy);
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        _spawning = false;
        OnWaveCompleted?.Invoke(_currentWaveIndex);
    }

    void SpawnEnemy(EnemyData data)
    {
        if (enemyPrefab == null || data == null) return;
        Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        var e = Instantiate(enemyPrefab, pos, Quaternion.identity);
        e.Setup(data);
        e.SetMoveTarget(baseTarget);
        OnEnemySpawned?.Invoke(e);
    }

    public void SetWaves(WaveData[] newWaves) => waves = newWaves;
    public void ResetWaves() => _currentWaveIndex = -1;
}
