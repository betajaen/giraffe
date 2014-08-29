using System;
using GiraffeInternal;
using UnityEngine;
using System.Collections;

public enum MissileType
{
  Laser,
  Bomb,
  Explosion
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
  public GameObject secondaryMissile;

  [SerializeField]
  public int secondaryMissileMin;

  [SerializeField]
  public int secondaryMissileMax;

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
  private int mMode;

  [NonSerialized]
  public Ship owner;

  [NonSerialized]
  public MissileFactory factory;

  void Awake()
  {
    mTransform = GetComponent<Transform>();
    mRigidBody = GetComponent<Rigidbody2D>();
    mCollider = GetComponent<BoxCollider2D>();
    mRenderer = GetComponent<GiraffeQuadSpriteRenderer>();
    mAnimator = GetComponent<GiraffeQuadSpriteAnimator>();
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
      mRenderer.visible = false;
      gameObject.SetActive(false);
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
        if (mAnimator.playing == false)
        {
          SpawnSecondary();
          mRenderer.visible = false;
          gameObject.SetActive(false);
        }
      }
      break;
      case MissileType.Explosion:
      {
        if (mAnimator.playing == false)
        {
          RaycastHit2D[] hits = Physics2D.CircleCastAll(mTransform.position, explosiveRadius, new Vector2(1, 0),
            explosiveRadius * 0.25f);
          foreach (var hit in hits)
          {
            var c = hit.collider.GetComponent<Ship>();
            if (c != null)
            {
              c.Hit(this);
            }
          }
          mRenderer.visible = false;
          gameObject.SetActive(false);
        }
      }
      break;
    }
  }

  void SpawnSecondary()
  {
    int count = UnityEngine.Random.Range(secondaryMissileMin, secondaryMissileMax);

    Vector2 pos = mTransform.position;
    Vector2 vel = mRigidBody.velocity;

    for (int i = 0; i < secondaryMissileMax; i++)
    {
      var go = factory.Add(secondaryMissile);
      var missile = go.GetComponent<Missile>();
      float angle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
      float distance = UnityEngine.Random.Range(0.0f, explosiveRadius);
      float xOffset = Mathf.Cos(angle) * distance;
      float yOffset = Mathf.Sin(angle) * distance;
      missile.Fire(owner, pos + new Vector2(xOffset, yOffset), vel);
    }

  }

  public void Fire(Ship ship, Vector2 position, Vector2 velocity)
  {
    mTransform.position = position;
    mTransform.rotation = Quaternion.identity;
    mRigidBody.velocity = velocity;
    mRigidBody.angularVelocity = 0.0f;
    mRenderer.visible = true;
    mTimer = 0.0f;
    mMode = 0;
    owner = ship;

    if (mAnimator != null)
    {
      mAnimator.playing = true;
      mAnimator.time = 0.0f;
    }

  }


}
