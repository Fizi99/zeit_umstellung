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
