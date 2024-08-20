using System;
using System.Collections.Generic;
using MyBox;
using Unity.Mathematics;
using UnityEngine;
using static EnemySpawnGroupDefinition;

public class EnemyWaveSpawner : MonoBehaviour
{
    public  float spawnDistance = 25f;
    public EnemyWave CurrentWave;
    [SerializeField][ReadOnly]private float waveTimer = 0f;
    [SerializeField][ReadOnly]private int spawnGroupIndex = 0;
    private Transform playerTransform;

    public bool IsActive() {
        return spawnGroupIndex < CurrentWave.Count;
    }

    [Serializable]
    public struct EnemyWave {
        public EnemySpawnGroup[] SpawnGroups;

        public int Count { get { return SpawnGroups.Length; } }

        public float TimerFor(int spawnGroupIndex) {
            float timer = 0f;
            var timerCount = math.min(spawnGroupIndex + 1, SpawnGroups.Length);
            for (int i = 0; i < timerCount; i++) {
                timer += SpawnGroups[i].Timer;
            }
            return timer;
        }

        public bool IsEmpty() {
            return SpawnGroups.Length == 0;
        }

        public void Clear() {
            SpawnGroups = new EnemySpawnGroup[0];
        }

        public static EnemyWave Empty { get { return new EnemyWave(); } }

        public float WaveDuration { get { return TimerFor(Count); } }

        public static bool operator ==(EnemyWave lhs, EnemyWave rhs) => lhs.Equals(rhs);
        public static bool operator !=(EnemyWave lhs, EnemyWave rhs) => !(lhs == rhs);
        public override readonly int GetHashCode() => SpawnGroups.GetHashCode();
        public override readonly bool Equals(object obj) 
        {
            if (!(obj is EnemyWave))
                return false;

            EnemyWave mys = (EnemyWave) obj;
            return mys.SpawnGroups == this.SpawnGroups;
        }
    }

    [Serializable]
    public struct EnemySpawnGroup {
        public float Timer;
        public EnemySpawnGroupDefinition Definition;
        public List<Spawn> Spawns { get { return Definition.Spawns; } }

        public GameObject[] GenerateSpawnResults() {
            return Definition.GenerateSpawnResults();
        }

        public GameObject[] GenerateSpawnList() {
            return Definition.GenerateSpawnList();
        }

        public void TestGenerateSpawnResults() {
            Definition.TestGenerateSpawnResults();
        }

        public void TestGenerateSpawnList() {
            Definition.TestGenerateSpawnList();
        }
    }

    void Start() {
        if(CurrentWave.SpawnGroups == null) {
            CurrentWave.SpawnGroups = new EnemySpawnGroup[0];
        }
        waveTimer = 0f;
        spawnGroupIndex = 0;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update() {
        if(!IsActive()) {
            return;
        }

        waveTimer += Time.deltaTime;

        while(IsActive() && waveTimer > CurrentWave.TimerFor(spawnGroupIndex)) {
            SpawnEnemies(CurrentWave.SpawnGroups[spawnGroupIndex].GenerateSpawnList());
            spawnGroupIndex++;
        }
    }

    public void SpawnEnemies(GameObject[] enemyPrefabs)
    {
        foreach(var enemyPrefab in enemyPrefabs) {
            if(enemyPrefab != null) {
                SpawnEnemy(enemyPrefab);
            }
        }
    }

    public void SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 outOfViewPosition = GetOutOfViewPosition();
        Instantiate(enemyPrefab, outOfViewPosition, Quaternion.identity, transform);
    }

    private Vector3 GetOutOfViewPosition()
    {
        Vector2 randomDirection = UnityEngine.Random.onUnitSphere;
        randomDirection = randomDirection.normalized;
        Vector2 ranOffset = randomDirection * spawnDistance;
        return playerTransform.position + new Vector3(ranOffset.x, -playerTransform.position.y, ranOffset.y);
    }

    public void StartWave(EnemyWave wave, float initialTimer = 0f) {
        CurrentWave = wave;
        spawnGroupIndex = 0;
        waveTimer = initialTimer;
    }

    public void Clear() {
        waveTimer = 0f;
        spawnGroupIndex = 0;
        CurrentWave.Clear();
    }
}
