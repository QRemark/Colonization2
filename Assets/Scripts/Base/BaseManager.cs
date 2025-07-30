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

    public IReadOnlyList<Base> GetAllBases() => _allBases;

    private void Awake()
    {
        _allBases = new List<Base>();
    }


    public void RegisterBase(Base baseRef)
    {
        if (!_allBases.Contains(baseRef))
            _allBases.Add(baseRef);
    }

    private void Start()
    {
        CreateInitialBase(_initialBasePosition.position);
    }

    private void CreateInitialBase(Vector3 position)
    {
        Base baseComponent = _baseSpawner.SpawnBase(position);
        baseComponent.Init(
            _globalUnitHandler.GetComponent<UnitSpawner>(),
            _resourceStorage,
            _globalUnitHandler,
            this
        );

        for (int i = 0; i < _initialUnitCount; i++)
        {
            _globalUnitHandler.SpawnUnitForBase(position, baseComponent);
        }

        RegisterBase(baseComponent);
    }

    public void CreateNewBase(Vector3 position, Unit builder)
    {
        Base newBase = _baseSpawner.SpawnBase(position);
        newBase.Init(
            _globalUnitHandler.GetComponent<UnitSpawner>(),
            _resourceStorage,
            _globalUnitHandler,
            this
        );

        RegisterBase(newBase);
        _globalUnitHandler.TransferUnitToBase(builder, newBase, position);
    }
}
