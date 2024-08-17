using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public ItemDefinition Definition;

    public InventoryItem(ItemDefinition itemDefinition)
    {
        this.Definition = itemDefinition;
    }
}
