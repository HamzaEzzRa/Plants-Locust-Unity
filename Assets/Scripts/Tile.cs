using UnityEngine;

[System.Serializable]
public enum TileType
{
    NONE = 0,
    PATH_TILE,
    TREE_TILE,
    DESTINATION_TILE,
    CROP_TILE,
    FIRE_PLANT_TILE,
    GRENADE_PLANT_TILE,
    POISON_PLANT_TILE,
    SPAWN_TILE,
    EMPTY_TILE,
    CARROT_PLANT_TILE
}

[System.Serializable]
public enum Direction
{
    North,
    East,
    South,
    West
}

[System.Serializable]
public enum DirectionChange
{
    None,
    TurnRight,
    TurnLeft,
    TurnAround
}

[System.Serializable]
public static class DirectionExtensions
{
    private static readonly Quaternion[] rotations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),
        Quaternion.Euler(0f, 180f, 0f),
        Quaternion.Euler(0f, 270f, 0f)
    };

    private static readonly Vector3[] halfVectors = {
        Vector3.forward * 0.5f,
        Vector3.right * 0.5f,
        Vector3.back * 0.5f,
        Vector3.left * 0.5f
    };

    public static Quaternion GetRotation(this Direction direction) { return rotations[(int)direction]; }

    public static DirectionChange GetDirectionChangeTo(this Direction current, Direction next)
    {
        if (current == next)
        {
            return DirectionChange.None;
        }
        else if (current + 1 == next || current - 3 == next)
        {
            return DirectionChange.TurnRight;
        }
        else if (current - 1 == next || current + 3 == next)
        {
            return DirectionChange.TurnLeft;
        }
        return DirectionChange.TurnAround;
    }

    public static float GetAngle(this Direction direction) { return (float)direction * 90f; }

    public static Vector3 GetHalfVector(this Direction direction) { return halfVectors[(int)direction]; }
}

[System.Serializable]
public class Tile : MonoBehaviour
{
    public bool HasPath => distance != int.MaxValue;
    
    public bool IsAlternative
    {
        get => isAlternative;
        set
        {
            isAlternative = value;
        }
    }

    public Tile NextTileOnPath => nextOnPath;

    public Vector3 ExitPoint
    {
        get => exitPoint;
        private set
        {
            exitPoint = value;
        }
    }

    public Direction PathDirection
    {
        get => pathDirection;
        private set
        {
            pathDirection = value;
        }
    }

    public TileContent Content
    {
        get => content;
    }
    
    public void ReplaceContent(TileContent newContent, bool reclaim=true)
    {
        if (newContent == null)
        {
            return;
        }
        if (reclaim && content != null)
        {
            content.Recycle();
        }

        content = newContent;
        content.tile = this;
        content.transform.localPosition = new Vector3(transform.localPosition.x, content.transform.localPosition.y, transform.localPosition.z);
    }

    public TileType Type
    {
        get => type;
        set
        {
            type = value;
        }
    }

    public Transform selection = default;

    [SerializeField] private Transform arrow = default;
    [SerializeField] private TileType type = TileType.PATH_TILE;

    [SerializeField, HideInInspector] private Vector3 exitPoint;
    [SerializeField, HideInInspector] private Direction pathDirection;

    [SerializeField, HideInInspector] private TileContent content;

    [SerializeField, HideInInspector] private Tile north, east, south, west, nextOnPath;

    [SerializeField, HideInInspector] private int distance; // Number of tiles until destination

    [SerializeField, HideInInspector] private bool isAlternative;

    private static Quaternion
        northRotation = Quaternion.Euler(90f, 0f, 0f),
        eastRotation = Quaternion.Euler(90f, 90f, 0f),
        southRotation = Quaternion.Euler(90f, 180f, 0f),
        westRotation = Quaternion.Euler(90f, 270f, 0f);

    public static void MakeEastWestNeighbors(Tile east, Tile west)
    {
        west.east = east;
        east.west = west;
    }

    public static void MakeNorthSouthNeighbors(Tile north, Tile south)
    {
        north.south = south;
        south.north = north;
    }

    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void MarkAsDestination()
    {
        distance = 0;
        nextOnPath = null;
        ExitPoint = transform.localPosition;
    }

    public void ShowPath()
    {
        if (distance == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == north ? northRotation :
            nextOnPath == east ? eastRotation :
            nextOnPath == south ? southRotation :
            westRotation;
    }

    public void HidePath()
    {
        arrow.gameObject.SetActive(false);
    }

    private Tile GrowPathTo(Tile neighbor, Direction direction)
    {
        if (!HasPath || neighbor == null || neighbor.HasPath)
        {
            return null;
        }

        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector();
        neighbor.PathDirection = direction;
        return neighbor.Content.BlocksPath ? null : neighbor;
    }

    public Tile GrowPathNorth() => GrowPathTo(north, Direction.South);

    public Tile GrowPathEast() => GrowPathTo(east, Direction.West);
    
    public Tile GrowPathSouth() => GrowPathTo(south, Direction.North);
    
    public Tile GrowPathWest() => GrowPathTo(west, Direction.East);

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(Vector3.zero, "Tile");
    }
}
