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

    public void Init(UnitSpawner unitSpawner, ResourceStorage resourceStorage)
    {
        _unitSpawner = unitSpawner;
        _resourceStorage = resourceStorage;

        _assigner.Init(_activeTasks, _resourceStorage);
        _resourceUI.Initialize(_resourceCounter);
        _scanner.ResourcesUpdated += OnResourcesUpdated;
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

        _assigner.AssignTasks(_unitSpawner.Units, _availableResources);
    }

    private void CheckUnits()
    {
        foreach (Unit unit in _unitSpawner.Units)
        {
            if (!_subscribedUnits.Contains(unit))
            {
                unit.ResourceDelivered += OnUnitDelivered;
                _subscribedUnits.Add(unit);
            }
        }
    }

    private void OnUnitDelivered(Unit unit, Resource resource)
    {
        _activeTasks.Remove(resource);
        _resourceStorage.UnregisterResource(resource);
        _resourceCounter.Increment();
        resource.Collect();
    }
}
