using System;
using System.IO;
using UnityEngine;
using System.Collections;

public class Tilesets : MonoBehaviour
{

  public TextAsset spriteMap;

  private GiraffeAtlas mAtlas;
  private GiraffeLayer mLayer;

  private int mapWidth, mapHeight, tileWidth, tileHeight;
  private int[] map;

  void Start()
  {
    mLayer = GetComponent<GiraffeLayer>();
    mAtlas = mLayer.atlas;

    ReadTileMap();


    int area = mapWidth * mapHeight;
    int x = 0;
    int y = 0;
    int screenOriginX = Screen.width / 2 - (mapWidth * tileWidth) / 2;
    int screenOriginY = Screen.height / 2 - (mapHeight * tileHeight) / 2;

    mLayer.Begin(area);
    for (int i = 0; i < area; i++)
    {

      mLayer.Add(screenOriginX + x * tileWidth, screenOriginY + y * tileHeight, mAtlas.GetSpriteAt(map[i]));

      x++;
      if (x >= mapWidth)
      {
        x = 0;
        y++;
      }
    }
    mLayer.End();

  }

  // Update is called once per frame
  void Update()
  {

  }

  void ReadTileMap()
  {
    String dataString = null;

    // This is a really simple, and error prone tilemap reader, exported from Tiled.
    // You really shouldn't use this in anything, it's just an example.
    using (StringReader reader = new StringReader(spriteMap.text))
    {
      String line;
      while ((line = reader.ReadLine()) != null)
      {
        line = line.Trim();

        if (String.IsNullOrEmpty(line))
          continue;

        int i = line.IndexOf('=');
        if (i == -1)
          continue;

        String key = line.Substring(0, i).ToLower();
        String value = line.Substring(i + 1);

        if (key == "width")
        {
          mapWidth = Int32.Parse(value);
        }
        else if (key == "height")
        {
          mapHeight = Int32.Parse(value);
        }
        else if (key == "tilewidth")
        {
          tileWidth = Int32.Parse(value);
        }
        else if (key == "tileheight")
        {
          tileHeight = Int32.Parse(value);
        }
        else if (key == "data")
        {
          dataString = value;
        }
      }
    }

    if (String.IsNullOrEmpty(dataString) == false && mapWidth > 0 && mapHeight > 0)
    {
      String[] b = dataString.Split(',');
      if (b.Length != mapWidth * mapHeight)
      {
        Debug.LogError("incorrect tile count");
      }
      map = new int[mapWidth * mapHeight];
      int i = 0;
      foreach (var s in b)
      {
        map[i++] = Int32.Parse(s.Trim()) - 1;
      }
    }
    else
    {
      Debug.LogError("missing data");
    }

  }

}
