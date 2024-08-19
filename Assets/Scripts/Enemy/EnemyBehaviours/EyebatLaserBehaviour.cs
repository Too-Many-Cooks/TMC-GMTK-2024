using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyebatLaserBehaviour : EnemyBehaviourBase
{
    [SerializeField]
    float speed = 1f;
    [SerializeField]
    float despawnDistance = 30f;

    // Update is called once per frame
    void Update()
    {
        Vector3 distanceToPlayer = (-transform.position + playerTransform.position);
        if (distanceToPlayer.sqrMagnitude > despawnDistance * despawnDistance)
        {
            Destroy(gameObject);
        }
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
