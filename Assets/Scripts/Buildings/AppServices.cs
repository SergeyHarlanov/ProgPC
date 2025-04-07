using UnityEngine;

public static class AppServices
{
    private static InputManager _input;
    private static BuildingGrid _grid;
    private static GameObject _servicesRoot;

    public static InputManager Input 
    {
        get
        {
            if (_input == null)
            {
                _servicesRoot = GameObject.Find("[SERVICES]");
                if (_servicesRoot != null)
                {
                    _input = _servicesRoot.GetComponentInChildren<InputManager>(true);
                    if (_input == null)
                        Debug.LogError("InputManager не найден в [SERVICES]!");
                }
            }
            return _input;
        }
    }

    public static BuildingGrid Grid 
    {
        get
        {
            if (_grid == null)
            {
                _servicesRoot = GameObject.Find("[SERVICES]");
                if (_servicesRoot != null)
                {
                    _grid = _servicesRoot.GetComponentInChildren<BuildingGrid>(true);
                    if (_grid == null)
                        Debug.LogError("BuildingGrid не найден в [SERVICES]!");
                }
            }
            return _grid;
        }
    }
}