using System;
using UnityEngine;

public class TextRendering : MonoBehaviour
{

  private GiraffeAtlas mAtlas;
  private GiraffeLayer mLayer;

  private GiraffeSprite[] mBoxes;

  public GiraffeFont victoriaBold;

  void Start()
  {
    mLayer = GetComponent<GiraffeLayer>();

    String text = @"Hello World!";

    int textQuadLength = victoriaBold.Estimate(text);

    mLayer.Begin(textQuadLength);

    victoriaBold.AddTo(mLayer, 10, 10, text);

    mLayer.End();

  }

}
