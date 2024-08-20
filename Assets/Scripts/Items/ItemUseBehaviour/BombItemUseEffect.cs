using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BombItemUseEffect : ItemUseEffectBase
{

    [SerializeField]
    float damageToEnemies = 50f;
    [SerializeField]
    float damageToPlayer = 1f;
    [SerializeField]
    float range = 10f;

    [SerializeField]
    Transform bombEffectColliderTransform;

    [SerializeField]
    GameObject explosionPrefab;

    public override void ClickActivationTrigger(out bool destroyedOnUse)
    {
        bombEffectColliderTransform.GetComponent<PlayerItemDamageSender>().DamageEnemiesInCollider(damageToEnemies, damageToPlayer);
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        destroyedOnUse = true;
        Destroy(gameObject);
    }

    public override void UpdateTargetting(Vector3 targettingPositionOnPlane)
    {
        transform.position = targettingPositionOnPlane;
    }

    // Start is called before the first frame update
    void Start()
    {
        bombEffectColliderTransform.localScale = new Vector3(range, 1f, range);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
