

using UnityEngine;

public class BaseExpansion : MonoBehaviour
{
    [SerializeField] private int _resourcesToExpand = 5;
    [SerializeField] private GameObject _flagPrefab;

    private Vector3? _flagPosition;
    private bool _expanding = false;

    private Base _base;
    private ResourceCounter _counter;
    private GlobalUnitHandler _unitHandler;
    private BaseManager _baseManager;

    private GameObject _flagInstance;

    public void Init(Base baseRef, ResourceCounter counter, GlobalUnitHandler unitHandler, BaseManager baseManager)
    {
        _base = baseRef;
        _counter = counter;
        _unitHandler = unitHandler;
        _baseManager = baseManager;
    }

    public void SetFlag(Vector3 position)
    {
        _flagPosition = position;
        _expanding = true;

        _flagInstance = Instantiate(_flagPrefab, position, Quaternion.identity);
    }

    public void ResetFlagState(bool isActive)
    {
        _expanding = isActive;

        if (!isActive)
        {
            _flagPosition = null;

            if (_flagInstance != null)
            {
                Destroy(_flagInstance);
                _flagInstance = null;
            }
        }
    }

    private void Update()
    {
        if (!_expanding || _flagPosition == null)
            return;

        Unit builder = _unitHandler.FindFreeUnitFromBase(_base);
        if (builder == null)
        {
            Debug.Log("[BaseExpansion] No available builder unit found.");
            return;
        }

        if (_counter.Decrement(_resourcesToExpand) == false)
        {
            Debug.Log("[BaseExpansion] Not enough resources to start expansion.");
            return;
        }

        Debug.Log($"[BaseExpansion] Assigned unit {builder.name} to build base at {_flagPosition.Value}");
        builder.StartBaseBuildingTask(_flagPosition.Value);
        builder.OnArrived += BuildNewBase;

        _expanding = false;
        _base.SetExpansionMode(false);
    }

    private void BuildNewBase(Unit unit)
    {
        float distance = Vector3.Distance(unit.transform.position, _flagPosition.Value);
        if (distance > 7f)
        {
            Debug.Log($"[BaseExpansion] Unit {unit.name} is too far from flag to build. Distance: {distance}");
            return;
        }

        Debug.Log($"[BaseExpansion] Unit {unit.name} arrived. Building new base at {_flagPosition.Value}");

        _baseManager.CreateNewBase(_flagPosition.Value, unit);
        unit.OnArrived -= BuildNewBase;

        Destroy(_flagInstance);
        _flagInstance = null;
        _flagPosition = null;
        _base.SetExpansionMode(false);
    }
}
