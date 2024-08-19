using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Events;

public class EyebatBehaviour : EnemyBehaviourBase
{

    public UnityEvent OnShoot;

    [SerializeField]
    float speed = 2f;
    [SerializeField]
    float maxShootingRange = 10f;
    [SerializeField]
    float laserCooldown_sec = 3f;
    [SerializeField]
    GameObject laserPrefab;

    bool laserOffCooldown = true;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 distanceToPlayer = (-transform.position + playerTransform.position);
        Vector3 directionToPlayer = distanceToPlayer.normalized;
        if (distanceToPlayer.sqrMagnitude > maxShootingRange * maxShootingRange)
        {
            characterController.Move(directionToPlayer * speed * Time.deltaTime);
        }
        else
        {
            if(laserOffCooldown)
            {
                Instantiate(laserPrefab, transform.position, Quaternion.LookRotation(directionToPlayer, Vector3.up));
                laserOffCooldown = false;
                StartCoroutine(LaserCooldownCoroutine(laserCooldown_sec));

                OnShoot.Invoke();
            }
        }
        characterController.transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
    }

    private IEnumerator LaserCooldownCoroutine(float laserCooldown)
    {
        yield return new WaitForSeconds(laserCooldown);
        laserOffCooldown = true;
    }
}
