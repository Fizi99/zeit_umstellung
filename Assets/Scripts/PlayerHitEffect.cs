using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHitEffect : MonoBehaviour
{
    [Space(10)]
    [Header("Screen Shake")]
    [Space(10)]
    private bool toggleScreenShake = true;
    public AnimationCurve curve;
    public float duration = 1;

    [Space(10)]
    [Header("Vignette")]
    [Space(10)]
    private bool toggleVignette = true;
    public GameObject vignetteDisplay;
    public float fadeInTime = 0.05f;
    public float fadeOutTime = 0.3f;
    public float maxAlpha = 0.5f;

    public bool isPaused = false;

    public void SetToggleScreenshake(bool b)
    {
        this.toggleScreenShake = b;
    }
    public void SetToggleVignette(bool b)
    {
        this.toggleVignette = b;
    }

    public void TriggerEffect()
    {
        if (this.toggleScreenShake)
        {
            StartCoroutine(Shaking());
        }
        if (this.toggleVignette)
        {
            StartCoroutine(VignetteEffect());
        }
    }

    IEnumerator Shaking()
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            // Warten, bis unpaused
            while (isPaused)
            {
                yield return null;
            }

            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPos + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPos;
    }


    private IEnumerator VignetteEffect()
    {

        Image image = this.vignetteDisplay.GetComponent<Image>();
        Color color = image.color;

        // Fade-In
        float t = 0;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, maxAlpha, t / fadeInTime);
            image.color = color;
            yield return null;
        }

        // Fade-Out
        t = 0;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(maxAlpha, 0, t / fadeOutTime);
            image.color = color;
            yield return null;
        }

        // Final cleanup
        color.a = 0;
        image.color = color;
    }

    public void PauseShake()
    {
        isPaused = true;
    }

    public void ResumeShake()
    {
        isPaused = false;
    }
}
