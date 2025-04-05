using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Instance { get; private set; }

    [SerializeField] private float _gridSize = 1.0f;
    [SerializeField] private float _placementOffset = 0.1f;
    
    // Словарь для хранения объектов по их позициям на сетке
    private Dictionary<Vector3Int, List<BuildableObject>> _grid = new Dictionary<Vector3Int, List<BuildableObject>>();

    private void Awake()
    {
        // Реализация singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Округляет позицию до ближайшей точки на сетке
    /// </summary>
    public Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(
            Mathf.Round(position.x / _gridSize) * _gridSize,
            Mathf.Round(position.y / _gridSize) * _gridSize,
            Mathf.Round(position.z / _gridSize) * _gridSize
        );
    }

    /// <summary>
    /// Регистрирует объект в системе строительства
    /// </summary>
    public void RegisterObject(BuildableObject obj, Vector3 position)
    {
        Vector3Int gridPos = WorldToGridPosition(position);

        // Для кубов используем специальную логику - они всегда строятся от y=0
        if (obj.Type == BuildableType.Cube)
        {
            gridPos.y = 0;
        }
   
        // Инициализируем список объектов для этой позиции, если его еще нет
        if (!_grid.ContainsKey(gridPos))
        {
            _grid[gridPos] = new List<BuildableObject>();
        }
        
        _grid[gridPos].Add(obj);
        
        Debug.Log($"Added {obj.name} at {gridPos}. Total objects in grid: {_grid.Count}");
    }

    /// <summary>
    /// Удаляет объект из системы строительства
    /// </summary>
    public void UnregisterObject(BuildableObject obj, Vector3 position)
    {
        Vector3Int gridPos = WorldToGridPosition(position);
        
        // Для кубов используем специальную логику - они всегда строятся от y=0
        if (obj.Type == BuildableType.Cube)
        {
            gridPos.y = 0;
        }
        
        if (_grid.TryGetValue(gridPos, out var objects))
        {
            objects.Remove(obj);
        }
        
        Debug.Log($"Removed {obj.name} from {gridPos}. Total objects in grid: {_grid.Count}");
    }

    /// </summary>
    /// Находит свободную позицию для нового объекта заданного типа
    /// </summary>
    public Vector3 FindFreePositionForObject(Vector3 position, BuildableType type)
    {
        Vector3Int gridPos = WorldToGridPosition(position);
        
        // Для кубов используем специальную логику - они всегда строятся от y=0
        if (type == BuildableType.Cube)
        {
            gridPos.y = 0;
        }

        // Если в этой позиции уже есть объекты
        if (_grid.ContainsKey(gridPos) && _grid[gridPos].Count > 0)
        {
            Vector3 freePosition = Vector3.zero;
            int heightLevel = 0;

            // Проверяем все возможные уровни высоты, пока не найдем свободный
            while (true)
            {
                Vector3 checkPosition = new Vector3(gridPos.x, heightLevel, gridPos.z);
                
                // Если позиция занята, переходим на следующий уровень
                if (_grid[gridPos].Any(x => x.transform.position == checkPosition))
                {
                    heightLevel++;
                }
                else
                {
                    freePosition = checkPosition;
                    break;
                }
            }
            
            return freePosition;
        }
        
        // Если в позиции нет объектов, возвращаем исходную позицию
        return position;
    }

    /// <summary>
    /// Конвертирует мировые координаты в позицию на сетке
    /// </summary>
    private Vector3Int WorldToGridPosition(Vector3 worldPosition)
    {
        return new Vector3Int(
            Mathf.RoundToInt(worldPosition.x / _gridSize),
            Mathf.RoundToInt(worldPosition.y / _gridSize),
            Mathf.RoundToInt(worldPosition.z / _gridSize)
        );
    }
}