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

    private void Awake()
    {
        _allBases = new List<Base>();
    }

    private void Start()
    {
        CreateInitial(_initialBasePosition.position);
    }

    public bool TryReserveBaseSlot()
    {
        if (GetTotalPlannedBaseCount() >= _baseSpawner.GetMaxBaseCount())
            return false;

        _reservedBaseSlots++;
        return true;
    }

    public void ReleaseReservedSlot()
    {
        _reservedBaseSlots = Mathf.Max(0, _reservedBaseSlots - 1);
    }

    public void ConfirmBaseCreated()
    {
        ReleaseReservedSlot(); 
    }

    public int GetTotalPlannedBaseCount()
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
        return GetTotalPlannedBaseCount() >= _baseSpawner.GetMaxBaseCount();
    }

    public void CreateNew(Vector3 position, Unit builder)
    {
        Base newBase = InitializeBase(position);

        if (newBase == null)
        {
            ReleaseReservedSlot();
            return;
        }

        _globalUnitHandler.TransferToBase(builder, newBase, position);
        ConfirmBaseCreated(); 
    }

    private void CreateInitial(Vector3 position)
    {
        Base initialBase = InitializeBase(position);

        if (initialBase == null)
            return;

        for (int i = 0; i < _initialUnitCount; i++)
        {
            _globalUnitHandler.SpawnForBase(position, initialBase);
        }
    }

    private Base InitializeBase(Vector3 position)
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