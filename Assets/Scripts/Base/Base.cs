using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private BaseFlow _flow;
    [SerializeField] private UnitExpansion _unitExpansion;
    [SerializeField] private BaseExpansion _baseExpansion;
    [SerializeField] private ResourceCounter _resourceCounter;

    private bool _isInExpansionMode = false;
    public bool IsInExpansionMode => _isInExpansionMode;

    public void Init(UnitSpawner unitSpawner, ResourceStorage resourceStorage, GlobalUnitHandler globalUnitHandler, BaseManager baseManager)
    {
        _flow.Init(unitSpawner, resourceStorage, this);

        _unitExpansion.Init(this, _resourceCounter, globalUnitHandler);
        _baseExpansion.Init(this, _resourceCounter, globalUnitHandler, baseManager);
    }

    public void SetExpansionMode(bool isExpanding)
    {
        _isInExpansionMode = isExpanding;

        _unitExpansion.enabled = !isExpanding;
    }

    public void SetFlag(Vector3 position)
    {
        _baseExpansion.SetFlag(position);
    }
    public void OnUnitSpawned(Unit unit)
    {
        unit.BecameIdle += HandleUnitBecameIdle;
    }

    private void HandleUnitBecameIdle(Unit unit)
    {
        Debug.Log($"[Base] Пойман BecameIdle от {unit.name}, база {_baseExpansion.name}, isInExpansionMode: {_isInExpansionMode}");

        if (_isInExpansionMode)
        {
            _baseExpansion.OnUnitIdleFromThisBase(unit);
        }
    }
    public void OnResourceCountChanged(int count)
    {
        _baseExpansion.OnResourceCountChanged(count);
    }

}
