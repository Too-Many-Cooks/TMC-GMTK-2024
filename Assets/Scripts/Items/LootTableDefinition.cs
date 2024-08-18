using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLootTableDefinition", menuName = "ScriptableObjects/LootTableDefinition")]
public class LootTableDefinition : ScriptableObject
{
    [SerializeField] public List<LootTableRoll> Rolls;

    [Serializable]
    public struct LootTableEntry {
        public int DropWeight;
        public ItemDefinition Item;
    }

    [Serializable]
    public struct LootTableRoll {
        public int NoDropWeight;
        public List<LootTableEntry> ItemEntries;

        public int TotalWeight() {
            var weight = NoDropWeight;
            foreach(var item in ItemEntries) {
                weight += math.max(0, item.DropWeight);
            }
            return weight;
        }

        public ItemDefinition Roll() {
            var roll = UnityEngine.Random.Range(0, TotalWeight());
            foreach(var itemEntry in ItemEntries) {
                if(roll < itemEntry.DropWeight) {
                    return itemEntry.Item;
                }
                roll -= math.max(0, itemEntry.DropWeight);
            }
            return null;
        }
    }

    public ItemDefinition[] RollResults() {
        var items = new ItemDefinition[Rolls.Count];
        for(int i = 0; i < Rolls.Count; i++) {
            items[i] = Rolls[i].Roll();
        }
        return items;
    }

    public ItemDefinition[] RollDrops() {
        return RollResults().Where(i => i != null).ToArray();
    }

    [ButtonMethod]
    private void TestRollResults() {
        var drops = RollResults();
        var dropMessage = "Results from " + drops.Length + " rolls: [";
        for(int i = 0; i < drops.Length; i++) {
            dropMessage += (drops[i] != null ? drops[i].name : "Nothing");
            if(i+1 < drops.Length) {
                dropMessage += ", ";
            }
        }
        dropMessage += "]";
        Debug.Log(dropMessage);
    }

    [ButtonMethod]
    private void TestRollDrops() {
        var drops = RollDrops();
        var dropMessage = "" + drops.Length + " items dropped: [";
        for(int i = 0; i < drops.Length; i++) {
            dropMessage += (drops[i] != null ? drops[i].name : "Nothing");
            if(i+1 < drops.Length) {
                dropMessage += ", ";
            }
        }
        dropMessage += "]";
        Debug.Log(dropMessage);
    }
}
