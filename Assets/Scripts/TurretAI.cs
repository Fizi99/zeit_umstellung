using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurretAI : MonoBehaviour
{
    private GameManager gameManager;
    private Transform target;
    public TurretType name;
    public float range = 3f;
    public string enemyTag = "Enemy";
    public bool isSingleUse = false;
    public bool useTimeInsteadOfAmmo = false;
    public float initUseAmount = 1;
    public float useAmount = 1;
    public float buildingCost = 1f;
    public Image useBar;
    public bool calculateBuildingCost = false;    

    public bool isMoving = false;
    public float speed = 30f;
    public float stopAndShootRange = 5f;
    private float turretEfficiency = 0f;
    private bool isVisible = true;
    private bool invokedRepeating = false;


    public float fireRate = 15f;
    private float fireCountdown = 0f;

    public GameObject bulletPrefab;
    //bullet spawn position
    public Vector3 firePoint = new Vector3(0, 0, 0);
    public Vector3 firePointRight = new Vector3(0, 0, 0);
    public Vector3 firePointLeft = new Vector3(0, 0, 0);
    public Vector3 firePointUp = new Vector3(0, 0, 0);
    public Vector3 firePointDown = new Vector3(0, 0, 0);

    private float currentLookAngle;

    [Tooltip("Sprite for East (0), North (1), West (2), South (3)")]
    [SerializeField] private Sprite[] towerSprites = new Sprite[4]; // 0=Rechts, 1=Oben, 2=Links, 3=Unten
    private SpriteRenderer spriteRenderer;

    // Blob Shadow related
    public GameObject blobShadowPrefab;
    public Vector3 blobShadowOffset = new Vector3(0f, -0.1f, 0f);
    public Vector3 blobShadowScale = new Vector3(1f, 1f, 1f);
    public Vector3 blobShadowRotation = Vector3.zero;

    private float explosionMult = 1f;

    private float threshold = 3f;  // Only start toggling below this
    private float minFrequency = 0.5f; //max and min visibility toggling frequency
    private float maxFrequency = 0.1f;
    private Coroutine toggleCoroutine;

    // bool, to check if flicker is currently in on state
    private bool on = true;
    private float currFlickerTime = 0f;

    public bool isPosessed = false;
    public float uhraniumPrice = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        useAmount = initUseAmount;
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        if (!isSingleUse)
        {
            useBar.fillAmount = 0f;
        }

        CreateBlobShadow();
    }

    /*public float getCalculatedBuildingCost()
    {

        bulletAI bulletAI = this.bulletPrefab.GetComponent<bulletAI>();
        //wenn drohne hol dessen bullet
        if(bulletAI == null)
        {
            TurretAI drohnenAI = this.bulletPrefab.GetComponent<TurretAI>();
            bulletAI = drohnenAI.bulletPrefab.GetComponent<bulletAI>();
            if (bulletAI.explosionRadius > 0)
            {
                this.explosionMult = 5;
            }
            buildingCost = (drohnenAI.fireRate * bulletAI.damage*explosionMult) + (range + drohnenAI.initUseAmount) / 5;
            this.turretEfficiency = (fireRate * bulletAI.damage * initUseAmount) / buildingCost;
        }
        else
        {
            if (bulletAI.explosionRadius > 0)
            {
                this.explosionMult = 5;
            }
            buildingCost = (fireRate * bulletAI.damage * explosionMult) + (range + initUseAmount) / 5;
            this.turretEfficiency = (fireRate * bulletAI.damage * initUseAmount) / buildingCost;

        }
        Debug.Log("calculated Building cost: " + buildingCost);
        return buildingCost;
    }*/

    public float getTurretEfficiency()
    {
        //getCalculatedBuildingCost();
        return this.turretEfficiency;
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        target = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (shortestDistance > distanceToEnemy)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isSingleUse && useTimeInsteadOfAmmo)
        {
            UpdateUseAmount(Time.deltaTime);
            UpdateVisibility();
        }

        // deactivate linerenderer for laser if it has no target
        if (name == TurretType.LASER)
        {
            gameObject.GetComponent<LineRenderer>().enabled = false;
        }

        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            //langsamere rotation in Video, allerdings nicht hingekriegt
            Vector3 rotation = lookRotation.eulerAngles;
            //anpassen in 3D
            //transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            // Choose one of 4 sprites (north, east, south, west) based on look direction
            currentLookAngle = Mathf.Atan2(dir.normalized.y, dir.normalized.x) * Mathf.Rad2Deg;
            if (currentLookAngle < 0) currentLookAngle += 360;
            int spriteIndex = GetSpriteIndex(currentLookAngle);
            spriteRenderer.sprite = towerSprites[spriteIndex];

            if (isMoving)
            {
                Debug.Log("useamount: " + useAmount);
                float distanceThisFrame = speed * Time.deltaTime;


                //transform.LookAt(target);
                if (dir.magnitude > stopAndShootRange)
                {
                    transform.Translate(dir.normalized * distanceThisFrame, Space.World);
                }
                else if (fireCountdown <= 0f && dir.magnitude <= stopAndShootRange)
                {
                    Shoot();
                    fireCountdown = 1f / fireRate;

                }
            }
            else if (fireCountdown <= 0f && !isMoving)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
            
            fireCountdown -= Time.deltaTime;
        }
        else
        {
            return;
        }

        UpdateTarget();
    }

    void Shoot()
    {
        //define firepoint depending on turret before using this
        //Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        GameObject currentBullet;
        if (bulletPrefab.tag == "Drone")
        {
            currentBullet = (GameObject)Instantiate(bulletPrefab, transform.position + this.firePoint, transform.rotation);
        }
        else
        {
            currentBullet = (GameObject)Instantiate(bulletPrefab, transform.position + this.firePoint, Quaternion.Euler(0f, 0f, currentLookAngle));
        }

        if(this.name == TurretType.LASER)
        {
            LineRenderer lr = gameObject.GetComponent<LineRenderer>();
            lr.widthMultiplier = 0.2f;
            lr.enabled = true;
            lr.SetPosition(0,transform.position + this.firePoint);
            lr.SetPosition(1, target.position);
        }
        currentBullet.transform.parent = this.gameManager.turretContainer.transform;
        bulletAI bullet = currentBullet.GetComponent<bulletAI>();
        if (bullet != null)
        {
            bullet.SetBulletTarget(target);
        }
        if (isSingleUse && !useTimeInsteadOfAmmo)
        {
            UpdateUseAmount(1);
        }
    }

    void UpdateUseAmount(float usageUsed)
    {
        useAmount = useAmount - usageUsed;
        useBar.fillAmount =  useAmount / initUseAmount;
        //if(useAmount <= 3f && !invokedRepeating)
        //{
        //    //toggleCoroutine = StartCoroutine(ToggleLoop());
        //    invokedRepeating = true;

        //}
        if (useAmount <= 0)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void UpdateVisibility()
    {

        if (useAmount <= 3f)
        {
            float normalizedValue = Mathf.Clamp01(useAmount / threshold);
            float frequency = Mathf.Lerp(maxFrequency, minFrequency, normalizedValue);

            this.currFlickerTime += Time.deltaTime;
            float interpolation = this.currFlickerTime / frequency;
            float alpha;
            if (on)
            {
                alpha = Mathf.Lerp(0.6f, 1f, interpolation);
            }
            else
            {
                alpha = Mathf.Lerp(1f, 0.6f, interpolation);
            }
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
            if (this.currFlickerTime >= frequency)
            {
                on = !on;
                this.currFlickerTime = 0;
            }

            // Also recolor decay timer to red
            //Transform child = transform.Find(".../...");
            var urgentColor = Color.red;
            if (useBar != null && useBar.color != urgentColor)
                useBar.GetComponent<Image>().color = urgentColor;
        }
    }

    //IEnumerator ToggleLoop()
    //{

    //    while (true)
    //    {

    //        // Calculate interval
    //        float normalizedValue = 0f;
    //        if (isMoving)
    //        {
    //            normalizedValue = Mathf.Clamp01((useAmount/2) / threshold);

    //        }
    //        else
    //        {
    //            normalizedValue = Mathf.Clamp01(useAmount / threshold);
    //        }
    //        float interval = Mathf.Lerp(maxFrequency, minFrequency, normalizedValue);
    //        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

    //        Debug.Log("interval: " + interval);
    //        if (this.on)
    //        {
    //            this.on = false;
    //            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.6f);
    //        }
    //        else
    //        {
    //            this.on = true;
    //            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1f);
    //        }

    //        // Toggle visibility
    //        //GetComponent < SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;

    //        yield return new WaitForSeconds(interval);
    //    }
    //}

    private int GetSpriteIndex(float angle)
    {
        // 4 Richtungen: 0°=Rechts, 90°=Oben, 180°=Links, 270°=Unten
        if (angle >= 315 || angle < 45)
        {
            this.firePoint = this.firePointRight;
            return 0;      // Rechts
        }
        else if (angle >= 45 && angle < 135)
        {
            this.firePoint = this.firePointUp;
            return 1;  // Oben
        }
        else if (angle >= 135 && angle < 225)
        {
            this.firePoint = this.firePointLeft;
            return 2; // Links
        }
        else
        {
            this.firePoint = this.firePointDown;
            return 3;// Unten
        }
    }

    private void CreateBlobShadow()
    {
        if (transform.Find("BlobShadow") != null) return;

        GameObject shadow = Instantiate(blobShadowPrefab, transform);
        shadow.name = "BlobShadow";
        shadow.transform.localPosition = blobShadowOffset;
        shadow.transform.localScale = blobShadowScale;
        shadow.transform.localEulerAngles = blobShadowRotation;

        // Transparenz anpassen
        Renderer renderer = shadow.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material; 
            if (mat.HasProperty("_Color"))
            {
                Color c = mat.color;
                c.a = 0.3f;
                mat.color = c;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);

    }
}
