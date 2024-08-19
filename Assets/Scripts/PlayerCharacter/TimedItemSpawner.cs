using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedItemSpawner : MonoBehaviour
{
    [SerializeField]
    float minItemSpawnTimeInterval_sec = 10f;
    [SerializeField]
    float maxItemSpawnTimeInterval_sec = 30f;

    [SerializeField]
    ItemDefinition[] spawnableItems;

    [SerializeField]
    float spawnRange = 15f;

    void Start()
    {
        StartCoroutine(ItemSpawnCoroutine());
    }

    private IEnumerator ItemSpawnCoroutine()
    {
        while(true)
        {
            var timeInterval_sec = Random.Range(minItemSpawnTimeInterval_sec, maxItemSpawnTimeInterval_sec);
            yield return new WaitForSeconds(timeInterval_sec);
            int spawnID = Random.Range(0, spawnableItems.Length);

            var dropRelativePosition = Random.insideUnitCircle;
            var itemPosition = transform.position + spawnRange * new Vector3(dropRelativePosition.x, 0, dropRelativePosition.y);

            var itemObject = Instantiate(spawnableItems[spawnID].WorldItemPrefab, itemPosition, Quaternion.identity);
            itemObject.GetComponent<WorldItem>().Definition = spawnableItems[spawnID];
        }
    }
}
