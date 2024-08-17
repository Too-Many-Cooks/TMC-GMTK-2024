using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemVisual : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image itemImage;

    float cellSize = 0f;
    public InventoryItem inventoryItem;

    Action<InventoryItem> OnItemClickedCallback;

    public void Init(float cellSize, InventoryItem inventoryItem, Action<InventoryItem> OnItemClickedCallback)
    {
        SetCellSize(cellSize);
        SetItem(inventoryItem);
        UpdateSize();
        this.OnItemClickedCallback = OnItemClickedCallback;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        OnItemClickedCallback.Invoke(inventoryItem);
    }
}
