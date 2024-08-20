using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class EnemyDamageSender : MonoBehaviour
{
    [SerializeField]
    bool damagesOnTouch = true;
    [SerializeField]
    float damageDealtOnTouch = 1f;
    [SerializeField]
    float damageCooldown_sec = 1f;
    bool damageOnCooldown = false;

    [SerializeField]
    UnityEvent OnDealingDamage;

    Collider ownCollider;

    // Start is called before the first frame update
    void Start()
    {
        ownCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryToDealTouchDamage(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        TryToDealTouchDamage(other.gameObject);
    }

    private void TryToDealTouchDamage(GameObject collidingObject)
    {
        if (damagesOnTouch && !damageOnCooldown && collidingObject.CompareTag("Player"))
        {
            DealDamageToPlayer(damageDealtOnTouch);
            StartCoroutine(DamageCooldownCoroutine());
        }
    }

    public void DealDamageToPlayer(float damage)
    {
        PlayerDamageReceiver playerDamageManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDamageReceiver>();
        playerDamageManager.DealDamageToPlayer(damage);
        OnDealingDamage.Invoke();
    }

    private IEnumerator DamageCooldownCoroutine()
    {
        damageOnCooldown = true;
        yield return new WaitForSeconds(damageCooldown_sec);
        damageOnCooldown = false;
    }
}
