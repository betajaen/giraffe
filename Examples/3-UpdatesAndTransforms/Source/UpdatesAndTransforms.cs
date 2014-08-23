using UnityEngine;

public class UpdatesAndTransforms : MonoBehaviour
{

  private GiraffeAtlas mAtlas;
  private GiraffeLayer mLayer;

  private GiraffeSprite[] mBoxes;
  private Vector2[] mTranslations;
  private float[] mRotations;
  private Vector2[] mScales;

  private int mCount;

  void Start()
  {
    mLayer = GetComponent<GiraffeLayer>();
    mAtlas = mLayer.atlas;

    mBoxes = new GiraffeSprite[4];
    mBoxes[0] = mAtlas.GetSprite("Box");
    mBoxes[1] = mAtlas.GetSprite("Box1");

    mCount = 50 + UnityEngine.Random.Range(1, 20);
    mTranslations = new Vector2[mCount];
    mRotations = new float[mCount];
    mScales = new Vector2[mCount];

    for (int i = 0; i < mCount; i++)
    {
      float time = UnityEngine.Random.Range(0.0f, 8.0f);
      float cx = UnityEngine.Random.Range(0, Screen.width);
      float cy = UnityEngine.Random.Range(0, Screen.width);

      GiraffeSprite sprite = mBoxes[i % 2];

      mTranslations[i] = new Vector2(cx, cy);
      mRotations[i] = time;
      mScales[i] = new Vector2(sprite.width, sprite.height) * Mathf.Sin(mRotations[i]) * 8.0f;

    }

  }

  void FixedUpdate()
  {
    mLayer.Begin(mCount);

    for (int i = 0; i < mCount; i++)
    {
      GiraffeSprite sprite = mBoxes[i % 2];
      mRotations[i] += Time.fixedDeltaTime;
      mScales[i] = new Vector2(sprite.width, sprite.height) * Mathf.Sin(mRotations[i]) * 8.0f;
      Matrix2D transform = Matrix2D.TRS(mTranslations[i], mRotations[i], mScales[i]);
      mLayer.Add(transform, sprite);

    }

    mLayer.End();
  }

}
