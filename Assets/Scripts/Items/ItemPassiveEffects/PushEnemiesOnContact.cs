using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerItemDamageSender))]
[RequireComponent(typeof(Collider))]
public class PushEnemiesOnContact : MonoBehaviour
{
    public float force = 0.2f;
    Collider pulseCollider;

    public float damage;

    PlayerItemDamageSender damageSender;

    // Start is called before the first frame update
    void Start()
    {
        damageSender = GetComponent<PlayerItemDamageSender>();
        pulseCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(damage != 0f)
            damageSender.DamageEnemiesInCollider(damage);
        PushEnemy(other);
    }

    private void OnTriggerStay(Collider other)
    {
        PushEnemy(other);
    }

    void PushEnemy(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyLayer"))
        {
            Vector3 diff = -transform.position + other.transform.position;
            diff.y = 0f;
            Vector3 dir = diff.normalized;

            other.GetComponent<CharacterController>().Move(dir * force);
        }
    }
}
