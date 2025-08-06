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

    public bool TryAddForBase(Vector3 position, Base ownerBase)
    {
        int unitCount = _allUnits.Count(unit => unit.GetAssignedBase() == ownerBase);

        if (unitCount >= _maxUnitsPerBase)
            return false;

        Unit unit = _unitSpawner.CreateSingleUnit(position);

        if (unit == null)
            return false;

        _allUnits.Add(unit);
        unit.AssignToBase(ownerBase);
        ownerBase.SubscribeToUnit(unit);

        return true;
    }

    public Unit SpawnForBase(Vector3 position, Base ownerBase)
    {
        Unit unit = _unitSpawner.CreateSingleUnit(position);

        if (unit != null)
        {
            _allUnits.Add(unit);
            unit.AssignToBase(ownerBase);
            ownerBase.SubscribeToUnit(unit);
        }

        return unit;
    }

    public void TransferToBase(Unit unit, Base newBase, Vector3 newBasePosition)
    {
        Base oldBase = unit.GetAssignedBase();

        if (oldBase != null)
            oldBase.UnsubscribeFromUnit(unit);

        unit.AssignToBase(newBase);
        unit.Initialize(newBasePosition);

        newBase.SubscribeToUnit(unit);
    }


    public List<Unit> GetAll()
    {
        return _allUnits; 
    }

    public int GetCountForBase(Base baseRef)
    {
        return _allUnits.Count(unit => unit.GetAssignedBase() == baseRef);
    }
}
