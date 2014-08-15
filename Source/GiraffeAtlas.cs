using System;
using UnityEngine;

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


}

