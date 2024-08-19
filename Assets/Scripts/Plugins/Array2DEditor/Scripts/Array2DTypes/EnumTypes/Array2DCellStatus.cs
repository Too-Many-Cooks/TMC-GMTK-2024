using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using static GridInventory.InventoryCell;


namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DCellStatus : Array2D<CellStatus>
    {
        [SerializeField]
        CellRowCellStatus[] cells = new CellRowCellStatus[Consts.defaultGridSize];

        protected override CellRow<CellStatus> GetCellRow(int idx)
        {
            return cells[idx];
        }

        public void SetGridSize(Vector2Int gridSize) {
            var oldGridSize = this.gridSize;
            var oldCells = cells;

            this.gridSize = gridSize;
            cells = new CellRowCellStatus[gridSize.y];
            for(int y = 0; y < gridSize.y; y++) {
                cells[y] = new CellRowCellStatus();
                cells[y].row = new CellStatus[gridSize.x];
            }
            Vector2Int copyGridSize = Vector2Int.Min(gridSize, oldGridSize);
            for(int x = 0; x < copyGridSize.x; x++) {
                for(int y = 0; y < copyGridSize.y; y++) {
                    SetCell(x, y, oldCells[y][x]);
                }
            }
        }
    }

    [System.Serializable]
    public class CellRowCellStatus : CellRow<CellStatus> { }
}
