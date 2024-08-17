using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class PlayerItemDamageSender : MonoBehaviour
{
    [SerializeField]
    bool canDamagePlayer = false;

    Collider itemDamageZoneCollider;

    List<EnemyDamageReceiver> enemyDamageReceiversInCollider;

    // Start is called before the first frame update
    void Start()
    {
        itemDamageZoneCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<EnemyDamageReceiver>() != null)
        {
            enemyDamageReceiversInCollider.Add(other.gameObject.GetComponent<EnemyDamageReceiver>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyDamageReceiver>() != null)
        {
            enemyDamageReceiversInCollider.Remove(other.gameObject.GetComponent<EnemyDamageReceiver>());
        }
    }

    public void DamageEnemiesInCollider(float damage)
    {
        foreach(var enemyDReceiver in enemyDamageReceiversInCollider)
        {
            enemyDReceiver.DealDamageToEnemy(damage);
        }
    }
}
