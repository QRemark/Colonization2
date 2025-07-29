using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    [SerializeField] private float _zoomSpeed = 500f;
    [SerializeField] private float _minZoom = 50f;
    [SerializeField] private float _maxZoom = 150f;
    
    private Camera _camera;

    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }

    public void Zoom(float input)
    {
        if (input == 0f)
            return;

        float zoomDelta = -input * _zoomSpeed * Time.deltaTime;
        float newSize = _camera.orthographicSize + zoomDelta;
        _camera.orthographicSize = Mathf.Clamp(newSize, _minZoom, _maxZoom);
    }
}
