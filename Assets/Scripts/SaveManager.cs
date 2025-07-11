using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SaveManager
{

    private const string FreshInstallKey = "FreshInstallDone";

    public static bool IsFreshInstall()
    {
        return PlayerPrefs.GetInt(FreshInstallKey, 0) == 0;
    }

    public static void MarkFreshInstallHandled()
    {
        PlayerPrefs.SetInt(FreshInstallKey, 1);
        PlayerPrefs.Save();
    }

    public static void SaveUhraniumHighscore(float score)
    {
        PlayerPrefs.SetFloat("Highscore", score);
        PlayerPrefs.Save();
        //Debug.Log("Current All-Time Highscore: " + PlayerPrefs.GetFloat("Highscore", 0f));
    }

    public static float LoadUhraniumHighscore()
    {
        //Debug.Log("Loaded Highscore: " + PlayerPrefs.GetFloat("Highscore", 0f));
        return PlayerPrefs.GetFloat("Highscore", 0f);
    }

    public static void SaveUhranium(float uhraniumCount)
    {
        PlayerPrefs.SetFloat("Uhranium", uhraniumCount);
        PlayerPrefs.Save();
    }

    public static float LoadUhranium()
    {
        float uhranium = PlayerPrefs.GetFloat("Uhranium", 0f);
        if (uhranium != null)
        {
            return uhranium;
        }
        else
        {
            return 0f;
        }
    }


    public static void SaveFirstTimePlaying(bool firstTime)
    {
        PlayerPrefs.SetInt("FirstPlay", firstTime ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool LoadFirstTimePlaying()
    {
        int firstPlay = PlayerPrefs.GetInt("FirstPlay", 1);
        return (firstPlay == 1);
    }

    public static void SaveToggleTutorialState(bool enabled)
    {
        PlayerPrefs.SetInt("ShowTutorialEveryTime", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool LoadToggleTutorialState()
    {
        return PlayerPrefs.GetInt("ShowTutorialEveryTime", 0) == 1;
    }

    /*
     * Because PlayerPrefs can only handle primitives, we serialize a list of enum values to a string
     * and deserialize it when loading 
     */

    public static void SavePurchasedTurrets(List<TurretType> purchasedTurrets)
    {
        string data = string.Join(",", purchasedTurrets.Select(e => e.ToString()));
        PlayerPrefs.SetString("PurchasedTurrets", data);
        PlayerPrefs.Save();
    }

    public static List<TurretType> LoadPurchasedTurrets()
    {
        string serializedData = PlayerPrefs.GetString("PurchasedTurrets", "");

        if (string.IsNullOrEmpty(serializedData))
            return new List<TurretType>();

        return serializedData
            .Split(',')
            .Select(s => (TurretType)Enum.Parse(typeof(TurretType), s))
            .ToList();
    }

    public static void SaveLoadout(int loadOutIndex, List<TurretType> chosenTurrets)
    {
        if (4 >= loadOutIndex && loadOutIndex > 0)
        {

            string data = string.Join(",", chosenTurrets.Select(e => e.ToString()));
            PlayerPrefs.SetString("Loadout" + loadOutIndex, data);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("loadoutindex out of range");
        }
    }

    public static List<TurretType> LoadLoadout(int loadOutIndex)
    {
        if (4 >= loadOutIndex && loadOutIndex > 0)
        {
            string serializedData = PlayerPrefs.GetString("Loadout" + loadOutIndex, "");

            if (string.IsNullOrEmpty(serializedData))
                return new List<TurretType>() {
                    TurretType.STANDARD,
                    TurretType.MISSILE,
                    TurretType.DRONEBASE,
                    TurretType.LASER };

            return serializedData
                .Split(',')
                .Select(s => (TurretType)Enum.Parse(typeof(TurretType), s))
                .ToList();
        }
        else
        {
            Debug.Log("loadoutindex out of range");
            return new List<TurretType>();
        }
    }

    public static void SaveVibrationEnabled(bool enabled)
    {
        PlayerPrefs.SetInt("VibrationEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool LoadVibrationEnabled()
    {
        return PlayerPrefs.GetInt("VibrationEnabled", 1) == 1;
    }

    public static void SaveToggle(SettingOption key, bool value)
    {
        PlayerPrefs.SetInt(key.ToString(), value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool LoadToggle(SettingOption key, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(key.ToString(), defaultValue ? 1 : 0) == 1;
    }

    public static void SaveVolume(SettingOption key, float value)
    {
        PlayerPrefs.SetFloat(key.ToString(), value);
        PlayerPrefs.Save();
    }

    public static float LoadVolume(SettingOption key, float defaultValue = 0.3f)
    {
        return PlayerPrefs.GetFloat(key.ToString(), defaultValue);
    }
}
