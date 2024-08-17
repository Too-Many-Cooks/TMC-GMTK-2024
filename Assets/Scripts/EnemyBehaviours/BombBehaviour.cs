using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehaviour : EnemyBehaviourBase
{
    [SerializeField]
    float speed = 10f;
    [SerializeField]
    float bombingRange = 2f;
    [SerializeField]
    GameObject explosionZonePrefab;
    [SerializeField]
    private float timeToExplode_sec = 3f;
    [SerializeField]
    private float explosionSize = 5f;
    //bool isExploding = false;

    new void Start()
    {
        base.Start();
        StartCoroutine(MoveToPlayerAndExplode());
    }

    private IEnumerator MoveToPlayerAndExplode()
    {
        Vector3 distanceToPlayer;
        do
        {
            distanceToPlayer = (-transform.position + playerTransform.position);
            characterController.Move(distanceToPlayer.normalized * speed * Time.deltaTime);
            yield return null;
        } while (distanceToPlayer.sqrMagnitude > bombingRange * bombingRange);
        GameObject explosionZone = Instantiate(explosionZonePrefab, transform.position, transform.rotation, transform);
        explosionZone.transform.localScale = new Vector3(explosionSize, explosionZone.transform.localScale.y, explosionSize);
        StartCoroutine(WaitForExplosion(timeToExplode_sec));
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 distanceToPlayer = (-transform.position + playerTransform.position);
        if (distanceToPlayer.sqrMagnitude > bombingRange * bombingRange)
        {
            characterController.Move(distanceToPlayer.normalized * speed * Time.deltaTime);
        }
        else
        {
            Instantiate(explosionZone, transform.position, transform.rotation, transform);
            StartCoroutine(WaitForExplosion(timeToExplode_sec));
        }*/
    }

    private IEnumerator WaitForExplosion(float timeToExplode_sec)
    {
        yield return new WaitForSeconds(timeToExplode_sec);
        Destroy(gameObject);
    }
}
