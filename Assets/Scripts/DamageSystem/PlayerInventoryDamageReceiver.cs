using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static GridInventory.InventoryCell;

public class PlayerInventoryDamageReceiver : PlayerDamageReceiver
{
    [SerializeField]
    UnityEvent<int> OnInventorySlotsDamaged;

    [SerializeField]
    bool invincibleAfterReceivingDamage = false;
    [SerializeField]
    float invincibilityDuration = 0.5f;
    [SerializeField]
    CellStatus damagedCellStatus = CellStatus.Damaged;
    public enum DamageMode { Default, DontPrioritizeIsolated, Random }
    [SerializeField]
    DamageMode damageMode = DamageMode.Default;
    bool invincible = false;


    public override void DealDamageToPlayer(float damage)
    {
        if (invincible)
            return;

        var inventory = gameObject.GetComponent<CharacterInventory>()?.inventory;

        if(inventory == null)
            return;

        switch(damageMode) {
            case DamageMode.Default:
            case DamageMode.DontPrioritizeIsolated:
                var slotsToDamage = InventoryUtility.WhatInventorySlotsBecomeDamaged(inventory, (int)damage, damageMode == DamageMode.Default);
                foreach(var slotToDamage in slotsToDamage) {
                    inventory.Rows[slotToDamage.y].Columns[slotToDamage.x].CellState = damagedCellStatus;
                }
                break;
            case DamageMode.Random:
                var unlockedSlots = new List<GridInventory.InventoryCell>(inventory.GetCellsWithState(CellStatus.Unlocked));
                for(int i = 0; i < (int)damage; i++) {
                    if(unlockedSlots.Count == 0) break;
                    var slot = Random.Range(0, unlockedSlots.Count);
                    unlockedSlots[slot].CellState = damagedCellStatus;
                    unlockedSlots.RemoveAt(i);
                }
                break;
        }

        OnInventorySlotsDamaged.Invoke((int)damage);
        if (invincibleAfterReceivingDamage)
            StartCoroutine(InvincibilityDurationCoroutine(invincibilityDuration));
    }

    private IEnumerator InvincibilityDurationCoroutine(float invincibilityDuration)
    {
        invincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        invincible = false;
    }
}
