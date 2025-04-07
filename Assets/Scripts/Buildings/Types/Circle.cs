using UnityEngine;

public class Circle : Buildable
{
    [SerializeField] private float _placementOffset = 0.5f;

    public override BuildableType Type => BuildableType.Circle;

    protected override void UpdatePlacement()
    {
        Ray ray = AppServices.Input.MouseRay;
        if (Physics.Raycast(ray, out RaycastHit hit, _maxPlacementDistance, _placementLayers))
        {
            Vector3 newPosition = CalculatePosition(hit);
            newPosition = AppServices.Grid.FindFreePositionForObject(newPosition, Type);
            
            bool isFree = AppServices.Grid.IsPositionFree(newPosition, Type);
            bool isValid = isFree && IsPositionValid(hit);

            UpdateVisuals(newPosition, isValid);

            if (AppServices.Input.LeftMouseButtonUp && isValid)
            {
                CompleteBuilding(AppServices.Grid.SnapToGrid(newPosition));
            }
            else if (AppServices.Input.RightMouseButtonDown)
            {
                CancelBuilding();
            }
        }
        else
        {
            transform.position = ray.GetPoint(_maxPlacementDistance * 0.5f);
            _meshRenderer.material.color = _followColor;
        }
    }

    protected override Vector3 CalculatePosition(RaycastHit hit)
    {
        Vector3 offset = hit.normal * (transform.localScale.y * _placementOffset);
        Vector3 position = hit.point + offset;
        return AppServices.Grid.SnapToGrid(position);
    }

    protected override bool IsPositionValid(RaycastHit hit)
    {
        return hit.transform.CompareTag("Wall");
    }
}