using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbItemUseEffect : ItemUseEffectBase
{
    [SerializeField]
    Transform orbEffectColliderTransform;


    [SerializeField]
    float damage = 10f;
    [SerializeField]
    float orbDuration = 5f;
    [SerializeField]
    float orbSpeed = 30f;
    [SerializeField]
    float orbSize = 3f;

    [SerializeField]
    GameObject orbPrefab;

    Transform playerTransform;

    public override void ClickActivationTrigger(out bool destroyedOnUse)
    {
        destroyedOnUse = true;
        Vector3 spawnOffset = orbEffectColliderTransform.forward * 0.5f * orbEffectColliderTransform.localScale.x;
        //Vector3 orbMoveDirection = Vector3.Cross(orbEffectColliderTransform.forward, Vector3.up);

        Vector3 centerPosition = playerTransform.position + -Vector3.up * (playerTransform.position.y - 0.5f);

        var orb1 = Instantiate(orbPrefab, centerPosition + spawnOffset, Quaternion.identity);
        orb1.transform.localScale = Vector3.one * orbSize;
        orb1.GetComponent<OrbBehaviour>().Init(damage, orbDuration, orbSpeed, playerTransform.position);
        
        var orb2 = Instantiate(orbPrefab, centerPosition - spawnOffset, Quaternion.identity);
        orb2.transform.localScale = Vector3.one * orbSize; 
        orb2.GetComponent<OrbBehaviour>().Init(damage, orbDuration, orbSpeed, playerTransform.position);

    }

    public override void UpdateTargetting(Vector3 targettingPositionOnPlane)
    {
        if(playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        Vector3 diffPos = -playerTransform.position + targettingPositionOnPlane;
        diffPos.y = 0f;
        float radius = 2f * (-playerTransform.position + targettingPositionOnPlane).magnitude;
        orbEffectColliderTransform.localScale = new Vector3(radius, 1f, radius);
        orbEffectColliderTransform.rotation = Quaternion.LookRotation(diffPos, Vector3.up);
    }


    // Start is called before the first frame update
    void Start()
    {
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
