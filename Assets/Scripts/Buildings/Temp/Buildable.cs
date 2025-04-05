using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(Collider))]
public abstract class Buildable : MonoBehaviour
{
    [SerializeField] protected Color _validColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] protected Color _invalidColor = new Color(1, 0, 0, 0.5f);
    [SerializeField] protected Color _followColor = new Color(1, 1, 1, 0.5f);
    [SerializeField] protected LayerMask _placementLayers = ~0;
    [SerializeField] protected float _maxPlacementDistance = 5f;

    protected MeshRenderer _meshRenderer;
    private Collider _objectCollider;
    private Color _originalColor;
    private Vector3 _originalPosition;
    private float _rotationAngle;
    private bool _isBuilding;

    public abstract BuildableType Type { get; }

    protected virtual void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _objectCollider = GetComponent<Collider>();
        _originalColor = _meshRenderer.material.color;
    }

    protected virtual void Update()
    {
        if (!_isBuilding) return;

        UpdatePlacement();
        UpdateRotation();
    }

    private void OnMouseDown()
    {
        if (!_isBuilding)
        {
            StartBuilding();
        }
    }

    private void StartBuilding()
    {
        _isBuilding = true;
        _originalPosition = transform.position;
        SetVisualState(_followColor, false);
        BuildingGrid.Instance.UnregisterObject(this, _originalPosition);
    }

    protected void CompleteBuilding(Vector3 finalPosition)
    {
        _isBuilding = false;
        transform.position = finalPosition;
        SetVisualState(_originalColor, true);
        BuildingGrid.Instance.RegisterObject(this, finalPosition);
    }

    protected void CancelBuilding()
    {
        _isBuilding = false;
        transform.position = _originalPosition;
        SetVisualState(_originalColor, true);
    }
    
    private void UpdateRotation()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            _rotationAngle += Mathf.Sign(scroll) * 45f;
            transform.rotation = Quaternion.Euler(0, _rotationAngle, 0);
        }
    }

    protected void UpdateVisuals(Vector3 position, bool isValid)
    {
        transform.position = position;
        _meshRenderer.material.color = isValid ? _validColor : _invalidColor;
    }

    private void SetVisualState(Color color, bool colliderEnabled)
    {
        _meshRenderer.material.color = color;
        _objectCollider.enabled = colliderEnabled;
    }
    
    protected abstract void UpdatePlacement();

    protected abstract Vector3 CalculatePosition(RaycastHit hit);
    protected abstract bool IsPositionValid(RaycastHit hit);
}