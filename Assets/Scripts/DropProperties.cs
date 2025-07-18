using UnityEngine;

public class DropProperties : MonoBehaviour
{

    public float dropAmount = 0;
    public float survivalDuration = 10;
    public float remainingDuration = 10;
    public float minFrequency = 0.5f; //max and min visibility toggling frequency
    public float maxFrequency = 0.2f;

    public SpriteRenderer sprite;
    private float threshold = 5;
    private float currFlickerTime = 0;
    private bool on = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.remainingDuration -= Time.deltaTime;
        if(this.remainingDuration <= this.threshold)
        {
            UpdateVisibility();
        }
        if(this.remainingDuration <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateVisibility()
    {

        
            float normalizedValue = Mathf.Clamp01(remainingDuration / threshold);
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
            SpriteRenderer renderer = this.sprite;
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
            if (this.currFlickerTime >= frequency)
            {
                on = !on;
                this.currFlickerTime = 0;
            }
    }
}
