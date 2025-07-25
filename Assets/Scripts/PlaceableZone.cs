using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceableZone : MonoBehaviour
{
    public Sprite overlaySprite;       // halbtransparent blau
    public Sprite maskSprite;          // wei�er Kreis oder Quadrat f�r Masken
    public Transform turretContainer;  // Parent-Objekt aller T�rme
    public Vector2 zoneSize = new Vector2(50, 30); // Gr��e der Map
    public Vector2 zoneCenter = new Vector2(0, 0); // Mittelpunkt der Map
    public GameObject bgContainer;
    public float pulseFrequency = 0.5f;
    private float currentPulseTime = 0f;
    private bool pulseDirection = false;
    public bool pulseOn = true;
    public Tilemap bgTilemap;

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
        if (pulseOn)
        {
            UpdatePulse();
        }
       
    }

    private void RebuildMasks()
    {
        // L�sche alle bisherigen Masken
        foreach (Transform child in overlayObj.transform)
        {
            if (child.GetComponent<SpriteMask>() != null)
            {
                Destroy(child.gameObject);
            }
        }

        // Erzeuge neue Masken f�r alle aktuellen T�rme
        foreach (Transform turret in turretContainer)
        {
            if (!turret.CompareTag("Turret")) continue;

            SpriteRenderer turretRenderer = turret.GetComponent<SpriteRenderer>();
            if (turretRenderer == null || turretRenderer.sprite == null) continue;

            GameObject maskObj = new GameObject("TurretMask");
            var mask = maskObj.AddComponent<SpriteMask>();
            mask.sprite = turretRenderer.sprite;

            maskObj.transform.localScale = turret.localScale * 1.3f;
            Vector3 slightYOffset = new Vector3(0f, -0.1f, 0f);
            maskObj.transform.position = turret.position + slightYOffset;
            maskObj.transform.rotation = turret.rotation;
            maskObj.transform.parent = overlayObj.transform;
        }

        busStopMask(1.25f);
    }

    public void busStopMask(float scaleFactor)
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

                busStopMaskObj.transform.localScale = busStopGO.transform.localScale * scaleFactor;
                busStopMaskObj.transform.position = busStopGO.transform.position;
                busStopMaskObj.transform.rotation = busStopGO.transform.rotation;

                busStopMaskObj.transform.parent = overlayObj.transform;
            }
        }
    }

    public void ShowPlaceableZone()
    {
        HidePlaceableZone();

        if (overlayObj != null) Destroy(overlayObj);

        /*// Hole Bounds von BGContainer
        Renderer[] renderers = bgContainer.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("BGContainer enth�lt keine Renderer!");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds); // vereint alle Bounds zu einem Gesamtbereich
        }

        Vector2 zoneSize = new Vector2(bounds.size.x, bounds.size.y);
        Vector2 zoneCenter = new Vector2(bounds.center.x, bounds.center.y);*/

        // Zellbereich der Tilemap
        BoundsInt cellBounds = bgTilemap.cellBounds;

        // Welt-Positionen der Tilemap-Ecken berechnen
        Vector3 worldMin = bgTilemap.CellToWorld(cellBounds.min);
        Vector3 worldMax = bgTilemap.CellToWorld(cellBounds.max);

        // Gr��e und Mittelpunkt berechnen
        Vector2 zoneSize = worldMax - worldMin;
        Vector2 zoneCenter = (Vector2)worldMin + zoneSize / 2f;

        // Ab hier wie gehabt

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

                maskObj.transform.localScale = turret.localScale * 5f;
                maskObj.transform.position = turret.position;
                maskObj.transform.rotation = turret.rotation;

                maskObj.transform.parent = overlayObj.transform;
            }
        }

        // Zus�tzliche Maske f�r BusStopGO erzeugen
        busStopMask(1.25f);

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

    private void UpdatePulse()
    {
        if(this.overlayObj != null)
        {
            this.currentPulseTime += Time.deltaTime;
            float interpolation = this.currentPulseTime / pulseFrequency;
            float alpha;
            if (pulseDirection)
            {
                alpha = Mathf.Lerp(0.6f, 1f, interpolation);
            }
            else
            {
                alpha = Mathf.Lerp(1f, 0.6f, interpolation);
            }
            SpriteRenderer renderer = this.overlayObj.GetComponent<SpriteRenderer>();
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
            if (this.currentPulseTime >= pulseFrequency)
            {
                pulseDirection = !pulseDirection;
                this.currentPulseTime = 0;
            }
        }
        
    }
}
