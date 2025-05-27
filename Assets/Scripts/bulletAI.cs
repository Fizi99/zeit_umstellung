using UnityEngine;

public class bulletAI : MonoBehaviour
{
    private Transform target;
    public float speed = 70f;
    public int damage = 5;
    public float explosionRadius = 0;
    public bool useFreeze = false;
    public bool shootSelf = false;
    public int SlowMultiplier = 0;
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
            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
            transform.LookAt(target);

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
        whereToGo e = enemy.GetComponent<whereToGo>();
        if (e != null)
        {
            e.TakeDamage(damage);
        }
    }

    void Slow(Transform enemy)
    {
        whereToGo e = enemy.GetComponent<whereToGo>();
        if (e != null)
        {
            e.Slow(SlowMultiplier, freezeDuration);
        }
    }

    void Explode()
    {
        Collider[] collidersHit= Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider collider in collidersHit)
        {
            if(collider.tag == "Enemy")
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
