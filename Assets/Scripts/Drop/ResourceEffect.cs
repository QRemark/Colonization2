using UnityEngine;

public class ResourceEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _fire;

    public void Play()
    {
        _fire.Clear();
        _fire.Play();
    }

    public void Stop()
    {
        _fire.Stop();
    }
}
