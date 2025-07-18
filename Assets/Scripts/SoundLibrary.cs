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
    [SerializeField] public AudioClip sfxUhraniumSaved;
    [SerializeField] public AudioClip sfxUhraniumSavable;

    [Space(10)]

    [Header("Soundeffects Game")]
    [SerializeField] public AudioClip sfxLevelFinished;
    [SerializeField] public AudioClip sfxLevelStarted;
    [SerializeField] public AudioClip sfxHighscoreBroken;
    [SerializeField] public AudioClip sfxCountdown;

    [Space(10)]

    [Header("Soundeffects EnemieDrops")]
    [SerializeField] public AudioClip sfxZeitsandDropped;
    [SerializeField] public AudioClip sfxZeitsandCollected;

    [Space(10)]
    [Header("Soundeffects UI")]
    [SerializeField] public AudioClip sfxTurretPlaced;
    [SerializeField] public AudioClip sfxButtonTapped;
    [SerializeField] public AudioClip sfxTurretPurchased;
    [SerializeField] public AudioClip sfxUIError;
    [SerializeField] public AudioClip sfxUIToggle;

}
