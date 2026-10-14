using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    [Header("오디오 소스")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _effectSource;

    private GameEvents _events;

    public void Initialize(GameEvents events)
    {
        Shutdown();
        _events = events;
        _events.SoundRequested += PlayEffect;
    }

    public void Configure(AudioSource musicSource, AudioSource effectSource)
    {
        _musicSource = musicSource;
        _effectSource = effectSource;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (_musicSource == null || clip == null)
        {
            return;
        }

        _musicSource.clip = clip;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void PlayEffect(AudioClip clip)
    {
        if (_effectSource != null && clip != null)
        {
            _effectSource.PlayOneShot(clip);
        }
    }

    public void StopMusic()
    {
        if (_musicSource != null)
        {
            _musicSource.Stop();
        }
    }

    public void Shutdown()
    {
        if (_events != null)
        {
            _events.SoundRequested -= PlayEffect;
            _events = null;
        }

        StopMusic();
    }

    private void OnDestroy()
    {
        Shutdown();
    }
}
