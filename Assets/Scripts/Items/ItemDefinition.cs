using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Array2DEditor;

[CreateAssetMenu(fileName = "NewItemDefenition", menuName = "ScriptableObjects/ItemDefinition")]
public class ItemDefinition : ScriptableObject
{
    public string Name;
    public GameObject ItemModelPrefab;
    public GameObject WorldItemPrefab;
    public GameObject ItemUseEffectPrefab;
    public GameObject InventoryPassiveEffectPrefab;
    public Sprite Icon;
    public Array2DBool shape;
}
