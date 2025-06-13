using UnityEngine;
using UnityEngine.UI;

public class FillDisplay : MonoBehaviour
{
    private GameManager gameManager;

    public Image targetImage;                // Image-Objekt im Canvas
    public Sprite[] fillSprites;             // Array mit 7 Sprites
    public float score;                        // Testvariable 

    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        // Anzahl der Stufen
        int steps = fillSprites.Length;

        score = this.gameManager.player.zeitsand;

        // Score auf Bereich 0–100 clampen (sicherheitshalber)
        float clampedScore = Mathf.Clamp(score, 0, 100);

        // Index berechnen basierend auf Score
        int spriteIndex = Mathf.FloorToInt((clampedScore / 100f) * (steps - 1));

        // Sprite setzen
        targetImage.sprite = fillSprites[spriteIndex];
    }
}
