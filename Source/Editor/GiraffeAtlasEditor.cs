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
  public static void CreateAtlas()
  {

    string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    if (path == "")
    {
      path = "Assets";
    }
    else if (Path.GetExtension(path) != "")
    {
      path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
    }

    GiraffeAtlas atlas = CreateAtlas(path, "New Giraffe Atlas");
    EditorUtility.FocusProjectWindow();
    Selection.activeObject = atlas;
  }

  public static GiraffeAtlas CreateAtlas(String path, String assetName)
  {

    GiraffeAtlas atlas = ScriptableObject.CreateInstance<GiraffeAtlas>();
    System.Random rng = new System.Random();
    atlas.atlasIdA = rng.Next();
    atlas.atlasIdB = rng.Next();

    string atlasPath = AssetDatabase.GenerateUniqueAssetPath(String.Format("{0}/{1}.asset", path, assetName));
    string texturePath = AssetDatabase.GenerateUniqueAssetPath(String.Format("{0}/{1} Texture.png", path, assetName));

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
    AssetDatabase.SaveAssets();

    return atlas;
  }

  private GiraffeAtlas mAtlas;
  private int mMode;

  void OnEnable()
  {
    mAtlas = (GiraffeAtlas)this.target;
    mAtlas.RefreshSprites();
    TryResolveEditorData(mAtlas);
    mMode = 0;
  }

  public override void OnInspectorGUI()
  {

    GUILayout.BeginVertical();
    GUILayout.Label("Giraffe Atlas", EditorStyles.boldLabel);

    GUILayout.Space(4);
    GUILayout.EndVertical();

    EditorGUILayout.BeginHorizontal();

    Color col = GUI.backgroundColor;

    if (mMode != 0)
    {
      GUI.backgroundColor = col * 0.75f;
    }

    if (GUILayout.Button("Atlas", EditorStyles.miniButtonLeft))
    {
      mMode = 0;
    }

    if (mMode != 1)
    {
      GUI.backgroundColor = col * 0.75f;
    }
    else
      GUI.backgroundColor = col;

    if (GUILayout.Button("Sprites", EditorStyles.miniButtonMid))
    {
      mMode = 1;
    }

    if (mMode != 2)
    {
      GUI.backgroundColor = col * 0.75f;
    }
    else
      GUI.backgroundColor = col;

    if (GUILayout.Button("Importer", EditorStyles.miniButtonRight))
    {
      mMode = 2;
    }

    GUI.backgroundColor = col;

    EditorGUILayout.EndHorizontal();

    if (mMode == 0)
      InspectAtlas();
    else if (mMode == 1)
      InspectEdit();
    else if (mMode == 2)
      InspectImport();

  }

  private Vector2 mInspectSpritesScroll;
  static private GUIStyle msSpriteThumbStyle;

  void InspectAtlas()
  {
    bool changed = false;
    GUILayout.BeginVertical();
    GUILayout.Label("Texture", EditorStyles.boldLabel);
    EditorGUI.indentLevel++;

    GUI.changed = false;
    mAtlas.texture = EditorGUILayout.ObjectField("Texture", mAtlas.texture, typeof(Texture2D), false) as Texture2D;

    if (mAtlas.texture != null)
    {
      EditorGUILayout.LabelField("Size", String.Format("{0} x {1} px", mAtlas.texture.width, mAtlas.texture.height));
    }

    if (GUI.changed)
      changed = true;


    EditorGUI.indentLevel--;
    GUILayout.EndVertical();

    GUILayout.BeginVertical();
    GUILayout.Label("Material", EditorStyles.boldLabel);
    EditorGUI.indentLevel++;

    GUI.changed = false;
    mAtlas.useCustomMaterial = EditorGUILayout.Toggle("Custom Material", mAtlas.useCustomMaterial);
    if (GUI.changed)
      changed = true;

    if (mAtlas.useCustomMaterial)
    {
      EditorGUI.indentLevel++;

      mAtlas.customMaterial = EditorGUILayout.ObjectField("Material", mAtlas.customMaterial, typeof(Material), false) as Material;

      EditorGUILayout.LabelField(String.Empty, "Note: Texture will be assigned to Material.mainTexture when the Atlas is enabled", EditorStyles.wordWrappedMiniLabel);
      EditorGUI.indentLevel--;
    }
    EditorGUI.indentLevel--;
    GUILayout.EndVertical();


    if (changed)
    {
      EditorUtility.SetDirty(mAtlas);
    }
  }

  private String selectedSpriteName = String.Empty;

  void InspectEdit()
  {

    bool changed = false;

    if (msSpriteThumbStyle == null)
    {
      msSpriteThumbStyle = new GUIStyle(GUI.skin.button);
      msSpriteThumbStyle.imagePosition = ImagePosition.ImageAbove;
    }

    mInspectSpritesScroll = EditorGUILayout.BeginScrollView(mInspectSpritesScroll);

    foreach (var sprite in mAtlas.sprites)
    {
      bool isSelected = selectedSpriteName == sprite.name;

      GUI.changed = false;

      GUILayout.Toggle(isSelected, sprite.name, EditorStyles.foldout);
      if (GUI.changed)
      {
        if (isSelected)
        {
          selectedSpriteName = String.Empty;
        }
        else
        {
          selectedSpriteName = sprite.name;
        }
      }

      if (isSelected)
      {
        const int kPadding = 8;

        EditorGUI.indentLevel++;

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb, GUILayout.Width(sprite.width + kPadding * 2), GUILayout.Height(sprite.height + kPadding * 2));
        Rect baseRect = GUILayoutUtility.GetRect(sprite.width + kPadding * 2, sprite.height + kPadding * 2);

        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        Rect spriteRect = new Rect(baseRect.x + kPadding, baseRect.y + kPadding, sprite.width, sprite.height);

        GUI.DrawTextureWithTexCoords(spriteRect, mAtlas.texture, new Rect(sprite.x0, sprite.y0, sprite.x1 - sprite.x0, sprite.y1 - sprite.y0), true);

        EditorGUILayout.LabelField("Size", String.Format("{0}, {1} px", sprite.width, sprite.height));

        GUI.changed = false;

        sprite.scale = EditorGUILayout.IntSlider("Scale", sprite.scale, 1, 16);

        if (GUI.changed)
          changed = true;


        GUILayout.BeginHorizontal();
        GUI.changed = false;

        sprite.offsetX = EditorGUILayout.IntField("Offset", sprite.offsetX);
        sprite.offsetY = EditorGUILayout.IntField(sprite.offsetY);

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        EditorGUI.indentLevel--;
      }

    }

    EditorGUILayout.EndScrollView();

    if (changed)
    {
      EditorUtility.SetDirty(mAtlas);
    }
  }

  enum FriendlyPartType
  {
    Texture,
    SquareTileSheet,
    RectangularTileSheet
  }

  private Vector2 mImportScroll;

  void InspectImport()
  {

    if (mAtlas._importDataResolved == 2)
    {
      GUILayout.BeginVertical();
      GUILayout.Label("Importer File", EditorStyles.boldLabel);
      GUILayout.EndVertical();

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
    else
    {
      GUILayout.BeginVertical();
      GUILayout.Label("Import tool", EditorStyles.boldLabel);
      GUILayout.EndVertical();
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
        case FriendlyPartType.SquareTileSheet:
        {
          GiraffeAtlasImportDataPart part = new GiraffeAtlasImportDataPart();
          part.type = GiraffeAtlasImportDataType.TileSheetSquare;
          mAtlas._importData.parts.Add(part);
          changed = true;
          break;
        }
        case FriendlyPartType.RectangularTileSheet:
        {
          GiraffeAtlasImportDataPart part = new GiraffeAtlasImportDataPart();
          part.type = GiraffeAtlasImportDataType.TilesheetRectangular;
          mAtlas._importData.parts.Add(part);
          changed = true;
          break;
        }
      }
    }

    GUILayout.EndHorizontal();
    GUILayout.EndHorizontal();


    GUILayout.BeginVertical();

    GUI.changed = false;

    mAtlas._importData.generateWhiteTexture = EditorGUILayout.Toggle("Make 'Giraffe/White' sprite",
      mAtlas._importData.generateWhiteTexture);

    if (GUI.changed)
      changed = true;


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
        changed = true;
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
            changed = true;
          }

        }
        break;
        case GiraffeAtlasImportDataType.TileSheetSquare:
        {
          GUILayout.BeginVertical();
          GUI.changed = false;
          bool refreshCount = false;
          part.textureAsset = EditorGUILayout.ObjectField(part.textureAsset, typeof(Texture2D), false) as Texture2D;

          if (GUI.changed)
          {
            refreshCount = true;
            changed = true;
          }

          GUI.changed = false;
          part.height = part.width = EditorGUILayout.IntSlider("Tile size", part.width, 2, 512);

          if (GUI.changed)
          {
            refreshCount = true;
            changed = true;
          }

          if (refreshCount)
          {
            refreshCount = false;
            if (part.textureAsset == null)
            {
              part.count = 0;
            }
            else
            {
              part.count = (part.textureAsset.width * part.textureAsset.height) / (part.width * part.height);
            }
          }

          EditorGUILayout.LabelField("Count", part.count.ToString());
          GUILayout.EndVertical();
        }
        break;

        case GiraffeAtlasImportDataType.TilesheetRectangular:
        {
          GUILayout.BeginVertical();
          GUI.changed = false;
          bool refreshCount = false;
          part.textureAsset = EditorGUILayout.ObjectField(part.textureAsset, typeof(Texture2D), false) as Texture2D;

          if (GUI.changed)
          {
            refreshCount = true;
            changed = true;
          }

          GUI.changed = false;
          part.width = EditorGUILayout.IntSlider("Tile width", part.width, 2, 512);

          if (GUI.changed)
          {
            refreshCount = true;
            changed = true;
          }

          GUI.changed = false;
          part.height = EditorGUILayout.IntSlider("Tile height", part.height, 2, 512);

          if (GUI.changed)
          {
            refreshCount = true;
            changed = true;
          }

          if (refreshCount)
          {
            refreshCount = false;
            if (part.textureAsset == null)
            {
              part.count = 0;
            }
            else
            {
              part.count = (part.textureAsset.width * part.textureAsset.height) / (part.width * part.height);
            }
          }

          EditorGUILayout.LabelField("Count", part.count.ToString());
          GUILayout.EndVertical();
        }
        break;
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


  const int kWhiteTexSize = 4;

  void BuildAtlas()
  {
    mAtlas._importData.atlasOutOfDate = false;

    Texture2D whiteTex = null;
    GiraffeAtlasBuilder builder = new GiraffeAtlasBuilder();

    builder.Begin(mAtlas.texture, mAtlas._importData.border, mAtlas._importData.padding);

    if (mAtlas._importData.generateWhiteTexture)
    {
      whiteTex = new Texture2D(kWhiteTexSize, kWhiteTexSize, TextureFormat.ARGB32, false);

      whiteTex.name = "Giraffe/White";
      Color32[] col = new Color32[kWhiteTexSize * kWhiteTexSize];
      for (int i = 0; i < kWhiteTexSize * kWhiteTexSize; i++)
        col[i] = new Color32(255, 255, 255, 255);
      whiteTex.SetPixels32(col);
      whiteTex.Apply(true, false);

      builder.Add(whiteTex.name, whiteTex);
    }

    foreach (var p in mAtlas._importData.parts)
    {
      switch (p.type)
      {
        case GiraffeAtlasImportDataType.None:
        break;
        case GiraffeAtlasImportDataType.Texture2D:
        {
          if (p.textureAsset != null)
          {
            builder.Add(p.textureAsset.name, p.textureAsset);

          }
        }
        break;
        case GiraffeAtlasImportDataType.TileSheetSquare:
        case GiraffeAtlasImportDataType.TilesheetRectangular:
        {
          if (p.textureAsset != null)
          {
            var input = builder.Add(p.textureAsset.name, p.textureAsset, false);
            int x = 0;
            int y = p.textureAsset.height - p.height;
            for (int i = 0; i < p.count; i++)
            {
              input.Add(x, y, p.width, p.height);
              x += p.width;
              if (x >= p.textureAsset.width)
              {
                x = 0;
                y -= p.height;
              }
            }
          }
        }
        break;
      }
    }

    var sprites = builder.End();

    if (whiteTex != null)
    {
      Object.DestroyImmediate(whiteTex);
    }

    mAtlas.ClearSprites();
    foreach (var s in sprites)
    {
      var sprite = new GiraffeSprite();
      sprite.name = s.name;
      sprite.left = s.x;
      sprite.top = s.y;
      sprite.width = s.w;
      sprite.height = s.h;
      sprite.refreshNeeded = true;
      mAtlas.AddSprite(sprite);
    }

    EditorUtility.SetDirty(mAtlas._importData);
    EditorUtility.SetDirty(mAtlas);

    mAtlas.RefreshSprites();

    AssetDatabase.Refresh();
  }

}
