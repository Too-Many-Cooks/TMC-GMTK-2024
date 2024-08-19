using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterInventory))]
public class WorldItemPickup : MonoBehaviour
{
    CharacterInventory characterInventory;
    private InputAction click;

    // Start is called before the first frame update
    void Start()
    {
        characterInventory = GetComponent<CharacterInventory>();

        click = new InputAction(binding: "<Mouse>/leftButton");
        //click.performed += ctx => RaycastToWorldItem();
    }

    private void RaycastToWorldItem()
    {
        print("click");

        RaycastHit hit;
        Vector3 coor = Mouse.current.position.ReadValue();

        LayerMask layerMask = LayerMask.GetMask("WorldItem");

        Ray ray = Camera.main.ScreenPointToRay(coor);

        if (Physics.Raycast(ray, out hit, 500f, layerMask))
        {
            print("hit");

            WorldItem worldItem = hit.collider.GetComponent<WorldItem>();
            if (worldItem == null)
            {
                Debug.LogError("GameObject with WorldItem layer has no WorldItem component attached. This is not allowed.");
                return;
            }
            characterInventory.PickUpWorldItem(worldItem);
        }
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            RaycastToWorldItem();
    }
}
