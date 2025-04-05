using UnityEngine;

public class Cube : BuildableObject
{
    [SerializeField] private float _placementOffset = 0.5f;
    
    protected override void Start()
    {
        base.Start();
        _type = BuildableType.Cube;
    }
    protected override Vector3 CalculatePosition(RaycastHit hit)
    {
        Vector3 position = hit.point + hit.normal * transform.localScale.z / 2 ;

        Vector3 positionGrid = BuildingSystem.Instance.SnapToGrid(position);
        
        return positionGrid;
    }

    protected override bool IsPositionValid(RaycastHit hit)
    {
        bool isValidSurface = hit.transform.CompareTag("Ground");
        
        return isValidSurface;
    }
}