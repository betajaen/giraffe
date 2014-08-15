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

    string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Giraffe Atlas.asset");

    AssetDatabase.CreateAsset(atlas, assetPathAndName);

    AssetDatabase.SaveAssets();
    EditorUtility.FocusProjectWindow();
    Selection.activeObject = atlas;
  }

  public override void OnInspectorGUI()
  {
    GiraffeAtlas t = (GiraffeAtlas)this.target;

    t.texture = EditorGUILayout.ObjectField("Texture", t.texture, typeof(Texture2D)) as Texture2D;

  }

}
