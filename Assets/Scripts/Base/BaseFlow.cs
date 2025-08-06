using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseFlow : MonoBehaviour
{
    [SerializeField] private ResourceScanScheduler _scanner;
    [SerializeField] private TaskAssigner _assigner;
    [SerializeField] private ResourceCounter _resourceCounter;
    [SerializeField] private BaseResourceUI _resourceUI;

    private UnitSpawner _unitSpawner;
    private ResourceStorage _resourceStorage;

    private readonly Dictionary<Resource, Unit> _activeTasks = new();
    private readonly List<Unit> _subscribedUnits = new();
    private List<Resource> _availableResources = new();

    private Base _base;

    public void Init(UnitSpawner unitSpawner, ResourceStorage resourceStorage, Base baseRef)
    {
        _unitSpawner = unitSpawner;
        _resourceStorage = resourceStorage;
        _base = baseRef;

        _assigner.Init(_activeTasks, _resourceStorage);
        _resourceUI.Initialize(_resourceCounter);
        _scanner.ResourcesUpdated += OnResourcesUpdated;
        _resourceCounter.CountChanged += _base.OnResourceCountChanged;
    }

    private void OnDestroy()
    {
        _scanner.ResourcesUpdated -= OnResourcesUpdated;

        foreach (Unit unit in _subscribedUnits)
        {
            unit.ResourceDelivered -= OnUnitDelivered;
        }

        _subscribedUnits.Clear();
        _availableResources.Clear();
        _activeTasks.Clear();
    }

    private void OnResourcesUpdated(List<Resource> scannedResources)
    {
        CheckUnits();

        _availableResources = scannedResources
            .Where(resource => _resourceStorage.AvailableResources.Contains(resource))
            .ToList();

        var ownUnits = _unitSpawner.Units
            .Where(unit => unit.GetAssignedBase() == _base)
            .ToList();

        _assigner.Distribute(ownUnits, _availableResources);
    }

    private void CheckUnits()
    {
        foreach (Unit unit in _unitSpawner.Units)
        {
            if (_subscribedUnits.Contains(unit) == false && unit.GetAssignedBase() == this.GetComponent<Base>())
            {
                unit.ResourceDelivered += OnUnitDelivered;
                _subscribedUnits.Add(unit);
            }
        }
    }

    private void OnUnitDelivered(Unit unit, Resource resource)
    {
        if (unit.GetAssignedBase() != this.GetComponent<Base>())
            return; 

        _activeTasks.Remove(resource);
        _resourceStorage.Unregist(resource);
        _resourceCounter.Increment();
        resource.Collect();
    }
}
