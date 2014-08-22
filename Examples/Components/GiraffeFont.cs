using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class GiraffeFontGlyph
{
  [NonSerialized]
  private GiraffeSprite sprite;

  [SerializeField]
  public String spriteName;

  [SerializeField]
  public int x;

  [SerializeField]
  public int y;

  [SerializeField]
  public int width;

  [SerializeField]
  public int height;

  [SerializeField]
  public int xOffset;

  [SerializeField]
  public int yOffset;

  [SerializeField]
  public int xAdvance;

  [SerializeField]
  public int[] kerning;
}

public class GiraffeFont : ScriptableObject
{

  [SerializeField]
  public GiraffeFontGlyph[] glyphs;

  [SerializeField]
  public int spaceAdvance;

  [SerializeField]
  public int lineHeight;

  [SerializeField]
  public GiraffeAtlas atlas;

  [SerializeField]
  public String spriteName;

  void OnEnable()
  {

  }

}
