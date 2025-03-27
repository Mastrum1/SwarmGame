using System.Collections;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource MainMusicSource { get => _mainMusicSource; private set => _mainMusicSource = value; }
    public AudioSource SfxSource { get => _sfxSource; private set => _sfxSource = value; }

    [SerializeField] private AudioSource _mainMusicSource;
    [SerializeField] private AudioSource _sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
	    transform.SetParent(null);
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void PlayClip(AudioClip clip)
    {
        SfxSource.PlayOneShot(clip);
    }

    public void FadeAudioSourceVolume(AudioSource source, float time, float volume)
    {
        StartCoroutine(StartFade(source, time, volume));
    }

    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}
