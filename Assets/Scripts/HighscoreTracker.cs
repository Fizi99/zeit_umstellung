using UnityEngine;
using TMPro;

public class HighscoreTracker : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private TextMeshProUGUI highscoreText;

    private float current;
    private float sessionMax = 0f;     
    private float allTimeHighscore;   
    private bool newHighscoreReached = false;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        highscoreText.gameObject.SetActive(false);
    }

    void Update()
    {
        allTimeHighscore = SaveManager.LoadUhraniumHighscore();

        if (gameManager.gameState == GameState.LEVELPLAYING)
        {
            current = SaveManager.LoadUhranium();
            sessionMax = Mathf.Max(current, sessionMax);

            // Prüfen, ob neuer Allzeit-Highscore erreicht wurde
            if (sessionMax > allTimeHighscore)
            {
                // Highscore Text zeigen wenn nicht bereits geschehen
                if (!newHighscoreReached)
                {
                    newHighscoreReached = true;
                    highscoreText.gameObject.SetActive(true);
                }

                // Live Text aktualisieren
                highscoreText.text = "Allzeit-Rekord: " + sessionMax.ToString("0.0");

                // Neuen Highscore als Highscore deklarieren und speichern
                allTimeHighscore = sessionMax;
                SaveManager.SaveUhraniumHighscore(allTimeHighscore);
            }
        }
        else
        {
            resetTracker();
        }
    }

    public void resetTracker()
    {
        newHighscoreReached = false;
        current = 0f;
        sessionMax = 0f;
        allTimeHighscore = 0f;
        highscoreText.gameObject.SetActive(false);
    }
}