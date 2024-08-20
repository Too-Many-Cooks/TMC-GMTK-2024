using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using Unity.VisualScripting;
using UnityEngine;
using static WaveSetDefinition;
using static WaveSystem.WaveSet;

[RequireComponent(typeof(EnemyWaveSpawner))]
[RequireComponent(typeof(ItemWaveSpawner))]
public class WaveSystem : MonoBehaviour
{
    [SerializeField] List<WaveSet> waveSets = new List<WaveSet>();

    private EnemyWaveSpawner enemyWaveSpawner;
    private ItemWaveSpawner itemWaveSpawner;

    [SerializeField][ReadOnly]private int waveSetIndex = 0;
    [SerializeField][ReadOnly]private int waveSetRepetitions = 0;
    [SerializeField][ReadOnly]private int waveIndex = 0;
    [SerializeField][ReadOnly]private float waveTimer = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyWaveSpawner = GetComponent<EnemyWaveSpawner>();
        itemWaveSpawner = GetComponent<ItemWaveSpawner>();
    }

    void Update()
    {
        if(waveSetIndex < waveSets.Count && waveIndex >= waveSets[waveSetIndex].Count) {
            var currentWaveSet = waveSets[waveSetIndex];
            waveIndex = 0;
            waveTimer = 0f;
            switch (currentWaveSet.RepetitionMode) {
                case WaveRepetitionMode.Once:
                    waveSetRepetitions=0;
                    waveSetIndex++;
                    break;
                case WaveRepetitionMode.Repeat:
                    waveSetRepetitions++;
                    if(waveSetRepetitions >= currentWaveSet.NumberOfRepetitions) {
                        waveSetRepetitions = 0;
                        waveSetIndex++;
                    }
                    break;
                case WaveRepetitionMode.RepeatContinual:
                    waveSetRepetitions++;
                    break;
            }
        }
        if(waveSetIndex >= waveSets.Count) {
            waveIndex = 0;
            waveSetRepetitions = 0;
            waveTimer = 0f;
            return;
        }

        waveTimer += Time.deltaTime;

        var currentWave = waveSets[waveSetIndex].Waves[waveIndex];

        if(waveTimer < currentWave.WaveStartTime) {
            enemyWaveSpawner.Clear();
        } else if(enemyWaveSpawner.CurrentWave != currentWave.EnemyWave) {
            enemyWaveSpawner.StartWave(currentWave.EnemyWave, waveTimer - currentWave.WaveStartTime);
            enemyWaveSpawner.CurrentWave = currentWave.EnemyWave;
        }

        if(waveTimer >= currentWave.WaveTotalTime) {
            waveTimer -= currentWave.WaveTotalTime;
            waveIndex++;
        }
    }

    [Serializable]
    public class WaveSet {
        public enum WaveRepetitionMode { Once, Repeat, RepeatContinual }
        public WaveRepetitionMode RepetitionMode;

        [ConditionalField(nameof(RepetitionMode), false, WaveRepetitionMode.Repeat)]
        [Min(1)]
        public int NumberOfRepetitions;
        public WaveSetDefinition Definition;

        public List<Wave> Waves  { get { return Definition.Waves; } }
        public int Count { get { return Waves.Count; } }
    }
}
