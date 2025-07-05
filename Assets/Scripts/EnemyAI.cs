using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using IEnumerator = System.Collections.IEnumerator;

public class EnemyAI : MonoBehaviour
{
    public EnemyType name;
    public float initSpeed = 3f;
    private float speed = 3f;
    public float initHealth = 100;
    private float health = 100;
    public Image healthbar;
    public bool isSplitter = false;
    public int splitAmount = 0;
    public Transform splitUnit;
    private GameManager gameManager;
    private AudioManager audioManager;

    public int auraRadius =0;
    public float auraEffectStrength = 2f;
    public float auraDuration = 2f;

    public GameObject dieParticles;

    public List<GameObject> targets = new List<GameObject>();
    public GameObject currentTarget;
    public int waypointIndex = 0;
    private float slowCountdown = 0f;
    private Vector3 direction;

    public int cost = 10;
    public int randomWeight = 1;
    private bool hasDrop = false;
    [Header("1 in DropProbability chance to drop zeitsand")]
    public int dropProbability = 10;
    public float dropAmount = 0;
    public int dropMaximum = 10;
    public int dropMinimum = 5;
    public GameObject itemToDrop;
    public float dropDuration = 10;

    public int damage = 1;

    // Blob Shadow related
    public GameObject blobShadowPrefab;
    public Vector3 blobShadowOffset = new Vector3(0f, -0.1f, 0f);
    public Vector3 blobShadowScale = new Vector3(1f, 1f, 1f);
    public Vector3 blobShadowRotation = Vector3.zero;

    public Image healthBar; // z.B. grün
    public Image delayedHealthBar; // z.B. rot
    public float maxWidth;
    public float damageLerpSpeed = 2f;
    private float targetHealthRatio;
    private Coroutine damageAnim;

    public List<Sprite> sprites;

    void Awake()
    {
        speed=initSpeed;
        health = initHealth;
        if(Random.Range(1, dropProbability)==1){
            hasDrop = true;
            dropAmount = (float) (Random.Range(dropMinimum, dropMaximum));
        }
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        CreateBlobShadow();
        ChooseSprite();
    }

    private void ChooseSprite()
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
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    private void UpdateLayerPosition()
    {

    }

    void setTargetList(List<GameObject> targetList)
    {
        targets = targetList;
        currentTarget = targetList[waypointIndex];
    }

    void Update()
    {
        //might be Vector2

        direction = currentTarget.transform.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        // Mirror sprite if needed to look at the direction of movement
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (direction.x < 0)
            sr.flipX = true; // Mirror (left side)
        else if (direction.x > 0)
            sr.flipX = false;  // Original (right side)

        if (slowCountdown > 0)
        {
            slowCountdown -= Time.deltaTime;
            if (0 >= slowCountdown)
            {
                speed = initSpeed;
            }
        }

        if (Vector3.Distance(transform.position, currentTarget.transform.position) < 0.2f)
        {
            GetNextWaypoint();
        }
        if (auraRadius > 0)
        {
            AuraCollider();

        }
    }

    void AuraCollider()
    {
        Collider[] collidersHit = Physics.OverlapSphere(transform.position, auraRadius);
        foreach (Collider collider in collidersHit)
        {
            if (collider.tag == "Enemy")
            {
                AuraEffect(collider.transform);

            }
        }

    }

    void AuraEffect(Transform enemy)
    {
        EnemyAI e = enemy.GetComponent<EnemyAI>();
        if (e != null)
        {
            e.Slow(auraEffectStrength, Time.deltaTime);
        }
    }

    void GetNextWaypoint()
    {
        if (waypointIndex >= targets.Count - 1)
        {
            EndPath();
            return;
        }
        waypointIndex++;
        currentTarget = targets[waypointIndex];
    }

    void EndPath()
    {
        this.gameManager.player.TakeDamage(damage);
        //Debug.Log("lives: " + playerStats.lives);
        Destroy(gameObject);
        return;
    }

    void CollideWithBusStop()
    {
        this.gameManager.player.TakeDamage(damage);
        //Debug.Log("lives: " + playerStats.lives);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Busstop"))
        {
            this.audioManager.PlaySfx(this.audioManager.soundLibrary.sfxBusstopHit);
            CollideWithBusStop();
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        health = Mathf.Max(health, 0);
        targetHealthRatio = health / initHealth;

        // Wenn keine Healthbar gesetzt: alte Variante nutzen (sicherer Fallback)
        if (healthbar == null || delayedHealthBar == null)
        {
            if (healthbar != null)
                healthbar.fillAmount = targetHealthRatio;

            if (health <= 0) Die();
            return;
        }

        // neue Variante mit zwei Balken
        healthbar.fillAmount = targetHealthRatio;

        if (damageAnim != null) StopCoroutine(damageAnim);
        damageAnim = StartCoroutine(AnimateDelayedBar());

        if (health <= 0) Die();
    }

    public void Slow(float slowMultiplier, float freezeDuration) {
        if (speed == initSpeed)
        {
            speed *= slowMultiplier;
            slowCountdown = freezeDuration;
        }
    }


    void Die()
    {
        if (isSplitter)
        {
            for (int i = 0; i < splitAmount; i++)
            {
                Transform newEnemy = Instantiate(splitUnit, targets[waypointIndex].transform.position-i* direction.normalized, targets[waypointIndex].transform.rotation);
                newEnemy.GetComponent<EnemyAI>().targets = targets;
                newEnemy.GetComponent<EnemyAI>().currentTarget = newEnemy.GetComponent<EnemyAI>().targets[waypointIndex];
                newEnemy.GetComponent<EnemyAI>().waypointIndex = waypointIndex;
                newEnemy.transform.parent = gameManager.enemyContainer.transform;
                newEnemy.GetComponent<EnemyAI>().hasDrop = false;
                newEnemy.GetComponent<EnemyAI>().dropAmount = 0;

            }
            //StartCoroutine(waiter());
        }
        if (hasDrop)
        {
            GameObject droppedItem = Instantiate(itemToDrop, transform.position, transform.rotation);
            droppedItem.GetComponent<DropProperties>().dropAmount = this.dropAmount;
            droppedItem.GetComponent<DropProperties>().survivalDuration = dropDuration;
            droppedItem.GetComponent<DropProperties>().remainingDuration = dropDuration;
            this.audioManager.PlaySfx(this.audioManager.soundLibrary.sfxZeitsandDropped);
            //Destroy(droppedItem, dropDuration);
        }

        if (this.dieParticles != null)
        {
            GameObject particles = Instantiate(this.dieParticles, transform.position, this.dieParticles.transform.rotation);
            Destroy(particles, 2f);
        }

        Destroy(gameObject);
    }

   

    /*private void CreateGroundShadow()
    {
        GameObject shadow = new GameObject("Shadow");
        SpriteRenderer sr = shadow.AddComponent<SpriteRenderer>();
        sr.sprite = GetComponent<SpriteRenderer>().sprite;
        sr.color = new Color(0, 0, 0, 0.3f);
        sr.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;

        shadow.transform.parent = transform;
        shadow.transform.localPosition = new Vector3(0, -0.05f, 0);
        shadow.transform.localScale = new Vector3(1f, 0.3f, 1f);
    }*/

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

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(1);
    }

    IEnumerator AnimateDelayedBar()
    {
        float flashSpeed = 0.02f;
        int flashDurationInCycles = 3;

        delayedHealthBar.color = Color.white;
        for (int i = 0; i < flashDurationInCycles; i++)
        {
            
            yield return new WaitForSeconds(flashSpeed);
            //delayedHealthBar.color = new Color(33f / 255f, 34f / 255f, 52f / 255f); // Can be turned off (i.e. set to White aswell) for a smoother (less pixelated) experience
            delayedHealthBar.color = Color.white;
            yield return new WaitForSeconds(flashSpeed);
            delayedHealthBar.color = Color.white;
            yield return new WaitForSeconds(flashSpeed);
        }
        delayedHealthBar.color = Color.white;

        float start = delayedHealthBar.fillAmount;
        float t = 0f;

        while (t < 0.6f)
        {
            t += Time.deltaTime * damageLerpSpeed;
            float eased = Mathf.SmoothStep(start, targetHealthRatio, t);
            delayedHealthBar.fillAmount = eased;
            yield return null;
        }

        delayedHealthBar.fillAmount = targetHealthRatio;
    }

    void SetBarWidth(RectTransform bar, float ratio)
    {
        bar.sizeDelta = new Vector2(maxWidth * ratio, bar.sizeDelta.y);
    }
}
