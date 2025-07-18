using UnityEngine;

public class bulletAI : MonoBehaviour
{
    private Transform target;
    public float speed = 70f;
    public float damage = 5;
    public float explosionRadius = 0;
    public bool useFreeze = false;
    public bool shootSelf = false;
    public float SlowMultiplier = 0;
    public float freezeDuration = 5f;
    public TurretType turretType = TurretType.STANDARD;

    public GameObject hitParticle;

    private GameManager gameManager;
    private AudioManager audioManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }


    private void PlayHitAudio()
    {
        AudioClip clip = null;
        switch (turretType)
        {
            case TurretType.STANDARD:
                clip = this.audioManager.soundLibrary.sfxTurretWristwatchArtilleryHit;
                break;
            case TurretType.DRONE:
                clip = this.audioManager.soundLibrary.sfxTurretDroneHit;
                break;
            case TurretType.DRONEBASE:
                // clip = this.audioManager.soundLibrary.sfxTurretWristwatchArtilleryFire;
                break;
            case TurretType.DYNAMITE:
                // clip = this.audioManager.soundLibrary.sfxTurretWristwatchArtilleryFire;
                break;
            case TurretType.FREEZE:
                // clip = this.audioManager.soundLibrary.sfxTurretWristwatchArtilleryFire;
                break;
            case TurretType.LASER:
                clip = this.audioManager.soundLibrary.sfxTurretSundailLaserHit;
                break;
            case TurretType.MISSILE:
                clip = this.audioManager.soundLibrary.sfxTurretDigitalRocketlauncherHit;
                break;

            default:
                break;
        }

        if (clip != null)
        {
            this.audioManager.PlaySfx(clip);
        }

    }

    public void SetBulletTarget(Transform targetSetter)
    {
        target = targetSetter;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            if (turretType == TurretType.DYNAMITE)
            {
                this.damage = 0;
                Destroy(gameObject, 0.5f);
            }
            else
            {
                Destroy(gameObject);
            }
            return;
        }

        if (shootSelf)
        {
            
            HitTarget();
            return;
        }
        else
        {
            Vector3 dir = target.position - transform.position;
            float distanceThisFrame = speed * Time.deltaTime;


            if (dir.magnitude <= distanceThisFrame)
            {
                HitTarget();
                return;
            }

            // ALT:
            //transform.rotation = Quaternion.LookRotation(dir.normalized) * Quaternion.Euler(0, 90, 0);
            //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
            //Debug.Log("[LOG] Winkel: " + (angle-90));
            //transform.localScale = new Vector3(1, 1, 1);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);  
            //Debug.Log(transform.localScale); // sollte (1,1,1) sein

            // Rotate projectile sprite to always point at the target ('s direction)
            /*Quaternion lookRotation = Quaternion.LookRotation(dir.normalized);
            transform.rotation = lookRotation;
            Vector3 rotation = lookRotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation.z);*/

            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
            //transform.LookAt(target);
        }
    }

    void HitTarget()
    {
        if (explosionRadius > 0)
        {
            Explode();
        }
        else
        {
            DealDamage(target);
        }

        if(this.hitParticle!= null)
        {
            GameObject particles;
            if (turretType == TurretType.DYNAMITE)
            {
                 particles = Instantiate(this.hitParticle, transform.position, this.hitParticle.transform.rotation);
            }
            else
            {
                 particles = Instantiate(this.hitParticle, target.position, this.hitParticle.transform.rotation);
            }
            Destroy(particles, 2f);
        }
        PlayHitAudio();
        if(turretType == TurretType.DYNAMITE)
        {

            this.damage = 0;
            Destroy(gameObject, 0.5f);
        }
        else
        {
            Destroy(gameObject);
        }
        //do bullet effect here
    }

    //change to game object of enemy
    void DealDamage(Transform enemy)
    {
        EnemyAI e = enemy.GetComponent<EnemyAI>();
        if (e != null)
        {
            e.TakeDamage(damage);
        }
    }

    void Slow(Transform enemy)
    {
        EnemyAI e = enemy.GetComponent<EnemyAI>();
        if (e != null)
        {
            e.Slow(SlowMultiplier, freezeDuration);
        }
    }

    void Explode()
    {
        int kills = 0;
        Collider[] collidersHit = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in collidersHit)
        {
            if (collider.tag == "Enemy")
            {
                if (useFreeze)
                {
                    Slow(collider.transform);
                }
                DealDamage(collider.transform);
                if (!collider.GetComponent<EnemyAI>().IsAlive())
                {
                    kills++;
                }
            }
        }

        // Check for multikill
        if (kills >= 5)
        { 
            this.gameManager.uiManager.TriggerMultikill(transform.position); 
        }
    }
}
