using UnityEngine;

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

    int count = 50 + UnityEngine.Random.Range(1, 20);

    mLayer.Begin(count);

    for (int i = 0; i < count; i++)
    {
      int x = UnityEngine.Random.Range(0, Screen.width);
      int y = UnityEngine.Random.Range(0, Screen.height);
      int b = UnityEngine.Random.Range(0, mBoxes.Length);
      mLayer.Add(x, y, mBoxes[b]);
    }

    mLayer.End();

  }

}
