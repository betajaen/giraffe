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

  [SerializeField]
  public int scale = 1;

  [SerializeField]
  public int offsetX = 0;

  [SerializeField]
  public int offsetY = 0;

  public Vector2 size
  {
    get { return new Vector2(width, height); }
  }

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
  private List<GiraffeSprite> mSprites;

  [SerializeField]
  public Texture2D texture;

  [NonSerialized]
  public GiraffeImportData _importData;

  [NonSerialized]
  public int _importDataResolved;

  [NonSerialized]
  private Material mMaterial;

  [SerializeField]
  public bool useCustomMaterial;

  [SerializeField]
  public Material customMaterial;

  [NonSerialized]
  private GiraffeSprite mWhiteSprite;

  void OnEnable()
  {
    if (mSprites == null)
    {
      mSprites = new List<GiraffeSprite>(4);
    }
  }

  public IEnumerable<GiraffeSprite> sprites
  {
    get
    {
      return mSprites;
    }
  }

  public int spritesCount
  {
    get
    {
      return mSprites.Count;
    }
  }

  public Material material
  {
    get
    {
      if (mMaterial == null)
      {
        if (customMaterial)
        {
          mMaterial = customMaterial;
        }
        else
        {
          Shader shader = Shader.Find("Giraffe/Standard");
          mMaterial = new Material(shader);
        }
        mMaterial.mainTexture = texture;
      }
      return mMaterial;
    }
  }

  public void ClearSprites()
  {
    mSprites.Clear();
    mWhiteSprite = null;
  }

  public void AddSprite(GiraffeSprite sprite)
  {
    mSprites.Add(ProcessSprite(sprite));
  }

  public GiraffeSprite GetSpriteAt(int index)
  {
    if (index < 0 || index >= mSprites.Count)
      return whiteSprite;

    return ProcessSprite(mSprites[index]);
  }

  public GiraffeSprite GetSprite(String name)
  {
    GiraffeSprite sprite = FindSprite(name);
    if (sprite == null)
    {
      sprite = whiteSprite;
    }
    return ProcessSprite(sprite);
  }

  private GiraffeSprite FindSprite(String name)
  {
    int spriteCount = mSprites.Count;
    for (int i = 0; i < spriteCount; i++)
    {
      if (mSprites[i].name == name)
        return ProcessSprite(mSprites[i]);
    }
    return null;
  }

  GiraffeSprite ProcessSprite(GiraffeSprite sprite)
  {
    if (sprite.refreshNeeded)
    {
      float invTexWidth = 1.0f / texture.width;
      float invTexHeight = 1.0f / texture.height;
      sprite.Refresh(invTexWidth, invTexHeight);
    }
    return sprite;
  }

  public GiraffeSprite whiteSprite
  {
    get
    {
      if (mWhiteSprite == null)
      {
        mWhiteSprite = FindSprite("Giraffe/White");
        if (mWhiteSprite == null)
        {
          mWhiteSprite = new GiraffeSprite()
          {
            width = 1,
            height = 1,
            refreshNeeded = true
          };
        }
      }
      return ProcessSprite(mWhiteSprite);
    }
  }

  public void RefreshSprites()
  {
    foreach (var sprite in mSprites)
    {
      float invTexWidth = 1.0f / texture.width;
      float invTexHeight = 1.0f / texture.height;
      sprite.Refresh(invTexWidth, invTexHeight);
    }
  }

  public static void _GetNames(GiraffeAtlas atlas, ref String[] names)
  {
    if (atlas == null)
    {
      names = new String[1]
      {
        "Giraffe/White"
      };
      return;
    }

    if (names == null || names.Length != atlas.mSprites.Count)
    {
      names = new String[atlas.mSprites.Count];
    }

    for (int i = 0; i < names.Length; i++)
    {
      names[i] = atlas.mSprites[i].name;
    }
  }

  public static int _FindSpriteIndex(GiraffeAtlas atlas, String spriteName)
  {
    if (atlas == null || String.IsNullOrEmpty(spriteName))
      return 0;

    for (int i = 0; i < atlas.mSprites.Count; i++)
    {
      if (spriteName == atlas.mSprites[i].name)
      {
        return i;
      }
    }
    return 0;
  }

  public static bool _ContainsSprite(GiraffeAtlas atlas, String spriteName)
  {
    if (atlas == null || String.IsNullOrEmpty(spriteName))
      return false;

    for (int i = 0; i < atlas.mSprites.Count; i++)
    {
      if (spriteName == atlas.mSprites[i].name)
      {
        return true;
      }
    }
    return false;
  }
}


