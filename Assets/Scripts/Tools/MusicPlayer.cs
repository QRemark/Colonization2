using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private float _fadeDuration = 3f; 
    [SerializeField] private float _targetVolume = 1f; 

    private float _defoultVolume = 0f;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _audioSource.clip = _musicClip;
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
        _audioSource.volume = _defoultVolume;
    }

    private void Start()
    {
        _audioSource.Play();
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float progressTime = 0f;

        while (progressTime < _fadeDuration)
        {
            progressTime += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(_defoultVolume, _targetVolume, progressTime / _fadeDuration);
            yield return null;
        }

        _audioSource.volume = _targetVolume;
    }
}
