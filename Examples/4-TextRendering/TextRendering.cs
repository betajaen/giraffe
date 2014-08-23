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
  }

  void FixedUpdate()
  {
    String text = String.Format("{0:N1}", Time.time);

    int textQuadLength = victoriaBold.Estimate(text);

    mLayer.Begin(textQuadLength);

    victoriaBold.AddTo(mLayer, 10, 10, text);

    mLayer.End();
  }

}
