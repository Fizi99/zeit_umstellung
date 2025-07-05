using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class TempTileCreator
{
    [MenuItem("Assets/Create/2D/Tile")]
    public static void CreateTileAsset()
    {
        // Erstelle das Tile
        Tile tile = ScriptableObject.CreateInstance<Tile>();

        // Pfad festlegen – im aktuellen Ordner im Project-Fenster
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }
        else if (!System.IO.Directory.Exists(path))
        {
            path = System.IO.Path.GetDirectoryName(path);
        }

        string fullPath = AssetDatabase.GenerateUniqueAssetPath(path + "/NewTile.asset");

        // Asset speichern
        AssetDatabase.CreateAsset(tile, fullPath);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tile;
    }
}
