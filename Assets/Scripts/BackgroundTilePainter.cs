using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundTilePainter : MonoBehaviour
{
    public Tilemap tilemap;

    [Header("Base Tiles")]
    public TileBase prehistoricBaseTile;
    public TileBase pharaohBaseTile;
    public TileBase medievalBaseTile;

    [Header("Variation Tiles")]
    public TileBase[] prehistoricVariations;
    public TileBase[] pharaohVariations;
    public TileBase[] medievalVariations;

    [Header("Tilemap Settings")]
    public int width = 32;
    public int height = 18;
    public int variationChance = 30;

    public void GenerateBackground(Epoch epoch)
    {
        tilemap.ClearAllTiles();

        TileBase baseTile = null;
        TileBase[] variations = null;

        switch (epoch)
        {
            case Epoch.PREHISTORIC:
                baseTile = prehistoricBaseTile;
                variations = prehistoricVariations;
                break;
            case Epoch.PHARAOH:
                baseTile = pharaohBaseTile;
                variations = pharaohVariations;
                break;
            case Epoch.MEDIEVAL:
                baseTile = medievalBaseTile;
                variations = medievalVariations;
                break;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool useVar = Random.Range(0, variationChance) == 0;
                TileBase tile = (useVar && variations.Length > 0)
                    ? variations[Random.Range(0, variations.Length)]
                    : baseTile;

                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }
}
