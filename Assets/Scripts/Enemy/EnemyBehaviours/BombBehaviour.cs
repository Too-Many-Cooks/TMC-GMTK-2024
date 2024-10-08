using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

public class BombBehaviour : EnemyBehaviourBase
{
    [SerializeField] float speed = 10f;
    [SerializeField] float bombingRange = 2f;
    [SerializeField] GameObject explosionZonePrefab;
    [SerializeField] private float timeToExplode_sec = 3f;
    [SerializeField] private float explosionSize = 5f;
    //bool isExploding = false;

    [SerializeField]
    public GameObject explosionPrefab;

    GameObject explosionZone;

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
            distanceToPlayer.y = transform.position.y;

            characterController.Move(distanceToPlayer.normalized * speed * Time.deltaTime);
            characterController.transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
            yield return null;
        } while (distanceToPlayer.sqrMagnitude > bombingRange * bombingRange);
        explosionZone = Instantiate(explosionZonePrefab, transform.position, transform.rotation, transform);
        explosionZone.transform.localScale = new Vector3(explosionSize, explosionZone.transform.localScale.y, explosionSize);
        StartCoroutine(WaitForExplosion(timeToExplode_sec, explosionZone.GetComponent<BombExplosionZoneBehaviour>()));
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 distanceToPlayer = (-transform.position + playerTransform.position);
        if (distanceToPlayer.sqrMagnitude > bombingRange * bombingRange)
        {
            characterController.Move(distanceToPlayer.normalized * characterSpeed * Time.deltaTime);
        }
        else
        {
            Instantiate(explosionZone, transform.position, transform.rotation, transform);
            StartCoroutine(WaitForExplosion(timeToExplode_sec));
        }*/
    }

    private IEnumerator WaitForExplosion(float timeToExplode_sec, BombExplosionZoneBehaviour bombExplosionZoneBehaviour)
    {
        yield return new WaitForSeconds(timeToExplode_sec);
        bombExplosionZoneBehaviour.Explode();
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        if (gameObject.HasComponent<EnemyDamageReceiver>()) {
            gameObject.GetComponent<EnemyDamageReceiver>().KillEnemy();
        } else {
            Destroy(gameObject);
        }
    }
}
