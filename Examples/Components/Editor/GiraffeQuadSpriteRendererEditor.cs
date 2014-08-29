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
  private bool mIsPrefab;

  void OnEnable()
  {
    mRenderer = (GiraffeQuadSpriteRenderer)this.target;
    FindParts();
    CacheSprites();
  }

  void FindParts()
  {
    mLayer = GiraffeInternal.GiraffeUtils.FindRecursiveComponentBackwards<GiraffeLayer>(mRenderer.transform);
    if (mLayer != null)
    {
      mAtlas = mLayer.atlas;
    }
  }

  void CacheSprites()
  {
    if (mIsPrefab == false)
    {
      GiraffeAtlas._GetNames(mAtlas, ref mSpriteNames);
      mCurrentSpriteNameId = GiraffeAtlas._FindSpriteIndex(mAtlas, mRenderer.spriteName);
    }
  }

  public override void OnInspectorGUI()
  {
    bool changed = false;

    GUILayout.BeginVertical();
    GUILayout.Label("Sprite", EditorStyles.boldLabel);
    GUILayout.Space(4);
    EditorGUI.indentLevel++;

    if (mLayer != null)
    {
      GUI.changed = false;
      mCurrentSpriteNameId = EditorGUILayout.Popup("Sprite", mCurrentSpriteNameId, mSpriteNames);
      if (GUI.changed)
      {
        mRenderer.spriteName = mSpriteNames[mCurrentSpriteNameId];
        changed = true;
      }
    }
    else
    {
      GUI.changed = false;
      mRenderer.spriteName = EditorGUILayout.TextField("Sprite", mRenderer.spriteName);
      if (GUI.changed)
      {
        changed = true;
      }
    }

    GUI.changed = false;
    mRenderer.scale = EditorGUILayout.FloatField("Scale", mRenderer.scale);
    if (GUI.changed)
    {
      changed = true;
    }

    EditorGUI.indentLevel--;

    GUILayout.EndVertical();


    BoxCollider2D collider2D = mRenderer.GetComponent<BoxCollider2D>();

    if (mLayer != null && collider2D != null && mAtlas != null)
    {
      GUILayout.BeginVertical();
      GUILayout.Label("Collider", EditorStyles.boldLabel);
      GUILayout.Space(4);
      EditorGUI.indentLevel++;

      if (GUILayout.Button("Resize Collider to sprite"))
      {
        GiraffeSprite sprite = mAtlas.GetSprite(mRenderer.spriteName);
        collider2D.size = new Vector2(sprite.width, sprite.height);
        changed = true;
      }

      EditorGUI.indentLevel--;

      GUILayout.EndVertical();
    }

    if (changed)
    {
      EditorUtility.SetDirty(mRenderer);
    }

  }
}
