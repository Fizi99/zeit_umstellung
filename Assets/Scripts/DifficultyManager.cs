using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public Difficulty currentDifficulty = Difficulty.NORMAL;

    [SerializeField] private float zeitSandRateEasy = 3f;
    [SerializeField] private float zeitSandRateNormal = 2f;
    [SerializeField] private float zeitSandRateHard = 1.5f;

    [SerializeField] private float uhraniumRateEasy = 8f;
    [SerializeField] private float uhraniumRateNormal = 10f;
    [SerializeField] private float uhraniumRateHard = 12f;
    // modified variables: -> more with higher difficulty
    private float defaultZeitsandRate;
    private float defaultUhraniumRate;

    private GameManager gameManager;

    private void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    public void ChangeDifficulty(Difficulty newDifficulty)
    {
        this.currentDifficulty = newDifficulty;
        ApplyDifficulty();
    }

    private void ApplyDifficulty()
    {
        float zeitsandRate = 0f;
        float uhraniumRate = 0f;

        switch (this.currentDifficulty)
        {
            case Difficulty.EASY:
                zeitsandRate = this.zeitSandRateEasy;
                uhraniumRate = this.uhraniumRateEasy;
                break;
            case Difficulty.NORMAL:
                zeitsandRate = this.zeitSandRateNormal;
                uhraniumRate = this.uhraniumRateNormal;
                break;
            case Difficulty.HARD:
                zeitsandRate = this.zeitSandRateHard;
                uhraniumRate = this.uhraniumRateHard;
                break;
            default:
                break;
        }

        this.gameManager.player.zeitsandRatePerSec = zeitsandRate;
        this.gameManager.player.uhraniumRatePerSec = uhraniumRate;
        
    }

}
