using UnityEngine;
using System.Collections;


public class DrawCircle : MonoBehaviour
{
    // keine ahnung wie man die farbe ändert. DER RADIUS SKALIERT MIT DER SCALE DES OBJEKTS
    [Range(0, 50)]
    public int segments = 50;
    [Range(0, 5)]
    public float xradius = 5;
    [Range(0, 5)]
    public float yradius = 5;
    public LineRenderer line;

    void Start()
    {
        line.SetVertexCount(segments + 1);
        line.SetWidth(0.1f, 0.1f);
        Color c1 = new Color(0f, 0f, 0f, 1);
        line.useWorldSpace = false;
        CreatePoints();
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
