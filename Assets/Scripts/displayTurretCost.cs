using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class displayTurretCost : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text costText;
    public float cost;
    public GameObject turret;
    public GameObject DragObject;
    public GameObject ChangedDragObject;

    private GameManager gameManager;
    public bool isTowerSelected = false;
    public bool isHovering = false;

    public Sprite ButtonSprite;

    public Texture texture;

    public Button button;
    public buildManager BuildManager;
    public TurretType turretType;
    public GameObject lockImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        
        updateTurretCostText();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void Update()
    {



        // Desaturate button image if not enough zeitsand to buy the tower
        var childImage = gameObject.GetComponentInChildren<RawImage>();

        if (turret.GetComponent<TurretAI>().buildingCost > gameManager.player.zeitsand)
        {
            childImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // "grau getönt" = pseudo schwarz-weiß
        }
        else
        {
            if (!isTowerSelected) childImage.color = Color.white;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer entered button!");
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exited button!");
        isHovering = false;
    }

    void OnButtonClick()
    {
        if(this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            BuildManager.highlightTowerSelected(button);
            BuildManager.SetTurretToBuild(turret);
            BuildManager.SetDragObject(DragObject);

        }

    }

    public void updateTurretCostText(){
    
        costText.text = (turret.GetComponent<TurretAI>().buildingCost).ToString();
    }
}
