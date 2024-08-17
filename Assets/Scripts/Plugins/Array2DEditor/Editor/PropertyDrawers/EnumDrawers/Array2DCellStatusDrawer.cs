using UnityEditor;
using static GridInventory.InventoryCell;

namespace Array2DEditor
{
    [CustomPropertyDrawer(typeof(Array2DCellStatus))]
    public class Array2DCellStatusDrawer : Array2DEnumDrawer<CellStatus> {}
}
