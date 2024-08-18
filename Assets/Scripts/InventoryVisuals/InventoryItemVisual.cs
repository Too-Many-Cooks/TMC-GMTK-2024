using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemVisual : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;

    float cellSize = 0f;
    public InventoryItem inventoryItem;

    public void Init(float cellSize, InventoryItem inventoryItem)
    {
        SetCellSize(cellSize);
        SetItem(inventoryItem);
        UpdateSize();
    }

    private void SetCellSize(float size)
    {
        cellSize = size;
    }

    private void SetItem(InventoryItem inventoryItem)
    {
        this.inventoryItem = inventoryItem;
        itemImage.sprite = inventoryItem.Definition.Icon;
    }

    private void UpdateSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(inventoryItem.Definition.shape.GridSize.x, inventoryItem.Definition.shape.GridSize.y) * cellSize;
        itemImage.rectTransform.sizeDelta = rectTransform.sizeDelta;
    }
}
