using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private float _fadeDuration = 3f; 
    [SerializeField] private float _targetVolume = 1f; 

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _audioSource.clip = _musicClip;
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
        _audioSource.volume = 0f;
    }

    private void Start()
    {
        _audioSource.Play();
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(0f, _targetVolume, elapsed / _fadeDuration);
            yield return null;
        }

        _audioSource.volume = _targetVolume;
    }
}
