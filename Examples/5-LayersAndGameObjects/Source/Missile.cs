using System;
using GiraffeInternal;
using UnityEngine;
using System.Collections;

public enum MissileType
{
  Laser,
  Bomb
}

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Missile : MonoBehaviour
{
  [SerializeField]
  public int damage;

  [SerializeField]
  public MissileType type;

  [SerializeField]
  public int explosiveRadius;

  [SerializeField]
  private GiraffeSpriteAnimation animation;

  [SerializeField]
  public GiraffeSpriteAnimation explosionAnimation;

  [NonSerialized]
  private Transform mTransform;

  [NonSerialized]
  private Rigidbody2D mRigidBody;

  [NonSerialized]
  private BoxCollider2D mCollider;

  [NonSerialized]
  private GiraffeQuadSpriteRenderer mRenderer;

  [NonSerialized]
  private GiraffeQuadSpriteAnimator mAnimator;

  [NonSerialized]
  private float mTimer;

  [NonSerialized]
  public bool isActive;

  [NonSerialized]
  private int mMode;

  [NonSerialized]
  public Ship owner;

  void Awake()
  {
    mTransform = GetComponent<Transform>();
    mRigidBody = GetComponent<Rigidbody2D>();
    mCollider = GetComponent<BoxCollider2D>();
    mRenderer = GetComponent<GiraffeQuadSpriteRenderer>();
    mAnimator = GetComponent<GiraffeQuadSpriteAnimator>();
    if (mAnimator != null)
    {
      animation = mAnimator.animation;
    }
  }

  void FixedUpdate()
  {
    mTimer += Time.fixedDeltaTime * Time.timeScale;

    float boundsX0 = 0.0f, boundsY0 = 0.0f;
    float boundsX1 = Screen.width / mRenderer.layer.scale;
    float boundsY1 = Screen.height / mRenderer.layer.scale;

    Vector2 position = mTransform.position;
    if (position.x < boundsX0 || position.x > boundsX1 || position.y < boundsY0 || position.y > boundsY1)
    {
      mRigidBody.velocity = Vector2.zero;
      mRigidBody.angularVelocity = 0.0f;
      isActive = false;
      mRenderer.visible = false;
      return;
    }

    switch (type)
    {
      case MissileType.Laser:
      {

      }
      break;
      case MissileType.Bomb:
      {
        if (mMode == 0)
        {
          if (mAnimator.playing == false)
          {
            mAnimator.animation = explosionAnimation;
            mAnimator.time = 0.0f;
            mAnimator.playing = true;
            mMode = 1;
          }
        }
        else if (mMode == 1)
        {
          if (mAnimator.playing == false)
          {
            Destroy(gameObject);
          }
        }
      }
      break;
    }
  }

  public void Fire(Ship ship, Vector2 position, Vector2 velocity)
  {
    mTransform.position = position;
    mTransform.rotation = Quaternion.identity;
    mRigidBody.velocity = velocity;
    mRigidBody.angularVelocity = 0.0f;
    mRenderer.visible = true;
    isActive = true;
    mTimer = 0.0f;
    mMode = 0;
    owner = ship;

    if (mAnimator != null)
    {
      mAnimator.animation = animation;
      mAnimator.playing = true;
      mAnimator.time = 0.0f;
    }

  }

}
