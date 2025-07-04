using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateBackGroundSprite()
    {
        Sprite sprite;
        switch (this.gameManager.epochChooser.currentEpoch)
        {
            case Epoch.PREHISTORIC:
                sprite = sprites[0];
                break;
            case Epoch.PHARAOH:
                sprite = sprites[1];
                break;
            case Epoch.MEDIEVAL:
                sprite = sprites[2];
                break;
            default:
                sprite = sprites[0];
                break;
        }

        this.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
