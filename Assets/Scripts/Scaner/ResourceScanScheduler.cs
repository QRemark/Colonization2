using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceDetector))]
public class ResourceScanScheduler : MonoBehaviour
{
    [SerializeField] private float _scanInterval = 5f;

    public event Action<List<Resource>> ResourcesUpdated;

    private ResourceDetector _detector;
    private WaitForSeconds _waitDelay;

    private void Awake()
    {
        _detector = GetComponent<ResourceDetector>();
        _waitDelay = new WaitForSeconds(_scanInterval);
    }

    private void Start()
    {
        StartCoroutine(ScanRoutine());
    }

    private IEnumerator ScanRoutine()
    {
        while (enabled)
        {
            yield return _waitDelay;
            PerformScan();
        }
    }

    private void PerformScan()
    {
        List<Resource> scannedResources = _detector.DetectNearby();
        ResourcesUpdated?.Invoke(scannedResources);
    }
}
