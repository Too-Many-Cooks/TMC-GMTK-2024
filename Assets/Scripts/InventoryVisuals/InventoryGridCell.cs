using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InventoryGridCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector2 cellID;
    private Action<Vector2> onPointerClickCallback;
    private Action<Vector2, bool> onPointerHoverCallback;

    [SerializeField]
    GameObject damagedWarning;

    public void Init(Vector2 cellID, float cellSize, GridInventory.InventoryCell.CellStatus cellState, Action<Vector2> onPointerClickCallback, Action<Vector2, bool> onPointerHoverCallback)
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

    internal void Init(Vector2 vector2, GridInventory.InventoryCell.CellStatus cellState, object onCellClick, Action<Vector2> onCellHoverChange)
    {
        throw new NotImplementedException();
    }
}
