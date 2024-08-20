using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObstacleManager : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] FloorManager.SpawnablePrefabs[] spawnableObstacles;
    [SerializeField] Texture2D[] blueNoiseSpawnMaps;

    [Header("References")]
    [SerializeField] Transform playerTransform;

    Dictionary<Vector2Int, GameObject> tileFolderDictionary = new Dictionary<Vector2Int, GameObject>();
    const int numberOfTilesPerZone = 32;

    private void EnableObstacleFolder(Vector2Int folderVectorIdentifier)
        => ObtainObstacleFolder(folderVectorIdentifier).SetActive(true);

    private void DisableObstacleFolder(Vector2Int folderVectorIdentifier)
    => ObtainObstacleFolder(folderVectorIdentifier).SetActive(false);

    private GameObject ObtainObstacleFolder(Vector2Int folderVectorIdentifier)
    {
        // If the folder, already exists, we return it.
        if (tileFolderDictionary.ContainsKey(folderVectorIdentifier))
            return tileFolderDictionary[folderVectorIdentifier];

        // If the folder doesn't exist yet, we create it.
        GameObject folder = Instantiate(new GameObject(), transform);
        folder.transform.position = new Vector3(folderVectorIdentifier.x * 2 * numberOfTilesPerZone, 
                                                transform.position.y, 
                                                folderVectorIdentifier.y * 2 * numberOfTilesPerZone);
        folder.name = "ObstacleFolder_[" + folderVectorIdentifier.x + ", " + folderVectorIdentifier.y + "]";
        tileFolderDictionary.Add(folderVectorIdentifier, folder);

        FillFolderWithRandomObstacles(folder.transform, folderVectorIdentifier);
        return folder;
    }

    private void FillFolderWithRandomObstacles(Transform folderToFill, Vector2Int folderVectorIdentifier)
    {
        int blueNoiseMapIndex = Mathf.Abs((folderVectorIdentifier.x + 30000) / 2 
                + (folderVectorIdentifier.y + 30000) / 2) % blueNoiseSpawnMaps.Length;

        Vector2Int originPointInBlueNoiseTexture = new Vector2Int(Mathf.Abs(folderVectorIdentifier.x % 2) * 32,
            Mathf.Abs(folderVectorIdentifier.y % 2) * 32);


        for(int x = 0; x < numberOfTilesPerZone; x++)
        {
            for(int y = 0; y < numberOfTilesPerZone; y++)
            {
                if(blueNoiseSpawnMaps[blueNoiseMapIndex].GetPixel
                    (originPointInBlueNoiseTexture.x + x + 1, originPointInBlueNoiseTexture.y + y + 1).grayscale == 1)
                {
                    PlaceRandomItemInsideFolder(folderToFill, new Vector2Int(x, y));
                }
            }
        }
    }

    private void PlaceRandomItemInsideFolder(Transform folder, Vector2Int position)
    {
        GameObject obstacle = Instantiate<GameObject>(spawnableObstacles[WhatObstaclePrefabToUse()].prefab, folder);
        obstacle.transform.localPosition = new Vector3(position.x * 2 + 1, 0, position.y * 2 + 1);
    }

    private float _maxTileVariationSumChance;
    private float MaxTileVariationSumChance
    {
        get
        {
            if (_maxTileVariationSumChance == 0)
            {
                for (int i = 0; i < spawnableObstacles.Length; i++)
                {
                    _maxTileVariationSumChance += spawnableObstacles[i].spawnMultiplier;
                }
            }

            return _maxTileVariationSumChance;
        }
    }

    private int WhatObstaclePrefabToUse()
    {
        float spawnRandomValue = Random.Range(0, MaxTileVariationSumChance);
        int tileVariationIndex = -1;

        while (spawnRandomValue > 0 && tileVariationIndex < spawnableObstacles.Length - 1)
        {
            tileVariationIndex++;
            spawnRandomValue -= spawnableObstacles[tileVariationIndex].spawnMultiplier;
        }

        return tileVariationIndex;
    }


    private void Awake()
    {
        InitialPlacingOfObstaclesAroundPlayer();
    }

    private void InitialPlacingOfObstaclesAroundPlayer()
    {
        oldZoneOriginPoint = ZoneOriginPoint;

        foreach (Vector2Int zone in ZonesAroundAnOriginZone(oldZoneOriginPoint))
        {
            EnableObstacleFolder(zone);
            oldZonePositions.Add(zone);
        }
    }

    private void Update()
    {
        Vector2Int newOriginPoint = ZoneOriginPoint;
        Vector2Int originPointDelta = newOriginPoint - oldZoneOriginPoint;

        if (originPointDelta == Vector2Int.zero)
            return;

        List<Vector2Int> newZonePositions = new List<Vector2Int>();
        List<Vector2Int> zonesThatRequireInitialization = new List<Vector2Int>();

        foreach (Vector2Int zone in ZonesAroundAnOriginZone(newOriginPoint))
        {
            if (oldZonePositions.Contains(zone))
            {
                oldZonePositions.Remove(zone);
                newZonePositions.Add(zone);
            }
            else
            {
                newZonePositions.Add(zone);
                zonesThatRequireInitialization.Add(zone);
            }
        }

        // Whatever zones remain here are to be disabled.
        foreach(Vector2Int zoneToRemove in oldZonePositions)
            DisableObstacleFolder(zoneToRemove);

        // Enabling new Zones
        foreach (Vector2Int zoneToInitialize in newZonePositions)
            EnableObstacleFolder(zoneToInitialize);

        // Updating our old variables.
        oldZoneOriginPoint = newOriginPoint;
        oldZonePositions = newZonePositions;
    }

    private Vector2Int oldZoneOriginPoint = Vector2Int.zero;
    private List<Vector2Int> oldZonePositions = new List<Vector2Int>();

    private Vector2Int[] ZonesAroundAnOriginZone(Vector2Int originZone)
    {
        return new Vector2Int[] 
        {
            originZone + new Vector2Int(-1, 1), originZone + new Vector2Int(0, 1), originZone + new Vector2Int(1, 1),
            originZone + new Vector2Int(-1, 0), originZone + new Vector2Int(0, 0), originZone + new Vector2Int(1, 0),
            originZone + new Vector2Int(-1, -1), originZone + new Vector2Int(0, -1), originZone + new Vector2Int(1, -1)
        };
    }

    public static Vector2Int NearestZoneToPlayer(Transform player)
    {
        Vector2Int result = new Vector2Int((int)player.position.x / (numberOfTilesPerZone * 2),
                          (int)player.position.z / (numberOfTilesPerZone * 2));


        if (player.position.x < 0)
            result.x--;

        if (player.position.z < 0)
            result.y--;

        return result;
    }

    private Vector2Int ZoneOriginPoint => NearestZoneToPlayer(playerTransform);
}
