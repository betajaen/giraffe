using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Ship : MonoBehaviour
{
  public abstract void Hit(Missile missile);
  public abstract void Hit(Ship ship);

  void OnTriggerEnter2D(Collider2D col)
  {
    Missile missile = col.GetComponent<Missile>();
    if (missile != null)
      Hit(col.GetComponent<Missile>());
    else
    {
      Ship ship = col.GetComponent<Ship>();
      if (ship != null)
      {
        Hit(col.GetComponent<Ship>());
      }
    }
  }

}
