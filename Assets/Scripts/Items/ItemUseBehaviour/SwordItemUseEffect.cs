using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwordItemUseEffect : ItemUseEffectBase
{
    [SerializeField]
    float damageToEnemies = 50f;
    [SerializeField]
    Vector2 range = new Vector2(10f, 5f);

    [SerializeField]
    Transform swordEffectColliderTransform;

    Transform playerTransform;

    public UnityEvent OnSwing;

    public override void ClickActivationTrigger(out bool destroyedOnUse)
    {
        swordEffectColliderTransform.GetComponent<PlayerItemDamageSender>().DamageEnemiesInCollider(damageToEnemies);
        OnSwing.Invoke();
        destroyedOnUse = true;
        Destroy(gameObject);
    }

    public override void UpdateTargetting(Vector3 targettingPositionOnPlane)
    {
        transform.rotation = Quaternion.LookRotation(-transform.position + targettingPositionOnPlane, Vector3.up);
    }

    // Start is called before the first frame update
    void Start()
    {
        swordEffectColliderTransform.localScale = new Vector3(range.x, 1f, range.y);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        transform.parent = playerTransform;
        transform.localPosition = Vector3.zero;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
