using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GridGenerator : MonoBehaviour
{
    public GameObject verticalTablePrefab;
    public GameObject horizontalTablePrefab;

    private GameObject selectedTable; // Selected table instance
    private GameObject clonedTable; // Cloned table instance
    private bool isPlacingTable; // Indicates if the player is currently placing a table

    public GameObject tilePrefab;
    public Sprite[] textures; // Array to hold the textures
    public float tileSize;

    public string jsonText; // JSON data as string

    private TerrainGridData terrainGridData;

    void Start()
    {
        // Deserialize JSON data into objects
        terrainGridData = JsonConvert.DeserializeObject<TerrainGridData>(jsonText);

        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int i = 0; i < terrainGridData.TerrainGrid.Count; i++)
        {
            for (int j = 0; j < terrainGridData.TerrainGrid[i].Count; j++)
            {
                Tile tile = Instantiate(tilePrefab, new Vector3(tileSize * i, 0, tileSize * j), Quaternion.Euler(90f, 0f, 0f),transform).GetComponent<Tile>(); // Instantiate tile at position
                int tileType = terrainGridData.TerrainGrid[i][j].TileType;
                SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

                // Assign texture based on tileType
                if (tileType >= 0 && tileType < textures.Length)
                {
                    renderer.sprite = textures[tileType];
                    if (tileType == 3) 
                    {
                        tile.SetTileTypeToWood();
                    }
                    //renderer.material.mainTexture = textures[tileType];
                }
                else
                {
                    Debug.LogWarning("Invalid TileType: " + tileType);
                }
            }
        }
    }


    void Update()
    {
        // Check for mouse down event
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the clicked object is a table
                if (hit.collider.gameObject.CompareTag("Table"))
                {
                    // Store the selected table
                    selectedTable = hit.collider.gameObject;

                    // Create a clone of the selected table
                    clonedTable = Instantiate(selectedTable);
                    isPlacingTable = true;
                }
            }
        }

        // Check if the player is currently placing a table
        if (isPlacingTable && clonedTable != null)
        {
            // Move the cloned table with the cursor
            MoveClonedTableWithCursor();

            // Check for mouse up event
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 position = hit.point;
                    int x = Mathf.RoundToInt(position.x);
                    int z = Mathf.RoundToInt(position.z);

                    // Check if the location is valid for placing the cloned table
                    if (IsValidTableLocation(x, z))
                    {
                        // Place the cloned table at the valid location
                        PlaceClonedTable(x, z);
                    }
                    else
                    {
                        Debug.LogError("Invalid location for placing the table.");

                        // Destroy the cloned table as the location is invalid
                        Destroy(clonedTable);
                    }
                }

                // Reset table placement state
                isPlacingTable = false;
                selectedTable = null;
                clonedTable = null;
            }
        }
    }

    bool IsValidTableLocation(int x, int z)
    {
        // Check if the clicked position is within the bounds of the grid and not occupied by another table
        return x >= 0 && x < terrainGridData.TerrainGrid.Count &&
               z >= 0 && z < terrainGridData.TerrainGrid[x].Count &&
               !terrainGridData.TerrainGrid[x][z].HasTable;
    }

    void MoveClonedTableWithCursor()
    {
        if (clonedTable != null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                // Move the cloned table to the mouse cursor's position
                clonedTable.transform.position = hit.point;
            }
        }
    }

    void PlaceClonedTable(int x, int z)
    {
        // Instantiate the appropriate table at the valid location
        GameObject table = Instantiate(clonedTable, new Vector3(x, 0, z), clonedTable.transform.rotation);

        // Mark the tiles at the valid location and its adjacent positions as having a table
        MarkTilesAsTable(x, z);
    }

    void MarkTilesAsTable(int x, int z)
    {
        // Mark the tiles at the valid location and its adjacent positions as having a table
        terrainGridData.TerrainGrid[x][z].HasTable = true;
    }
}


public class TerrainGridData
{
    public List<List<Tile>> TerrainGrid { get; set; }
}
