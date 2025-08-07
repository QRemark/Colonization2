using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CameraMover), typeof(ZoomCamera), typeof(CameraRotator))]
public class CameraHandler : MonoBehaviour
{
    [SerializeField] private CameraMover _cameraMover;
    [SerializeField] private CameraRotator _cameraRotator;
    [SerializeField] private ZoomCamera _zoomCamera;
    [SerializeField] private Camera _mainCamera;

    private Vector2 _moveInput;
    private float _zoomInput;
    private float _rotateInput;

    private void Awake()
    {
        _zoomCamera.SetCamera(_mainCamera);
    }

    private void Update()
    {
        _cameraMover.ChangePosition(_moveInput);
        _zoomCamera.Zoom(_zoomInput);
        _cameraRotator.Rotate(_rotateInput);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        _zoomInput = context.ReadValue<float>();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        _rotateInput = context.ReadValue<float>();
    }
}
