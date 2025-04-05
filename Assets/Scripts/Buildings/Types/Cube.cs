using UnityEngine;

public class Cube : Buildable
{
    [SerializeField] private float _placementOffset = 0.5f;

    public override BuildableType Type => BuildableType.Cube;

    protected override void UpdatePlacement()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, _maxPlacementDistance, _placementLayers))
        {
            Vector3 newPosition = CalculatePosition(hit);
            newPosition = BuildingGrid.Instance.FindFreePositionForObject(newPosition, Type);

            bool isValid = IsPositionValid(hit);
            UpdateVisuals(newPosition, isValid);

            if (Input.GetMouseButtonUp(0) && isValid)
            {
                CompleteBuilding(BuildingGrid.Instance.SnapToGrid(newPosition));
            }
            else if (Input.GetMouseButtonDown(1))
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
        return BuildingGrid.Instance.SnapToGrid(position);
    }

    protected override bool IsPositionValid(RaycastHit hit)
    {
        return hit.transform.CompareTag("Ground");
    }
}