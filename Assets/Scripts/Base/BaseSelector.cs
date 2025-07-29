using UnityEngine;
using UnityEngine.InputSystem;

public class BaseSelector : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _baseLayer;

    private Base _selectedBase;
    private CameraControl _input;

    private void Awake()
    {
        _input = new CameraControl();

    }

    private void OnEnable()
    {
        _input.Base.SelectBase.performed += OnSelectBase;
        _input.Base.PlaceFlag.performed += OnPlaceFlag;
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Base.SelectBase.performed -= OnSelectBase;
        _input.Base.PlaceFlag.performed -= OnPlaceFlag;
        _input.Disable();
    }

    public void OnSelectBase(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _baseLayer))
        {
            if (hit.collider.TryGetComponent(out Base newBase))
            {
                if (_selectedBase != null)
                    _selectedBase.SetExpansionMode(false);

                _selectedBase = newBase;
                _selectedBase.SetExpansionMode(true);
            }
        }
    }

    public void OnPlaceFlag(InputAction.CallbackContext context)
    {
        if (!context.performed || _selectedBase == null) return;

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            _selectedBase.SetFlag(hit.point);
        }
    }

}
