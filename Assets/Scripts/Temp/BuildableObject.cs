using UnityEngine;

public abstract class BuildableObject : MonoBehaviour
{
    public BuildableType Type => _type;

    [Header("Base Settings")]
    [SerializeField] protected Color _colorValid = new Color(0, 1, 0, 0.5f);
    [SerializeField] protected Color _colorInvalid = new Color(1, 0, 0, 0.5f);
    [SerializeField] protected Color _colorFollow = new Color(255, 255, 255, 0.5f);
    [SerializeField] protected LayerMask _placementLayers;
    [SerializeField] protected float _maxPlacementDistance = 5f;

    protected Color _originalColor;
    protected MeshRenderer _renderer;
    protected Collider _collider;
    protected bool _isBuilding;
    protected float _rotationAngle;
    protected Vector3 _originalPosition;
    protected BuildableType _type;

    protected virtual void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
        
        _originalColor = _renderer.material.color;
    }

    protected virtual void Update()
    {
        if (!_isBuilding) return;
        
        HandleMovement();
        HandleRotation();
        HandlePlacement();
    }

    protected virtual void HandleMovement()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      
        if (Physics.Raycast(ray, out RaycastHit hit, _maxPlacementDistance, _placementLayers))
        {
            Vector3 newPosition = CalculatePosition(hit);
            
           
            newPosition = BuildingSystem.Instance.FindFreePositionForObject(newPosition, BuildableType.Cube) * transform.localScale.y;

           if (BuildingSystem.Instance.IsFree(newPosition, _type))
            {
                UpdateAppearance(newPosition, IsPositionValid(hit));
            }
        }
        else
        {
            transform.position = ray.GetPoint(_maxPlacementDistance * 0.5f);
            _renderer.material.color = _colorFollow;
        }
    }

    protected abstract Vector3 CalculatePosition(RaycastHit hit);
    protected abstract bool IsPositionValid(RaycastHit hit);
    
    protected virtual void UpdateAppearance(Vector3 position, bool isValid)
    {
        transform.position = position;
        _renderer.material.color = isValid ? _colorValid : _colorInvalid;
    }

    protected virtual void HandleRotation()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Debug.Log("SCROLL");
            _rotationAngle += Mathf.Sign(scroll) * 45f;
            transform.rotation = Quaternion.Euler(0, _rotationAngle, 0);
        }
    }

    protected virtual void HandlePlacement()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, _maxPlacementDistance, _placementLayers))
        {
            if ( Input.GetMouseButtonUp(0) &&  IsPositionValid(hit))
            {
                PlaceObject(hit);
                return;
            }
        }
        
        if (Input.GetMouseButtonDown(1))
            CancelBuilding();
    }

    protected virtual void PlaceObject(RaycastHit hit)
    {
        _isBuilding = false;
        _renderer.material.color = _originalColor;
        _collider.enabled = true;
        
        Vector3 finalPosition = BuildingSystem.Instance.SnapToGrid(transform.position);
        transform.position = finalPosition;
        BuildingSystem.Instance.RegisterObject(this, finalPosition);
    }

    protected virtual void CancelBuilding()
    {
        _isBuilding = false;
        transform.position = _originalPosition;
        _renderer.material.color = _originalColor;
        _collider.enabled = true;
    }

    private void OnMouseDown()
    {
        if (!_isBuilding)
        {
            StartBuilding();
        }
    }

    protected virtual void StartBuilding()
    {
        _isBuilding = true;
        _originalPosition = transform.position;
        _renderer.material.color = _colorFollow;
        _collider.enabled = false;
        
        BuildingSystem.Instance.UnregisterObject(this, _originalPosition);
    }
}