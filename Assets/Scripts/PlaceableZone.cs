using UnityEngine;

public class PlaceableZone : MonoBehaviour
{
    public Sprite overlaySprite;       // halbtransparent blau
    public Sprite maskSprite;          // weißer Kreis oder Quadrat für Masken
    public Transform turretContainer;  // Parent-Objekt aller Türme
    public Vector2 zoneSize = new Vector2(10, 6); // Größe der Map
    public Vector2 zoneCenter = new Vector2(0, 0); // Mittelpunkt der Map
    public GameObject bgContainer;

    private GameObject overlayObj;

    public void ShowPlaceableZone()
    {
        if (overlayObj != null) Destroy(overlayObj);

        // Hole Bounds von BGContainer
        Renderer[] renderers = bgContainer.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("BGContainer enthält keine Renderer!");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds); // vereint alle Bounds zu einem Gesamtbereich
        }

        Vector2 zoneSize = new Vector2(bounds.size.x, bounds.size.y);
        Vector2 zoneCenter = new Vector2(bounds.center.x, bounds.center.y);

        // Overlay erzeugen
        overlayObj = new GameObject("PlaceableZoneOverlay");
        var sr = overlayObj.AddComponent<SpriteRenderer>();
        sr.sprite = overlaySprite;
        sr.color = new Color(0f, 0.5f, 1f, 0.5f);
        sr.sortingLayerName = "Default";
        sr.sortingOrder = -2;
        sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        overlayObj.transform.position = zoneCenter;
        overlayObj.transform.localScale = zoneSize;

        // Masken generieren
        foreach (Transform turret in turretContainer)
        {
            if (turret.CompareTag("Turret"))
            {
                GameObject maskObj = new GameObject("TurretMask");
                var mask = maskObj.AddComponent<SpriteMask>();
                mask.sprite = maskSprite;
                maskObj.transform.position = turret.position;

                Collider col = turret.GetComponent<Collider>();
                float scale = 1f;
                if (col != null)
                {
                    scale = Mathf.Max(col.bounds.size.x, col.bounds.size.y);
                }
                maskObj.transform.localScale = Vector3.one * scale;
                maskObj.transform.parent = overlayObj.transform;
            }
        }
    }

    public void HidePlaceableZone()
    {
        if (overlayObj != null)
            Destroy(overlayObj);
    }
}
