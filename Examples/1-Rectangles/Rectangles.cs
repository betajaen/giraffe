using UnityEngine;
using System.Collections;

public class Rectangles : MonoBehaviour
{

  private GiraffeAtlas mAtlas;
  private GiraffeLayer mLayer;

  private GiraffeSprite[] mBoxes;

  void Start()
  {
    mLayer = GetComponent<GiraffeLayer>();
    mAtlas = mLayer.atlas;

    mBoxes = new GiraffeSprite[4];
    mBoxes[0] = mAtlas.GetSprite("Box");
    mBoxes[1] = mAtlas.GetSprite("Box2");
    mBoxes[2] = mAtlas.GetSprite("Box3");
    mBoxes[3] = mAtlas.GetSprite("Box4");

    mLayer.Begin(4);
    mLayer.Quad(10, 10, mBoxes[0]);
    mLayer.Quad(100, 10, mBoxes[1]);
    mLayer.Quad(200, 10, mBoxes[2]);
    mLayer.Quad(30, 70, mBoxes[3]);
    mLayer.End();

  }

  // Update is called once per frame
  void Update()
  {

  }
}
