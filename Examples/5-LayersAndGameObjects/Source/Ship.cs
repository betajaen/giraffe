using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Ship : MonoBehaviour
{
  public abstract void Hit(Missile missile);
  public abstract void Hit(Ship ship);
}
