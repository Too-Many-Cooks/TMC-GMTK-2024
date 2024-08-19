using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] public GameObject[] floorTileVariations;
    [SerializeField] public Texture2D colorGrading;
    [SerializeField] public Transform playerTransform;

    [Header("Variables")]
    private int initialFloorSize = 100;
    [SerializeField] public Vector2Int poolSizeAroundPlayer = new Vector2Int(20, 10);
    [SerializeField] public Vector2Int poolPositionDisplacement = new Vector2Int(-5, 5);

    private List<MeshRenderer> floorTiles = new List<MeshRenderer>();
    private List<MeshRenderer> disabledTiles = new List<MeshRenderer>();

    private List<Vector2Int> oldTilePositions = new List<Vector2Int>();
    private Vector2Int oldOriginPoint;

    private Quaternion TileRotation => Quaternion.Euler(-90, 0, 0);

    #region Creating the tiles at Awake

    void Awake()
    {
        CreateFloorTiles();
        InitialPlacingOfTiles();
    }

    private void CreateFloorTiles()
    {
        // Instantiating the floor.
        for (int i = 0; i < initialFloorSize; i++)
        {
            disabledTiles.Add(Instantiate(floorTileVariations[Random.Range(0, floorTileVariations.Length)],
                Vector3.zero, TileRotation, this.transform).GetComponent<MeshRenderer>());
            disabledTiles[i].gameObject.SetActive(false);
        }
    }

    private void InitialPlacingOfTiles()
    {
        List<Vector2Int> positionsToInitialized = new List<Vector2Int>();

        oldOriginPoint = OriginPoint;

        for (int x = 0; x < poolSizeAroundPlayer.x; x++)
        {
            for (int y = 0; y < poolSizeAroundPlayer.y; y++)
            {                                                           // x2 cause tiles are 2x2m.
                positionsToInitialized.Add(oldOriginPoint + new Vector2Int(x * 2, y * 2));
                oldTilePositions.Add(oldOriginPoint + new Vector2Int(x * 2, y * 2));
            }
        }

        PoolTilesToPosition(positionsToInitialized.ToArray());
    }

    #endregion

    #region Pooling the tiles during Update

    private void Update()
    {
        Vector2Int newOrigin = OriginPoint;
        Vector2Int originPointDelta = newOrigin - oldOriginPoint;

        if (originPointDelta == Vector2Int.zero)
            return;

        List<Vector2Int> newTilePositions = new List<Vector2Int>();
        List<Vector2Int> tilesThatRequireInitialization = new List<Vector2Int>();

        for (int x = 0; x < poolSizeAroundPlayer.x; x++)
        {
            for (int y = 0; y < poolSizeAroundPlayer.y; y++)
            {
                Vector2Int tilePosition = newOrigin + new Vector2Int(x * 2, y * 2);

                if (oldTilePositions.Contains(tilePosition))
                {
                    oldTilePositions.Remove(tilePosition);
                    newTilePositions.Add(tilePosition);
                }
                else
                {
                    newTilePositions.Add(tilePosition);
                    tilesThatRequireInitialization.Add(tilePosition);
                }
            }
        }

        // Whatever tiles remain here are to be returned to the pool.
        ReturnTilesToThePool(oldTilePositions.ToArray());
        PoolTilesToPosition(tilesThatRequireInitialization.ToArray());

        // Updating our old variables.
        oldOriginPoint = newOrigin;
        oldTilePositions = newTilePositions;
    }

    #endregion


    private Vector2Int NearestTileToPlayer
        => new Vector2Int((int)playerTransform.transform.position.x / 2 * 2, (int)playerTransform.transform.position.z / 2 * 2);

    // Calculating the origin location of the pool.  
    // We want to move half of the distance in poolSizeAroundPlayer, but that is cancelled by tiles been 2x2m.
    private Vector2Int OriginPoint => NearestTileToPlayer - poolSizeAroundPlayer + poolPositionDisplacement;


    private void PoolTilesToPosition(Vector2Int[] newTilePosition)
    {
        int tilesLeft = newTilePosition.Length - disabledTiles.Count;

        // Adding tiles if we are missing them.
        for (int i = 0; i < tilesLeft; i++)
        {
            disabledTiles.Add(Instantiate(floorTileVariations[Random.Range(0, floorTileVariations.Length)],
                Vector3.zero, TileRotation, this.transform).GetComponent<MeshRenderer>());
        }

        // Enabling the tiles.
        for(int i = 0; i < newTilePosition.Length; i++)
        {
            MeshRenderer pooledTile = disabledTiles[0];
            pooledTile.gameObject.SetActive(true);
            pooledTile.name = "FloorTile_[" + newTilePosition[i].x + ", " + newTilePosition[i].y + "]";

            // Moving our tile from one list to the other.
            floorTiles.Add(pooledTile);
            disabledTiles.RemoveAt(0);

            // Setting up the position of the tile.
            pooledTile.transform.position = new Vector3(newTilePosition[i].x, 0, newTilePosition[i].y);

            // Setting up the tile's color.
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

            Color newColor = colorGrading.GetPixel(newTilePosition[i].x * 4, newTilePosition[i].y * 4);
            propertyBlock.SetVector("_ColorVariation",
                new Vector3(newColor.r * 255, newColor.g * 255, newColor.b * 255));

            // Apply the MaterialPropertyBlock to the GameObject
            pooledTile.SetPropertyBlock(propertyBlock);
        }
    }

    private void ReturnTilesToThePool(Vector2Int[] tilePosition)
    {
        List<MeshRenderer> tilesToReturnToPool = new List<MeshRenderer>();
        
        for(int i = 0; i < floorTiles.Count; i++)
        {
            string[] tileCoordinateString = floorTiles[i].name.Substring(11).Trim(']').Split(", ");
            Vector2Int coordinates = 
                new Vector2Int (int.Parse(tileCoordinateString[0]), int.Parse(tileCoordinateString[1]));

            if (tilePosition.Contains(coordinates))
            {
                tilesToReturnToPool.Add(floorTiles[i]);
            }
        }

        if(tilesToReturnToPool.Count != tilePosition.Length)
        {
            Debug.LogError("Error in the floor's pool logic. " +
                "Some of the tiles cannot be returned to the pool because they do not exist there.");

            if (tilesToReturnToPool.Count == 0)
                return;
        }

        foreach (MeshRenderer tile in tilesToReturnToPool)
        {
            floorTiles.Remove(tile);
            disabledTiles.Add(tile);
            tile.gameObject.SetActive(false);
        }
    }
}
