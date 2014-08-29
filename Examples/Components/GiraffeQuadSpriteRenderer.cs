using System;
using UnityEngine;
using System.Collections;


public class GiraffeQuadSpriteRenderer : MonoBehaviour, IGirrafeQuadEventListener
{

  [SerializeField]
  private String mSpriteName;

  [NonSerialized]
  private GiraffeSprite mSprite;

  [SerializeField]
  private float mScale = 1.0f;

  [NonSerialized]
  private Transform mTransform;

  [NonSerialized]
  private Matrix2D mTransform2D;

  [NonSerialized]
  private GiraffeLayer mLayer;

  [NonSerialized]
  private GiraffeQuadRendererManager mManager;

  [NonSerialized]
  private bool mApplicationIsQuitting;

  [SerializeField]
  public bool visible = true;

  void Awake()
  {
    mApplicationIsQuitting = false;
  }

  void OnApplicationQuit()
  {
    mApplicationIsQuitting = true;
  }

  void OnEnable()
  {
    if (mManager != null)
    {
      mManager.Add(this);
    }
  }

  void OnDisable()
  {
    if (mManager != null && mApplicationIsQuitting == false)
    {
      mManager.Remove(this);
    }
  }

  void Start()
  {
    mTransform = GetComponent<Transform>();

    FindParts();

    RefreshSprites();
    RefreshTransform2D();

    mManager.Add(this);
  }

  void FixedUpdate()
  {
    if (mTransform.hasChanged)
    {
      RefreshTransform2D();
    }

    mTransform.hasChanged = false;
  }

  void RefreshTransform2D()
  {
    mTransform2D = Matrix2D.TRS(mTransform.position, 0.0f, sprite.size * mScale);
  }

  public GiraffeLayer layer
  {
    get { return mLayer; }
  }

  public GiraffeSprite sprite
  {
    get
    {
      return mSprite;
    }
    set
    {
      if (mSprite == value)
        return;
      mSprite = value;
      mSpriteName = sprite.name;
    }
  }

  public String spriteName
  {
    get
    {
      return mSpriteName;
    }
    set
    {
      mSpriteName = value;
      if (Application.isPlaying)
      {
        mSprite = atlas.GetSprite(mSpriteName);
        RefreshTransform2D();
      }
    }
  }

  public GiraffeAtlas atlas
  {
    get
    {
      return mLayer.atlas;
    }
  }

  public float scale
  {
    get { return mScale; }
    set
    {
      mScale = value;
    }
  }

  void FindParts()
  {
    mLayer = GiraffeInternal.GiraffeUtils.FindRecursiveComponentBackwards<GiraffeLayer>(mTransform);
    mManager = GiraffeInternal.GiraffeUtils.FindRecursiveComponentBackwards<GiraffeQuadRendererManager>(mTransform);
  }

  void RefreshSprites()
  {
    mSprite = atlas.GetSprite(spriteName);
  }

  public int GetQuadCount()
  {
    return visible ? 1 : 0;
  }

  public void DrawTo(GiraffeLayer layer)
  {
    if (visible)
    {
      layer.Add(mTransform2D, mSprite);
    }
  }


}
