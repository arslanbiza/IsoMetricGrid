using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GridGenerator : MonoBehaviour
{
    public GameObject verticalTablePrefab;
    public GameObject horizontalTablePrefab;

    public GameObject selectedTable; // Selected table instance
    private GameObject clonedTable; // Cloned table instance
    private bool isPlacingTable; // Indicates if the player is currently placing a table

    public GameObject tilePrefab;
    public Sprite[] textures; // Array to hold the textures
    public float tileSize;

    public string jsonText; // JSON data as string

    private TerrainGridData terrainGridData;
    private bool isTablePlacementInProgress = false;

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
            // Only allow table placement if a table placement process is not in progress
            if (!isTablePlacementInProgress)
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
                        isTablePlacementInProgress = true; // Set the flag to indicate that a table placement process is ongoing
                    }
                }
            }
        }

        // Check if the player is currently placing a table
        if (isPlacingTable && clonedTable != null)
        {
            // Move the cloned table with the cursor
            MoveClonedTableWithCursor();



            Vector3 Tposition = GetMouseWorldPosition();
            int tx = Mathf.RoundToInt(Tposition.x);
            int tz = Mathf.RoundToInt(Tposition.z);

            SpriteRenderer renderer = clonedTable.GetComponent<SpriteRenderer>();
            if (IsValidTableLocation(tx, tz))
            {
                // Set the color to green if the location is valid
                renderer.color = Color.green;
            }
            else
            {
                // Set the color to red if the location is invalid
                renderer.color = Color.red;
            }





            // Check for mouse up event
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 position = GetMouseWorldPosition();
                int x = Mathf.RoundToInt(position.x - position.z);
                int z = Mathf.RoundToInt(position.x + position.z);

                // Check if the location is valid for placing the cloned table
                if (IsValidTableLocation(x, z))
                {
                    // Place the cloned table at the valid location

                    PlaceClonedTable(x, z);
                }
                else
                {
                    Debug.LogError("Invalid location for placing the table.");
                    isTablePlacementInProgress = false;
                    // Destroy the cloned table as the location is invalid
                    Destroy(clonedTable);
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
            Vector3 position = GetMouseWorldPosition();
            clonedTable.transform.position = new Vector3(position.x, 0+ 0.073f, position.z);
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero); // Create a plane at the ground level
        float distance;
        if (plane.Raycast(ray, out distance)) // Cast a ray onto the plane
        {
            return ray.GetPoint(distance); // Return the intersection point
        }
        return Vector3.zero;
    }

    void PlaceClonedTable(int x, int z)
    {
        // Move the cloned table to the valid position on the grid
    clonedTable.transform.position = new Vector3(x, 0, z);

        // Change the tag of the cloned table to "Default"
       // clonedTable.tag = "Default";

        // Mark the tiles at the valid location and its adjacent positions as having a table
        MarkTilesAsTable(x, z);

        // Set the flag to indicate that the table placement process is completed
        isTablePlacementInProgress = false;
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
