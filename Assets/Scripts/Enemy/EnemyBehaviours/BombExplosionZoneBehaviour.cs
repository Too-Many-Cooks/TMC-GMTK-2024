using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(EnemyDamageSender))]
public class BombExplosionZoneBehaviour : MonoBehaviour
{
    [SerializeField]
    float explosionDamage = 5f;

    EnemyDamageSender damageSender;

    bool touchingPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        damageSender = GetComponent<EnemyDamageSender>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            touchingPlayer = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            touchingPlayer = false;
    }

    internal void Explode()
    {
        if (touchingPlayer)
            damageSender.DealDamageToPlayer(explosionDamage);
        Destroy(gameObject);
    }
}
