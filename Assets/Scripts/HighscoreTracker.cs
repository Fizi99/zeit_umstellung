using UnityEngine;
using TMPro;

public class HighscoreTracker : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private TextMeshProUGUI highscoreText;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        highscoreText.gameObject.SetActive(false);
    }

    void Update()
    {
       /* if (gameManager.gameState == GameState.LEVELPLAYING)
        {
            float savedUhranium = SaveManager.LoadUhranium();
            float allTimeHighscore = SaveManager.LoadUhraniumHighscore();

            if (savedUhranium > allTimeHighscore)
            {
                highscoreText.gameObject.SetActive(true);
                SaveManager.SaveUhraniumHighscore(savedUhranium);
                Debug.Log("[!!!] NEW HIGH SCORE (" + savedUhranium + ")");
            }
        }
        else
        {
            resetTracker();
        }*/
    }

    public void SetHighscoreDisplayVisibility(bool isVisible)
    {
        highscoreText.gameObject.SetActive(isVisible);
    }

    public void resetTracker()
    {
        highscoreText.gameObject.SetActive(false);
    }
}
