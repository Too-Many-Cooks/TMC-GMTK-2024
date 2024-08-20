using System;
using UnityEngine;

[RequireComponent(typeof(PlayerItemDamageSender))]
[RequireComponent(typeof(Collider))]
internal class PassiveSwordProjectile : MonoBehaviour
{
    float damage;
    float speed;

    PlayerItemDamageSender damageSender;

    Transform playerTransform;

    internal void Init(float damage, float speed, float size)
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        this.damage = damage;
        this.speed = speed;
        transform.localScale = transform.localScale * size;

        damageSender = GetComponent<PlayerItemDamageSender>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("EnemyLayer"))
        {
            damageSender.DamageEnemiesInCollider(damage);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        if ((transform.position - playerTransform.position).sqrMagnitude > 25f * 25f)
            Destroy(gameObject);
    }
}