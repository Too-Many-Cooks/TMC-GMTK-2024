using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyWaveSpawner;
using static ItemWaveSpawner;

[CreateAssetMenu(fileName = "NewWaveSetDefinition", menuName = "Wave Set", order = 2004)]
public class WaveSetDefinition : ScriptableObject
{
    public List<Wave> Waves;

    [Serializable]
    public struct Wave {
        public float WaveStartDelay;
        public float WaveEndDelay;
        public EnemyWave EnemyWave;
        public ItemWave ItemWave;

        public float WaveDuration { get { return math.max(EnemyWave.WaveDuration, ItemWave.WaveDuration); } }
        public float WaveStartTime { get { return WaveStartDelay; } }
        public float WaveEndTime { get { return WaveStartTime + WaveDuration; } }
        public float WaveTotalTime { get { return WaveEndTime + WaveEndDelay; } }
    }
}
