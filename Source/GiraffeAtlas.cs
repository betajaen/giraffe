using System;
using System.Collections.Generic;
using UnityEngine;


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

  [NonSerialized]
  public bool refreshNeeded;

  public GiraffeSprite()
  {
    refreshNeeded = true;
  }

  public void Refresh(float invTexWidth, float invTexHeight)
  {
    refreshNeeded = false;
    x0 = left * invTexWidth;
    x1 = (left + width) * invTexWidth;
    y0 = (top * invTexHeight);
    y1 = ((top + height) * invTexHeight);
  }

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

  [NonSerialized]
  public GiraffeSprite whiteSprite;

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

  public GiraffeSprite GetSprite(String name)
  {
    GiraffeSprite sprite = FindSprite(name);
    if (sprite == null)
    {
      if (whiteSprite == null)
      {
        whiteSprite = FindSprite("Giraffe/White");
        if (whiteSprite == null)
        {
          whiteSprite = new GiraffeSprite()
          {
            width = 1,
            height = 1,
            refreshNeeded = true
          };
        }
      }
      sprite = whiteSprite;
    }
    if (sprite.refreshNeeded)
    {

      float invTexWidth = 1.0f / mMaterial.mainTexture.width;
      float invTexHeight = 1.0f / mMaterial.mainTexture.height;
      sprite.Refresh(invTexWidth, invTexHeight);
    }
    return sprite;
  }

  private GiraffeSprite FindSprite(String name)
  {
    int spriteCount = sprites.Count;
    for (int i = 0; i < spriteCount; i++)
    {
      if (sprites[i].name == name)
        return sprites[i];
    }
    return null;
  }

  public void RefreshSprites()
  {
    foreach (var sprite in sprites)
    {
      float invTexWidth = 1.0f / mMaterial.mainTexture.width;
      float invTexHeight = 1.0f / mMaterial.mainTexture.height;
      sprite.Refresh(invTexWidth, invTexHeight);
    }
  }

}


