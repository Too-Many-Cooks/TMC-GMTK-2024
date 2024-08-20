using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyWaveSpawner;

[CreateAssetMenu(fileName = "NewWaveSetDefinition", menuName = "Wave Set", order = 2004)]
public class WaveSetDefinition : ScriptableObject
{
    public List<Wave> Waves;

    [Serializable]
    public struct Wave {
        public float WaveStartDelay;
        public float WaveEndDelay;
        public EnemyWave EnemyWave;

        public float WaveStartTime { get { return WaveStartDelay; } }
        public float WaveEndTime { get { return WaveStartTime + EnemyWave.WaveDuration; } }
        public float WaveTotalTime { get { return WaveStartTime + EnemyWave.WaveDuration + WaveEndDelay; } }
    }
}
