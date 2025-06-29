using UnityEngine;

public static class SaveManager
{
    public static void SaveUhraniumHighscore(float score)
    {
        PlayerPrefs.SetFloat("Highscore", score);
        PlayerPrefs.Save();
        //Debug.Log("Current All-Time Highscore: " + PlayerPrefs.GetFloat("Highscore", 0f));
    }

    public static float LoadUhraniumHighscore()
    {
        //Debug.Log("Loaded Highscore: " + PlayerPrefs.GetFloat("Highscore", 0f));
        return PlayerPrefs.GetFloat("Read of Highscore Value gives: ", 0f);
    }
}
