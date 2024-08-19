using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(DoRegularDamageToEnemies))]
internal class OrbBehaviour : MonoBehaviour
{
    float orbDuration;
    float speed;
    Vector3 orbitCenter;

    DoRegularDamageToEnemies regularDamager;

    internal void Init(float damage, float orbDuration, float orbSpeed, Vector3 position)
    {
        regularDamager = GetComponent<DoRegularDamageToEnemies>();
        this.regularDamager.damage = damage;
        this.orbDuration = orbDuration;
        this.speed = orbSpeed;
        orbitCenter = position;

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
}