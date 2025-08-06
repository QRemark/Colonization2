using UnityEngine;

public class UnitExpansion : MonoBehaviour
{
    [SerializeField] private int _resourcesPerUnit = 3;

    private Base _base;
    private ResourceCounter _counter;
    private GlobalUnitHandler _globalUnitHandler;

    private void Update()
    {
        HandleExpansion();
    }

    public void Init(Base baseRef, ResourceCounter counter, GlobalUnitHandler unitHandler)
    {
        _base = baseRef;
        _counter = counter;
        _globalUnitHandler = unitHandler;
    }

    private void HandleExpansion()
    {
        if (_base.CanProduceUnits == false)
            return;

        if (_counter.Count < _resourcesPerUnit)
            return;

        Vector3 spawnPos = _base.transform.position;

        if (_globalUnitHandler.TryAddForBase(spawnPos, _base))
        {
            _counter.Decrement(_resourcesPerUnit);
        }
    }
}
