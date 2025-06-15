using UnityEngine;
using UnityEngine.UI;

public class buildManager : MonoBehaviour
{

    public static buildManager instance;

    public GameObject turretToBuild;

    public GameObject turretContainer;

    public GameObject turretArtillery;
    public GameObject turretLaser;
    public GameObject turretRocket;
    public GameObject turretFreeze;
    public GameObject turretBomb;
    public GameObject turretDrone;
    public Camera mainCamera;
    private float turretLoadoutEfficiency = 1f;
    private float loadOutSize = 4f;

    public PlaceableZone placeableZone;
    private GameObject lastlySelectedButton;

    public bool isBuildPossible = false;

    private GameManager gameManager;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("more than one BuildManager!");
            return;
        }
        instance = this;

    }

    void Start()
    {
        /*turretLoadoutEfficiency = ((turretArtillery.GetComponent <TurretAI>().getTurretEfficiency()+
            turretLaser.GetComponent<TurretAI>().getTurretEfficiency()+
            turretRocket.GetComponent<TurretAI>().getTurretEfficiency()+
            turretDrone.GetComponent<TurretAI>().getTurretEfficiency() )/ loadOutSize)/4;*/
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public float getLoadoutEfficiency()
    {
        return turretLoadoutEfficiency;
    }

    void Update()
    {
        // only place turrets, when lvl is in session
        if (this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //spawnTurret(turretArtillery);
                //spawnTurret(turretFreeze);
                spawnTurret(turretToBuild);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                //spawnTurret(turretLaser);
                spawnTurret(turretBomb);
            }
            else if (Input.GetMouseButtonDown(2))
            {
                //spawnTurret(turretRocket);
                spawnTurret(turretDrone);
            }
        }


    }

    public void setIsBuild(bool isBuild)
    {
        isBuildPossible = isBuild;
    }

    public void SetTurretToBuild(GameObject turret)
    {
        turretToBuild = turret;
        setIsBuild(true);
        placeableZone.ShowPlaceableZone();
    }

    public void highlightTowerSelected(GameObject button)
    {
        lastlySelectedButton = button;
        var childImage = button.GetComponentInChildren<RawImage>();
        button.GetComponent<displayTurretCost>().isTowerSelected = true;
        childImage.color = new Color(0.6f, 0.8f, 1f, 1f); // leichter blauer Tint
    }

    public void clearHighlight()
    {
        if (lastlySelectedButton != null)
        {
            var image = lastlySelectedButton.GetComponentInChildren<RawImage>();
            if (image != null)
                image.color = Color.white;

            lastlySelectedButton.GetComponent<displayTurretCost>().isTowerSelected = false;
            lastlySelectedButton = null;
        }
    }

    void spawnTurret(GameObject turret)
    {
        if (isBuildPossible)
        {
            setIsBuild(false);

            Vector3 mousePosition = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 spawnPosition = hit.point;
                spawnPosition.z = 0; // Make the tower be in the base depth

                Collider[] collidersHit = Physics.OverlapSphere(spawnPosition, 0.1f);
                bool turretOverlap = false;
                foreach (Collider collider in collidersHit)
                {
                    if (collider.tag == "Turret")
                    {
                        turretOverlap = true;
                    }
                }
                /*if (turret.GetComponent<TurretAI>().calculateBuildingCost)
                {
                    if (!turretOverlap && turret.GetComponent<TurretAI>().buildingCost <= gameManager.player.zeitsand)
                    {
                        //gameManager.player.SetZeitsand(gameManager.player.zeitsand - turret.GetComponent<TurretAI>().getCalculatedBuildingCost());
                        GameObject newTurret = Instantiate(turret, spawnPosition, Quaternion.identity);
                        newTurret.transform.parent = turretContainer.transform;
                        placeableZone.HidePlaceableZone();
                    }
                    else
                    {
                        Debug.LogWarning("Turret already there");
                    }
                }
                else
                {*/
                if (!turretOverlap && turret.GetComponent<TurretAI>().buildingCost <= gameManager.player.zeitsand)
                {
                    gameManager.player.SetZeitsand(gameManager.player.zeitsand - turret.GetComponent<TurretAI>().buildingCost);
                    GameObject newTurret = Instantiate(turret, spawnPosition, Quaternion.identity);
                    newTurret.transform.parent = turretContainer.transform;
                }
                else
                {
                    Debug.LogWarning("Turret already there or not enough sand");
                }
                placeableZone.HidePlaceableZone();
                clearHighlight();
            }
        }
    }
}
