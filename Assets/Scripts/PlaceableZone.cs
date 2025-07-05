using System.Collections.Generic;
using UnityEngine;

public class PlaceableZone : MonoBehaviour
{
    public Sprite overlaySprite;       // halbtransparent blau
    public Sprite maskSprite;          // weißer Kreis oder Quadrat für Masken
    public Transform turretContainer;  // Parent-Objekt aller Türme
    public Vector2 zoneSize = new Vector2(50, 30); // Größe der Map
    public Vector2 zoneCenter = new Vector2(0, 0); // Mittelpunkt der Map
    public GameObject bgContainer;

    private GameObject overlayObj;

    private List<GameObject> currentMasks = new List<GameObject>();
    private bool isZoneVisible = false;

    private GameManager gameManager;

    private void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (!isZoneVisible || overlayObj == null) return;

        RebuildMasks();
    }

    private void RebuildMasks()
    {
        // Lösche alle bisherigen Masken
        foreach (Transform child in overlayObj.transform)
        {
            if (child.GetComponent<SpriteMask>() != null)
            {
                Destroy(child.gameObject);
            }
        }

        // Erzeuge neue Masken für alle aktuellen Türme
        foreach (Transform turret in turretContainer)
        {
            if (!turret.CompareTag("Turret")) continue;

            SpriteRenderer turretRenderer = turret.GetComponent<SpriteRenderer>();
            if (turretRenderer == null || turretRenderer.sprite == null) continue;

            GameObject maskObj = new GameObject("TurretMask");
            var mask = maskObj.AddComponent<SpriteMask>();
            mask.sprite = turretRenderer.sprite;

            maskObj.transform.position = turret.position;
            maskObj.transform.rotation = turret.rotation;
            maskObj.transform.localScale = turret.localScale * 1.3f;
            maskObj.transform.parent = overlayObj.transform;
        }

        busStopMask(new Vector2(1.1f, 1.1f));
    }

    public void busStopMask(Vector2 scaleVector)
    {
        GameObject busStopGO = this.gameManager.busStopGO;
        if (busStopGO != null)
        {
            SpriteRenderer busStopRenderer = busStopGO.GetComponent<SpriteRenderer>();
            if (busStopRenderer != null && busStopRenderer.sprite != null)
            {
                GameObject busStopMaskObj = new GameObject("BusStopMask");
                var mask = busStopMaskObj.AddComponent<SpriteMask>();
                mask.sprite = busStopRenderer.sprite;

                busStopMaskObj.transform.position = busStopGO.transform.position;
                busStopMaskObj.transform.rotation = busStopGO.transform.rotation;
                busStopMaskObj.transform.localScale = busStopGO.transform.localScale * scaleVector;

                busStopMaskObj.transform.parent = overlayObj.transform;
            }
        }
    }

    public void ShowPlaceableZone()
    {
        HidePlaceableZone();

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
                // ALTER ANSATZ:
                /*GameObject maskObj = new GameObject("TurretMask");
                var mask = maskObj.AddComponent<SpriteMask>();
                mask.sprite = maskSprite;
                //mask.sprite = turret.GetComponent<Sprite>();
                maskObj.transform.position = turret.position;

                Collider col = turret.GetComponent<Collider>();
                float scale = 1f;
                if (col != null)
                {
                    scale = Mathf.Max(col.bounds.size.x, col.bounds.size.y);
                }
                maskObj.transform.localScale = Vector3.one * scale;
                maskObj.transform.parent = overlayObj.transform;*/


                SpriteRenderer turretRenderer = turret.GetComponent<SpriteRenderer>();
                if (turretRenderer == null || turretRenderer.sprite == null)
                {
                    Debug.LogWarning("Turret ohne SpriteRenderer oder Sprite gefunden.");
                    continue;
                }

                GameObject maskObj = new GameObject("TurretMask");
                var mask = maskObj.AddComponent<SpriteMask>();
                mask.sprite = turretRenderer.sprite;

                maskObj.transform.position = turret.position;
                maskObj.transform.rotation = turret.rotation;
                maskObj.transform.localScale = turret.localScale * new Vector2(5f,5f);

                maskObj.transform.parent = overlayObj.transform;
            }
        }

        // Zusätzliche Maske für BusStopGO erzeugen
        busStopMask(new Vector2(1.1f, 1.1f));

        isZoneVisible = true;
    }

    public void HidePlaceableZone()
    {
        if (overlayObj != null)
            Destroy(overlayObj);

        foreach (var mask in currentMasks)
        {
            if (mask != null)
                Destroy(mask);
        }
        currentMasks.Clear();

        isZoneVisible = false;
    }
}
