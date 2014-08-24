using System;
using UnityEngine;
using System.Collections;


public class GiraffeQuadSpriteRenderer : MonoBehaviour, IGirrafeQuadEventListener
{

  [SerializeField]
  private String mSpriteName;

  [SerializeField]
  public GiraffeSpriteAnimation[] animation;

  [NonSerialized]
  private GiraffeSprite mSprite;

  [SerializeField]
  public int offsetX;

  [SerializeField]
  public int offsetY;

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

  void Awake()
  {
    mApplicationIsQuitting = false;
  }

  void OnApplicationQuit()
  {
    mApplicationIsQuitting = true;
  }

  void OnDisable()
  {
    if (mApplicationIsQuitting == false)
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
    mTransform2D = Matrix2D.TRS(mTransform.position, 0.0f, sprite.size);
  }

  public GiraffeSprite sprite
  {
    get
    {
      return mSprite;
    }
    set
    {
      mSprite = sprite;
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
    return 1;
  }

  public void DrawTo(GiraffeLayer layer)
  {
    layer.Add(mTransform2D, mSprite);
  }


}
