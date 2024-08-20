using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using Unity.Mathematics;
using UnityEngine;
using static EnemySpawnPoolDefinition;

[Serializable]
[CreateAssetMenu(fileName = "NewEnemySpawnGroupDefinition", menuName = "Spawn Group", order = 2003)]
public class EnemySpawnGroupDefinition : ScriptableObject
{
    [SerializeField] public List<Spawn> Spawns;

    [Serializable]
    public struct Spawn {
        public int NoSpawnWeight;
        public EnemySpawnPoolDefinition SpawnPool;

        public List<SpawnEntry> PossibleSpawns { get { return SpawnPool?.PossibleSpawns; }} 

        public int TotalWeight() {
            return SpawnPool ? SpawnPool.TotalWeight() + NoSpawnWeight : NoSpawnWeight;
        }

        public GameObject SpawnEnemy() {
            var roll = UnityEngine.Random.Range(0, TotalWeight());
            var possibleSpawns = PossibleSpawns;

            if(possibleSpawns == null) return null;
            
            foreach(var spawn in possibleSpawns) {
                if(roll < spawn.SpawnWeight) {
                    return spawn.EnemyPrefab;
                }
                roll -= math.max(0, spawn.SpawnWeight);
            }
            return null;
        }
    }

    public GameObject[] GenerateSpawnResults() {
        var items = new GameObject[Spawns.Count];
        for(int i = 0; i < Spawns.Count; i++) {
            items[i] = Spawns[i].SpawnEnemy();
        }
        return items;
    }

    public GameObject[] GenerateSpawnList() {
        return GenerateSpawnResults().Where(i => i != null).ToArray();
    }

    [ButtonMethod]
    public void TestGenerateSpawnResults() {
        var drops = GenerateSpawnResults();
        Debug.Log(SpawnResultDebugString(drops));
    }

    [ButtonMethod]
    public void TestGenerateSpawnList() {
        var drops = GenerateSpawnList();
        Debug.Log(SpawnListDebugString(drops));
    }

    public static string SpawnResultDebugString(GameObject[] spawns) {
        var dropMessage = "Results from " + spawns.Length + " spawns: [";
        for(int i = 0; i < spawns.Length; i++) {
            dropMessage += (spawns[i] != null ? spawns[i].name : "No Enemy");
            if(i+1 < spawns.Length) {
                dropMessage += ", ";
            }
        }
        dropMessage += "]";
        return dropMessage;
        
    }

    public static string SpawnListDebugString(GameObject[] spawns) {
        var dropMessage = "" + spawns.Length + " enemies spawned: [";
        for(int i = 0; i < spawns.Length; i++) {
            dropMessage += (spawns[i] != null ? spawns[i].name : "No Enemy");
            if(i+1 < spawns.Length) {
                dropMessage += ", ";
            }
        }
        dropMessage += "]";
        return dropMessage;

    }
}
