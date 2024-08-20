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
    public Rect WaveIndicatorPosition;
    public GUIStyle WaveIndicatorStyle = new GUIStyle();

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
            enemyWaveSpawner.Clear();
            itemWaveSpawner.Clear();
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
            itemWaveSpawner.Clear();
        } else {
            if(enemyWaveSpawner.CurrentWave != currentWave.EnemyWave) {
                enemyWaveSpawner.StartWave(currentWave.EnemyWave, waveTimer - currentWave.WaveStartTime);
            }
            if(itemWaveSpawner.CurrentWave != currentWave.ItemWave) {
                itemWaveSpawner.StartWave(currentWave.ItemWave, waveTimer - currentWave.WaveStartTime);
            }
        }

        if(waveTimer >= currentWave.WaveTotalTime) {
            waveTimer -= currentWave.WaveTotalTime;
            waveIndex++;
        }
    }

    void OnGUI() {
        GUI.Label(WaveIndicatorPosition, "Wave " + waveSetIndex + 1, WaveIndicatorStyle);
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

    [ButtonMethod]
    public void NextWaveSet () {
        var currentWaveSet = waveSets[waveSetIndex];
        waveIndex = 0;
        waveTimer = 0f;
        enemyWaveSpawner.Clear();
        itemWaveSpawner.Clear();
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

    [ButtonMethod]
    public void Add1SToTimer () {
        waveTimer += 1f;
        enemyWaveSpawner.waveTimer += 1f;
        itemWaveSpawner.waveTimer += 1f;
    }

    [ButtonMethod]
    public void Add5SToTimer () {
        waveTimer += 5f;
        enemyWaveSpawner.waveTimer += 5f;
        itemWaveSpawner.waveTimer += 5f;
    }

    [ButtonMethod]
    public void Add10SToTimer () {
        waveTimer += 10f;
        enemyWaveSpawner.waveTimer += 10f;
        itemWaveSpawner.waveTimer += 10f;
    }

    [ButtonMethod]
    public void Add30SToTimer () {
        waveTimer += 30f;
        enemyWaveSpawner.waveTimer += 30f;
        itemWaveSpawner.waveTimer += 30f;
    }

    [ButtonMethod]
    public void Add60SToTimer () {
        waveTimer += 60f;
        enemyWaveSpawner.waveTimer += 60f;
        itemWaveSpawner.waveTimer += 60f;
    }
}
