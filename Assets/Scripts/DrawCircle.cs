using UnityEngine;
using System.Collections;

public class DrawCircle : MonoBehaviour
{
    [Range(0, 50)]
    public int segments = 50;
    [Range(0, 5)]
    public float xradius = 5;
    [Range(0, 5)]
    public float yradius = 5;
    public LineRenderer line;

    public float lineWidth = 0.05f;

    void Start()
    {
        line.positionCount = segments + 1;

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        line.useWorldSpace = false;
        CreatePoints();

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = new Color(0, 0, 1, 0.5f);  // Startfarbe
        line.endColor = new Color(0, 0, 1, 0.5f);    // Endfarbe
    }

    void CreatePoints()
    {
        float x;
        float y;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            line.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / segments);
        }
    }
}
