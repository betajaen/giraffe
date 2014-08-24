using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GiraffeFont))]
public class GiraffeFontEditor : Editor
{

  [MenuItem("Assets/Create/Giraffe Font")]
  static void CreateAtlas()
  {
    GiraffeFont font = ScriptableObject.CreateInstance<GiraffeFont>();

    string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    if (path == "")
    {
      path = "Assets";
    }
    else if (Path.GetExtension(path) != "")
    {
      path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
    }

    string fontPath = AssetDatabase.GenerateUniqueAssetPath(path + "/New Giraffe Font.asset");

    AssetDatabase.CreateAsset(font, fontPath);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();

    EditorUtility.FocusProjectWindow();
    Selection.activeObject = font;
  }

  private GiraffeFont mFont;
  private int mMode;
  private TextAsset mFontTextAsset;
  private String[] mSpriteNames;
  private int mCurrentSpriteNameId;

  private static GUIStyle msWarningLabelStyle;

  void OnEnable()
  {
    mFont = (GiraffeFont)this.target;
    mMode = 0;
    MakeSpriteNames();
  }

  public override void OnInspectorGUI()
  {

    if (msWarningLabelStyle == null)
    {
      msWarningLabelStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
      msWarningLabelStyle.richText = true;
      msWarningLabelStyle.alignment = TextAnchor.MiddleCenter;
      msWarningLabelStyle.padding = new RectOffset(0, 0, 12, 12);
    }

    GUILayout.BeginVertical();
    GUILayout.Label("Giraffe Font", EditorStyles.boldLabel);
    GUILayout.Space(4);
    GUILayout.EndVertical();
    EditorGUILayout.BeginHorizontal();

    Color col = GUI.backgroundColor;

    if (mMode != 0)
    {
      GUI.backgroundColor = col * 0.75f;
    }

    if (GUILayout.Button("Font and Atlas", EditorStyles.miniButtonLeft))
    {
      mMode = 0;
    }

    if (mMode != 1)
    {
      GUI.backgroundColor = col * 0.75f;
    }
    else
      GUI.backgroundColor = col;

    if (GUILayout.Button("Importers", EditorStyles.miniButtonRight))
    {
      mMode = 1;
    }

    GUI.backgroundColor = col;

    EditorGUILayout.EndHorizontal();

    if (mMode == 0)
      InspectFontAndAtlas();
    else if (mMode == 1)
      InspectImporters();

  }

  private void InspectFontAndAtlas()
  {
    bool changed = false;

    GUILayout.BeginVertical();
    GUILayout.Label("Atlas", EditorStyles.boldLabel);
    EditorGUI.indentLevel++;

    GUI.changed = false;

    mFont.atlas = EditorGUILayout.ObjectField("Atlas", mFont.atlas, typeof(GiraffeAtlas)) as GiraffeAtlas;

    if (GUI.changed)
    {
      MakeSpriteNames();
      mFont.spriteName = mSpriteNames[mCurrentSpriteNameId];
      changed = true;
    }

    GUI.changed = false;

    mCurrentSpriteNameId = EditorGUILayout.Popup("Sprite", mCurrentSpriteNameId, mSpriteNames);

    if (GUI.changed)
    {
      mFont.spriteName = mSpriteNames[mCurrentSpriteNameId];
      mImporterRefreshPreview = true;
      changed = true;
    }

    EditorGUI.indentLevel--;
    GUILayout.EndVertical();

    GUILayout.BeginVertical();
    EditorGUI.indentLevel++;
    GUILayout.Label("Font", EditorStyles.boldLabel);
    EditorGUILayout.LabelField("Glyphs", mFont.glyphs == null ? "0" : mFont.glyphs.Length.ToString());

    GUI.changed = false;
    mFont.spaceAdvance = EditorGUILayout.IntField("Space Advance", mFont.spaceAdvance);
    if (GUI.changed)
      changed = true;

    GUI.changed = false;
    mFont.lineHeight = EditorGUILayout.IntField("Line Height", mFont.lineHeight);
    if (GUI.changed)
      changed = true;

    EditorGUI.indentLevel--;
    GUILayout.EndVertical();

    if (changed)
    {
      EditorUtility.SetDirty(mFont);
    }

  }

  enum ImporterType
  {
    None,
    Grid,
    // AngelCodeBitmapFontGenerator
  }

  private ImporterType mImporterType;
  private TextAsset mImporterAsset;
  private String mImporterString;
  private int mImporterCharWidth;
  private int mImporterCharHeight;
  private bool mImporterRefreshPreview;

  private static readonly String kPreviewText = "Crazy Frederick bought many very exquisite opal jewels.";
  private const int kPreviewTextLength = 55;

  //private static readonly String kPreviewText = @"!""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
  //private const int kPreviewTextLength = 94;

  private Rect[] mPreviewRects = new Rect[kPreviewTextLength];
  private Rect[] mPreviewCoords = new Rect[kPreviewTextLength];
  private int mPreviewRectWidth = 0;
  private int mPreviewRectHeight = 0;

  private void InspectImporters()
  {

    GUILayout.BeginVertical();
    GUILayout.Label("Importer", EditorStyles.boldLabel);
    EditorGUI.indentLevel++;

    GUI.changed = false;

    mImporterType = (ImporterType)EditorGUILayout.EnumPopup("Importer", mImporterType);

    if (GUI.changed)
    {
      mImporterAsset = null;
      mImporterString = @"!""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
      mImporterCharWidth = 9;
      mImporterCharHeight = 9;
      mImporterRefreshPreview = false;
      RefreshGridPreview();
    }

    EditorGUI.indentLevel--;
    GUILayout.EndVertical();

    switch (mImporterType)
    {
      case ImporterType.Grid:
      {
        GUILayout.BeginVertical();
        GUILayout.Label("Grid Importer", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Characters", EditorStyles.textArea);
        EditorGUI.indentLevel--;

        GUI.changed = false;
        mImporterString = EditorGUILayout.TextArea(mImporterString, GUILayout.ExpandWidth(true), GUILayout.Height(96));

        if (GUI.changed)
          mImporterRefreshPreview = true;

        EditorGUI.indentLevel++;
        GUILayout.EndHorizontal();

        GUI.changed = false;
        mImporterCharWidth = EditorGUILayout.IntField("Width", mImporterCharWidth);

        if (GUI.changed)
          mImporterRefreshPreview = true;

        GUI.changed = false;
        mImporterCharHeight = EditorGUILayout.IntField("Height", mImporterCharHeight);

        if (GUI.changed)
          mImporterRefreshPreview = true;

        if (mFont.atlas == null)
        {
          GUILayout.Label("<b>Warning</b> Atlas has not been selected", msWarningLabelStyle);
          GUI.enabled = false;
          GUILayout.Button("Build");
          GUI.enabled = true;
        }
        else
        {
          if (GUILayout.Button(String.Format("Build using {0}", mFont.spriteName)))
          {
            BuildGrid();
          }
        }
        EditorGUI.indentLevel--;
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Preview", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        if (mFont.atlas != null && mFont.atlas.texture != null)
        {
          const int kPadding = 8;

          GUILayout.BeginHorizontal();
          GUILayout.FlexibleSpace();
          GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb, GUILayout.Width(mPreviewRectWidth + kPadding * 2),
          GUILayout.Height(mPreviewRectHeight + kPadding * 2));


          Rect baseRect = GUILayoutUtility.GetRect(mPreviewRectWidth + kPadding * 2, mPreviewRectHeight + kPadding * 2);

          if (GUI.Button(new Rect(baseRect.xMax - 5, baseRect.yMax - 5, 10, 10), String.Empty, EditorStyles.miniButton))
          {
            mImporterRefreshPreview = true;
          }

          GUILayout.EndHorizontal();
          GUILayout.FlexibleSpace();

          GUILayout.EndHorizontal();

          Rect spriteRect = new Rect(baseRect.x + kPadding, baseRect.y + kPadding, mPreviewRectWidth, mPreviewRectHeight);

          for (int i = 0; i < kPreviewTextLength; i++)
          {
            Rect t = new Rect(spriteRect.x + mPreviewCoords[i].x, spriteRect.y + mPreviewCoords[i].y, mPreviewCoords[i].width, mPreviewCoords[i].height);
            GUI.DrawTextureWithTexCoords(t, mFont.atlas.texture, mPreviewRects[i], true);
          }
        }

        EditorGUI.indentLevel--;
        GUILayout.EndVertical();
      }
      break;
      //      case ImporterType.AngelCodeBitmapFontGenerator:
      //      break;
    }

    if (mImporterRefreshPreview)
    {
      switch (mImporterType)
      {
        case ImporterType.Grid:
        {
          RefreshGridPreview();
        }
        break;
      }
    }
  }

  private void MakeSpriteNames()
  {
    GiraffeAtlas._GetNames(mFont.atlas, ref mSpriteNames);
    mCurrentSpriteNameId = GiraffeAtlas._FindSpriteIndex(mFont.atlas, mFont.spriteName);
  }

  void RefreshGridPreview()
  {
    mImporterRefreshPreview = false;

    if (mFont.atlas == null)
    {
      for (int i = 0; i < kPreviewTextLength; i++)
      {
        mPreviewCoords[i] = new Rect(0, 0, 0, 0);
        mPreviewRects[i] = new Rect(0, 0, 0, 0);
      }
      return;
    }

    mPreviewRectWidth = kPreviewTextLength * mImporterCharWidth;
    mPreviewRectHeight = mImporterCharHeight;

    GiraffeSprite sprite = mFont.atlas.GetSprite(mFont.spriteName);
    float invWidth = 1.0f / mFont.atlas.texture.width;
    float invHeight = 1.0f / mFont.atlas.texture.height;
    sprite.Refresh(invWidth, invHeight);

    float cw = mImporterCharWidth * invWidth;
    float ch = mImporterCharHeight * invHeight;

    int charsPerLine = sprite.width / mImporterCharWidth;

    if (charsPerLine == 0)
    {
      for (int i = 0; i < kPreviewTextLength; i++)
      {
        mPreviewCoords[i] = new Rect(0, 0, mImporterCharWidth, mImporterCharHeight);
        mPreviewRects[i] = new Rect(0, 0, 0, 0);
      }
      return;
    }

    for (int i = 0; i < kPreviewTextLength; i++)
    {
      mPreviewCoords[i] = new Rect(i * mImporterCharWidth, 0, mImporterCharWidth, mImporterCharHeight);

      char c = kPreviewText[i];
      int p = mImporterString.IndexOf(c);
      if (p == -1)
      {
        mPreviewRects[i] = new Rect(0, 0, 0, 0);
        continue;
      }

      int x = p % charsPerLine;
      int y = p / charsPerLine;

      float x0 = sprite.x0 + cw * x;
      float y0 = sprite.y1 - (ch * y) - ch;

      mPreviewRects[i] = new Rect(x0, y0, cw, ch);
    }


  }

  void BuildGrid()
  {
    int glyphCount = mImporterString.Length;
    mFont.glyphs = new GiraffeFontGlyph[glyphCount];

    GiraffeSprite sprite = mFont.atlas.GetSprite(mFont.spriteName);
    int charsPerLine = sprite.width / mImporterCharWidth;

    for (int i = 0; i < glyphCount; i++)
    {
      int cx = i % charsPerLine;
      int cy = i / charsPerLine;

      GiraffeFontGlyph glyph = new GiraffeFontGlyph()
      {
        character = mImporterString[i],
        width = mImporterCharWidth,
        height = mImporterCharHeight,
        x = cx * mImporterCharWidth,
        y = sprite.height - ((cy + 1) * mImporterCharHeight),
        xAdvance = mImporterCharWidth,
        xOffset = 0,
        yOffset = 0
      };

      mFont.glyphs[i] = glyph;
    }

    mFont.lineHeight = mImporterCharHeight;
    mFont.spaceAdvance = mImporterCharWidth;

    EditorUtility.SetDirty(mFont);
    mImporterType = ImporterType.None;
  }

}
