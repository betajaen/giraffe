using UnityEngine;
using System.Collections;

public class GiraffeParticles : MonoBehaviour
{

  public GiraffeLayer layer;

  void Start()
  {
    if (layer == null)
    {
      layer = transform.parent.GetComponent<GiraffeLayer>();
    }
  }

  void Update()
  {

  }
}
