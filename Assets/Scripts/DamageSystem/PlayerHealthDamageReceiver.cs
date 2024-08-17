using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PlayerHealthDamageReceiver : PlayerDamageReceiver
{
    [SerializeField]
    float playerMaxSectionHealth = 100f;
    float playerSectionHealth;

    Collider ownCollider;
    [SerializeField]
    HealthBar lifeBar;

    [SerializeField]
    UnityEvent OnPlayerHealthSectionBroken;

    // Start is called before the first frame update
    void Start()
    {
        ownCollider = GetComponent<Collider>();
        playerSectionHealth = playerMaxSectionHealth;
    }

    public override void DealDamageToPlayer(float dmg)
    {
        playerSectionHealth -= dmg;
        if (playerSectionHealth < 0)
        {
            playerSectionHealth += playerMaxSectionHealth;
            // Should be used to break inventory
            OnPlayerHealthSectionBroken.Invoke();
        }

        UpdateHealthBar(playerSectionHealth, playerMaxSectionHealth);
    }

    private void UpdateHealthBar(float playerHealth, float playerMaxHealth)
    {
        lifeBar.UpdateHealthBar(playerHealth, playerMaxHealth);
    }
}
