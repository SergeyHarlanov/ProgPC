using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Camera _mainCam;

    public float MouseScrollDelta => Input.GetAxis("Mouse ScrollWheel");
    public bool LeftMouseButtonDown => Input.GetMouseButtonDown(0);
    public bool LeftMouseButtonUp => Input.GetMouseButtonUp(0);
    public bool RightMouseButtonDown => Input.GetMouseButtonDown(1);
    
    public Ray MouseRay 
    {
        get
        {
            if (_mainCam == null) 
                _mainCam = Camera.main;
            return _mainCam.ScreenPointToRay(Input.mousePosition);
        }
    }
}