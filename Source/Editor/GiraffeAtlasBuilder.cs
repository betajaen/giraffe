using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


class GiraffeAtlasBuilder
{

  private int mPadding;
  private int mBorder;

  class SpriteInput
  {
    public String name;
    public int x, y, w, h;
    public Texture2D texture;
  }

  class SpriteProcessed
  {
    public Texture2D original;
    public Texture2D image;
    public bool isModified;
    public int border;

    public struct Quad
    {
      // x, y, w, h are relative to the original image.
      // originX, originY is relative to x, y
      public int x, y, w, h;
      public String name;
    }

    public List<Quad> quads;

    public SpriteProcessed()
    {
      quads = new List<Quad>(1);
    }

    public void Release()
    {
      if (isModified && image != null)
      {
        UnityEngine.Object.DestroyImmediate(image);
      }
    }
  }

  public struct SpriteOutput
  {
    public String name;
    public int x, y, w, h;
  }

  private List<SpriteInput> mInputs;
  private List<SpriteProcessed> mProcessed;
  private List<SpriteOutput> mOutputs;
  private Texture2D[] mTexturesToPack;

  private Texture2D mOutputImage;

  public GiraffeAtlasBuilder()
  {
    mPadding = 2;
    mBorder = 2;
    mInputs = new List<SpriteInput>(4);
    mProcessed = new List<SpriteProcessed>(4);
    mOutputs = new List<SpriteOutput>(4);
  }

  public void Begin(Texture2D target, int border, int padding)
  {
    Release();
    mBorder = border;
    mPadding = padding;
    mOutputImage = target;
  }

  public void Add(String name, Texture2D texture, int x, int y, int w, int h)
  {
    SpriteInput input = new SpriteInput
    {
      name = name,
      texture = texture,
      x = x,
      y = y,
      w = w,
      h = h
    };
    mInputs.Add(input);
  }

  public List<SpriteOutput> End()
  {
    ProcessInputs();
    ProcessBorders(mBorder);
    PackTextures(mPadding);
    var output = new List<SpriteOutput>(mOutputs);
    Release();
    return output;
  }

  void Release()
  {
    mInputs.Clear();
    foreach (var processed in mProcessed)
    {
      processed.Release();
    }
    mProcessed.Clear();
    mOutputs.Clear();
    mOutputImage = null;
    mTexturesToPack = null;
  }

  void ProcessInputs()
  {
    foreach (var input in mInputs)
    {
      var processed = FindProcessedByOriginalImage(input.texture);
      if (processed == null)
      {
        processed = new SpriteProcessed();
        processed.original = input.texture;
        mProcessed.Add(processed);
      }

      SpriteProcessed.Quad quad = new SpriteProcessed.Quad();
      quad.x = input.x;
      quad.y = input.y;
      quad.w = input.w;
      quad.h = input.h;
      quad.name = input.name;
      processed.quads.Add(quad);
    }
  }

  int mod(int x, int m)
  {
    return (x % m + m) % m;
  }

  void ProcessBorders(int border)
  {
    if (border == 0)
    {
      foreach (var p in mProcessed)
      {
        p.image = p.original;
        p.isModified = false;
        p.border = 0;
      }
      return;
    }

    foreach (var p in mProcessed)
    {
      int originalWidth = p.original.width;
      int originalHeight = p.original.height;
      int imageWidth = p.original.width + border * 2;
      int imageHeight = p.original.height + border * 2;
      int imageSize = imageWidth * imageHeight;
      Color32[] original = p.original.GetPixels32();
      Color32[] image = new Color32[imageSize];

      // temp, magenta border.
      for (int i = 0; i < imageSize; i++)
      {
        image[i] = new Color32(255, 0, 255, 255); // temp.
      }

      //   Copy image. (Could probably do this in one run, using a modulus - so it wraps around x/y)
      for (int j = 0; j < imageHeight; j++)
      {
        for (int i = 0; i < imageWidth; i++)
        {
          int ox = mod(i - border, originalWidth);
          int oy = mod(j - border, originalHeight);

          int oit = ox + (oy * originalWidth);
          Color32 s = original[oit];

          int iit = i + (j * imageWidth);
          image[iit] = s;
        }
      }

      p.image = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, false, false);
      p.image.SetPixels32(image, 0);
      p.image.Apply(true, false);
      p.isModified = true;
      p.border = border;
    }
  }

  SpriteProcessed FindProcessedByOriginalImage(Texture2D texture)
  {
    foreach (var p in mProcessed)
    {
      if (p.original == texture)
        return p;
    }
    return null;
  }

  void PackTextures(int padding)
  {
    mTexturesToPack = new Texture2D[mProcessed.Count];

    for (int i = 0; i < mProcessed.Count; i++)
    {
      mTexturesToPack[i] = mProcessed[i].image;
    }


    Texture2D texture = new Texture2D(4, 4);
    Rect[] textureRectangles = texture.PackTextures(mTexturesToPack, padding);

    int texWidth = texture.width;
    int texHeight = texture.height;

    byte[] bytes = texture.EncodeToPNG();
    System.IO.FileStream fs = System.IO.File.Create(AssetDatabase.GetAssetPath(mOutputImage));
    fs.Write(bytes, 0, bytes.Length);
    fs.Close();

    UnityEngine.Object.DestroyImmediate(texture);
    bytes = null;

    AssetDatabase.Refresh();

    for (int i = 0; i < mProcessed.Count; i++)
    {
      var p = mProcessed[i];
      var r = textureRectangles[i];
      int originX = (int)(r.x * texWidth) + p.border;
      int originY = (int)(r.y * texHeight) + p.border;
      foreach (var q in p.quads)
      {
        SpriteOutput output = new SpriteOutput();
        output.name = q.name;
        output.x = originX + q.x;
        output.y = originY + q.y;
        output.w = q.w;
        output.h = q.h;
        mOutputs.Add(output);
      }
    }

  }


}

