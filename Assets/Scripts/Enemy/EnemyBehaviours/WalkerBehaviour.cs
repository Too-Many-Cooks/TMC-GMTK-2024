using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WalkerBehaviour : EnemyBehaviourBase
{
    [SerializeField][Range(0, 10)] protected float walkingSpeed = 1f;

    // Update is called once per frame
    protected virtual void Update()
    {
        Move();
        characterController.transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
    }

    protected virtual void Move()
    {
        characterController.Move(new Vector3(-transform.position.x + playerTransform.position.x,
            0, -transform.position.z + playerTransform.position.z).normalized * walkingSpeed * Time.deltaTime);
    }
}