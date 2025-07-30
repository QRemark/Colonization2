using UnityEngine;
using UnityEngine.InputSystem;

public class BaseSelector : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _baseLayer;
    [SerializeField] private GlobalUnitHandler _unitHandler;

    private Base _selectedBase;

    public void OnSelectBase(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _baseLayer))
        {
            if (hit.collider.TryGetComponent(out Base newBase))
            {

                if (_unitHandler.GetUnitCountForBase(newBase) < 2)
                {
                    Debug.Log($"[BaseSelector] Базу {newBase.name} нельзя выбрать: юнитов меньше двух");

                    if (_selectedBase != null)
                    {
                        _selectedBase.SetExpansionMode(false);
                        _selectedBase = null;
                    }

                    return;
                }


                _selectedBase?.SetExpansionMode(false);
                _selectedBase = newBase;
                _selectedBase.SetExpansionMode(true);
            }
        }
    }


    public void OnPlaceFlag(InputAction.CallbackContext context)
    {
        if (!context.performed || _selectedBase == null)
            return;

        if (_unitHandler.GetUnitCountForBase(_selectedBase) < 2)
        {
            Debug.Log("[BaseSelector] Недостаточно юнитов для колонизации. Сброс выделения.");

            _selectedBase.SetExpansionMode(false);
            _selectedBase = null;

            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            _selectedBase.SetFlag(hit.point);
        }
    }

}