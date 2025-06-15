using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float liveTime = 2f;
    private float countdown = 0f;

    public float riseSpeed = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.countdown >= this.liveTime)
        {
            Destroy(gameObject);
        }
        else
        {
            this.countdown += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y + riseSpeed * Time.deltaTime, transform.position.z);
        }
    }
}
