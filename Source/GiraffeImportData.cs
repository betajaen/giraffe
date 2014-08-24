using System;
using System.Collections.Generic;
using UnityEngine;


public enum GiraffeAtlasImportDataType
{
  None,
  Texture2D,
  TileSheetSquare,
  TilesheetRectangular
}

[Serializable]
public class GiraffeAtlasImportDataPart
{
  [SerializeField]
  public GiraffeAtlasImportDataType type;

  [SerializeField]
  public TextAsset textAsset;

  [SerializeField]
  public Texture2D textureAsset;

  [SerializeField]
  public int width = 32;

  [SerializeField]
  public int height = 32;

  [SerializeField]
  public int count = 1;

}

public class GiraffeImportData : ScriptableObject
{

  [SerializeField]
  public int atlasIdA;

  [SerializeField]
  public int atlasIdB;

  [SerializeField]
  public List<GiraffeAtlasImportDataPart> parts;

  [SerializeField]
  public bool atlasOutOfDate;

  [SerializeField]
  public int border = 2;

  [SerializeField]
  public int padding = 0;

  [SerializeField]
  public bool generateWhiteTexture = true;

  [NonSerialized]
  public GiraffeAtlas _atlas;

  [NonSerialized]
  public int _atlasResolved;

  void OnEnable()
  {
    if (parts == null)
    {
      parts = new List<GiraffeAtlasImportDataPart>(2);
    }
  }

}