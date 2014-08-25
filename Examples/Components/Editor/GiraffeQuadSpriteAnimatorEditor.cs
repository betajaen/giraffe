using System;
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(GiraffeQuadSpriteAnimator))]
public class GiraffeQuadSpriteAnimatorEditor : Editor
{

  private GiraffeQuadSpriteAnimator mAnimator;

  void OnEnable()
  {
    mAnimator = (GiraffeQuadSpriteAnimator)this.target;
  }



  public override void OnInspectorGUI()
  {
    bool changed = false;
    GUILayout.BeginVertical();
    GUILayout.Label("Animator", EditorStyles.boldLabel);
    GUILayout.Space(4);
    EditorGUI.indentLevel++;

    GUI.changed = false;
    mAnimator.playing = EditorGUILayout.Toggle("Playing", mAnimator.playing);
    if (GUI.changed)
      changed = true;

    GUI.changed = false;

    mAnimator.animation = EditorGUILayout.ObjectField("Animation", mAnimator.animation, typeof(GiraffeSpriteAnimation), false) as GiraffeSpriteAnimation;

    if (GUI.changed)
    {
      changed = true;
    }

    EditorGUI.indentLevel--;
    GUILayout.EndVertical();

    if (changed)
    {
      EditorUtility.SetDirty(mAnimator);
    }

  }
}
