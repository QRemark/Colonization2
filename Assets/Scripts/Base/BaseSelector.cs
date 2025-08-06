using UnityEngine;
using UnityEngine.InputSystem;

public class BaseSelector : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _baseLayer;
    [SerializeField] private GlobalUnitHandler _unitHandler;
    [SerializeField] private BaseManager _baseManager;
    [SerializeField] private float _minUnitCountForExpansion = 2;
    [SerializeField] private float _minDistanceToPlaceFlag = 50f;

    private Base _selectedBase;
    private float _rayDistance = 1000f;

    public void OnSelectBase(InputAction.CallbackContext context)
    {
        if (context.performed == false) 
            return;

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance, _baseLayer))
        {
            if (hit.collider.TryGetComponent(out Base newBase))
            {
                if (_selectedBase != null)
                {
                    _selectedBase.SetHighlight(false);
                    _selectedBase.SetExpansionMode(false);
                }

                if (_unitHandler.GetCountForBase(newBase) < _minUnitCountForExpansion)
                {
                    _selectedBase = null;
                    return;
                }

                _selectedBase = newBase;
                _selectedBase.SetExpansionMode(true);
                _selectedBase.SetHighlight(true);
            }
        }
    }

    public void OnPlaceFlag(InputAction.CallbackContext context)
    {
        if (context.performed == false || _selectedBase == null)
            return;

        if (_baseManager.IsLimitReached())
        {
            _selectedBase?.SetHighlight(false);
            _selectedBase?.SetExpansionMode(false);
            _selectedBase = null;
            return;
        }

        if (_unitHandler.GetCountForBase(_selectedBase) < _minUnitCountForExpansion)
        {
            _selectedBase.SetExpansionMode(false);
            _selectedBase = null;
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance))
        {
            Vector3 flagPosition = hit.point;

            Collider[] nearBases = Physics.OverlapSphere(flagPosition, _minDistanceToPlaceFlag, _baseLayer);

            foreach (Collider collider in nearBases)
            {
                if (collider.TryGetComponent(out Base otherBase) == false)
                    continue;

                if (_selectedBase != null)
                {
                    _selectedBase.SetHighlight(false);
                    _selectedBase.SetExpansionMode(false);
                    _selectedBase = null;
                }

                return;
            }

            if (_selectedBase.TryGetComponent(out BaseExpansion expansion) && expansion.IsLocked)
            {
                return;
            }

            _selectedBase.SetFlag(flagPosition);

            _selectedBase.SetHighlight(false);
            _selectedBase = null;
        }
    }
}