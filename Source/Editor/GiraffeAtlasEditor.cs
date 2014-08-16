using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GiraffeAtlas))]
public class GiraffeAtlasEditor : Editor
{

  [MenuItem("Assets/Create/Giraffe Atlas")]
  static void CreateAtlas()
  {
    GiraffeAtlas atlas = ScriptableObject.CreateInstance<GiraffeAtlas>();

    string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    if (path == "")
    {
      path = "Assets";
    }
    else if (Path.GetExtension(path) != "")
    {
      path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
    }

    string atlasPath = AssetDatabase.GenerateUniqueAssetPath(path + "/New Giraffe Atlas.asset");
    string texturePath = AssetDatabase.GenerateUniqueAssetPath(path + "/New Giraffe Atlas Texture.png");

    Texture2D pngTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false, false);
    pngTexture.hideFlags = HideFlags.HideAndDontSave;
    byte[] bytes = pngTexture.EncodeToPNG();
    System.IO.FileStream fs = System.IO.File.Create(texturePath);
    fs.Write(bytes, 0, bytes.Length);
    fs.Close();
    Object.DestroyImmediate(pngTexture);

    AssetDatabase.CreateAsset(atlas, atlasPath);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();

    Texture2D textureAsset = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
    atlas.texture = textureAsset;
    textureAsset.filterMode = FilterMode.Point;
    textureAsset.wrapMode = TextureWrapMode.Clamp;

    TextureImporter texImporter = TextureImporter.GetAtPath(texturePath) as TextureImporter;
    texImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
    texImporter.alphaIsTransparency = true;
    texImporter.filterMode = textureAsset.filterMode;
    texImporter.wrapMode = textureAsset.wrapMode;

    EditorUtility.SetDirty(atlas);
    EditorUtility.SetDirty(texImporter);

    EditorUtility.FocusProjectWindow();
    Selection.activeObject = atlas;
  }

  public override void OnInspectorGUI()
  {
    GiraffeAtlas t = (GiraffeAtlas)this.target;
    t.texture = EditorGUILayout.ObjectField("Texture", t.texture, typeof(Texture2D)) as Texture2D;
  }

}
