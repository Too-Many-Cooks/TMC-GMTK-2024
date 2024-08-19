using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehaviourBase : MonoBehaviour
{
    protected CharacterController characterController;
    protected Transform playerTransform;

    protected void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
public class lookAtPlayer : MonoBehaviour
{
    public Transform characterController, playerTransform;

    void Update()
    {
        characterController.transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
    }
}