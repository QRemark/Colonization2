using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Base))]
public class BaseFlow : MonoBehaviour
{
    [SerializeField] private ResourceScanScheduler _scanner;
    [SerializeField] private TaskAssigner _assigner;
    [SerializeField] private ResourceCounter _resourceCounter;
    [SerializeField] private BaseResourceUI _resourceUI;

    private UnitSpawner _unitSpawner;
    private ResourceStorage _resourceStorage;

    private Dictionary<Resource, Unit> _activeTasks;
    private List<Unit> _subscribedUnits;
    private List<Resource> _availableResources;

    private Base _base;

    private void Awake()
    {
        _base = GetComponent<Base>();

        _activeTasks = new Dictionary<Resource, Unit>();
        _subscribedUnits = new List<Unit>();
        _availableResources = new List<Resource>();
    }

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
        UnsubscribeAllUnits();

        _availableResources.Clear();
        _activeTasks.Clear();
    }

    private void OnResourcesUpdated(List<Resource> scannedResources)
    {
        CheckUnits();

        _availableResources = FilterAvailableResources(scannedResources);

        List<Unit> ownUnits = _unitSpawner.Units
            .Where(unit => unit.GetAssignedBase() == _base)
            .ToList();

        _assigner.Distribute(ownUnits, _availableResources);
    }

    private void CheckUnits()
    {
        foreach (Unit unit in _unitSpawner.Units)
        {
            if (_subscribedUnits.Contains(unit) == false && unit.GetAssignedBase() == _base)
            {
                unit.ResourceDelivered += OnUnitDelivered;
                _subscribedUnits.Add(unit);
            }
        }
    }

    private void OnUnitDelivered(Unit unit, Resource resource)
    {
        if (unit.GetAssignedBase() != _base)
            return;

        _activeTasks.Remove(resource);
        _resourceStorage.Unregist(resource);
        _resourceCounter.Increment();
        resource.Collect();
    }

    private List<Resource> FilterAvailableResources(List<Resource> scanned)
    {
        return scanned
            .Where(resource => _resourceStorage.AvailableResources.Contains(resource))
            .ToList();
    }

    private void UnsubscribeAllUnits()
    {
        foreach (Unit unit in _subscribedUnits)
            unit.ResourceDelivered -= OnUnitDelivered;

        _subscribedUnits.Clear();
    }
}
