using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Ship : MonoBehaviour
{

  public virtual void Hit(Ship other)
  {
  }

}
