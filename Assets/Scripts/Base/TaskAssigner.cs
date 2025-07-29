using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskAssigner : MonoBehaviour
{
    private Dictionary<Resource, Unit> _activeTasks;
    private ResourceStorage _resourceStorage;

    public void Init(Dictionary<Resource, Unit> activeTasks, ResourceStorage resourceStorage)
    {
        _activeTasks = activeTasks;
        _resourceStorage = resourceStorage;
    }

    public void AssignTasks(IEnumerable<Unit> units, List<Resource> availableResources)
    {
        foreach (Unit unit in units)
        {
            if (unit.IsBusy == false && unit.ReadyForNewTask)
            {
                TryAssignTaskToUnit(unit, availableResources);
            }
        }
    }

    private void TryAssignTaskToUnit(Unit unit, List<Resource> availableResources)
    {
        Resource closest = availableResources
            .Where(resource => resource != null && _activeTasks.ContainsKey(resource) == false)
            .OrderBy(resource => Vector3.Distance(unit.transform.position, resource.transform.position))
            .FirstOrDefault();

        if (closest == null)
            return;

        bool accepted = unit.SetTarget(closest);

        if (accepted)
        {
            _activeTasks[closest] = unit;
            _resourceStorage.TryReserveResource(closest);
        }
    }
}
