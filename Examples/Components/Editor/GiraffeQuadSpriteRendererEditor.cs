using System;
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(GiraffeQuadSpriteRenderer))]
public class GiraffeQuadSpriteRendererEditor : Editor
{

  private GiraffeQuadSpriteRenderer mRenderer;
  private GiraffeLayer mLayer;
  private GiraffeAtlas mAtlas;
  private String[] mSpriteNames;
  private int mCurrentSpriteNameId;

  void OnEnable()
  {
    mRenderer = (GiraffeQuadSpriteRenderer)this.target;

    FindParts();
    CacheSprites();
  }

  void FindParts()
  {
    mLayer = GiraffeInternal.GiraffeUtils.FindRecursiveComponentBackwards<GiraffeLayer>(mRenderer.transform);
    mAtlas = mLayer.atlas;
  }

  void CacheSprites()
  {
    GiraffeAtlas._GetNames(mAtlas, ref mSpriteNames);
    mCurrentSpriteNameId = GiraffeAtlas._FindSpriteIndex(mAtlas, mRenderer.spriteName);
  }

  public override void OnInspectorGUI()
  {
    GUILayout.BeginVertical();
    GUILayout.Label("Quad", EditorStyles.boldLabel);
    GUILayout.Space(4);
    EditorGUI.indentLevel++;

    GUI.changed = false;

    mCurrentSpriteNameId = EditorGUILayout.Popup("Sprite", mCurrentSpriteNameId, mSpriteNames);

    if (GUI.changed)
    {
      mRenderer.spriteName = mSpriteNames[mCurrentSpriteNameId];
    }

    EditorGUI.indentLevel--;
    GUILayout.EndVertical();

  }
}
