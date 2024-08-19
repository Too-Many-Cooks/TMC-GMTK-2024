using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerItemDamageSender))]
internal class OrbBehaviour : MonoBehaviour
{
    float damage;
    float orbDuration;
    float speed;
    Vector3 orbitCenter;

    PlayerItemDamageSender damageSender;

    internal void Init(float damage, float orbDuration, float orbSpeed, Vector3 position)
    {
        this.damage = damage;
        this.orbDuration = orbDuration;
        this.speed = orbSpeed;
        orbitCenter = position;

        damageSender = GetComponent<PlayerItemDamageSender>();

        StartCoroutine(OrbExpirationCoroutine());
    }

    private IEnumerator OrbExpirationCoroutine()
    {
        yield return new WaitForSeconds(orbDuration);
        Destroy(gameObject);
    }

    private void Update()
    {
        Vector3 diffToCenter = -orbitCenter + transform.position;
        Vector3 newPosition = Quaternion.AngleAxis(speed * Time.deltaTime, Vector3.up) * diffToCenter + orbitCenter;
        transform.position = newPosition;
    }

    private void FixedUpdate()
    {
        damageSender.DamageEnemiesInCollider(damage);
    }


}