using System.Collections.ObjectModel;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class Giraffe : MonoBehaviour
{

  private Camera mCamera;

  private List<GiraffeLayer> mLayers;

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

  void Update()
  {
    mCamera.orthographicSize = Screen.height * 0.5f;
  }

  void OnPostRender()
  {
    int count = mLayers.Count;
    for (int i = 0; i < count; i++)
    {
      mLayers[i].DrawLayer();
    }
  }

}
