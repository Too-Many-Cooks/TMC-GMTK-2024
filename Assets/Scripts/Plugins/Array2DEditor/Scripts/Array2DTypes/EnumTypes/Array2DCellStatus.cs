using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }

    [System.Serializable]
    public class CellRowCellStatus : CellRow<CellStatus> { }
}
