using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventoryDamageReceiver : PlayerDamageReceiver
{
    [SerializeField]
    UnityEvent<int> OnInventorySlotsDamaged;

    [SerializeField]
    bool invincibleAfterReceivingDamage = false;
    [SerializeField]
    float invincibilityDuration = 0.5f;
    bool invincible = false;


    public override void DealDamageToPlayer(float damage)
    {
        if (invincible)
            return;

        print((int)damage + " inventory slots damaged");
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
