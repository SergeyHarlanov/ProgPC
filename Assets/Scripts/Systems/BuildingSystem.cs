using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Instance { get; private set; }

    [SerializeField] private float _gridSize = 1.0f;
    [SerializeField] private float _placementOffset = 0.1f;
    
    private readonly Dictionary<Vector3Int, List<BuildableObject>> _grid = 
        new Dictionary<Vector3Int, List<BuildableObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
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

    public void RegisterObject(BuildableObject obj, Vector3 position)
    {
        Vector3Int gridPos = WorldToGridPosition(position);

        if (obj.Type == BuildableType.Cube)
        {
            gridPos.y = 0;
        }

        if (!_grid.TryGetValue(gridPos, out var objectsAtPosition))
        {
            objectsAtPosition = new List<BuildableObject>(1);
            _grid[gridPos] = objectsAtPosition;
        }
        
        objectsAtPosition.Add(obj);
    }

    public void UnregisterObject(BuildableObject obj, Vector3 position)
    {
        Vector3Int gridPos = WorldToGridPosition(position);
        
        if (obj.Type == BuildableType.Cube)
        {
            gridPos.y = 0;
        }
        
        if (_grid.TryGetValue(gridPos, out var objects))
        {
            objects.Remove(obj);
            
            if (objects.Count == 0)
            {
                _grid.Remove(gridPos);
            }
        }
    }

    public bool IsFree(Vector3 position, BuildableType type)
    {
        if (type == BuildableType.Circle)
        {
            Vector3Int gridPos = WorldToGridPosition(position);
            return  !_grid.ContainsKey(gridPos);
        }
        else
        {
            return true;
        }

    }
    
    public Vector3 FindFreePositionForObject(Vector3 position, BuildableType type)
    {
        Vector3Int gridPos = WorldToGridPosition(position);
        
        if (type == BuildableType.Cube)
        {
            gridPos.y = 0;
        }

        if (!_grid.TryGetValue(gridPos, out var objects) || objects.Count == 0)
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

    private Vector3Int WorldToGridPosition(Vector3 worldPosition)
    {
        float invGridSize = 1f / _gridSize;
        return new Vector3Int(
            Mathf.RoundToInt(worldPosition.x * invGridSize),
            Mathf.RoundToInt(worldPosition.y * invGridSize),
            Mathf.RoundToInt(worldPosition.z * invGridSize)
        );
    }
}