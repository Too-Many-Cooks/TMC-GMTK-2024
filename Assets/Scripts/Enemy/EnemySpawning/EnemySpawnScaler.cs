using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemySpawner))]
public class EnemySpawnScaler : MonoBehaviour
{
    public enum SpawnScalingMode
    {
        Linear,
        Exponential
    }

    [SerializeField]
    SpawnScalingMode scalingMode;

    [SerializeField]
    float scalingTimeInterval_sec = 10f;

    [SerializeField]
    float linearScalingAmountPerSec = 0.1f;
    [SerializeField] 
    float exponentialScalingFactorPerSec = 1.05f;

    EnemySpawner enemySpawner;

    // Start is called before the first frame update
    void Start()
    {
        enemySpawner = GetComponent<EnemySpawner>();
        StartCoroutine(ScalingCoroutine());
    }

    private IEnumerator ScalingCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(scalingTimeInterval_sec);
            switch (scalingMode)
            {
                case SpawnScalingMode.Linear:
                    enemySpawner.spawnsPerSec += linearScalingAmountPerSec;
                    break;
                case SpawnScalingMode.Exponential:
                    enemySpawner.spawnsPerSec *= exponentialScalingFactorPerSec;
                    break;
            }
        }
    }
}
