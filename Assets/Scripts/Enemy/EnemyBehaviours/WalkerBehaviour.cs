using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WalkerBehaviour : EnemyBehaviourBase
{
    [SerializeField]
    float speed = 1f;

    // Update is called once per frame
    void Update()
    {
        characterController.Move((-transform.position + playerTransform.position).normalized * speed * Time.deltaTime);
        characterController.transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
    }
}