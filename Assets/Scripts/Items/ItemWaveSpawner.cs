using System;
using System.Collections.Generic;
using MyBox;
using Unity.Mathematics;
using UnityEngine;
using static LootTableDefinition;

public class ItemWaveSpawner : MonoBehaviour
{
    public float dropRange = 15f;
    public Vector3 dropOffset = new Vector3(0, -1f, 0f);
    public ItemWave CurrentWave;
    [SerializeField][ReadOnly]private float waveTimer = 0f;
    [SerializeField][ReadOnly]private int dropGroupIndex = 0;
    private Transform playerTransform;

    public bool IsActive() {
        return dropGroupIndex < CurrentWave.Count;
    }

    [Serializable]
    public struct ItemWave {
        public ItemDropGroup[] DropGroups;

        public int Count { get { return DropGroups.Length; } }

        public float TimerFor(int dropGroupIndex) {
            float timer = 0f;
            var timerCount = math.min(dropGroupIndex + 1, DropGroups.Length);
            for (int i = 0; i < timerCount; i++) {
                timer += DropGroups[i].Timer;
            }
            return timer;
        }

        public bool IsEmpty() {
            return DropGroups.Length == 0;
        }

        public void Clear() {
            DropGroups = new ItemDropGroup[0];
        }

        public static ItemWave Empty { get { return new ItemWave(); } }

        public float WaveDuration { get { return TimerFor(Count); } }

        public static bool operator ==(ItemWave lhs, ItemWave rhs) => lhs.Equals(rhs);
        public static bool operator !=(ItemWave lhs, ItemWave rhs) => !(lhs == rhs);
        public override readonly int GetHashCode() => DropGroups.GetHashCode();
        public override readonly bool Equals(object obj) 
        {
            if (!(obj is ItemWave))
                return false;

            ItemWave mys = (ItemWave) obj;
            return mys.DropGroups == this.DropGroups;
        }
    }

    [Serializable]
    public struct ItemDropGroup {
        public float Timer;
        public LootTableDefinition Definition;
        public List<LootTableRoll> Rolls { get { return Definition.Rolls; } }

        public ItemDefinition[] RollResults() {
            return Definition.RollResults();
        }

        public ItemDefinition[] RollDrops() {
            return Definition.RollDrops();
        }

        public void TestRollResults() {
            Definition.TestRollResults();
        }

        public void TestRollDrops() {
            Definition.TestRollDrops();
        }
    }

    void Start() {
        if(CurrentWave.DropGroups == null) {
            CurrentWave.DropGroups = new ItemDropGroup[0];
        }
        waveTimer = 0f;
        dropGroupIndex = 0;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update() {
        if(!IsActive()) {
            return;
        }

        waveTimer += Time.deltaTime;

        while(IsActive() && waveTimer > CurrentWave.TimerFor(dropGroupIndex)) {
            SpawnItems(CurrentWave.DropGroups[dropGroupIndex].RollDrops());
            dropGroupIndex++;
        }
    }

    public void SpawnItems(ItemDefinition[] itemDefinitions)
    {
        foreach(var itemDefinition in itemDefinitions) {
            if(itemDefinition != null) {
                SpawnItem(itemDefinition);
            }
        }
    }

    public void SpawnItem(ItemDefinition itemDefinition)
    {
        Vector3 dropPosition = GetDropPosition();
        var itemDrop = Instantiate(itemDefinition.WorldItemPrefab, dropPosition, Quaternion.identity);
        itemDrop.GetComponent<WorldItem>().Definition = itemDefinition;
    }

    private Vector3 GetDropPosition()
    {
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle;
        Vector2 ranOffset = randomDirection * dropRange;
        return playerTransform.position + new Vector3(ranOffset.x, 0f, ranOffset.y) + dropOffset;
    }

    public void StartWave(ItemWave wave, float initialTimer = 0f) {
        CurrentWave = wave;
        dropGroupIndex = 0;
        waveTimer = initialTimer;
    }

    public void Clear() {
        waveTimer = 0f;
        dropGroupIndex = 0;
        CurrentWave.Clear();
    }
}
