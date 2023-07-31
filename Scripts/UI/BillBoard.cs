using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField] private Camera _camera = null;

    private void Awake()
    {
        // Make sure camera is not null, this can be replaced with an initializer to remove the need to manually set up camera / set specific camera you want to billboard towards
        if (_camera == null)
        {
            _camera = Camera.current;
        }
    }

    void Update()
    {
        transform.LookAt(_camera.transform);
    }
}
