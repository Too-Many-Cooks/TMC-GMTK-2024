using Array2DEditor;
using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GridInventory.InventoryCell;

public class InventoryTester : MonoBehaviour
{
    public CharacterInventory CharacterInventory;
    public float timer = 0.0f;
    public int index = 0;
    [SerializeField] public List<TimelineEvent> Timeline;
    public Array2DCellStatus InventoryStatus;

    public RectInt UpdateRange = new RectInt(0, 0, 0, 0);

    [ButtonMethod]
    private void MaxRange(){
        UpdateRange = new RectInt(0,0,InventoryStatus.GridSize.x, InventoryStatus.GridSize.y);
    }

    [ButtonMethod]
    private void LockRange(){
        var range = UpdateRange;
        range.x = Math.Max(range.x, 0);
        range.y = Math.Max(range.y, 0);
        range.xMax = Math.Min(range.xMax, InventoryStatus.GridSize.x);
        range.yMax = Math.Min(range.yMax, InventoryStatus.GridSize.y);

        foreach(var pos in range.allPositionsWithin) {
                InventoryStatus.SetCell(pos.x, pos.y, CellStatus.Locked);
        }
    }

    [ButtonMethod]
    private void UnlockRange(){
        var range = UpdateRange;
        range.x = Math.Max(range.x, 0);
        range.y = Math.Max(range.y, 0);
        range.xMax = Math.Min(range.xMax, InventoryStatus.GridSize.x);
        range.yMax = Math.Min(range.yMax, InventoryStatus.GridSize.y);

        foreach(var pos in range.allPositionsWithin) {
                InventoryStatus.SetCell(pos.x, pos.y, CellStatus.Unlocked);
        }
    }

    [ButtonMethod]
    private void DamageRange(){
        var range = UpdateRange;
        range.x = Math.Max(range.x, 0);
        range.y = Math.Max(range.y, 0);
        range.xMax = Math.Min(range.xMax, InventoryStatus.GridSize.x);
        range.yMax = Math.Min(range.yMax, InventoryStatus.GridSize.y);

        foreach(var pos in range.allPositionsWithin) {
                InventoryStatus.SetCell(pos.x, pos.y, CellStatus.Damaged);
        }
    }

    [ButtonMethod]
    private void BreakRange(){
        var range = UpdateRange;
        range.x = Math.Max(range.x, 0);
        range.y = Math.Max(range.y, 0);
        range.xMax = Math.Min(range.xMax, InventoryStatus.GridSize.x);
        range.yMax = Math.Min(range.yMax, InventoryStatus.GridSize.y);

        foreach(var pos in range.allPositionsWithin) {
                InventoryStatus.SetCell(pos.x, pos.y, CellStatus.Broken);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateCharacterInventory();
    }

    [ButtonMethod]
    private void UpdateCharacterInventory()
    {
        if (CharacterInventory.inventory.Rows.Capacity < InventoryStatus.GridSize.y)
        {
            CharacterInventory.inventory.Rows.Capacity = InventoryStatus.GridSize.y;
        }
        for (int y = 0; y < InventoryStatus.GridSize.y; y++)
        {
            if (CharacterInventory.inventory.Rows[y].Columns.Capacity < InventoryStatus.GridSize.x)
            {
                CharacterInventory.inventory.Rows[y].Columns.Capacity = InventoryStatus.GridSize.x;
            }
            for (int x = 0; x < InventoryStatus.GridSize.x; x++)
            {
                CharacterInventory.inventory.Rows[y].Columns[x].CellState = InventoryStatus.GetCell(x, y);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(index < Timeline.Count)
        {
            timer += Time.deltaTime;
            if(timer >= Timeline[index].Delay)
            {
                timer -= Timeline[index].Delay;
                Timeline[index].Do(CharacterInventory.inventory);
                index++;
            }
        } else {
            timer = 0;
        }
    }

    [System.Serializable]
    public class TimelineEvent
    {
        public TimelineEvent(float delay)
        {
            this.Delay = delay;
        }

        public TimelineEvent() : this(0) { }
        public enum ActionType { Add, Remove, Move }

        public float Delay;
        public ActionType Action;
        public Vector2Int Position;
        [ConditionalField(nameof(Action), false, ActionType.Move)] public Vector2Int Position2;
        [ConditionalField(nameof(Action), false, ActionType.Add)] public ItemDefinition ItemPrefab;
        public virtual void Do(GridInventory inventory)
        {

            switch(Action)
            {
                case ActionType.Add:
                    var itemToAdd = new InventoryItem(ItemPrefab);
                    inventory.TryAddOrMoveItem(Position.x, Position.y, itemToAdd);
                    break;
                case ActionType.Remove:
                    inventory.TryRemoveItem(Position.x, Position.y);
                    break;
                case ActionType.Move:
                    var itemToMove = inventory.GetInventoryItemAt(Position.x, Position.y);
                    inventory.TryAddOrMoveItem(Position2.x, Position2.y, itemToMove);
                    break;
                default:
                    break;
            }
        }
    }
}