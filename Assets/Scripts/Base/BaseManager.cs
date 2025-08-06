using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    [SerializeField] private BaseSpawner _baseSpawner;
    [SerializeField] private Transform _initialBasePosition;
    [SerializeField] private GlobalUnitHandler _globalUnitHandler;
    [SerializeField] private ResourceStorage _resourceStorage;
    [SerializeField] private int _initialUnitCount = 3;

    private  List<Base> _allBases;

    private void Awake()
    {
        _allBases = new List<Base>();
    }

    public void Register(Base baseRef)
    {
        if (_allBases.Contains(baseRef) == false)
            _allBases.Add(baseRef);
    }

    private void Start()
    {
        CreateInitial(_initialBasePosition.position);
    }

    private void CreateInitial(Vector3 position)
    {
        Base baseComponent = _baseSpawner.Create(position);

        if (baseComponent == null)
            return;

        if (_globalUnitHandler.TryGetComponent(out UnitSpawner unitSpawner) == false)
        {
            return;
        }

        baseComponent.Init(unitSpawner, _resourceStorage, _globalUnitHandler, this);

        for (int i = 0; i < _initialUnitCount; i++)
        {
            _globalUnitHandler.SpawnForBase(position, baseComponent);
        }

        Register(baseComponent);
    }

    public bool IsLimitReached()
    {
        return _baseSpawner.IsLimitReached();
    }

    public void CreateNew(Vector3 position, Unit builder)
    {
        Base newBase = _baseSpawner.Create(position);

        if (newBase == null)
            return;

        if (_globalUnitHandler.TryGetComponent(out UnitSpawner unitSpawner) == false)
        {
            return;
        }

        newBase.Init(unitSpawner, _resourceStorage, _globalUnitHandler, this);

        Register(newBase);
        _globalUnitHandler.TransferToBase(builder, newBase, position);
    }
}
