using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class PlayerItemDamageSender : MonoBehaviour
{
    [SerializeField]
    bool canDamagePlayer = false;

    Collider itemDamageZoneCollider;

    [SerializeField]
    List<GameObject> enemyDamageReceiversInCollider;
    bool isPlayerInCollider = false;

    GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        itemDamageZoneCollider = GetComponent<Collider>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<EnemyDamageReceiver>() != null)
        {
            enemyDamageReceiversInCollider.Add(other.gameObject);
        }
        else if(other.gameObject.GetComponent<PlayerDamageReceiver>() != null)
        {
            isPlayerInCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyDamageReceiver>() != null)
        {
            enemyDamageReceiversInCollider.Remove(other.gameObject);
        }
        else if (other.gameObject.GetComponent<PlayerDamageReceiver>() != null)
        {
            isPlayerInCollider = false;
        }
    }

    public void DamageEnemiesInCollider(float damage, float damageToPlayer = 0f)
    {
        foreach(var enemyDReceiver in enemyDamageReceiversInCollider)
        {
            if(enemyDReceiver != null)
                enemyDReceiver.GetComponent<EnemyDamageReceiver>().DealDamageToEnemy(damage);
        }
        if (isPlayerInCollider)
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDamageReceiver>().DealDamageToPlayer(damageToPlayer);
    }
}
