using System;
using UnityEngine;
using System.Collections;

public enum EnemyMovementStyle
{
  Forward,
  Circle,
  Target
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

  [NonSerialized]
  private EnemyFactory mFactory;

  void Awake()
  {
    isActive = true;
    isAlive = true;
    mTransform = GetComponent<Transform>();
    mRenderer = GetComponent<GiraffeQuadSpriteRenderer>();
    mAnimator = GetComponent<GiraffeQuadSpriteAnimator>();
  }

  void Start()
  {
  }

  void FixedUpdate()
  {
    if (isAlive && isActive)
    {
      UpdateMovement();
    }
    else if (!isAlive && isActive)
    {
      if (mAnimator.animation != explosionAnimation)
      {
        mAnimator.animation = explosionAnimation;
        mAnimator.time = 0.0f;
        mAnimator.playing = true;
      }

      if (mAnimator.playing == false)
      {
        isActive = false;
        mTransform.position = new Vector2(Screen.width + 100, 0);
      }
    }
  }

  public void Spawn(int x, int y, EnemyWave wave)
  {
    mOriginX = mX = x;
    mOriginY = mY = y;
    mAngle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI) * Mathf.Rad2Deg;
    isActive = true;
    isAlive = true;
    mWave = wave;

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
      case EnemyMovementStyle.Target:
      {
        float diff = Hero.msEnemyTargetY - mY;
        if (diff < 0)
          mY += speed * Time.fixedDeltaTime * (diff * 0.15f);
        else if (diff > 0)
          mY += speed * Time.fixedDeltaTime * (diff * 0.15f);
        mOriginY = mY;

        mOriginX -= speed * Time.fixedDeltaTime * 75.0f;
        mX = mOriginX;
      }
      break;
    }

    mTransform.position = new Vector3(mX, mY);

    mVelocityX = mX - mLastX;
    mVelocityY = mY - mLastY;

    if (mX < -100)
    {
      isAlive = false;
      isActive = false;
      mWave.enemies.Remove(this);
    }

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
    // Debug.Log("Enemy reports collision with other ship!!");
  }


}
