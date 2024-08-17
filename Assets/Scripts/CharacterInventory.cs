using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

public class CharacterInventory : MonoBehaviour
{
    [Foldout("GUI", true)] 
    public Sprite LockedSlotIcon;
    public Sprite EmptySlotIcon;
    public Sprite DamagedSlotIcon;
    public Sprite BrokenSlotIcon;
    public Vector2 InventoryScreenPosition;
    [Foldout("GUI")] 
    public bool EnableDebugGUI = false;

    public GridInventory inventory;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI() {
        if(EnableDebugGUI) {
            foreach (var (row, y) in inventory.Rows.Select((v,i)=>(v,i)))
            {
                foreach(var (cell, x) in row.Columns.Select((v,i)=>(v,i)))
                {
                    var drawRect = new Rect(InventoryScreenPosition.x + x * 32, InventoryScreenPosition.y + y * 32, 32, 32);
                    if (cell.Item == null || cell.Item.Definition == null) 
                    {
                        switch (cell.CellState)
                        {
                            case GridInventory.InventoryCell.CellStatus.Locked:
                                GUI.DrawTexture(drawRect, LockedSlotIcon.texture);
                                break;
                            case GridInventory.InventoryCell.CellStatus.Unlocked:
                                GUI.DrawTexture(drawRect, EmptySlotIcon.texture);
                                break;
                            case GridInventory.InventoryCell.CellStatus.Damaged:
                                GUI.DrawTexture(drawRect, DamagedSlotIcon.texture);
                                break;
                            case GridInventory.InventoryCell.CellStatus.Broken:
                                GUI.DrawTexture(drawRect, BrokenSlotIcon.texture);
                                break;
                            case GridInventory.InventoryCell.CellStatus.Null:
                                break;
                        }
                    } else
                    {
                        GUI.DrawTexture(drawRect, cell.Item.Definition.Icon.texture);
                    }
                }
            }
        } else {
            foreach (var (row, y) in inventory.Rows.Select((v,i)=>(v,i)))
            {
                foreach(var (cell, x) in row.Columns.Select((v,i)=>(v,i)))
                {
                    var drawRect = new Rect(InventoryScreenPosition.x + x * 32, InventoryScreenPosition.y + y * 32, 32, 32);
                    switch (cell.CellState)
                    {
                        case GridInventory.InventoryCell.CellStatus.Locked:
                            GUI.DrawTexture(drawRect, LockedSlotIcon.texture);
                            break;
                        case GridInventory.InventoryCell.CellStatus.Unlocked:
                            GUI.DrawTexture(drawRect, EmptySlotIcon.texture);
                            break;
                        case GridInventory.InventoryCell.CellStatus.Damaged:
                            GUI.DrawTexture(drawRect, DamagedSlotIcon.texture);
                            break;
                        case GridInventory.InventoryCell.CellStatus.Broken:
                            GUI.DrawTexture(drawRect, BrokenSlotIcon.texture);
                            break;
                        case GridInventory.InventoryCell.CellStatus.Null:
                            break;
                    }
                }
            }
            foreach(var item in inventory.ItemDrawPositions.Keys) {
                var itemPosition = inventory.ItemDrawPositions[item];
                var drawRect = new Rect(
                    InventoryScreenPosition.x + itemPosition.x * 32, InventoryScreenPosition.y + itemPosition.y * 32,
                    item.Definition.shape.GridSize.x * 32, item.Definition.shape.GridSize.y * 32
                );
                GUI.DrawTexture(drawRect, item.Definition.Icon.texture);
            }
        }
    }
}
