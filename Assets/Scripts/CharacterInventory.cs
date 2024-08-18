using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInventory : MonoBehaviour
{

    [Foldout("Debug View", true)] 
    [SerializeField][ReadOnly]DraggedItem currentlyDraggedItem;
    [SerializeField][ReadOnly]bool isDraggingItem;

    [Foldout("GUI", true)] 
    public Sprite LockedSlotIcon;
    public Sprite EmptySlotIcon;
    public Sprite DamagedSlotIcon;
    public Sprite BrokenSlotIcon;
    public Vector2 InventoryScreenPosition;
    [Foldout("GUI")] 
    public bool EnableDebugGUI = false;

    public GridInventory inventory;

    [SerializeField]
    private GameObject gridCellVisualsPrefab;
    [SerializeField]
    private GameObject inventoryItemVisualsPrefab;
    [SerializeField]
    private RectTransform inventoryAnchor;
    [SerializeField]
    private float cellSize = 32f;
    private InventoryGridCell[,] gridCellVisuals;
    private List<InventoryItemVisual> itemVisuals;

    private Vector2Int cachedGridSize;
    private float cachedCellSize;

    [ButtonMethod(order = 0)]
    private void ResizeGrid() {
        inventory.ResizeGrid();
    }

    [ButtonMethod(ButtonMethodDrawOrder.AfterInspector, nameof(inventory))]
    private void CalculateGridSize() {
        inventory.CalculateGridSize();
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateVisuals();
    }

    public bool BeginDraggingItem(InventoryItem inventoryItem, WorldItem worldItem = null) {
        if(isDraggingItem) return false;

        isDraggingItem = true;
        currentlyDraggedItem = new DraggedItem() { inventoryItem = inventoryItem, worldItem = worldItem };

        return true;
    }


    private void CreateVisuals()
    {
        foreach (Transform child in inventoryAnchor)
            Destroy(child.gameObject);

        gridCellVisuals = new InventoryGridCell[inventory.GridSize.x, inventory.GridSize.y];
        cachedGridSize = inventory.GridSize;
        cachedCellSize = cellSize;

        inventoryAnchor.anchoredPosition = new Vector3(-inventory.GridSize.x * cellSize - InventoryScreenPosition.x, -InventoryScreenPosition.y, 0f);

        foreach (var (row, y) in inventory.Rows.Select((v, i) => (v, i)))
        {
            GameObject rowGO = new GameObject("CellRow " + row);
            rowGO.AddComponent<RectTransform>();
            rowGO.transform.SetParent(inventoryAnchor);
            rowGO.transform.localPosition = new Vector3(0f, -y * cellSize, 0f);

            foreach (var (cell, x) in row.Columns.Select((v, i) => (v, i)))
            {
                GameObject gridCell = Instantiate(gridCellVisualsPrefab, rowGO.transform);
                gridCell.transform.localPosition = new Vector3((x+1) * cellSize, 0f, 0f);
                InventoryGridCell invGridCell = gridCell.GetComponent<InventoryGridCell>();
                gridCellVisuals[x, y] = invGridCell;

                invGridCell.Init(new Vector2(x, y), cellSize, cell.CellState, OnCellClick, OnCellHoverChange);
            }
        }
        itemVisuals = new List<InventoryItemVisual>();
    }

    private void UpdateInventoryCellVisuals()
    {
        // If something nasty changes -> Redo all visuals
        if(cachedCellSize != cellSize || cachedGridSize != inventory.GridSize)
        {
            CreateVisuals();
            return;
        }

        inventoryAnchor.anchoredPosition = new Vector3(-inventory.GridSize.x * cellSize - InventoryScreenPosition.x, -InventoryScreenPosition.y, 0f);

        foreach (var (row, y) in inventory.Rows.Select((v, i) => (v, i)))
        {
            foreach (var (cell, x) in row.Columns.Select((v, i) => (v, i)))
            {
                var cellVisual = gridCellVisuals[x, y];
                cellVisual.SetCellState(cell.CellState);
            }
        }
    }

    private void UpdateInventoryItemVisuals()
    {
        // Check if new items were added
        foreach (var (item, pos) in inventory.ItemDrawPositions)
        {
            if(!itemVisuals.Exists(x => x.inventoryItem == item))
            {
                GameObject inventoryItem = Instantiate(inventoryItemVisualsPrefab, inventoryAnchor);
                inventoryItem.transform.localPosition = new Vector3(pos.x * cellSize, -pos.y * cellSize, 0f);
                InventoryItemVisual invItemVisual = inventoryItem.GetComponent<InventoryItemVisual>();
                itemVisuals.Add(invItemVisual);

                invItemVisual.Init(cellSize, item, OnItemClicked);
            }
            else
            {
                itemVisuals.Find(x => x.inventoryItem == item).transform.localPosition = new Vector3(pos.x * cellSize, -pos.y * cellSize, 0f);
            }
        }

        List<InventoryItemVisual> deletedItems = new List<InventoryItemVisual>();
        // Check if items were removed
        foreach (InventoryItemVisual itemVisual in itemVisuals)
        {
            if(!inventory.ItemDrawPositions.ContainsKey(itemVisual.inventoryItem))
            {
                deletedItems.Add(itemVisual);
            }
        }
        foreach(var deletedItem in deletedItems)
        {
            itemVisuals.Remove(deletedItem);
            Destroy(deletedItem.gameObject);
        }
    }

    private void OnItemClicked(InventoryItem item)
    {
        BeginDraggingItem(item);
        print("Item " + item.Definition.Name + " clicked");
    }

    private bool DropDraggedItemOnCell(Vector2 cell)
    {
        if(!isDraggingItem) return false;

        var dropped = inventory.TryAddOrMoveItem(cell.x.RoundToInt(), cell.y.RoundToInt(), currentlyDraggedItem.inventoryItem);
        if(dropped && currentlyDraggedItem.worldItem != null) {
            Destroy(currentlyDraggedItem.worldItem.gameObject);
        }
        currentlyDraggedItem = null;
        isDraggingItem = false;

        return dropped;
    }

    private void OnCellClick(Vector2 id)
    {
        if(isDraggingItem) {
            DropDraggedItemOnCell(id);
        }
        print("Cell " + id + " clicked");
    }

    private void OnCellHoverChange(Vector2 id, bool enter)
    {
        // TODO: Do something with this (e.g. item dragging)
    }

    void OnGUI()
    {
        UpdateInventoryCellVisuals();
        UpdateInventoryItemVisuals();
        if(isDraggingItem) {
            var mousePosition = Mouse.current.position;
            var inventoryItem = currentlyDraggedItem.inventoryItem;
            var gridSize = inventoryItem.Definition.shape.GridSize;
            GUI.DrawTexture(
                new Rect(
                    mousePosition.x.value - cellSize / 2, Screen.height - mousePosition.y.value - cellSize / 2,
                    cellSize * gridSize.x, cellSize * gridSize.y
                ),
                inventoryItem.Definition.Icon.texture
            );
        }
    }
}
