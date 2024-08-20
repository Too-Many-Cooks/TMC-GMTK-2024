using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemySpawnPoolDefinition", menuName = "Spawn Pool", order = 2002)]
public class EnemySpawnPoolDefinition : ScriptableObject
{
    public List<SpawnEntry> PossibleSpawns;

    public int TotalWeight() {
        var weight = 0;
        foreach(var spawn in PossibleSpawns) {
            weight += math.max(0, spawn.SpawnWeight);
        }
        return weight;
    }

    public GameObject SpawnEnemy() {
        var roll = UnityEngine.Random.Range(0, TotalWeight());
        foreach(var spawn in PossibleSpawns) {
            if(roll < spawn.SpawnWeight) {
                return spawn.EnemyPrefab;
            }
            roll -= math.max(0, spawn.SpawnWeight);
        }
        return null;
    }

    [Serializable]
    public struct SpawnEntry {
        public int SpawnWeight;
        public GameObject EnemyPrefab;
    }
}
