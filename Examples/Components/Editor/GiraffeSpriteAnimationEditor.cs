using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(GiraffeSpriteAnimation))]
public class GiraffeQuadSpriteAnimationEditor : Editor
{

  [MenuItem("Assets/Create/Giraffe Sprite Animation")]
  static void CreateAtlas()
  {
    GiraffeSpriteAnimation font = ScriptableObject.CreateInstance<GiraffeSpriteAnimation>();

    string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    if (path == "")
    {
      path = "Assets";
    }
    else if (Path.GetExtension(path) != "")
    {
      path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
    }

    string fontPath = AssetDatabase.GenerateUniqueAssetPath(path + "/New Giraffe Spite Animation.asset");

    AssetDatabase.CreateAsset(font, fontPath);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();

    EditorUtility.FocusProjectWindow();
    Selection.activeObject = font;
  }


  private GiraffeSpriteAnimation mAnimation;
  private float mAnimationTime;
  private bool mAnimationPlay;
  private GiraffeSprite mAnimationSprite;
  private int mAnimationSpriteId;
  private bool mAnimation2XZoom;

  void OnEnable()
  {
    mAnimation = (GiraffeSpriteAnimation)this.target;
    GiraffeAtlas._GetNames(mAnimation.atlas, ref mSpriteNames);
    RefreshFrameNamesIds();
    if (mAnimation.atlas != null)
    {
      mAnimation.atlas.RefreshSprites();
      mAnimation.FetchSprites();
    }
    RefreshPreview();
  }

  private Vector2 mFramesScroll;
  private String[] mSpriteNames;
  private int[] mFrameNameIds;

  public override void OnInspectorGUI()
  {
    GUILayout.BeginVertical();
    GUILayout.Label("Animation", EditorStyles.boldLabel);
    GUILayout.Space(4);
    EditorGUI.indentLevel++;

    bool changed = false;

    EditorGUILayout.LabelField("Name", mAnimation.name);

    GUI.changed = false;
    mAnimation.atlas = EditorGUILayout.ObjectField("Atlas", mAnimation.atlas, typeof(GiraffeAtlas), false) as GiraffeAtlas;
    if (GUI.changed)
    {
      GiraffeAtlas._GetNames(mAnimation.atlas, ref mSpriteNames);
      if (mAnimation.atlas != null)
      {
        mAnimation.atlas.RefreshSprites();
      }
      changed = true;
    }

    GUI.changed = false;
    mAnimation.length = EditorGUILayout.FloatField("Length", mAnimation.length);
    if (GUI.changed)
      changed = true;

    GUI.changed = false;
    mAnimation.mode = (GiraffeAnimationMode)EditorGUILayout.EnumPopup("Mode", mAnimation.mode);
    if (GUI.changed)
      changed = true;


    EditorGUI.indentLevel--;

    GUILayout.BeginVertical();
    GUILayout.Label("Preview", EditorStyles.boldLabel);
    GUILayout.Space(4);
    GUILayout.EndVertical();

    GUILayout.BeginVertical(EditorStyles.objectFieldThumb, GUILayout.Height(128 + 30));

    GUILayout.BeginHorizontal(EditorStyles.toolbar);

    mAnimationPlay = GUILayout.Toggle(mAnimationPlay, mAnimationPlay ? "||" : "\u25B6", EditorStyles.toolbarButton,
      GUILayout.Width(25));

    if (GUI.changed)
    {
      RefreshPreview();
    }

    if (GUILayout.Button("|\u25C0", EditorStyles.toolbarButton, GUILayout.Width(25)))
    {
      mAnimationTime = 0.0f;
    }

    GUI.changed = false;
    mAnimationTime = GUILayout.HorizontalSlider(mAnimationTime, 0.0f, mAnimation.length);
    if (GUI.changed)
    {
      RefreshPreview();
    }

    mAnimation2XZoom = GUILayout.Toggle(mAnimation2XZoom, "2x", EditorStyles.toolbarButton, GUILayout.Width(25));

    int spriteWidth = 0, spriteHeight = 0;

    if (mAnimationSprite != null)
    {
      spriteWidth = mAnimationSprite.width;
      spriteHeight = mAnimationSprite.height;
    }

    if (mAnimation2XZoom)
    {
      spriteWidth *= 2;
      spriteHeight *= 2;
    }

    GUILayout.EndHorizontal();

    GUILayout.FlexibleSpace();
    GUILayout.BeginHorizontal();
    GUILayout.FlexibleSpace();

    GUILayout.BeginHorizontal(GUILayout.Width(spriteWidth), GUILayout.Height(spriteHeight));
    if (mAnimationSprite != null)
    {
      Rect baseRect = GUILayoutUtility.GetRect(spriteWidth, spriteHeight);

      GUI.DrawTextureWithTexCoords(baseRect, mAnimation.atlas.texture,
        new Rect(mAnimationSprite.x0, mAnimationSprite.y0, mAnimationSprite.x1 - mAnimationSprite.x0, mAnimationSprite.y1 - mAnimationSprite.y0), true);

    }
    GUILayout.EndHorizontal();
    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
    GUILayout.FlexibleSpace();
    GUILayout.Label(String.Format("{0} {1:F2}s", mAnimationSpriteId, mAnimationTime), EditorStyles.miniLabel);
    GUILayout.EndVertical();


    GUILayout.BeginVertical();
    GUILayout.Label("Frames", EditorStyles.boldLabel);
    GUILayout.Space(4);
    GUILayout.EndVertical();


    mFramesScroll = GUILayout.BeginScrollView(mFramesScroll);
    EditorGUI.indentLevel++;

    for (int i = 0; i < mAnimation.frames.Count; i++)
    {
      GUILayout.BeginHorizontal(EditorStyles.toolbar);
      GUILayout.Label(i.ToString(), EditorStyles.miniLabel, GUILayout.Width(25));
      GUI.changed = false;
      int newId = EditorGUILayout.Popup(mFrameNameIds[i], mSpriteNames, EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(true));
      if (GUI.changed)
      {
        mFrameNameIds[i] = newId;
        mAnimation.frames[i] = mSpriteNames[mFrameNameIds[i]];
        mAnimation.FetchSprites();
        RefreshPreview();
        changed = true;
      }

      GUI.enabled = false;
      if (i != 0)
        GUI.enabled = true;

      if (GUILayout.Button("\u25B2", EditorStyles.toolbarButton, GUILayout.Width(25)))
      {
        String a = mAnimation.frames[i];
        String b = mAnimation.frames[i - 1];
        mAnimation.frames[i] = b;
        mAnimation.frames[i - 1] = a;
        mAnimation.FetchSprites();
        RefreshFrameNamesIds();
        RefreshPreview();
        changed = true;

      }
      GUI.enabled = false;
      if ((i != mAnimation.frames.Count - 1))
        GUI.enabled = true;
      if (GUILayout.Button("\u25BC", EditorStyles.toolbarButton, GUILayout.Width(25)))
      {
        String a = mAnimation.frames[i];
        String b = mAnimation.frames[i + 1];
        mAnimation.frames[i] = b;
        mAnimation.frames[i + 1] = a;
        mAnimation.FetchSprites();
        RefreshFrameNamesIds();
        RefreshPreview();
        changed = true;
      }
      GUI.enabled = false;

      if (mAnimation.frames.Count > 1)
        GUI.enabled = true;

      if (GUILayout.Button("\u00D7", EditorStyles.toolbarButton, GUILayout.Width(25)))
      {
        mAnimation.frames.RemoveAt(i);
        mAnimation.FetchSprites();
        RefreshFrameNamesIds();
        RefreshPreview();
        changed = true;
        break;
      }
      GUI.enabled = true;

      GUILayout.EndHorizontal();
    }

    GUILayout.BeginHorizontal(EditorStyles.toolbar);
    GUILayout.FlexibleSpace();

    if (GUILayout.Button("\u002B", EditorStyles.toolbarButton, GUILayout.Width(25)))
    {

      String nextName = "Giraffe/White";
      String lastName = mAnimation.frames[mAnimation.frames.Count - 1];

      String[] parts = lastName.Split('/');
      int frameNumber = 0;
      if (Int32.TryParse(parts[parts.Length - 1], out frameNumber))
      {
        parts[parts.Length - 1] = (frameNumber + 1).ToString();
        String n = String.Join("/", parts);

        if (GiraffeAtlas._ContainsSprite(mAnimation.atlas, n))
        {
          nextName = n;
        }
      }

      mAnimation.frames.Add(nextName);
      RefreshFrameNamesIds();
    }

    GUILayout.EndHorizontal();

    GUILayout.EndScrollView();

    EditorGUI.indentLevel--;
    GUILayout.EndVertical();

    if (changed)
      EditorUtility.SetDirty(mAnimation);

  }

  void RefreshFrameNamesIds()
  {
    if (mFrameNameIds == null || mFrameNameIds.Length != mAnimation.frames.Count)
      mFrameNameIds = new int[mAnimation.frames.Count];

    for (int i = 0; i < mAnimation.frames.Count; i++)
    {
      mFrameNameIds[i] = 0;
      for (int j = 0; j < mSpriteNames.Length; j++)
      {
        if (mSpriteNames[j] == mAnimation.frames[i])
        {
          mFrameNameIds[i] = j;
          break;
        }
      }
    }
  }

  void RefreshPreview()
  {
    mAnimationSprite = null;

    if (mAnimation.atlas != null)
    {
      bool isPlayingIgnored = true;
      mAnimationSpriteId = GiraffeSpriteAnimation.Animate(mAnimation, mAnimationTime, ref isPlayingIgnored);
      mAnimationSprite = mAnimation.frameSprites[mAnimationSpriteId];
    }
  }



}
