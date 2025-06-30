using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretInfoUI : MonoBehaviour
{
    [Header("UI-Felder")]
    public GameObject nameText;
    public GameObject costText;
    public GameObject statsTextObj;

    public void DisplayFromAI(TurretAI turretAI)
    {
        nameText.GetComponent<TMPro.TextMeshProUGUI>().text = DisplayNames[turretAI.name];
        costText.GetComponent<TMPro.TextMeshProUGUI>().text = "Kosten: " + turretAI.buildingCost;

        statsTextObj.GetComponent<TMPro.TextMeshProUGUI>().text =
            $"Reichweite: {turretAI.range}\nFeuerrate: {turretAI.fireRate}";
    }

    public void DisplayFromData(TurretData data)
    {
        nameText.GetComponent<TMPro.TextMeshProUGUI>().text = data.turretName;
        costText.GetComponent<TMPro.TextMeshProUGUI>().text = "Kosten: " + data.cost;
        statsTextObj.GetComponent<TMPro.TextMeshProUGUI>().text = BuildStatText(data.stats);
    }

    string BuildStatText(List<StatEntry> stats)
    {
        if (stats == null || stats.Count == 0)
            return "";

        string result = "";
        foreach (var entry in stats)
            result += $"{entry.label}: {entry.value}\n";

        return result.TrimEnd('\n');
    }

    public static readonly Dictionary<TurretType, string> DisplayNames = new Dictionary<TurretType, string>
    {
        { TurretType.STANDARD, "Wristwatch Artillery" },
        { TurretType.DRONEBASE, "Holo Smartwatch" },
        { TurretType.DRONE, "Drone" },
        { TurretType.MISSILE, "Digital Rocketlauncher" },
        { TurretType.LASER, "Sundial Laser" },
        { TurretType.FREEZE, "Stopwatch Cryo" },
        { TurretType.DYNAMITE, "Dynamite Alarmclock" }
    };
}
