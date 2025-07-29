using UnityEngine;
using UnityEngine.InputSystem;

public class BaseSelector : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _baseLayer;

    private Base _selectedBase;

    public void OnSelectBase(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _baseLayer))
        {
            if (hit.collider.TryGetComponent(out Base newBase))
            {
                _selectedBase?.SetExpansionMode(false);

                _selectedBase = newBase;
                _selectedBase.SetExpansionMode(true);
            }
        }
    }

    public void OnPlaceFlag(InputAction.CallbackContext context)
    {
        if (!context.performed || _selectedBase == null) return;

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            _selectedBase.SetFlag(hit.point);
        }
    }
}
