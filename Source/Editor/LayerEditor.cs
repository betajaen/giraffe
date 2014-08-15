using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(GiraffeLayer))]
public class GiraffeLayerEditor : Editor
{
  public override void OnInspectorGUI()
  {
    GiraffeLayer t = (GiraffeLayer)this.target;

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
