using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance;
    public bool IsVibrationEnabled { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayVibration(long durationMs = 100, int amplitude = 255)
    {
        if (!IsVibrationEnabled)
        {
            Debug.Log("Vibration deaktiviert");
            return;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
        {
            AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                "createOneShot", durationMs, amplitude);
            vibrator.Call("vibrate", vibrationEffect);
        }
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#else
        Debug.Log($"[SIMULIERT] Vibration: {durationMs}ms @ Amplitude {amplitude}");
#endif
    }

    public void SetVibrationEnabled(bool enabled)
    {
        IsVibrationEnabled = enabled;
        SaveManager.SaveVibrationEnabled(enabled); // <-- dein SaveManager
    }

    private void LoadSettings()
    {
        IsVibrationEnabled = SaveManager.LoadVibrationEnabled(); // <-- dein SaveManager
    }
}
