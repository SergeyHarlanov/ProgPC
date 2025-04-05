using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingGrid : MonoBehaviour
{
    public static BuildingGrid Instance { get; private set; }

    [SerializeField] private float _gridSize = 1f;

    private readonly Dictionary<Vector3Int, List<Buildable>> _occupiedCells = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Vector3 SnapToGrid(Vector3 position)
    {
        float invGridSize = 1f / _gridSize;
        return new Vector3(
            Mathf.Round(position.x * invGridSize) * _gridSize,
            Mathf.Round(position.y * invGridSize) * _gridSize,
            Mathf.Round(position.z * invGridSize) * _gridSize
        );
    }

    public void RegisterObject(Buildable obj, Vector3 position)
    {
        Vector3Int gridPos = ToGridPosition(position, obj.Type);
        GetOrCreateCell(gridPos).Add(obj);
    }

    public void UnregisterObject(Buildable obj, Vector3 position)
    {
        Vector3Int gridPos = ToGridPosition(position, obj.Type);
        if (_occupiedCells.TryGetValue(gridPos, out var objects))
        {
            objects.Remove(obj);
            if (objects.Count == 0)
            {
                _occupiedCells.Remove(gridPos);
            }
        }
    }

    public bool IsPositionFree(Vector3 position, BuildableType type)
    {
        Vector3Int gridPos = ToGridPosition(position, type);
        return !_occupiedCells.ContainsKey(gridPos);
    }

    public Vector3 FindFreePositionForObject(Vector3 position, BuildableType type)
    {
        Vector3Int gridPos = ToGridPosition(position, type);
        
        if (type == BuildableType.Cube)
        {
            gridPos.y = 0;
        }

        if (!_occupiedCells.TryGetValue(gridPos, out var objects) || objects.Count == 0)
        {
            return position;
        }

        var occupiedPositions = new HashSet<Vector3>(objects.Select(x => x.transform.position));
        
        int heightLevel = 0;
        Vector3 checkPosition;
        
        do
        {
            checkPosition = new Vector3(gridPos.x, heightLevel++, gridPos.z);
        }
        while (occupiedPositions.Contains(checkPosition));
        
        return checkPosition;
    }

    private Vector3Int ToGridPosition(Vector3 worldPosition, BuildableType type)
    {
        float invGridSize = 1f / _gridSize;
        Vector3Int gridPos = new(
            Mathf.RoundToInt(worldPosition.x * invGridSize),
            Mathf.RoundToInt(worldPosition.y * invGridSize),
            Mathf.RoundToInt(worldPosition.z * invGridSize)
        );

        if (type == BuildableType.Cube)
        {
            gridPos.y = 0; // Кубы только на уровне земли
        }
        return gridPos;
    }

    private List<Buildable> GetOrCreateCell(Vector3Int gridPos)
    {
        if (!_occupiedCells.TryGetValue(gridPos, out var objects))
        {
            objects = new List<Buildable>();
            _occupiedCells[gridPos] = objects;
        }
        return objects;
    }
}