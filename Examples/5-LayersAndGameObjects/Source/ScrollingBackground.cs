using System;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{

  [SerializeField]
  public String[] randomSprites;

  [SerializeField]
  public float speed;

  [NonSerialized]
  private int mSpriteWidth;

  [NonSerialized]
  private int mSpriteHeight;

  [NonSerialized]
  private GiraffeLayer mLayer;

  [NonSerialized]
  private GiraffeSprite[] mSprites;

  [NonSerialized]
  private float mScrollY;

  void Start()
  {
    mLayer = GetComponent<GiraffeLayer>();
    var atlas = mLayer.atlas;
    mSprites = new GiraffeSprite[randomSprites.Length];

    for (int i = 0; i < randomSprites.Length; i++)
    {
      mSprites[i] = atlas.GetSprite(randomSprites[i]);
    }

    mSpriteWidth = mSprites[0].width;
    mSpriteHeight = mSprites[0].height;

    Draw();
  }

  void FixedUpdate()
  {
    mScrollY += Time.deltaTime * speed;
    Draw();
  }

  void Draw()
  {
    int sw = Screen.width / mLayer.scale;
    int sh = Screen.height / mLayer.scale;

    int cols = (sw / mSpriteWidth) + 2;
    int rows = (sh / mSpriteHeight) + 1;


    int scrollY = Mathf.FloorToInt(mScrollY);
    int offset = (int)mScrollY % mSpriteWidth;

    mLayer.Begin(cols * rows);

    int worldI = 0;
    int biome = 0;
    for (int i = 0; i < cols; i++)
    {

      worldI = scrollY + i;
      biome = (worldI / 231);

      uint u = hash((uint)worldI);

      for (int j = 0; j < rows; j++)
      {
        uint h = hash((uint)(worldI ^ j));
        if (h % 13 == j)
        {
          mLayer.Add(i * mSpriteWidth - (int)offset, j * mSpriteHeight, mSprites[(biome + h) % 4]);
        }
        else
        {
          mLayer.Add(i * mSpriteWidth - (int)offset, j * mSpriteHeight, mSprites[0]);
        }
      }
    }

    mLayer.End();
  }

  private uint hash(uint a)
  {
    a = (a ^ 61) ^ (a >> 16);
    a = a + (a << 3);
    a = a ^ (a >> 4);
    a = a * 0x27d4eb2d;
    a = a ^ (a >> 15);
    return a;
  }

}

