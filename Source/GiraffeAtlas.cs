using System;
using UnityEngine;
using GiraffeInternal;

namespace GiraffeInternal
{

  [Serializable]
  public class GiraffaAtlasEditorSpriteImport
  {
    [SerializeField]
    public String name;

    [SerializeField]
    public Texture2D texture;
  }

  [Serializable]
  public class GiraffaAtlasEditorData
  {
    [SerializeField]
    public GiraffaAtlasEditorSpriteImport sprites;

  }

}

[Serializable]
public class GiraffeSprite
{
  [SerializeField]
  public String name;

  [SerializeField]
  public int x0;

  [SerializeField]
  public int y0;

  [SerializeField]
  public int x1;

  [SerializeField]
  public int y1;
}

[Serializable]
public class GiraffeAtlas : ScriptableObject
{
  [SerializeField]
  public GiraffeSprite[] sprites;

  [SerializeField]
  public Texture2D texture;

  [SerializeField]
  public bool textureWritable;

  [SerializeField]
  private GiraffaAtlasEditorData mEditorData;

  [NonSerialized]
  private Material mMaterial;

  public Material material
  {
    get
    {
      if (mMaterial == null)
      {
        Shader shader = Shader.Find("Giraffe/Standard");
        mMaterial = new Material(shader);
        mMaterial.mainTexture = texture;
      }
      return mMaterial;
    }
  }

#if UNITY_EDITOR

  public GiraffaAtlasEditorData editorData
  {
    get { return mEditorData; }
  }

  void _CreateEditorData()
  {
    if (editorData == null)
    {
      mEditorData = new GiraffaAtlasEditorData();
    }
  }

#endif

}

