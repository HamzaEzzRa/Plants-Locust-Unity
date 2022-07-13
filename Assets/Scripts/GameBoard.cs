using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour, ISerializationCallbackReceiver
{
    public void OnBeforeSerialize()
    {
        searchFrontierList.Clear();
        foreach (Tile tile in searchFrontier)
        {
            searchFrontierList.Add(tile);
        }

        selectedTilesList.Clear();
        foreach (Tile tile in selectedTiles)
        {
            selectedTilesList.Add(tile);
        }

        spawnPointsList.Clear();
        foreach (Tile tile in spawnPoints)
        {
            spawnPointsList.Add(tile);
        }

        updatingContentList.Clear();
        foreach (TileContent tileContent in updatingContent)
        {
            updatingContentList.Add(tileContent);
        }
    }

    public void OnAfterDeserialize()
    {
        searchFrontier.Clear();
        foreach (Tile tile in searchFrontierList)
        {
            searchFrontier.Enqueue(tile);
        }

        selectedTiles.Clear();
        foreach (Tile tile in selectedTilesList)
        {
            selectedTiles.Add(tile);
        }

        spawnPoints.Clear();
        foreach (Tile tile in spawnPointsList)
        {
            spawnPoints.Add(tile);
        }

        updatingContent.Clear();
        foreach (TileContent tileContent in updatingContentList)
        {
            updatingContent.Add(tileContent);
        }
    }

    public HashSet<Tile> SelectedTiles => selectedTiles;

    public HashSet<Tile> SpawnPoints => spawnPoints;

    [SerializeField] private Transform ground = default;
    [SerializeField] private Tile tilePrefab = default;

    [SerializeField] private TileType defaultTileType = TileType.EMPTY_TILE;

    [SerializeField, HideInInspector] private Tile[] tiles;

    private Queue<Tile> searchFrontier = new Queue<Tile>();
    [SerializeField, HideInInspector] private List<Tile> searchFrontierList = new List<Tile>();

    private HashSet<Tile> selectedTiles = new HashSet<Tile>();
    [SerializeField, HideInInspector] private List<Tile> selectedTilesList = new List<Tile>();

    private HashSet<Tile> spawnPoints = new HashSet<Tile>();
    [SerializeField, HideInInspector] private List<Tile> spawnPointsList = new List<Tile>();

    private HashSet<TileContent> updatingContent = new HashSet<TileContent>();
    [SerializeField, HideInInspector] private List<TileContent> updatingContentList = new List<TileContent>();

    [SerializeField, HideInInspector] private TileContentFactory contentFactory;
    [SerializeField, HideInInspector] private Vector2Int size;

    private bool showGrid, showPaths;

    private const int groundMask = 1 << 8;

    public bool ShowPaths
    {
        get => showPaths;
        set
        {
            showPaths = value;
            if (showPaths)
            {
                foreach (Tile tile in tiles)
                {
                    tile.ShowPath();
                }
            }
            else
            {
                foreach (Tile tile in tiles)
                {
                    tile.HidePath();
                }
            }
        }
    }

    private void Start()
    {
        foreach (Tile tile in tiles)
        {
            if (tile != null)
            {
                tile.Content.SetOutlineEnabled(false);
            }
        }
    }

    public void Initialize(TileContentFactory contentFactory)
    {
        this.contentFactory = contentFactory;
        Vector3 groundSize = ground.GetComponent<Terrain>().terrainData.size;
        size = new Vector2Int(Mathf.RoundToInt(groundSize.x), Mathf.RoundToInt(groundSize.z));

        //Debug.Log("Actual Ground Size -> " + new Vector2(groundSize.x, groundSize.z));
        //Debug.Log("Approx Ground Size -> " + size);

        tiles = new Tile[size.x * size.y];

        Vector2 offset = new Vector2((groundSize.x - 1) * 0.5f, (groundSize.z - 1) * 0.5f);
        for (int i = 0, y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++, i++)
            {
                Tile tile = tiles[i] = Instantiate(tilePrefab);
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(x - offset.x, tilePrefab.transform.position.y, y - offset.y);
                if (x > 0)
                {
                    Tile.MakeEastWestNeighbors(tile, tiles[i - 1]);
                }
                if (y > 0)
                {
                    Tile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);
                }
                tile.IsAlternative = (x & 1) == 0;
                if ((y & 1) == 0)
                {
                    tile.IsAlternative = !tile.IsAlternative;
                }
                tile.ReplaceContent(contentFactory.Get(defaultTileType));
                tile.Content.Type = tile.Type = defaultTileType;
            }
        }
    }

    public void GameUpdate()
    {
        foreach (TileContent content in updatingContent)
        {
            content.GameUpdate();
        }
    }

    public void UpdateTiles()
    {
        foreach (Tile tile in tiles)
        {
            if (tile != null)
            {
                //if (tile.Content.Type != tile.Type)
                {
                    UpdateTile(tile, tile.Type);
                }
            }
        }

        FindPaths();
    }

    public void ClearBoard()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            Tile tile = tiles[i];
            updatingContent.Remove(tile.Content);
            updatingContentList.Remove(tile.Content);
            DestroyImmediate(tile.Content.gameObject);

            selectedTiles.Remove(tile);
            selectedTilesList.Remove(tile);
            spawnPoints.Remove(tile);
            spawnPointsList.Remove(tile);
            DestroyImmediate(tile.gameObject);
            tiles[i] = null;
        }
        updatingContent.Clear();
        updatingContentList.Clear();
        selectedTiles.Clear();
        selectedTilesList.Clear();
        spawnPoints.Clear();
        spawnPointsList.Clear();
        searchFrontierList.Clear();
        searchFrontier.Clear();
    }

    public Tile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundMask))
        {
            int x = (int)(hit.point.x + size.x * 0.5f);
            int y = (int)(hit.point.z + size.y * 0.5f);

            if (x >= 0 && x < size.x && y >= 0 && y < size.y)
            {
                return tiles[x + y * size.x];
            }
        }
        return null;
    }

    public HashSet<Tile> GetTiles(Rect rect)
    {
        HashSet<Tile> selection = new HashSet<Tile>();
        for (int y = 0; y <= rect.height; y += 20)
        {
            for (int x = 0; x <= rect.width; x += 20)
            {
                Vector2 position = new Vector2(rect.xMin + x, Screen.height - (rect.yMin + y));
                Tile tile = GetTile(Camera.main.ScreenPointToRay(position));
                if (tile != null && !selection.Contains(tile))
                {
                    selection.Add(tile);
                }
            }
        }
        return selection;
    }

    public void SetPath() { HandleDebug(TileType.PATH_TILE); }

    public void SetTree() { HandleDebug(TileType.TREE_TILE); }

    public void SetDestination() { HandleDebug(TileType.DESTINATION_TILE); }

    public void SetCrop() { HandleDebug(TileType.CROP_TILE); }

    public void SetFirePlant() { HandleDebug(TileType.FIRE_PLANT_TILE); }
    
    public void SetGrenadePlant() { HandleDebug(TileType.GRENADE_PLANT_TILE); }

    public void SetPoisonPlant() { HandleDebug(TileType.POISON_PLANT_TILE); }

    public void SetSpawn() { HandleDebug(TileType.SPAWN_TILE); }

    private void HandleDebug(TileType type)
    {
        if ((int)type > 0)
        {
            foreach (Tile tile in selectedTiles)
            {
                if (type != tile.Content.Type)
                {
                    UpdateTile(tile, type);
                }
            }

            FindPaths();
        }
    }

    private void UpdateTile(Tile tile, TileType type)
    {
        //tile.transform.position = new Vector3(tile.transform.position.x, tilePrefab.transform.position.y, tile.transform.position.z);
        if (tile.Content.Type == TileType.SPAWN_TILE)
        {
            if (type != TileType.SPAWN_TILE)
            {
                spawnPoints.Remove(tile);
            }

            tile.ReplaceContent(contentFactory.Get(type));
            if (TileContent.IsPlantType(type))
            {
                updatingContent.Add(tile.Content);
            }
        }
        else if (TileContent.IsPlantType(tile.Content.Type))
        {
            updatingContent.Remove(tile.Content);
            tile.ReplaceContent(contentFactory.Get(type));
            if (type == TileType.SPAWN_TILE)
            {
                spawnPoints.Add(tile);
            }
            else if (TileContent.IsPlantType(type))
            {
                updatingContent.Add(tile.Content);
            }
        }
        else
        {
            tile.ReplaceContent(contentFactory.Get(type));
            if (type == TileType.SPAWN_TILE)
            {
                spawnPoints.Add(tile);
            }
            else if (TileContent.IsPlantType(type))
            {
                updatingContent.Add(tile.Content);
            }
        }
    }

    public void MarkAsSelected(Tile tile, bool isDebug)
    {
        if (tile == null || selectedTiles.Contains(tile))
        {
            return;
        }

        selectedTiles.Add(tile);
        if (isDebug)
        {
            tile.selection.gameObject.SetActive(true);
        }
    }

    public void UnselectTile(Tile tile)
    {
        if (tile == null || !selectedTiles.Contains(tile))
        {
            return;
        }

        selectedTiles.Remove(tile);
        tile.selection.gameObject.SetActive(false);
    }

    public void ClearSelection()
    {
        foreach (Tile tile in selectedTiles)
        {
            tile.selection.gameObject.SetActive(false);
        }
        selectedTiles.Clear();
    }

    private void FindPaths()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            Tile tile = tiles[i];
            if (tile != null)
            {
                if (tile.Content.Type == TileType.DESTINATION_TILE)
                {
                    tile.MarkAsDestination();
                    searchFrontier.Enqueue(tile);
                }
                else
                    tile.ClearPath();
            }
        }
        if (searchFrontier.Count == 0)
        {
            foreach (Tile tile in tiles)
            {
                if (tile != null)
                {
                    tile.ClearPath();
                    tile.HidePath();
                }
            }
        }

        while (searchFrontier.Count > 0)
        {
            Tile tile = searchFrontier.Dequeue();
            if (tile != null)
            {
                if (tile.IsAlternative)
                {
                    searchFrontier.Enqueue(tile.GrowPathNorth());
                    searchFrontier.Enqueue(tile.GrowPathSouth());
                    searchFrontier.Enqueue(tile.GrowPathEast());
                    searchFrontier.Enqueue(tile.GrowPathWest());
                }
                else
                {
                    searchFrontier.Enqueue(tile.GrowPathWest());
                    searchFrontier.Enqueue(tile.GrowPathEast());
                    searchFrontier.Enqueue(tile.GrowPathSouth());
                    searchFrontier.Enqueue(tile.GrowPathNorth());
                }
            }   
        }

        if (showPaths)
        {
            foreach (Tile tile in tiles)
            {
                tile.ShowPath();
            }
        }
    }

    private void OnValidate()
    {
        if (size.x < 2)
        {
            size.x = 2;
        }
        if (size.y < 2)
        {
            size.y = 2;
        }
    }
}
