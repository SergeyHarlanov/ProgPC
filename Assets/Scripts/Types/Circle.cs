using UnityEngine;


public class Circle : BuildableObject
{
    [SerializeField] private float _placementOffset = 0.5f;

    protected override void Start()
    {
        base.Start();
        _type = BuildableType.Circle;
    }

    protected override Vector3 CalculatePosition(RaycastHit hit)
    {
        Vector3 position = hit.point + hit.normal * transform.localScale.z / 2;

        Vector3 positionGrid = BuildingSystem.Instance.SnapToGrid(position);
        
        return positionGrid;
    }

    protected override bool IsPositionValid(RaycastHit hit)
    {
        bool isValidSurface = hit.transform.CompareTag("Wall");

        return isValidSurface;
    }

}