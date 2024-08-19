using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] public GameObject[] floorTileVariations;
    [SerializeField] public Texture2D colorGrading;

    [Header("Variables")]
    [SerializeField] public Vector2Int floorSize = new Vector2Int(512, 512);
    [SerializeField][Range(0, 10)] public float tileSize = 1;

    private GameObject[][] floorTiles;
    
    // Start is called before the first frame update
    void Awake()
    {
        CreateFloorTiles();
    }

    public void CreateFloorTiles()
    {
        // Creating our arrays.
        floorTiles = new GameObject[floorSize.x][];

        for(int x = 0; x < floorSize.x; x++)
        {
            floorTiles[x] = new GameObject[floorSize.y];
        }

        // Instantiating the floor.
        Quaternion tileRotation = Quaternion.Euler(-90, 0, 0);

        for (int x = 0; x < floorSize.x; x++)
        {
            for(int y = 0; y < floorSize.y; y++)
            {
                floorTiles[x][y] = Instantiate(floorTileVariations[Random.Range(0, floorTileVariations.Length)],
                                               (new Vector3(x, 0, y) - new Vector3(floorSize.x, 0, floorSize.y) / 2) * tileSize,
                                               tileRotation,
                                               this.transform);

                floorTiles[x][y].name = "FloorTile(" + x + ", " + y + ")";


                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                Color newColor = colorGrading.GetPixel(x * 16, y * 16);
                propertyBlock.SetVector("_ColorAdjustment", 
                    new Vector3(newColor.r * 255, newColor.g * 255, newColor.b * 255));
                print(new Vector3(newColor.r * 255, newColor.g * 255, newColor.b * 255));

                // Apply the MaterialPropertyBlock to the GameObject
                floorTiles[x][y].GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
            }
        }


    }
}
