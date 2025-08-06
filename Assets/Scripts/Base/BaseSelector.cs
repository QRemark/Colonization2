using UnityEngine;
using UnityEngine.InputSystem;

public class BaseSelector : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _baseLayer;
    [SerializeField] private GlobalUnitHandler _unitHandler;
    [SerializeField] private BaseManager _baseManager;

    private Base _selectedBase;

    public void OnSelectBase(InputAction.CallbackContext context)
    {
        if (context.performed == false) 
            return;

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _baseLayer))
        {
            if (hit.collider.TryGetComponent(out Base newBase))
            {
                if (_selectedBase != null)
                {
                    _selectedBase.SetHighlight(false);
                    _selectedBase.SetExpansionMode(false);
                }

                if (_unitHandler.GetCountForBase(newBase) < 2)
                {
                    Debug.Log("������ �������");
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
            Debug.Log("[BaseSelector] ��������� ������ ������������� ��-�� ������.");
            _selectedBase?.SetHighlight(false);
            _selectedBase?.SetExpansionMode(false);
            _selectedBase = null;
            return;
        }

        if (_unitHandler.GetCountForBase(_selectedBase) < 2)
        {
            Debug.Log("[BaseSelector] ������������ ������ ��� �����������. ����� ���������.");

            _selectedBase.SetExpansionMode(false);
            _selectedBase = null;
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            Vector3 flagPosition = hit.point;

            const float minDistance = 50f;
            Collider[] nearBases = Physics.OverlapSphere(flagPosition, minDistance, _baseLayer);

            Debug.Log($"[DEBUG] OverlapSphere hit count: {nearBases.Length}");

            foreach (var col in nearBases)
            {
                Base otherBase = col.GetComponent<Base>();

                if (otherBase == null)
                    continue;

                Debug.Log($"[BaseSelector] ������� ������ � ���� {otherBase.name}. �����.");

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
                Debug.Log("[BaseSelector] ���� �������������, ���� ��� �������������.");
                return;
            }

            _selectedBase.SetFlag(flagPosition);

            _selectedBase.SetHighlight(false);
            _selectedBase = null;
        }
    }
}