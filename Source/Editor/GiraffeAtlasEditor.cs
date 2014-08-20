using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Object = UnityEngine.Object;

[CustomEditor(typeof(GiraffeAtlas))]
public class GiraffeAtlasEditor : Editor
{

  static void TryResolveEditorData(GiraffeAtlas atlas)
  {
    // atlas._sourceResolved:
    // 0 - no attempt
    // 1 - attempt, and found
    // 2 - attemped, and not found
    switch (atlas._importDataResolved)
    {
      case 0:
      case 2:
      {
        foreach (var n in AssetDatabase.GetAllAssetPaths())
        {
          if (n.StartsWith("Assets/") == false)
            continue;
          if (n.EndsWith(".asset") == false)
            continue;
          UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(n, typeof(GiraffeImportData));
          if (obj != null && obj is GiraffeImportData)
          {
            GiraffeImportData source = obj as GiraffeImportData;
            if (source.atlasIdA == atlas.atlasIdA && source.atlasIdB == atlas.atlasIdB)
            {
              atlas._importData = source;
              atlas._importDataResolved = 1;
              source._atlas = atlas;
              source._atlasResolved = 1;
              return;
            }
          }
        }
        atlas._importDataResolved = 2;
      }
      break;
    }
  }

  [MenuItem("Assets/Create/Giraffe Atlas")]
  static void CreateAtlas()
  {
    GiraffeAtlas atlas = ScriptableObject.CreateInstance<GiraffeAtlas>();
    System.Random rng = new System.Random();
    atlas.atlasIdA = rng.Next();
    atlas.atlasIdB = rng.Next();

    //GiraffeAtlasEditorData editorData = ScriptableObject.CreateInstance<GiraffeAtlasEditorData>();

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
    texImporter.isReadable = true;

    EditorUtility.SetDirty(atlas);
    EditorUtility.SetDirty(texImporter);

    EditorUtility.FocusProjectWindow();
    Selection.activeObject = atlas;
  }

  [MenuItem("Assets/Create/Giraffe Atlas Source (Temp)")]
  static void CreateAtlasData()
  {
    GiraffeImportData atlas = ScriptableObject.CreateInstance<GiraffeImportData>();
    //GiraffeAtlasEditorData editorData = ScriptableObject.CreateInstance<GiraffeAtlasEditorData>();

    string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    if (path == "")
    {
      path = "Assets";
    }
    else if (Path.GetExtension(path) != "")
    {
      path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
    }

    string atlasPath = AssetDatabase.GenerateUniqueAssetPath(path + "/New Giraffe Atlas Source.asset");

    AssetDatabase.CreateAsset(atlas, atlasPath);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();

    EditorUtility.SetDirty(atlas);

    EditorUtility.FocusProjectWindow();
    Selection.activeObject = atlas;
  }

  private GiraffeAtlas mAtlas;
  private int mSpriteMode;

  void OnEnable()
  {
    mAtlas = (GiraffeAtlas)this.target;
    TryResolveEditorData(mAtlas);
    mSpriteMode = 0;
  }

  public override void OnInspectorGUI()
  {
    mAtlas.texture = EditorGUILayout.ObjectField("Texture", mAtlas.texture, typeof(Texture2D)) as Texture2D;

    Color col = GUI.color;

    EditorGUILayout.BeginHorizontal();
    if (mSpriteMode == 0)
      GUI.color = Color.green;
    if (GUILayout.Button("General"))
    {
      mSpriteMode = 0;
    }
    if (mSpriteMode == 1)
      GUI.color = Color.green;
    else
      GUI.color = col;
    if (GUILayout.Button("Sprites"))
    {
      mSpriteMode = 1;
    }
    if (mSpriteMode == 2)
    {
      GUI.color = Color.green;
    }
    else
    {
      if (mAtlas._importData != null && mAtlas._importData.atlasOutOfDate)
        GUI.color = Color.yellow;
      else
        GUI.color = col;
    }


    if (GUILayout.Button("Import"))
    {
      mSpriteMode = 2;
    }

    GUI.color = col;
    EditorGUILayout.EndHorizontal();

    if (mSpriteMode == 0)
      InspectGeneral();
    else if (mSpriteMode == 1)
      InspectEdit();
    else if (mSpriteMode == 2)
      InspectImport();

  }

  private Vector2 mInspectSpritesScroll;
  static private GUIStyle msSpriteThumbStyle;

  void InspectGeneral()
  {
    // 
  }

  void InspectEdit()
  {
    if (msSpriteThumbStyle == null)
    {
      msSpriteThumbStyle = new GUIStyle(GUI.skin.button);
      msSpriteThumbStyle.imagePosition = ImagePosition.ImageAbove;
    }

    mInspectSpritesScroll = EditorGUILayout.BeginScrollView(mInspectSpritesScroll);

    foreach (var sprite in mAtlas.sprites)
    {
      GUILayout.Button(sprite.name);
    }

    EditorGUILayout.EndScrollView();
  }

  enum FriendlyPartType
  {
    Texture,
    //   CommaSeperatedFile
  }

  private Vector2 mImportScroll;

  void InspectImport()
  {
    if (mAtlas._importDataResolved == 2)
    {
      GUILayout.BeginVertical(EditorStyles.textArea);
      GUILayout.Label("An Giraffe Import Data file must be created before using, this is used by the editor to keep track of external resources to build the texture atlas.", EditorStyles.wordWrappedLabel);
      if (GUILayout.Button("Create"))
      {
        String path = EditorUtility.SaveFilePanelInProject("Atlas Import Data", String.Format("{0} ImportData", System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(mAtlas))), "asset",
          "Create an ImportData");

        if (String.IsNullOrEmpty(path) == false)
        {
          GiraffeImportData importData = ScriptableObject.CreateInstance<GiraffeImportData>();
          importData.atlasIdA = mAtlas.atlasIdA;
          importData.atlasIdB = mAtlas.atlasIdB;

          mAtlas._importData = importData;
          mAtlas._importDataResolved = 1;
          importData._atlas = mAtlas;
          importData._atlasResolved = 1;

          AssetDatabase.CreateAsset(importData, path);
          AssetDatabase.SaveAssets();
          AssetDatabase.Refresh();

          EditorUtility.SetDirty(importData);
        }
      }
      GUILayout.Label("Note: To stop the Import Data and any textures/files it references as part of the build, place them outside of the Resources folder.", EditorStyles.wordWrappedLabel);

      GUILayout.EndVertical();
      return;
    }

    bool changed = false;

    GUILayout.BeginHorizontal();

    Color col = GUI.color;

    if (mAtlas._importData.atlasOutOfDate)
    {
      GUI.color = Color.yellow;
    }

    if (GUILayout.Button("Build", EditorStyles.miniButton, GUILayout.MinWidth(120)))
    {
      BuildAtlas();
    }

    GUI.color = col;

    GUILayout.FlexibleSpace();
    GUILayout.BeginHorizontal(GUILayout.MinWidth(120));
    GUI.changed = false;

    GUILayout.Label("Add", GUILayout.Width(25));
    var v = (FriendlyPartType)EditorGUILayout.EnumPopup(FriendlyPartType.Texture);

    if (GUI.changed)
    {
      switch (v)
      {
        case FriendlyPartType.Texture:
        {
          GiraffeAtlasImportDataPart part = new GiraffeAtlasImportDataPart();
          part.type = GiraffeAtlasImportDataType.Texture2D;
          mAtlas._importData.parts.Add(part);
          changed = true;
          break;
        }
        //        case FriendlyPartType.CommaSeperatedFile:
        //        {
        //          GiraffeAtlasImportDataPart part = new GiraffeAtlasImportDataPart();
        //          part.type = GiraffeAtlasImportDataType.CSV;
        //          mAtlas._importData.parts.Add(part);
        //          changed = true;
        //          break;
        //        }
      }
    }

    GUILayout.EndHorizontal();
    GUILayout.EndHorizontal();


    GUILayout.BeginVertical();

    GUI.changed = false;
    mAtlas._importData.padding = EditorGUILayout.IntSlider("Padding", mAtlas._importData.padding, 0, 8);

    if (GUI.changed)
      changed = true;

    GUI.changed = false;
    mAtlas._importData.border = EditorGUILayout.IntSlider("Border", mAtlas._importData.border, 0, 32);

    if (GUI.changed)
      changed = true;


    GUILayout.EndVertical();


    mImportScroll = GUILayout.BeginScrollView(mImportScroll);

    foreach (var part in mAtlas._importData.parts)
    {
      GUILayout.BeginHorizontal();

      if (GUILayout.Button("x", GUILayout.Width(25)))
      {
        mAtlas._importData.parts.Remove(part);
        break;
      }

      switch (part.type)
      {
        case GiraffeAtlasImportDataType.Texture2D:
        {
          GUI.changed = false;
          part.textureAsset = EditorGUILayout.ObjectField(part.textureAsset, typeof(Texture2D), false) as Texture2D;

          if (GUI.changed)
          {
            Debug.Log("changed image");
            changed = true;
          }

        }
        break;
        //        case GiraffeAtlasImportDataType.CSV:
        //        {
        //          GUILayout.BeginVertical();
        //          GUI.changed = false;
        //          part.textAsset = EditorGUILayout.ObjectField(part.textureAsset, typeof(TextAsset), false) as TextAsset;
        //
        //          if (GUI.changed)
        //          {
        //            Debug.Log("changed csv text");
        //            changed = true;
        //          }
        //
        //          GUI.changed = false;
        //          part.textureAsset = EditorGUILayout.ObjectField(part.textureAsset, typeof(Texture2D), false) as Texture2D;
        //
        //          if (GUI.changed)
        //          {
        //            Debug.Log("changed csv image");
        //            changed = true;
        //          }
        //          GUILayout.EndVertical();
        //        }
        //        break;
      }

      GUILayout.EndHorizontal();
    }

    GUILayout.EndScrollView();

    if (changed)
    {
      mAtlas._importData.atlasOutOfDate = true;
      EditorUtility.SetDirty(mAtlas._importData);
      EditorUtility.SetDirty(mAtlas);
    }

  }

  class TextureCoordSet
  {
    public struct PixelCoords
    {
      public String name;
      public int x, y;
      public int w, h;
    }

    public List<PixelCoords> coords;

    public TextureCoordSet()
    {
      coords = new List<PixelCoords>(1);
    }
  }

  const int kWhiteTexSize = 4;

  void BuildAtlas()
  {
    mAtlas._importData.atlasOutOfDate = false;

    // Add white texture.
    var whiteTex = new Texture2D(kWhiteTexSize, kWhiteTexSize, TextureFormat.ARGB32, false);
    whiteTex.name = "Giraffe/White";
    Color32[] col = new Color32[kWhiteTexSize * kWhiteTexSize];
    for (int i = 0; i < kWhiteTexSize * kWhiteTexSize; i++)
      col[i] = new Color32(255, 255, 255, 255);
    whiteTex.SetPixels32(col);
    whiteTex.Apply(true, false);

    GiraffeAtlasBuilder builder = new GiraffeAtlasBuilder();
    builder.Begin(mAtlas.texture, mAtlas._importData.border, mAtlas._importData.padding);
    builder.Add(whiteTex.name, whiteTex, 0, 0, kWhiteTexSize, kWhiteTexSize);

    foreach (var p in mAtlas._importData.parts)
    {
      switch (p.type)
      {
        case GiraffeAtlasImportDataType.None:
        break;
        case GiraffeAtlasImportDataType.Texture2D:
        {
          builder.Add(p.textureAsset.name, p.textureAsset, 0, 0, p.textureAsset.width, p.textureAsset.height);
        }
        break;
      }
    }

    var sprites = builder.End();

    Object.DestroyImmediate(whiteTex);

    mAtlas.sprites.Clear();
    foreach (var s in sprites)
    {
      var sprite = new GiraffeSprite();
      sprite.name = s.name;
      sprite.left = s.x;
      sprite.top = s.y;
      sprite.width = s.w;
      sprite.height = s.h;
      sprite.refreshNeeded = true;
      mAtlas.sprites.Add(sprite);
    }

    EditorUtility.SetDirty(mAtlas._importData);
    EditorUtility.SetDirty(mAtlas);

    AssetDatabase.Refresh();
  }

}
