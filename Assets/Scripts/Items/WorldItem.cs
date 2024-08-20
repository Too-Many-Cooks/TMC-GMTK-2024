using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class WorldItem : ItemUseEffectBase
{
    public ItemDefinition Definition;
    public GameObject ItemModel;
    public float rotationSpeed = 60.0f;

    public override void ClickActivationTrigger(out bool destroyedOnUse)
    {
        destroyedOnUse = true;

        gameObject.SetActive(true);
        Instantiate(gameObject, transform.position, transform.rotation);
    }

    public override void UpdateTargetting(Vector3 targettingPositionOnPlane)
    {
        gameObject.SetActive(false);
        transform.position = targettingPositionOnPlane;
    }

    void Start()
    {
        //UpdateItemModel();
    }

    void Update() {
        ItemModel.transform.Rotate(ItemModel.transform.worldToLocalMatrix * Vector3.up, rotationSpeed * Time.deltaTime);
    }

    [ButtonMethod]
    public void UpdateItemModel() {
        var parent = this.transform;
        var name = "Item Model";
        if(ItemModel != null)
        {
            name = ItemModel.name;
            parent = ItemModel.transform.parent;
            GameObject.DestroyImmediate(ItemModel);
        }
        if(Definition != null && Definition.ItemModelPrefab != null) {
            ItemModel = GameObject.Instantiate(Definition.ItemModelPrefab, parent);
            ItemModel.name = name;
        } else {
            ItemModel = new GameObject(name);
            ItemModel.transform.parent = parent;
        }
    }
}
