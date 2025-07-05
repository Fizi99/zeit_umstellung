using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Scriptable Objects/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    

    [Header("Music")]
    [SerializeField] public AudioClip musicMainScore;

    [Space(10)]

    [Header("Soundeffects Turrets")]
    [SerializeField] public AudioClip sfxTurretWristwatchArtilleryFire;
    [SerializeField] public AudioClip sfxTurretWristwatchArtilleryHit;

    [SerializeField] public AudioClip sfxTurretSundailLaserFire;
    [SerializeField] public AudioClip sfxTurretSundailLaserHit;

    [SerializeField] public AudioClip sfxTurretDigitalRocketlauncherFire;
    [SerializeField] public AudioClip sfxTurretDigitalRocketlauncherHit;

    [SerializeField] public AudioClip sfxTurretDroneFire;
    [SerializeField] public AudioClip sfxTurretDroneHit;

    [SerializeField] public AudioClip sfxTurretDroneStandFire;

    [SerializeField] public AudioClip sfxTurretDynamiteFire;

    [SerializeField] public AudioClip sfxTurretFreezeFire;

    [Space(10)]

    [Header("Soundeffects Busstop")]
    [SerializeField] public AudioClip sfxBusstopHit;

    [Space(10)]
    [Header("Soundeffects UI")]
    [SerializeField] public AudioClip sfxTurretPlaced;

}
