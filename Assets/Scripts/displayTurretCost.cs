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
    public RawImage turretImage;
    public GameObject UhraniumImage;
    public GameObject UhraniumAmount;

    private Texture2D originalTex;
    private Texture2D blackTex;
    public Image zeitsymbolImage;
    public GameObject ZeitsandAmount;

    void Start()
    {
        
        updateTurretCostText();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }

        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            this.UhraniumAmount.SetActive(false);
            this.UhraniumImage.SetActive(false);
            this.zeitsymbolImage.gameObject.SetActive(true);
            this.ZeitsandAmount.SetActive(true);
        }
        else
        {
            this.UhraniumAmount.SetActive(true);
            this.UhraniumImage.SetActive(true);
            this.zeitsymbolImage.gameObject.SetActive(false);
            this.ZeitsandAmount.SetActive(false);

        }
            
        if (turretImage == null)
            turretImage = GetComponent<RawImage>();
    }

    private void Update()
    {
        var childImage = gameObject.GetComponentInChildren<RawImage>();

        if (turret.GetComponent<TurretAI>().buildingCost > gameManager.player.zeitsand)
        {
            childImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // grey tint if not enough zeitsand
        }
        else
        {
            if (!isTowerSelected) childImage.color = Color.white;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    void OnButtonClick()
    {
        if (this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            BuildManager.highlightTowerSelected(button);
            BuildManager.SetTurretToBuild(turret);
            BuildManager.SetDragObject(DragObject);
        }
    }

    public void updateTurretCostText()
    {
        costText.text = (turret.GetComponent<TurretAI>().buildingCost).ToString();
    }

    //assign and prepare the original texture
    public void AssignOriginalTexture(Texture tex)
    {
        if (tex == null)
        {
            Debug.LogError("Assigned texture is null!");
            return;
        }

        originalTex = tex as Texture2D;

        if (originalTex == null)
        {
            Debug.LogError("Assigned texture is not Texture2D! Please enable Read/Write on it.");
            return;
        }


        blackTex = new Texture2D(originalTex.width, originalTex.height, TextureFormat.RGBA32, false, true);
        blackTex.filterMode = FilterMode.Point;
        Color[] pixels = originalTex.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            float alpha = pixels[i].a;
            pixels[i] = new Color(0, 0, 0, alpha);
        }
        blackTex.SetPixels(pixels);
        blackTex.Apply();
    }

    public void showPosession(bool isPosessed)
    {
        if (turretImage == null)
            turretImage = GetComponent<RawImage>();

        if (isPosessed)
        {
            turretImage.texture = originalTex;
            this.UhraniumAmount.GetComponent<TMPro.TextMeshProUGUI>().color = this.ZeitsandAmount.GetComponent<TMPro.TextMeshProUGUI>().color;
            this.UhraniumImage.GetComponent<Image>().sprite = this.zeitsymbolImage.GetComponent<Image>().sprite;
            this.UhraniumAmount.GetComponent<TMPro.TextMeshProUGUI>().text = turret.GetComponent<TurretAI>().buildingCost.ToString();

        }
        else
        {
            turretImage.texture = blackTex;

            string uhraniumPrice = turret.GetComponent<TurretAI>().uhraniumPrice.ToString();
            if (uhraniumPrice.Length >= 4)
            {
                uhraniumPrice = (turret.GetComponent<TurretAI>().uhraniumPrice / 1000).ToString() + "k";
            }
            UhraniumAmount.GetComponent<TMPro.TextMeshProUGUI>().text = uhraniumPrice;
        }

        lockImage.SetActive(!isPosessed);
    }
}
