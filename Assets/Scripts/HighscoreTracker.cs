using UnityEngine;
using TMPro;
using System.Collections;

public class HighscoreTracker : MonoBehaviour
{
    private GameManager gameManager;
    private AudioManager audioManager;

    [SerializeField] private TextMeshProUGUI highscoreText;

    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseScaleMax = 1.1f;

    private Coroutine pulseCoroutine;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        highscoreText.gameObject.SetActive(false);
    }

    void Update()
    {
    }

    public void SetHighscoreDisplayVisibility(bool isVisible)
    {
        highscoreText.gameObject.SetActive(isVisible);
        if (isVisible)
        {
            // Starte Pulsieren nur, wenn sichtbar
            if (pulseCoroutine == null)
            {
                pulseCoroutine = StartCoroutine(PulseAnimation());
                this.audioManager.PlaySfx(this.audioManager.soundLibrary.sfxHighscoreBroken);
            }
               

        }
        else
        {
            // Stoppe Pulsieren und skaliere zurück
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
                highscoreText.transform.localScale = Vector3.one;
            }
        }
    }

    public void resetTracker()
    {
        highscoreText.gameObject.SetActive(false);
    }

    private IEnumerator PulseAnimation()
    {
        while (true)
        {
            float t = 0f;
            while (t < 1f)
            {
                float scale = Mathf.Lerp(1f, pulseScaleMax, Mathf.Sin(t * Mathf.PI));
                highscoreText.transform.localScale = Vector3.one * scale;
                t += Time.deltaTime * pulseSpeed;
                yield return null;
            }
        }
    }
}
