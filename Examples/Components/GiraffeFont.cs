using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[Serializable]
public class GiraffeFontGlyph
{
  [NonSerialized]
  public GiraffeSprite sprite;

  [SerializeField]
  public int character;

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

  [NonSerialized]
  public Dictionary<char, GiraffeFontGlyph> characters;

  [NonSerialized]
  public int characterRangeMin;

  [NonSerialized]
  public int characterRangeMax;

  [NonSerialized]
  public GiraffeSprite sprite;

  private void OnEnable()
  {
    if (Application.isPlaying)
    {
      Initialise();
    }
  }

  void Initialise()
  {

    sprite = atlas.GetSprite(spriteName);

    float invTexWidth = 1.0f / atlas.texture.width;
    float invTexHeight = 1.0f / atlas.texture.height;

    characters = new Dictionary<char, GiraffeFontGlyph>(glyphs.Length);
    characterRangeMin = char.MaxValue;
    characterRangeMax = char.MinValue;

    for (int i = 0; i < glyphs.Length; i++)
    {
      GiraffeFontGlyph glyph = glyphs[i];

      char c = (char)glyph.character;
      characters.Add(c, glyph);

      if (c < characterRangeMin)
        characterRangeMin = c;

      if (c > characterRangeMax)
        characterRangeMax = c;


      GiraffeSprite glyphSprite = new GiraffeSprite();
      glyph.sprite = glyphSprite;

      glyphSprite.left = glyph.x + sprite.left;
      glyphSprite.top = glyph.y + sprite.top;
      glyphSprite.width = glyph.width;
      glyphSprite.height = glyph.height;

      glyphSprite.Refresh(invTexWidth, invTexHeight);
    }

  }

  public int Estimate(String text)
  {
    if (characters == null)
      Initialise();

    int length = text.Length;
    int estimation = 0;
    for (int i = 0; i < length; i++)
    {
      char c = text[i];
      if (c >= characterRangeMin && c <= characterRangeMax && characters.ContainsKey(c))
        estimation++;
    }
    return estimation;
  }

  public void AddTo(GiraffeLayer layer, int x, int y, String text)
  {
    int length = text.Length;

    int p = x;

    for (int i = 0; i < length; i++)
    {
      char c = text[i];
      if (c >= characterRangeMin && c <= characterRangeMax && characters.ContainsKey(c))
      {
        GiraffeFontGlyph glyph = characters[c];
        layer.Add(p + glyph.xOffset, y + glyph.yOffset, glyph.sprite);
        p += glyph.xAdvance;
      }
      else if (c == ' ')
      {
        p += spaceAdvance;
      }
    }
  }
}
