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
        Vector3 distance = (-transform.position + playerTransform.position);
        distance.y = transform.position.y;
        characterController.Move(distance.normalized * speed * Time.deltaTime);
        characterController.transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
    }
}