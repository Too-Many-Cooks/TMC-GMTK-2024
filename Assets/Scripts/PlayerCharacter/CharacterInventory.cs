using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static GridInventory;
using static GridInventory.InventoryCell;

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
    public float damagedSlotStateDuration = 5f;

    private Dictionary<InventoryCell, float> damagedSlotTimers = new Dictionary<InventoryCell, float>();

    [SerializeField]
    private bool Test_hideItemVisualsWhileDragging = true;
    [SerializeField]
    private bool Test_allowReplacingItems = true;

    [SerializeField]
    private GameObject gridCellVisualsPrefab;
    [SerializeField]
    private GameObject inventoryItemVisualsPrefab;
    [SerializeField]
    private RectTransform inventoryAnchor;
    [SerializeField]
    private RectTransform gridCellVisualsAnchor;
    [SerializeField]
    private RectTransform itemVisualsAnchor;
    [SerializeField]
    private float cellSize = 32f;
    private InventoryGridCell[,] gridCellVisuals;
    private List<InventoryItemVisual> itemVisuals;

    private Vector2Int cachedGridSize;
    private float cachedCellSize;

    [SerializeField]
    private float Debug_worldItemDistance = 10f;

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

    public bool BeginDraggingItem(InventoryItem inventoryItem) {
        if(isDraggingItem) return false;

        if (Test_hideItemVisualsWhileDragging)
        {
            //itemVisuals.Find(x => x.inventoryItem == itemInCell).gameObject.SetActive(false);
            inventory.TryRemoveItem(inventoryItem);
        }

        isDraggingItem = true;

        var createdWorldItem = new GameObject(inventoryItem.Definition.Name).AddComponent<WorldItem>();
        createdWorldItem.Definition = inventoryItem.Definition;
        createdWorldItem.gameObject.SetActive(false);

        ItemUseEffectBase itemUseEffectVisual = null;
        if (inventoryItem.Definition.ItemUseEffectPrefab != null)
        {
            var itemUseEffectVisualGO = Instantiate(inventoryItem.Definition.ItemUseEffectPrefab);
            itemUseEffectVisualGO.SetActive(false);
            if(!itemUseEffectVisualGO.HasComponent<ItemUseEffectBase>())
            {
                Debug.LogError("ItemUseEffect Prefab does not contain an IItemUseEffectBase.");
            }
            else
            {
                itemUseEffectVisual = itemUseEffectVisualGO.GetComponent<ItemUseEffectBase>();
            }
        }
        else
        {
            Debug.LogError("No itemUseEffectPrefab attached to item scriptable object");
        }

        currentlyDraggedItem = new DraggedItem() { inventoryItem = inventoryItem, worldItem = createdWorldItem, itemUseEffect = itemUseEffectVisual };

        return true;
    }


    private void CreateVisuals()
    {
        foreach (Transform child in gridCellVisualsAnchor)
            Destroy(child.gameObject);
        foreach (Transform child in itemVisualsAnchor)
            Destroy(child.gameObject);

        gridCellVisuals = new InventoryGridCell[inventory.GridSize.x, inventory.GridSize.y];
        cachedGridSize = inventory.GridSize;
        cachedCellSize = cellSize;

        inventoryAnchor.anchoredPosition = new Vector3(-inventory.GridSize.x * cellSize - InventoryScreenPosition.x, -InventoryScreenPosition.y, 0f);

        foreach (var (row, y) in inventory.Rows.Select((v, i) => (v, i)))
        {
            GameObject rowGO = new GameObject("CellRow " + row);
            rowGO.AddComponent<RectTransform>();
            rowGO.transform.SetParent(gridCellVisualsAnchor);
            rowGO.transform.localPosition = new Vector3(0f, -y * cellSize, 0f);

            foreach (var (cell, x) in row.Columns.Select((v, i) => (v, i)))
            {
                GameObject gridCell = Instantiate(gridCellVisualsPrefab, rowGO.transform);
                gridCell.transform.localPosition = new Vector3((x+1) * cellSize, 0f, 0f);
                InventoryGridCell invGridCell = gridCell.GetComponent<InventoryGridCell>();
                gridCellVisuals[x, y] = invGridCell;

                invGridCell.Init(new Vector2Int(x, y), cellSize, cell.CellState, OnCellClick, OnCellHoverChange);
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
                GameObject inventoryItem = Instantiate(inventoryItemVisualsPrefab, itemVisualsAnchor);
                inventoryItem.transform.localPosition = new Vector3(pos.x * cellSize, -pos.y * cellSize, 0f);
                InventoryItemVisual invItemVisual = inventoryItem.GetComponent<InventoryItemVisual>();
                itemVisuals.Add(invItemVisual);

                invItemVisual.Init(cellSize, item);//, OnItemClicked);
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

    private bool DropDraggedItemOnCell(Vector2 cell)
    {
        if(!isDraggingItem) return false;

        if (inventory.CanAddOrMoveItem(cell.x.RoundToInt(), cell.y.RoundToInt(), currentlyDraggedItem.inventoryItem))
        {
            // Deactivate hover visuals
            ChangeHoverCellHighlights(new Vector2Int(cell.x.RoundToInt(), cell.y.RoundToInt()), false);

            var dropped = inventory.TryAddOrMoveItem(cell.x.RoundToInt(), cell.y.RoundToInt(), currentlyDraggedItem.inventoryItem);
            if (dropped && currentlyDraggedItem.worldItem != null)
            {
                Destroy(currentlyDraggedItem.worldItem.gameObject);
                Destroy(currentlyDraggedItem.itemUseEffect.gameObject);
            }
            currentlyDraggedItem = null;
            isDraggingItem = false;

            
            return dropped;
        }
        else if (Test_allowReplacingItems)
        {
            InventoryItem toBeReplacedItem;
            if (inventory.CanReplaceItem(cell.x.RoundToInt(), cell.y.RoundToInt(), currentlyDraggedItem.inventoryItem, out toBeReplacedItem))
            {
                // Deactivate hover visuals
                ChangeHoverCellHighlights(new Vector2Int(cell.x.RoundToInt(), cell.y.RoundToInt()), false);

                bool replaced = inventory.TryRemoveItem(toBeReplacedItem);
                replaced &= inventory.TryAddOrMoveItem(cell.x.RoundToInt(), cell.y.RoundToInt(), currentlyDraggedItem.inventoryItem);

                if (currentlyDraggedItem.worldItem != null)
                {
                    Destroy(currentlyDraggedItem.worldItem.gameObject);
                    Destroy(currentlyDraggedItem.itemUseEffect.gameObject);
                }

                isDraggingItem = false;
                BeginDraggingItem(toBeReplacedItem);

                return replaced;
            }
        }
        return false;
    }

    private void OnCellClick(Vector2Int id)
    {
        if(isDraggingItem) {
            DropDraggedItemOnCell(id);
        }
        else
        {
            InventoryItem itemInCell = inventory.GetInventoryItemAt(id.x, id.y);
            if(itemInCell.Definition != null)
            {
                BeginDraggingItem(itemInCell);
            }
        }
        //print("Cell " + id + " clicked");
    }

    private void OnCellHoverChange(Vector2Int id, bool enter)
    {
        if (!isDraggingItem) return;

        ChangeHoverCellHighlights(id, enter);

        // If cell is entered/left -> change between 2d/3d dragged item (should catch leaving the inventory entirely
        currentlyDraggedItem.currenctViewMode = enter ? DraggedItem.ViewMode.InventoryMode : DraggedItem.ViewMode.WorldMode;
        currentlyDraggedItem.worldItem.gameObject.SetActive(!enter);
        currentlyDraggedItem.itemUseEffect.gameObject.SetActive(!enter);
    }

    private void ChangeHoverCellHighlights(Vector2Int id, bool activate)
    {

        Array2DEditor.Array2DBool draggedItemShape = currentlyDraggedItem.inventoryItem.Definition.shape;
        bool possibleToPlace = inventory.CanAddOrMoveItem(id.x, id.y, currentlyDraggedItem.inventoryItem);
        if (Test_allowReplacingItems)
            possibleToPlace |= inventory.CanReplaceItem(id.x, id.y, currentlyDraggedItem.inventoryItem, out _);

        for (int shapeX = 0; shapeX < draggedItemShape.GridSize.x; ++shapeX)
        {
            for (int shapeY = 0; shapeY < draggedItemShape.GridSize.y; ++shapeY)
            {
                if (!draggedItemShape.GetCell(shapeX, shapeY))
                    continue;
                if (id.x + shapeX >= gridCellVisuals.GetLength(0) || id.y + shapeY >= gridCellVisuals.GetLength(1))
                    continue;

                gridCellVisuals[id.x + shapeX, id.y + shapeY].SetHighlight(activate, possibleToPlace);
            }
        }
    }

    void OnGUI()
    {
        UpdateInventoryCellVisuals();
        UpdateInventoryItemVisuals();
        if(isDraggingItem) {
            if (currentlyDraggedItem.currenctViewMode == DraggedItem.ViewMode.InventoryMode)
            {
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

    private void Update()
    {
        HandleDamagedSlots();

        if (currentlyDraggedItem != null && currentlyDraggedItem.currenctViewMode == DraggedItem.ViewMode.WorldMode)
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var ray = Camera.main.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 1f));
            Vector3 positionOnY0Plane = ray.origin - (ray.origin.y / ray.direction.y) * ray.direction;

            currentlyDraggedItem.worldItem.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Debug_worldItemDistance));
            currentlyDraggedItem.itemUseEffect.UpdateTargetting(positionOnY0Plane);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                currentlyDraggedItem.itemUseEffect.ClickActivationTrigger(out bool destroyedOnUse);
                if (destroyedOnUse)
                {
                    Destroy(currentlyDraggedItem.worldItem.gameObject);
                    Destroy(currentlyDraggedItem.itemUseEffect.gameObject);

                    currentlyDraggedItem = null;
                    isDraggingItem = false;
                }
            }

        }
    }

    private void HandleDamagedSlots()
    {
        for (int i = 0; i < damagedSlotTimers.Count; i++)
        {
            var entry = damagedSlotTimers.ElementAt(i);
            damagedSlotTimers[entry.Key] -= Time.deltaTime;
        }

        var damagedSlots = inventory.GetCellsWithState(CellStatus.Damaged);
        foreach (var damagedSlot in damagedSlots)
        {
            if (!damagedSlotTimers.ContainsKey(damagedSlot))
            {
                damagedSlotTimers.Add(damagedSlot, damagedSlotStateDuration);
            }
        }

        var removedCells = new List<InventoryCell>();
        for (int i = 0; i < damagedSlotTimers.Count; i++)
        {
            var entry = damagedSlotTimers.ElementAt(i);
            var timer = entry.Value;
            if (timer < 0)
            {
                var damagedSlot = entry.Key;
                damagedSlot.CellState = CellStatus.Locked;
                inventory.TryRemoveItem(damagedSlot.Item);
                removedCells.Add(damagedSlot);
            }
        }
        foreach (var damagedSlot in removedCells)
        {
            damagedSlotTimers.Remove(damagedSlot);
        }
    }
}
