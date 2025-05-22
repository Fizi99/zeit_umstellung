using UnityEngine;

public class whereToGo : MonoBehaviour
{
    public float speed = 3f;

    public Transform[] targets;
    public Transform currentTarget;
    private int waypointIndex = 0;

    void setTargetList(Transform[] targetList)
    {
        targets = targetList;
        currentTarget = targetList[waypointIndex];
    }

    void Update()
    {
        //might be Vector2
        Vector3 direction = currentTarget.position - transform.position;
        transform.Translate(direction.normalized*speed*Time.deltaTime, Space.World);

        if(Vector3.Distance(transform.position, currentTarget.position) < 0.2f)
        {
            GetNextWaypoint();
        }
    }

    void GetNextWaypoint()
    {
        if (waypointIndex >= targets.Length - 1)
        {
            Destroy(gameObject);
            return;
        }
        waypointIndex++;
        currentTarget = targets[waypointIndex];
    }
}
