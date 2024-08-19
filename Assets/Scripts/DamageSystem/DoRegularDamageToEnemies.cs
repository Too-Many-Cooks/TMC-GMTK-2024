using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerItemDamageSender))]
public class DoRegularDamageToEnemies : MonoBehaviour
{
    public float damage;
    PlayerItemDamageSender damageSender;

    // Start is called before the first frame update
    void Start()
    {
        damageSender = GetComponent<PlayerItemDamageSender>();
    }

    private void FixedUpdate()
    {
        damageSender.DamageEnemiesInCollider(damage);
    }

}
