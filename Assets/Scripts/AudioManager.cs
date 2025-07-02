using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public SoundLibrary soundLibrary;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    public void Start()
    {
        SfxVolume(0.3f);
        MusicVolume(0.3f);
        PlayMusic(this.soundLibrary.musicMainScore);
    }


    // Audio Controls
    public void SfxVolume(float volume)
    {
        this.sfxSource.volume = volume;
    }

    public void MusicVolume(float volume)
    {
        this.musicSource.volume = volume;
    }

    public void SfxMute(bool mute)
    {
        this.sfxSource.mute = mute;
    }

    public void MusicMute(bool mute)
    {
        this.musicSource.mute = mute;
    }
    public void PlaySfx(AudioClip audio)
    {
        this.sfxSource.PlayOneShot(audio);
    }

    public void PlayMusic(AudioClip audio)
    {
        if (this.musicSource.clip == audio) return;
        this.musicSource.clip = audio;
        this.musicSource.loop = true;
        this.musicSource.Play();
    }


}
