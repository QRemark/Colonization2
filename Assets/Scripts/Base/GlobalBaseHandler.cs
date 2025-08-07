using System.Collections.Generic;
using UnityEngine;

public class GlobalBaseHandler : MonoBehaviour
{
    [SerializeField] private BaseSpawner _baseSpawner;
    [SerializeField] private Transform _initialBasePosition;
    [SerializeField] private GlobalUnitHandler _globalUnitHandler;
    [SerializeField] private ResourceStorage _resourceStorage;
    [SerializeField] private int _initialUnitCount = 3;

    private List<Base> _allBases;

    private int _reservedBaseSlots = 0;
    private int _defoultReservedSlots = 0;

    private void Awake()
    {
        _allBases = new List<Base>();
    }

    private void Start()
    {
        CreateInitial(_initialBasePosition.position);
    }

    public bool TryReserveSlot()
    {
        if (GetTotalPlannedCount() >= _baseSpawner.GetMaxBaseCount())
            return false;

        _reservedBaseSlots++;
        return true;
    }

    public void ReleaseReservedSlot()
    {
        _reservedBaseSlots = Mathf.Max(_defoultReservedSlots, _reservedBaseSlots - 1);
    }

    public void ConfirmCreated()
    {
        ReleaseReservedSlot(); 
    }

    public int GetTotalPlannedCount()
    {
        return _allBases.Count + _reservedBaseSlots;
    }

    public void Register(Base baseRef)
    {
        if (_allBases.Contains(baseRef) == false)
            _allBases.Add(baseRef);
    }

    public bool IsLimitReached()
    {
        return GetTotalPlannedCount() >= _baseSpawner.GetMaxBaseCount();
    }

    public void Create(Vector3 position, Unit builder)
    {
        Base newBase = Initialize(position);

        if (newBase == null)
        {
            ReleaseReservedSlot();
            return;
        }

        _globalUnitHandler.TransferToBase(builder, newBase, position);
        ConfirmCreated(); 
    }

    private void CreateInitial(Vector3 position)
    {
        Base initialBase = Initialize(position);

        if (initialBase == null)
            return;

        for (int i = 0; i < _initialUnitCount; i++)
        {
            _globalUnitHandler.SpawnForBase(position, initialBase);
        }
    }

    private Base Initialize(Vector3 position)
    {
        Base baseComponent = _baseSpawner.Create(position);

        if (baseComponent == null)
            return null;

        if (_globalUnitHandler.TryGetComponent(out UnitSpawner unitSpawner) == false)
            return null;

        baseComponent.Init(unitSpawner, _resourceStorage, _globalUnitHandler, this);
        Register(baseComponent);

        return baseComponent;
    }
}