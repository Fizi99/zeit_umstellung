using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Scriptable Objects/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    [SerializeField] public AudioClip musicMainScore;
    [SerializeField] public AudioClip sfxTurretWristwatchArtilleryFire;
    
}
