using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    Transform playerTransform;
    [SerializeField]
    float spawnsPerSec = 0.1f;
    [SerializeField]
    private float spawnDistance = 25f;

    [Serializable]
    public struct SpawnableEnemy
    {
        public GameObject enemyPrefab;
        public float spawnWeight;
    }

    [SerializeField]
    private SpawnableEnemy[] spawnableEnemies;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForSpawnCoroutine());
    }

    private IEnumerator WaitForSpawnCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f/spawnsPerSec);
            SpawnRandomEnemy();
        }
    }

    private void SpawnRandomEnemy()
    {
        SpawnableEnemy chosenEnemy = ChooseRandomEnemy();
        SpawnSpecificEnemy(chosenEnemy);
    }

    public void SpawnSpecificEnemy(SpawnableEnemy enemy)
    {
        Vector3 outOfViewPosition = GetOutOfViewPosition();
        Instantiate(enemy.enemyPrefab, outOfViewPosition, Quaternion.identity, transform);
    }

    private Vector3 GetOutOfViewPosition()
    {
        Vector2 randomDirection = UnityEngine.Random.onUnitSphere;
        randomDirection = randomDirection.normalized;
        Vector2 ranOffset = randomDirection * spawnDistance;
        return playerTransform.position + new Vector3(ranOffset.x, 0f, ranOffset.y);
    }

    private SpawnableEnemy ChooseRandomEnemy()
    {
        float weightSum = 0f;
        foreach(var enemy in spawnableEnemies)
        {
            weightSum += enemy.spawnWeight;
        }
        float randomNumber = UnityEngine.Random.Range(0f, weightSum);
        SpawnableEnemy chosenEnemy;

        for (int i = 0; i < spawnableEnemies.Length; ++i)
        {
            chosenEnemy = spawnableEnemies[i];
            randomNumber -= chosenEnemy.spawnWeight;
            if (randomNumber <= 0f)
                return chosenEnemy;
        }
        return new SpawnableEnemy();
    }
}
