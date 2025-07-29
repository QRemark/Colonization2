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
        _flow.Init(unitSpawner, resourceStorage);

        _unitExpansion.Init(this, _resourceCounter, globalUnitHandler);
        _baseExpansion.Init(this, _resourceCounter, globalUnitHandler, baseManager);
    }

    public void SetExpansionMode(bool isExpanding)
    {
        _isInExpansionMode = isExpanding;

        _unitExpansion.enabled = !isExpanding;
        _baseExpansion.SetColonizationState(isExpanding);
    }

    public void SetFlag(Vector3 position)
    {
        _baseExpansion.SetFlag(position);
    }
}
