using UnityEngine;

public class ScanEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _scanEffect;
    private ParticleSystemStopBehavior _stopBehavior = ParticleSystemStopBehavior.StopEmittingAndClear;

    public void Play(float radius)
    {
        ParticleSystem.ShapeModule shape = _scanEffect.shape;
        shape.radius = radius;

        _scanEffect.Stop(true, _stopBehavior);
        _scanEffect.Play();
    }
}
