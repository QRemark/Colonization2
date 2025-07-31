using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GlobalUnitHandler : MonoBehaviour
{
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private int _maxUnitsPerBase = 10;

    private List<Unit> _allUnits;

    private void Awake()
    {
        _allUnits = new List<Unit>();
    }

    public bool TrySpawnUnitForBase(Vector3 position, Base ownerBase)
    {
        int unitCount = _allUnits.Count(u => u.GetAssignedBase() == ownerBase);
        if (unitCount >= _maxUnitsPerBase)
            return false;

        Unit unit = _unitSpawner.SpawnUnit(position);
        if (unit == null)
            return false;

        _allUnits.Add(unit);
        unit.AssignToBase(ownerBase);
        ownerBase.OnUnitSpawned(unit);
        return true;
    }

    public Unit SpawnUnitForBase(Vector3 position, Base ownerBase)
    {
        Unit unit = _unitSpawner.SpawnUnit(position);

        if (unit != null)
        {
            _allUnits.Add(unit);
            unit.AssignToBase(ownerBase);
            ownerBase.OnUnitSpawned(unit);
        }

        return unit;
    }

    public void TransferUnitToBase(Unit unit, Base newBase, Vector3 newBasePosition)
    {
        unit.AssignToBase(newBase);
        unit.Initialize(newBasePosition);
    }

    public List<Unit> GetAllUnits()
    {
        return _allUnits; 
    }

    public int GetUnitCountForBase(Base baseRef)
    {
        return _allUnits.Count(u => u.GetAssignedBase() == baseRef);
    }
}
