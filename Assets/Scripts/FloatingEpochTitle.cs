using TMPro;
using UnityEngine;

public class FloatingEpochTitle : MonoBehaviour
{
    public float liveTime = 3f;
    private float countdown = 0f;

    public float riseSpeed = 0.1f;
    public TMP_Text tmpText;
    private Color originalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalColor = Color.white;
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

            // Skalieren
            transform.localScale = new Vector3(transform.localScale.x + riseSpeed * Time.deltaTime * 0.5f, transform.localScale.y + riseSpeed * Time.deltaTime * 0.5f, transform.localScale.z);

            // Alpha verringern
            float alpha = Mathf.Lerp(originalColor.a, 0f, countdown / liveTime);
            tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
    }

    public void SetFloatingText(string text, Color color)
    {
        this.tmpText.text = text;
        this.tmpText.color = color;
    }
}
