using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryGridCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector2Int cellID;
    private Action<Vector2Int> onPointerClickCallback;
    private Action<Vector2Int, bool> onPointerHoverCallback;

    [SerializeField]
    GameObject damagedWarning;
    [SerializeField]
    GameObject gridCell;

    Color originalGridCellColor;
    Vector3 originalGridCellScale;

    private void Start()
    {
        originalGridCellColor = gridCell.GetComponent<Image>().color;
        originalGridCellScale = gridCell.transform.localScale;
    }

    public void Init(Vector2Int cellID, float cellSize, GridInventory.InventoryCell.CellStatus cellState, Action<Vector2Int> onPointerClickCallback, Action<Vector2Int, bool> onPointerHoverCallback)
    {
        this.cellID = cellID;
        this.onPointerClickCallback = onPointerClickCallback;
        this.onPointerHoverCallback = onPointerHoverCallback;
        SetCellSize(cellSize);
        SetCellState(cellState);
    }

    public void SetCellSize(float size)
    {
        float currentSize = GetComponent<RectTransform>().sizeDelta.x;
        transform.localScale = new Vector3(size / currentSize, size / currentSize, size / currentSize);
    }
    
    internal void SetCellState(GridInventory.InventoryCell.CellStatus cellState)
    {
        // Update cell state visuals
        switch (cellState)
        {
            case GridInventory.InventoryCell.CellStatus.Unlocked:
                damagedWarning.SetActive(false);
                gameObject.SetActive(true);
                break;
            case GridInventory.InventoryCell.CellStatus.Damaged:
                damagedWarning.SetActive(true);
                gameObject.SetActive(true);
                break;
            case GridInventory.InventoryCell.CellStatus.Locked:
            case GridInventory.InventoryCell.CellStatus.Broken:
            case GridInventory.InventoryCell.CellStatus.Null:
                gameObject.SetActive(false);
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onPointerClickCallback.Invoke(cellID);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerHoverCallback.Invoke(cellID, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerHoverCallback.Invoke(cellID, false);
    }

    internal void SetHighlight(bool activateHighlight, bool possibleToPlace)
    {
        if(activateHighlight)
        {
            gridCell.GetComponent<Image>().color = possibleToPlace ? Color.green : Color.red;
            //gridCell.transform.localScale = Vector3.one * 0.6f;
        }
        else
        {
            gridCell.GetComponent<Image>().color = originalGridCellColor;
            //gridCell.transform.localScale = originalGridCellScale;
        }
    }
}
