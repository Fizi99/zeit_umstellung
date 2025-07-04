using UnityEngine;

[CreateAssetMenu(fileName = "TutorialDataSObj", menuName = "Scriptable Objects/TutorialDataSObj")]
public class TutorialDataSObj : ScriptableObject
{
    [SerializeField] public string text;
    [SerializeField] public TutorialPart part;
}
