using UnityEngine;
using System;

public class Resource : MonoBehaviour
{
    [SerializeField] private ResourceEffect _resourceEffect;

    public event Action<Resource> OnCollected;

    public void Activate(Vector3 position)
    {
        transform.position = position;
        _resourceEffect?.Play();
    }

    public void MarkCollected()
    {
        _resourceEffect?.Stop();
    }

    public void Collect() 
    {
        _resourceEffect?.Stop();
        OnCollected?.Invoke(this);
    }
}
