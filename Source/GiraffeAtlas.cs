using System;
using System.Collections.Generic;
using GorillaInternal;
using UnityEngine;
using GiraffeInternal;

namespace GorillaInternal
{
}

[Serializable]
public class GiraffeSprite
{
  [SerializeField]
  public String name;

  [NonSerialized]
  public float x0;

  [NonSerialized]
  public float y0;

  [NonSerialized]
  public float x1;

  [NonSerialized]
  public float y1;

  [SerializeField]
  public int left;

  [SerializeField]
  public int top;

  [SerializeField]
  public int width;

  [SerializeField]
  public int height;
}

[Serializable]
public class GiraffeAtlas : ScriptableObject
{

  [SerializeField]
  public int atlasIdA;

  [SerializeField]
  public int atlasIdB;

  [SerializeField]
  public List<GiraffeSprite> sprites;

  [SerializeField]
  public Texture2D texture;

  [NonSerialized]
  public GiraffeImportData _importData;

  [NonSerialized]
  public int _importDataResolved;

  [SerializeField]
  private String mEditorData;

  [NonSerialized]
  private Material mMaterial;

  void OnEnable()
  {
    if (sprites == null)
    {
      sprites = new List<GiraffeSprite>(4);
    }
  }

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

}

