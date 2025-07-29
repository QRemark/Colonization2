using System.Collections.Generic;
using UnityEngine;

public class ResourceDetector : MonoBehaviour
{
    [SerializeField] private float _scanRadius = 110;
    [SerializeField] private ScanEffect _scanEffect;

    public List<Resource> DetectNearbyResources()
    {
        _scanEffect?.Play(_scanRadius);

        Collider[] colliders = Physics.OverlapSphere(transform.position, _scanRadius);
        List<Resource> scannedResources = new();

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Resource resource))
            {
                scannedResources.Add(resource);
            }
        }

        return scannedResources;
    }
}
