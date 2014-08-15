using System.Collections.ObjectModel;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class Giraffe : MonoBehaviour
{

  private Camera mCamera;

  private List<GiraffeLayer> mLayers;

  private Material mMaterial;
  void Awake()
  {
    mLayers = new List<GiraffeLayer>(4);

    mCamera = GetComponent<Camera>();
    mCamera.clearFlags = CameraClearFlags.Nothing;
    mCamera.cullingMask = 0;
    mCamera.nearClipPlane = 0.0f;
    mCamera.farClipPlane = 10.0f;
    mCamera.depth = 100;
    mCamera.orthographic = true;

    transform.position = Vector3.zero;
    transform.rotation = Quaternion.identity;
  }

  void Start()
  {
    mMaterial = new Material(Shader.Find("Giraffe/Standard"));
  }

  public void AddLayer(GiraffeLayer layer)
  {
    mLayers.Add(layer);
    DrawOrderChanged();
  }

  public void RemoveLayer(GiraffeLayer layer)
  {
    mLayers.Remove(layer);
    DrawOrderChanged();
  }

  public void DrawOrderChanged()
  {
    mLayers.Sort((a, b) => a.zOrder.CompareTo(b.zOrder));
  }

  private int counter = 0;

  void Update()
  {
    mCamera.orthographicSize = Screen.height * 0.5f;

    int count = mLayers.Count;
    for (int i = 0; i < count; i++)
    {
      mLayers[i].UpdateLayer();
    }
  }

  void OnPostRender()
  {
    mMaterial.SetPass(0);
    int count = mLayers.Count;
    for (int i = 0; i < count; i++)
    {
      mLayers[i].DrawLayer();
    }
  }

}
