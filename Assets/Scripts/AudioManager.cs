using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public SoundLibrary soundLibrary;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopingSfxSource;

    private Dictionary<AudioClip, AudioSource> activeLoopingSfx = new Dictionary<AudioClip, AudioSource>();

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
        this.loopingSfxSource.volume = volume;
    }

    public void MusicVolume(float volume)
    {
        this.musicSource.volume = volume;
    }

    public void SfxMute(bool mute)
    {
        this.sfxSource.mute = mute;
        this.loopingSfxSource.mute = mute;
    }

    public void MusicMute(bool mute)
    {
        this.musicSource.mute = mute;
    }
    public void PlaySfx(AudioClip audio)
    {
        this.sfxSource.PlayOneShot(audio);
    }

    /// <summary>
    /// //////////////TODO: looping audio
    /// </summary>
    /// <param name="audio"></param>
    public void PlayLoopingSfx(AudioClip audio)
    {
        if (audio == null) return;

        // Verhindern, dass der Clip mehrfach abgespielt wird
        if (activeLoopingSfx.ContainsKey(audio)) return;

        // Neue AudioSource erzeugen
        AudioSource newLoopSource = gameObject.AddComponent<AudioSource>();
        newLoopSource.clip = audio;
        newLoopSource.loop = true;
        newLoopSource.volume = sfxSource.volume;
        newLoopSource.mute = sfxSource.mute;
        newLoopSource.Play();

        activeLoopingSfx[audio] = newLoopSource;
    }

    public void StopLoopingSfx(AudioClip audio)
    {
        if (audio == null) return;

        if (activeLoopingSfx.TryGetValue(audio, out AudioSource source))
        {
            source.Stop();
            Destroy(source);
            activeLoopingSfx.Remove(audio);
        }
    }
    /// <summary>
    /// //////////////TODO:  looping audio
    /// </summary>
    /// <param name="audio"></param>

    public void PlayMusic(AudioClip audio)
    {
        // break if trying to init same music twice
        if (this.musicSource.clip == audio) return;
        // else play music
        this.musicSource.clip = audio;
        this.musicSource.loop = true;
        this.musicSource.Play();
    }


}
