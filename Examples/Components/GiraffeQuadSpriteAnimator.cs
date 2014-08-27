using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GiraffeQuadSpriteRenderer))]
public class GiraffeQuadSpriteAnimator : MonoBehaviour
{

  [SerializeField]
  private bool mPlaying = true;

  [SerializeField]
  private GiraffeSpriteAnimation mAnimation;

  [NonSerialized]
  private GiraffeQuadSpriteRenderer mRenderer;

  [NonSerialized]
  private bool mApplicationIsQuitting;

  [NonSerialized]
  private float mTime;

  [NonSerialized]
  private int mCurrentFrame;

  void Awake()
  {
    mApplicationIsQuitting = false;
    mRenderer = GetComponent<GiraffeQuadSpriteRenderer>();
    mCurrentFrame = -1;
  }

  void OnApplicationQuit()
  {
    mApplicationIsQuitting = true;
  }

  void OnDisable()
  {
  }

  void Start()
  {
    if (mAnimation != null && Application.isPlaying)
    {
      mAnimation.FetchSprites();
      UpdateAnimation();
    }
  }

  void Update()
  {
    if (mPlaying)
    {
      time += Time.deltaTime * Time.timeScale;
    }
  }

  public float time
  {
    get
    {
      return mTime;
    }

    set
    {
      mTime = value;
      UpdateAnimation();
    }
  }

  public GiraffeSpriteAnimation animation
  {
    get
    {
      return mAnimation;
    }
    set
    {
      if (mAnimation == value)
        return;
      mAnimation = value;
      mCurrentFrame = -1;
      if (Application.isPlaying && mAnimation != null)
      {
        mAnimation.FetchSprites();
        UpdateAnimation();
      }
    }
  }

  public bool playing
  {
    get
    {
      return mPlaying;
    }
    set
    {
      if (mPlaying == value)
        return;
      mPlaying = value;
    }
  }

  void UpdateAnimation()
  {
    if (mAnimation == null)
      return;

    int frame = GiraffeSpriteAnimation.Animate(mAnimation, mTime, ref mPlaying);

    if (frame != mCurrentFrame)
    {
      GiraffeSprite sprite = mAnimation.frameSprites[frame];
      mRenderer.sprite = sprite;
      mCurrentFrame = frame;
    }
  }

}
