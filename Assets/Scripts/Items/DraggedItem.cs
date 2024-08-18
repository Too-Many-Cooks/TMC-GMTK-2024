using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DraggedItem
{
    public InventoryItem inventoryItem;
    public WorldItem worldItem;
    public ItemUseEffectBase itemUseEffect;

    public enum ViewMode
    {
        InventoryMode,
        WorldMode
    };
    public ViewMode currenctViewMode;

}
