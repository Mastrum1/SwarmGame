using UnityEngine;
using Random = UnityEngine.Random;

public class AudioSourceVariationShifter : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private bool _playOnAwake;

    private void Start()
    {
        if (_playOnAwake) Play();
    }

    public void Play()
    {
        Randomize();
        _audioSource.Play();
    }

    public void PlayOneShot(AudioClip _clip)
    {
        Randomize();
        _audioSource.PlayOneShot(_clip);
    }

    private void Randomize()
    {
        _audioSource.volume = Random.Range(0.7f, 1f);
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
    }
}