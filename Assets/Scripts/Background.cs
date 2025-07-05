using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [Header("Base Sprites")]
    [SerializeField] private Sprite prehistoricBaseSprite;
    [SerializeField] private Sprite pharaohBaseSprite;
    [SerializeField] private Sprite medievalBaseSprite;

    [Header("Variation Sprites")]
    [SerializeField] private List<Sprite> prehistoricVariations;
    [SerializeField] private List<Sprite> pharaohVariations;
    [SerializeField] private List<Sprite> medievalVariations;

    [SerializeField] private int variationChance = 10;

    private GameManager gameManager;

    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        
    }

    public void UpdateBackGroundSprite()
    {
        Epoch current = gameManager.epochChooser.currentEpoch;
        Sprite baseSprite = null;
        List<Sprite> variations = null;

        switch (current)
        {
            case Epoch.PREHISTORIC:
                baseSprite = prehistoricBaseSprite;
                variations = prehistoricVariations;
                break;
            case Epoch.PHARAOH:
                baseSprite = pharaohBaseSprite;
                variations = pharaohVariations;
                break;
            case Epoch.MEDIEVAL:
                baseSprite = medievalBaseSprite;
                variations = medievalVariations;
                break;
        }

        Sprite chosenSprite = baseSprite;

        // Mit Chance 1/variationChance eine Variation auswählen
        bool useVariation = Random.Range(0, variationChance) == 0;

        if (variations != null && variations.Count > 0 && useVariation)
        {
            int randIndex = Random.Range(0, variations.Count);
            chosenSprite = variations[randIndex];
        }

        this.GetComponent<SpriteRenderer>().sprite = chosenSprite;
    }
}
