using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(GiraffeLayer))]
public class GiraffeLayerEditor : Editor
{
  public override void OnInspectorGUI()
  {
    GiraffeLayer t = (GiraffeLayer)this.target;

    GUI.changed = false;
    GiraffeAtlas atlas = EditorGUILayout.ObjectField("Atlas", t.atlas, typeof(GiraffeAtlas), false) as GiraffeAtlas;
    if (GUI.changed)
    {
      t.atlas = atlas;
      if (Application.isPlaying == false)
      {
        EditorUtility.SetDirty(t);
      }
    }

    GUI.changed = false;
    int zOrder = EditorGUILayout.IntField("Z-Order", t.zOrder);
    if (GUI.changed)
    {
      t.zOrder = zOrder;
      if (Application.isPlaying == false)
      {
        EditorUtility.SetDirty(t);
      }
    }

    GUI.changed = false;
    int scale = EditorGUILayout.IntSlider("Scale", t.scale, 1, 8);
    if (GUI.changed)
    {
      t.scale = scale;
      if (Application.isPlaying == false)
      {
        EditorUtility.SetDirty(t);
      }
    }


  }
}
