using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyDamageReceiver : MonoBehaviour
{
    [SerializeField]
    bool canBeDamaged = true;
    [SerializeField]
    float enemyMaxHealth = 100f;
    float enemyHealth;

    Collider ownCollider;
    [SerializeField]
    HealthBar lifeBar;

    // Start is called before the first frame update
    void Start()
    {
        ownCollider = GetComponent<Collider>();
        enemyHealth = enemyMaxHealth;
    }

    public void DealDamageToEnemy(float dmg)
    {
        enemyHealth -= dmg;
        if (enemyHealth < 0)
            KillEnemy();
        UpdateHealthBar(enemyHealth, enemyMaxHealth);
        
    }

    private void UpdateHealthBar(float enemyHealth, float enemyMaxHealth)
    {
        lifeBar.UpdateHealthBar(enemyHealth, enemyMaxHealth);
    }

    public void KillEnemy()
    {
        // TODO: Maybe do something cooler
        Destroy(gameObject);
    }
}
