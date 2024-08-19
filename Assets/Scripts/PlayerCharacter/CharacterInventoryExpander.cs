using MyBox;
using UnityEngine;
using UnityEngine.Events;

public class CharacterInventoryExpander : MonoBehaviour
{
    public CharacterInventory characterInventory;
    public UnityEvent OnAddInventorySlot;
    public float initialInventoryTimer = 5f;
    public float timerIncreaseFactor = 1f;
    public int slotBatchSize = 1;
    [ReadOnly][SerializeField]float itemTimer = 0.0f;
    [ReadOnly][SerializeField]float nextItemTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        itemTimer = initialInventoryTimer;
        nextItemTimer = initialInventoryTimer * timerIncreaseFactor;
        if(characterInventory == null) {
            TryGetComponent<CharacterInventory>(out characterInventory);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (characterInventory == null) return;

        itemTimer -= Time.deltaTime;
        if(itemTimer <= 0) {
            if(AddInventorySlot()) {
                itemTimer += nextItemTimer;
                nextItemTimer *= timerIncreaseFactor;
            } else {
                itemTimer = 0f;
            }
        }
    }

    bool AddInventorySlot() {
        if (characterInventory == null) return false;

        var inventory = characterInventory.inventory;

        var slotsToUnlock = InventoryUtility.WhatInventorySlotsToUnlock(inventory);

        foreach(var slot in slotsToUnlock) {
            inventory.Rows[slot.y].Columns[slot.x].CellState = GridInventory.InventoryCell.CellStatus.Unlocked;
        }
        if(slotsToUnlock.Length != 0) {
            OnAddInventorySlot.Invoke();
            return true;
        }

        return false;
    }
}
