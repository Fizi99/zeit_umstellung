using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTurretData", menuName = "Game/TurretData")]
public class TurretData : ScriptableObject
{
    public TurretType turretType;
    public string turretName;
    public string description;
    public Sprite turretIcon;
    public Texture2D turretIconFramed;
    public Texture2D turretIconTexture;
    public int cost;
    public List<StatEntry> stats = new List<StatEntry>();
}

[System.Serializable]
public class StatEntry
{
    public string label;
    public string value;
}
