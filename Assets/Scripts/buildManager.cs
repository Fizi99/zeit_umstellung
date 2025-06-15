using UnityEngine;


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

    public PlaceableZone placeableZone;

    public bool isBuildPossible = false;

    private GameManager gameManager;

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("more than one BuildManager!");
            return;
        }
        instance = this;

    }

    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        // only place turrets, when lvl is in session
        if(this.gameManager.gameState == GameState.LEVELPLAYING)
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
                spawnPosition.z = 0; // Set tower depth

                Collider[] collidersHit = Physics.OverlapSphere(spawnPosition, 0.1f);
                bool turretOverlap = false;
                foreach (Collider collider in collidersHit)
                {
                    if (collider.CompareTag("Turret"))
                    {
                        turretOverlap = true;
                        break;
                    }
                }

                bool enoughResources = turret.GetComponent<TurretAI>().buildingCost <= gameManager.player.zeitsand;

                if (!turretOverlap && enoughResources)
                {
                    gameManager.player.SetZeitsand(
                        gameManager.player.zeitsand - turret.GetComponent<TurretAI>().buildingCost
                    );

                    GameObject newTurret = Instantiate(turret, spawnPosition, Quaternion.identity);
                    newTurret.transform.parent = turretContainer.transform;

                    placeableZone.HidePlaceableZone();
                }
                else
                {
                    Debug.LogWarning("Turret already there or not enough Zeitsand");

                    // Reset build state and hide overlay
                    setIsBuild(false);
                    turretToBuild = null;
                    placeableZone.HidePlaceableZone();
                }
            }
        }
    }

}
