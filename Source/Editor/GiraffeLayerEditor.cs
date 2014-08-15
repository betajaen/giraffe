using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(GiraffeLayer))]
public class GiraffeLayerEditor : Editor
{
  public override void OnInspectorGUI()
  {
    GiraffeLayer t = (GiraffeLayer)this.target;

    GiraffeAtlas atlas = EditorGUILayout.ObjectField("Atlas", t.atlas, typeof(GiraffeAtlas)) as GiraffeAtlas;
    if (atlas != t.atlas)
    {
      t.atlas = atlas;
      if (Application.isPlaying == false)
      {
        EditorUtility.SetDirty(t);
      }
    }


    int zOrder = EditorGUILayout.IntField("Z-Order", t.zOrder);
    if (zOrder != t.zOrder)
    {
      t.zOrder = zOrder;
      if (Application.isPlaying == false)
      {
        EditorUtility.SetDirty(t);
      }
    }
  }
}
