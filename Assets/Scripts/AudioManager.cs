using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public SoundLibrary soundLibrary;
    [SerializeField] public AudioSource musicSource; // Testweise auf public gestellt (hoffe ich änder es zurück)
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private GameObject loopingSfxSourceContainer;

    [SerializeField] private GameObject audioSourcePrefab;
    private List<AudioSource> audioSourcePool = new List<AudioSource>();

    private Dictionary<GameObject, AudioSource> activeLoopingSounds = new Dictionary<GameObject, AudioSource>();


    private float musicVolume = 0.3f;
    private float sfxVolume = 0.3f;
    private bool musicMute = false;
    private bool sfxMute = false;

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
        //this.loopingSfxSource.volume = volume;
        this.sfxVolume = volume;
        foreach(AudioSource source in audioSourcePool)
        {
            source.volume = volume;
        }
    }

    public void MusicVolume(float volume)
    {
        this.musicSource.volume = volume;
        this.musicVolume = volume;
    }

    public void SfxMute(bool mute)
    {
        this.sfxSource.mute = mute;
        //this.loopingSfxSource.mute = mute;
        this.sfxMute = mute;

        foreach (AudioSource source in audioSourcePool)
        {
            source.mute = mute;
        }
    }

    public void MusicMute(bool mute)
    {
        this.musicSource.mute = mute;
        this.musicMute = mute;
    }
    public void PlaySfx(AudioClip audio)
    {
        this.sfxSource.PlayOneShot(audio);
    }

    /// <summary>
    /// //////////////TODO: looping audio
    /// </summary>
    /// <param name="audio"></param>
    /// 
    public bool IsLoopingSoundPlaying(GameObject obj, AudioClip clip)
    {
        if (activeLoopingSounds.TryGetValue(obj, out AudioSource source))
        {
            return source != null && source.isPlaying && source.clip == clip;
        }
        return false;
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in audioSourcePool)
        {
            if (!source.isPlaying)
                return source;
        }

        var newSource = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
        audioSourcePool.Add(newSource);
        newSource.transform.parent = loopingSfxSourceContainer.transform;
        return newSource;
    }

    public void PlayLoopingSoundIfNotPlaying(GameObject obj, AudioClip clip)
    {
        if (IsLoopingSoundPlaying(obj, clip))
            return;

        AudioSource source = GetAvailableAudioSource();
        source.clip = clip;
        source.volume = this.sfxVolume;
        source.mute = this.sfxMute;
        source.loop = true;
        //source.transform.position = obj.transform.position; // optional für 3D-Sound
        source.Play();

        activeLoopingSounds[obj] = source;
    }

    public void StopAllLoopingSounds()
    {
        List<GameObject> keys = new List<GameObject>();
        foreach(KeyValuePair<GameObject, AudioSource> kvPair in activeLoopingSounds)
        {
            keys.Add(kvPair.Key);
        }

        foreach(GameObject go in keys)
        {
            StopLoopingSound(go);
        }
    }

    public void StopLoopingSound(GameObject obj)
    {
        if (activeLoopingSounds.TryGetValue(obj, out AudioSource source))
        {
            if (source != null && source.isPlaying)
                source.Stop();
        }

        activeLoopingSounds.Remove(obj);
    }

    public void FadeOutAndStop(GameObject obj, float duration = 0.5f)
    {
        if (activeLoopingSounds.TryGetValue(obj, out AudioSource source))
        {
            if (source != null && source.isPlaying)
            {
                StartCoroutine(FadeOutCoroutine(obj, source, duration));
            }
        }
    }

    private IEnumerator FadeOutCoroutine(GameObject obj, AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            source.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume; // zurücksetzen für spätere Wiederverwendung
        source.clip = null;
        source.loop = false;

        activeLoopingSounds.Remove(obj);
    }
    /*
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
    }*/
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
