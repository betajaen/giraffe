using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GiraffeQuadSpriteRenderer), typeof(GiraffeQuadSpriteAnimator))]
public class Hero : Ship
{

  private int kMovement_Up = 1;
  private int kMovement_Down = 2;
  private int kMovement_Left = 4;
  private int kMovement_Right = 8;

  private int kAction_Laser = 1;
  private int kAction_Shield = 2;
  private int kAction_Bomb = 4;

  private static readonly Vector2[] kMovements =
  {
    new Vector3(+0, +0, +0),  // r l d u
    new Vector3(+0, -1, +0),  // r l d U
    new Vector3(+0, +1, +0),  // r l D u
    new Vector3(+0, +0, +0),  // r l D U
    new Vector3(-1, +0, +0),  // r L d u
    new Vector3(-1, -1, +0),  // r L d U
    new Vector3(-1, +1, +0),  // r L D u
    new Vector3(-1, +0, +0),  // r L D U
    new Vector3(+1, +0, +0),  // R l d u
    new Vector3(+1, -1, +0),  // R l d U
    new Vector3(+1, +1, +0),  // R l D u
    new Vector3(+1, +0, +0),  // R l D U
    new Vector3(+0, +0, +0),  // R L d u
    new Vector3(+0, -1, +0),  // R L d U
    new Vector3(+0, +1, +0),  // R L D u
    new Vector3(+0, +0, +0),  // R L D U
  };

  private int mMovementFlags;
  private int mAction;
  private float mSpeed;

  private GiraffeQuadSpriteRenderer mRenderer;
  private GiraffeQuadSpriteAnimator mAnimator;
  private Transform mTransform;

  public GiraffeSpriteAnimation[] verticalAnimations;

  public GameObject bombPrefab;
  public GameObject[] missilePrefabs;

  public MissileFactory missileFactory;

  private bool bombPressed, wasBombPressed;
  private bool laserPressed, wasLaserPressed;
  private float mFireTimer;
  private int mWidth, mHeight;

  public float fireTime;

  void Start()
  {
    mRenderer = GetComponent<GiraffeQuadSpriteRenderer>();
    mAnimator = GetComponent<GiraffeQuadSpriteAnimator>();
    mTransform = GetComponent<Transform>();
    mSpeed = 100;
    mWidth = mRenderer.sprite.width;
    mHeight = mRenderer.sprite.height;
  }

  void Update()
  {
    if (Input.GetKey(KeyCode.W))
      mMovementFlags |= kMovement_Up;
    else
      mMovementFlags &= ~kMovement_Up;

    if (Input.GetKey(KeyCode.S))
      mMovementFlags |= kMovement_Down;
    else
      mMovementFlags &= ~kMovement_Down;

    if (Input.GetKey(KeyCode.D))
      mMovementFlags |= kMovement_Right;
    else
      mMovementFlags &= ~kMovement_Right;

    if (Input.GetKey(KeyCode.A))
      mMovementFlags |= kMovement_Left;
    else
      mMovementFlags &= ~kMovement_Left;

    wasBombPressed = bombPressed;
    bombPressed = Input.GetKey(KeyCode.Q);

    if (wasBombPressed && bombPressed == false)
    {
      GameObject go = Instantiate(bombPrefab, transform.position, Quaternion.identity) as GameObject;
      go.transform.parent = mTransform.parent;
    }

    wasLaserPressed = laserPressed;
    laserPressed = Input.GetKey(KeyCode.Return);

    if (laserPressed)
    {
      mFireTimer += Time.deltaTime;
    }

  }

  void FixedUpdate()
  {
    Vector3 nextMovement = kMovements[mMovementFlags];
    mTransform.localPosition = mTransform.localPosition + nextMovement * mSpeed * Time.deltaTime;


    int hw = mHeight / 2;
    int y1 = (Screen.height / mRenderer.layer.scale) - hw;

    if (mTransform.localPosition.y < hw)
    {
      mTransform.localPosition = new Vector2(mTransform.localPosition.x, hw);
    }
    else if (mTransform.localPosition.y > y1)
    {
      mTransform.localPosition = new Vector2(mTransform.localPosition.x, y1);
    }

    if (nextMovement.y < 0)
      mAnimator.animation = verticalAnimations[0];
    else if (nextMovement.y > 0)
      mAnimator.animation = verticalAnimations[2];
    else
      mAnimator.animation = verticalAnimations[1];

    if (mFireTimer > fireTime)
    {
      mFireTimer = 0.0f;
      var missile = missileFactory.Add(missilePrefabs[0]);

      Vector2 position = mTransform.position;
      position.x += mRenderer.sprite.width * 0.5f;

      missile.Fire(position, new Vector2(450.0f, 0.0f));
    }
  }

}
