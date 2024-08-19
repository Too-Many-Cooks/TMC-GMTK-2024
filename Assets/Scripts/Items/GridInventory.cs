using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using MyBox;
using System.Linq;
using System;
using System.Data;

[System.Serializable]
public class GridInventory
{

    public Vector2Int GridSize {
        get { CalculateGridSize(); return _gridSize; }
        set { _gridSize = value; ResizeGrid(value.x, value.y); }
    }
    [SerializeField] private Vector2Int _gridSize;

    [ReadOnly] public List<InventoryRow> Rows;
    [SerializeField] public Dictionary<InventoryItem, Vector2Int> ItemDrawPositions = new Dictionary<InventoryItem, Vector2Int>();

    public bool CanAddOrMoveItem(int x, int y, InventoryItem item)
    {
        if(item == null || item.Definition == null) return false;
        for (int xi = 0; xi < item.Definition.shape.GridSize.x; xi++)
        {
            for (int yi = 0; yi < item.Definition.shape.GridSize.y; yi++)
            {
                if (!item.Definition.shape.GetCell(xi, yi))
                    continue;

                var col = x + xi;
                var row = y + yi;
                if (row < 0 || row >= Rows.Count) return false;
                if (col < 0 || col >= Rows[row].Columns.Count) return false;
                if (!Rows[row].Columns[col].IsEmpty() && Rows[row].Columns[col].Item != item) return false;
                if (Rows[row].Columns[col].CellState != InventoryCell.CellStatus.Unlocked) return false;
            }
        }
        return true;
    }

    public bool CanReplaceItem(int x, int y, InventoryItem toPlaceItem, out InventoryItem toBeReplacedItem)
    {
        toBeReplacedItem = null;

        if (toPlaceItem== null || toPlaceItem.Definition == null) return false;
        for (int xi = 0; xi < toPlaceItem.Definition.shape.GridSize.x; xi++)
        {
            for (int yi = 0; yi < toPlaceItem.Definition.shape.GridSize.y; yi++)
            {
                if (!toPlaceItem.Definition.shape.GetCell(xi, yi))
                    continue;

                var col = x + xi;
                var row = y + yi;
                if (row < 0 || row >= Rows.Count) return false;
                if (col < 0 || col >= Rows[row].Columns.Count) return false;
                if (!Rows[row].Columns[col].IsEmpty() && Rows[row].Columns[col].Item != toPlaceItem)
                {
                    if (toBeReplacedItem == null)
                        toBeReplacedItem = Rows[row].Columns[col].Item;
                    else if(toBeReplacedItem != Rows[row].Columns[col].Item)
                        return false;
                }
                if (Rows[row].Columns[col].CellState != InventoryCell.CellStatus.Unlocked) return false;
            }
        }
        return true;

    }

    public bool TryAddOrMoveItem(int x, int y, InventoryItem item)
    {
        if(!CanAddOrMoveItem(x, y, item)) return false;

        TryRemoveItem(item);

        ItemDrawPositions.Add(item, new Vector2Int(x, y));
        
        for(int xi = 0; xi < item.Definition.shape.GridSize.x; xi++)
        {
            for (int yi = 0; yi < item.Definition.shape.GridSize.y; yi++)
            {
                if (!item.Definition.shape.GetCell(xi, yi))
                    continue;

                var col = x + xi;
                var row = y + yi;
                Rows[row].Columns[col].Item = item;
            }
        }

        return true;
    }

    public bool TryRemoveItem(InventoryItem item) {
        var found = false;
        if(item != null)
        {
            foreach(var row in Rows) {
                foreach(var cell in row.Columns) {
                    if(cell.Item == item) {
                        cell.Item = null;
                        found = true;
                    }
                }
            }
            if(ItemDrawPositions.ContainsKey(item)) {
                found = true;
                ItemDrawPositions.Remove(item);
            }
        }
        return found;
    }

    public bool TryRemoveItem(int x, int y) {
        InventoryItem item = null;
        if(y >= 0 && y < Rows.Count) {
            if(x >= 0 && x < Rows[y].Columns.Count) {
                item = Rows[y].Columns[x].Item;
            }
        }
        return TryRemoveItem(item);
    }

    public InventoryCell GetCell(int x, int y) {
        if(y < 0 || x < 0 || y >= Rows.Count || x >= Rows[y].Columns.Count)
            return null;
        return Rows[y].Columns[x];
    }

    public InventoryCell GetCell(Vector2Int pos) {
        return GetCell(pos.x, pos.y);
    }

    public InventoryItem GetInventoryItemAt(int x, int y) {
        return GetCell(x, y)?.Item;
    }

    public InventoryCell[] GetCellsWithStates(InventoryCell.CellStatus[] cellStates) {
        var cells = new List<InventoryCell>();
        foreach(var row in Rows) {
            foreach(var cell in row.Columns) {
                foreach(var cellState in cellStates) {
                    if(cell.CellState == cellState){
                        cells.Add(cell);
                        break;
                    }
                }
            }
        }
        return cells.ToArray();
    }

    public InventoryCell[] GetCellsWithState(InventoryCell.CellStatus cellState) {
        var cellStates = new InventoryCell.CellStatus[1];
        cellStates[0] = cellState;
        return GetCellsWithStates(cellStates);
    }

    public Vector2Int[] GetCellIndexesWithStates(InventoryCell.CellStatus[] cellStates) {
        var cellIndexes = new List<Vector2Int>();
        foreach(var (row, y) in Rows.Select((v, i) => (v, i))) {
            foreach(var (cell, x) in row.Columns.Select((v, i) => (v, i))) {
                foreach(var cellState in cellStates) {
                    if(cell.CellState == cellState){
                        cellIndexes.Add(new Vector2Int(x, y));
                        break;
                    }
                }
            }
        }
        return cellIndexes.ToArray();
    }

    public Vector2Int[] GetCellIndexesWithState(InventoryCell.CellStatus cellState) {
        var cellStates = new InventoryCell.CellStatus[1];
        cellStates[0] = cellState;
        return GetCellIndexesWithStates(cellStates);
    }

    public void ResizeGrid (int x, int y) {
        if(y > Rows.Count) {
            for(int row = Rows.Count; row < y; row++) {
                Rows.Add(new InventoryRow() { Columns = new List<InventoryCell>(x) });
            }
        } else if (y < Rows.Count) {
            Rows.RemoveRange(y, Rows.Count - y);
        }
        foreach(var row in Rows) {
            if(x > row.Columns.Count) {
                for(int col = row.Columns.Count; col < x; col++) {
                    row.Columns.Add(new InventoryCell() { CellState = InventoryCell.CellStatus.Locked, Item = null});
                }
            } else if (x < row.Columns.Count) {
                row.Columns.RemoveRange(x, row.Columns.Count - x);
            }
        }
    }

    public void ResizeGrid () {
        ResizeGrid(_gridSize.x, _gridSize.y);
    }

    public void CalculateGridSize() {
            _gridSize.x = int.MaxValue;
            _gridSize.y = Rows.Count;
            for(int row = 0; row < _gridSize.y; row++) {
                _gridSize.x = math.min(_gridSize.x, Rows[row].Columns.Count);
            }
    }

    internal void ClearInventory()
    {
        foreach(var row in Rows) {
            foreach(var cell in row.Columns) {
                cell.Item = null;
            }
        }
        ItemDrawPositions.Clear();
    }

    public void RegenerateItemDrawPositions() {
        ItemDrawPositions.Clear();
        foreach(var (row, y) in Rows.Select((v, i) => (v, i))) {
            foreach(var (cell, x) in row.Columns.Select((v, i) => (v, i))) {
                if (cell.IsEmpty()) continue;
                var cellPosition = new Vector2Int(x, y);
                if(!ItemDrawPositions.ContainsKey(cell.Item)) {
                    ItemDrawPositions.Add(cell.Item, cellPosition);
                } else {
                    var topLeft = ItemDrawPositions[cell.Item];
                    topLeft = Vector2Int.Min(topLeft, cellPosition);
                    ItemDrawPositions[cell.Item] = topLeft;
                }
            }
        }
        
    }

    [System.Serializable]
    public class InventoryCell
    {
        public enum CellStatus { Locked, Unlocked, Damaged, Broken, Null }
        [SerializeField] public CellStatus CellState = CellStatus.Locked;
        #nullable enable
        [SerializeField] public InventoryItem? Item = null;
        #nullable disable

        public bool IsEmpty() {
            return Item == null || Item.Definition == null;
        }
    }

    [System.Serializable]
    public class InventoryRow
    {
        public List<InventoryCell> Columns;
    }
}
