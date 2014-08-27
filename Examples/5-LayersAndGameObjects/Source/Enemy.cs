using System;
using UnityEngine;
using System.Collections;

public enum EnemyMovementStyle
{
  Forward,
  Circle
}

public class Enemy : Ship
{

  [SerializeField]
  public EnemyMovementStyle movementStyle;

  [SerializeField]
  public float movementRadius = 10.0f;

  [SerializeField]
  public float speed;

  [SerializeField]
  public float angleSpeed;

  [SerializeField]
  public Missile weapon;

  [SerializeField]
  public float fireRate;

  [NonSerialized]
  private float mOriginX;

  [NonSerialized]
  private float mOriginY;

  [NonSerialized]
  private float mX;

  [NonSerialized]
  private float mY;

  [NonSerialized]
  private float mLastX;

  [NonSerialized]
  private float mLastY;

  [NonSerialized]
  private float mAngle;

  [HideInInspector]
  [NonSerialized]
  public bool isActive;

  [HideInInspector]
  [NonSerialized]
  public bool isAlive;

  [NonSerialized]
  private GiraffeQuadSpriteAnimator mAnimator;

  [NonSerialized]
  private GiraffeQuadSpriteRenderer mRenderer;

  [NonSerialized]
  private Transform mTransform;

  [SerializeField]
  public GiraffeSpriteAnimation explosionAnimation;

  [SerializeField]
  public GiraffeSpriteAnimation[] verticalAnimations;

  [NonSerialized]
  private float mVelocityX;

  [NonSerialized]
  private float mVelocityY;

  [NonSerialized]
  private EnemyWave mWave;

  void Awake()
  {
    isActive = true;
    isAlive = true;
    mTransform = GetComponent<Transform>();
    mRenderer = GetComponent<GiraffeQuadSpriteRenderer>();
    mAnimator = GetComponent<GiraffeQuadSpriteAnimator>();
  }

  void FixedUpdate()
  {
    if (isAlive && isActive)
    {
      UpdateMovement();
    }
  }

  public void Spawn(int x, int y, EnemyWave wave)
  {
    mOriginX = mX = x;
    mOriginY = mY = y;
    mAngle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI) * Mathf.Rad2Deg;
    isActive = true;
    isAlive = true;

    if (verticalAnimations.Length == 1)
      mAnimator.animation = verticalAnimations[0];
    else if (verticalAnimations.Length == 3)
      mAnimator.animation = verticalAnimations[1];

    mAnimator.playing = true;
    mAnimator.time = 0.0f;

    UpdateMovement();
  }

  void UpdateMovement()
  {
    mLastX = mX;
    mLastY = mY;

    switch (movementStyle)
    {
      case EnemyMovementStyle.Forward:
      {
        mOriginX -= speed * Time.fixedDeltaTime;
        mX = mOriginX;
      }
      break;
      case EnemyMovementStyle.Circle:
      {
        mAngle += angleSpeed * Time.fixedDeltaTime;
        mOriginX -= speed * Time.fixedDeltaTime;
        mX = mOriginX + (Mathf.Cos(mAngle) * movementRadius);
        mY = mOriginY + (Mathf.Sin(mAngle) * movementRadius);
      }
      break;
    }

    mTransform.position = new Vector3(mX, mY);

    mVelocityX = mX - mLastX;
    mVelocityY = mY - mLastY;

    if (verticalAnimations.Length == 3)
    {
      if (mVelocityY < 0.0f && mAnimator.animation != verticalAnimations[0])
      {
        mAnimator.animation = verticalAnimations[0];
        mAnimator.time = 0.0f;
        mAnimator.playing = true;
      }
      else if (mVelocityY > 0.0f && mAnimator.animation != verticalAnimations[2])
      {
        mAnimator.animation = verticalAnimations[2];
        mAnimator.time = 0.0f;
        mAnimator.playing = true;
      }
      else if (mAnimator.animation != verticalAnimations[1])
      {
        mAnimator.animation = verticalAnimations[1];
        mAnimator.time = 0.0f;
        mAnimator.playing = true;
      }
    }

  }

  public override void Hit(Missile missile)
  {
    if (missile.owner == this)
      return;
    isAlive = false;
    mWave.enemies.Remove(this);
  }

  public override void Hit(Ship ship)
  {
    Debug.Log("Collision!");
  }

}
