using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    private List<Resource> _availableResources;
    private List<Resource> _reservedResources;

    public IReadOnlyList<Resource> AvailableResources => _availableResources;

    private void Awake()
    {
        _availableResources = new List<Resource>();
        _reservedResources = new List<Resource>();
    }

    public void Regist(Resource resource)
    {
        if (_availableResources.Contains(resource) == false && _reservedResources.Contains(resource) == false)
        {
            _availableResources.Add(resource);
        }
    }

    public void Unregist(Resource resource)
    {
        _availableResources.Remove(resource);
        _reservedResources.Remove(resource);
    }

    public bool TryReserve(Resource resource)
    {
        if (_availableResources.Remove(resource))
        {
            _reservedResources.Add(resource);
            return true;
        }

        return false;
    }
}
