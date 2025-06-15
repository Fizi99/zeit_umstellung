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

    public void SetBulletTarget(Transform targetSetter)
    {
        target = targetSetter;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
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

            // Rotate projectile sprite to always point at the target ('s direction)
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = lookRotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

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
        Destroy(gameObject);
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
            }
        }
    }
}
