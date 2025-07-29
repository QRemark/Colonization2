using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private BaseFlow _flow;
    [SerializeField] private UnitExpansion _unitExpansion;
    [SerializeField] private BaseExpansion _baseExpansion;
    [SerializeField] private ResourceCounter _resourceCounter;

    public void Init(UnitSpawner unitSpawner, ResourceStorage resourceStorage, GlobalUnitHandler globalUnitHandler, BaseManager baseManager)
    {
        _flow.Init(unitSpawner, resourceStorage);

        _unitExpansion.Init(this, _resourceCounter, globalUnitHandler);
        _baseExpansion.Init(this, _resourceCounter, globalUnitHandler, baseManager);
    }
}
