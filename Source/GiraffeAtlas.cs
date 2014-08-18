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

