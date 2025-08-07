using UnityEngine;
using UnityEngine.InputSystem;

public class BaseSelector : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _baseLayer;
    [SerializeField] private GlobalUnitHandler _unitHandler;
    [SerializeField] private GlobalBaseHandler _baseHandler;
    [SerializeField] private float _minUnitCountForExpansion = 2;
    [SerializeField] private float _minDistanceToPlaceFlag = 50f;

    private Base _selectedBase;
    private float _rayDistance = 1000f;

    public void OnSelectBase(InputAction.CallbackContext context)
    {
        if (context.performed == false)
            return;

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance, _baseLayer) &&
            hit.collider.TryGetComponent(out Base newBase))
        {
            DeselectBase();

            if (HasEnoughUnits(newBase) == false)
                return;

            _selectedBase = newBase;
            _selectedBase.SetExpansionMode(true);
            _selectedBase.SetHighlight(true);
        }
    }

    public void OnPlaceFlag(InputAction.CallbackContext context)
    {
        if (context.performed == false || _selectedBase == null)
            return;

        if (_baseHandler.IsLimitReached())
        {
            DeselectBase();
            return;
        }

        if (HasEnoughUnits(_selectedBase) == false)
        {
            DeselectBase();
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance))
        {
            Vector3 flagPosition = hit.point;

            if (IsTooCloseToOtherBases(flagPosition))
            {
                DeselectBase();
                return;
            }

            if (_selectedBase.TryGetComponent(out BaseExpansion expansion) && expansion.IsLocked)
                return;

            _selectedBase.SetFlag(flagPosition);
            _selectedBase.SetHighlight(false);
            _selectedBase = null;
        }
    }

    private void DeselectBase()
    {
        if (_selectedBase != null)
        {
            _selectedBase.SetHighlight(false);
            _selectedBase.SetExpansionMode(false);
            _selectedBase = null;
        }
    }

    private bool HasEnoughUnits(Base baseRef)
    {
        return _unitHandler.GetCountForBase(baseRef) >= _minUnitCountForExpansion;
    }

    private bool IsTooCloseToOtherBases(Vector3 position)
    {
        Collider[] nearBases = Physics.OverlapSphere(position, _minDistanceToPlaceFlag, _baseLayer);

        foreach (Collider collider in nearBases)
        {
            if (collider.TryGetComponent(out Base otherBase))
                return true;
        }

        return false;
    }
}