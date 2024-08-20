using System.Collections;
using UnityEngine;

public class EyebatBehaviour : EnemyBehaviourBase
{
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float maxShootingRange = 10f;
    [SerializeField] protected float laserCooldown_sec = 3f;
    [SerializeField] protected GameObject laserPrefab;
    [SerializeField] protected Transform laserSpawnPoint;

    Timer _laserTimer;
    Timer LaserTimer
    {
        get
        {
            if (_laserTimer == null)
                _laserTimer = new Timer(laserCooldown_sec);

            return _laserTimer;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        LaserTimer.Update(Time.deltaTime);

        Vector3 distanceToPlayer = new Vector3(-transform.position.x + playerTransform.position.x,
            0, -transform.position.z + playerTransform.position.z);

        Vector3 directionToPlayer = distanceToPlayer.normalized;

        if (distanceToPlayer.sqrMagnitude > maxShootingRange * maxShootingRange)
        {
            characterController.Move(directionToPlayer * speed * Time.deltaTime);
        }
        else
        {
            if(LaserTimer.IsComplete)
            {
                ShootLaser(directionToPlayer);
                LaserTimer.Reset();
            }
        }

        characterController.transform.LookAt(
            new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
    }

    protected virtual void ShootLaser(Vector3 directionToPlayer)
    {
        Instantiate(laserPrefab, laserSpawnPoint.position, Quaternion.LookRotation(directionToPlayer, Vector3.up));
    }
}
