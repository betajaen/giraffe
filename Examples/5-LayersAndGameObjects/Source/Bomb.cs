using System;
using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{

  [SerializeField]
  public GiraffeSpriteAnimation explosion;

  [SerializeField]
  public float explosionRadius;

  [NonSerialized]
  private GiraffeQuadSpriteAnimator mAnimator;

  [NonSerialized]
  private int mode;

  void Start()
  {
    mAnimator = GetComponent<GiraffeQuadSpriteAnimator>();
  }

  void Update()
  {
    if (mode == 0)
    {
      if (mAnimator.playing == false)
      {
        mAnimator.animation = explosion;
        mAnimator.time = 0.0f;
        mAnimator.playing = true;
        mode = 1;
      }
    }
    else if (mode == 1)
    {
      if (mAnimator.playing == false)
      {
        Destroy(gameObject);
      }
    }
  }

}
