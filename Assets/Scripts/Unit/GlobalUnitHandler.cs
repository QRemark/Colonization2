using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GlobalUnitHandler : MonoBehaviour
{
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private int _maxUnitsPerBase = 10;

    private List<Unit> _allUnits = new();

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
        return true;
    }

    public Unit SpawnUnitForBase(Vector3 position, Base ownerBase)
    {
        Unit unit = _unitSpawner.SpawnUnit(position);

        if (unit != null)
        {
            _allUnits.Add(unit);
            unit.AssignToBase(ownerBase);
        }

        return unit;
    }


    public void ReturnUnit(Unit unit)
    {
        if (_allUnits.Contains(unit))
        {
            _unitSpawner.ReturnToPool(unit);
            _allUnits.Remove(unit);
        }
    }

    public Unit FindFreeUnitFromBase(Base baseRef)
    {
        return _allUnits
            .Where(u => u.GetAssignedBase() == baseRef && u.ReadyForNewTask)
            .FirstOrDefault();
    }

    public int GetTotalUnitCount() => _allUnits.Count;

    public int GetUnitCountForBase(Base baseRef)
    {
        return _allUnits.Count(u => u.GetAssignedBase() == baseRef);
    }
}
