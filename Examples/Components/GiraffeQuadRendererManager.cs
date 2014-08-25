using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGirrafeQuadEventListener
{
  int GetQuadCount();
  void DrawTo(GiraffeLayer layer);
}

// GiraffeQuadRendererManager is an example component for Giraffe that allows multiple
// un-related MonoBehaviours to hook themselves into the Begin/End draw cycle.
// 
// By inheriting from the interface 'IGirrafeQuadEventListener' any MonoBehaviour 
// can listen to the Quad counting and draw events. They can then optionally draw
// as many quads as they like.
// 
// The only restriction is that there can't be any other MonoBehaviours of the same
// layer calling Begin/End, as that will undo the work of the GiraffeQuadRendererManager.
//
// The GiraffeQuadRendererManager MonoBehaviour must be attached to the GiraffeLayer to
// function. Any MonoBehaviours who listen in must be as usual a child gameObject of
// that layer.
//
// Also see:
//    GiraffeQuadSpriteRenderer
//    GiraffeQuadParticleRenderer
public class GiraffeQuadRendererManager : MonoBehaviour
{

  [NonSerialized]
  private List<IGirrafeQuadEventListener> mListeners;

  [NonSerialized]
  private List<IGirrafeQuadEventListener> mListenersToAdd;

  [NonSerialized]
  private List<IGirrafeQuadEventListener> mListenersToRemove;

  [NonSerialized]
  private GiraffeLayer mLayer;

  GiraffeQuadRendererManager()
  {
    mListeners = new List<IGirrafeQuadEventListener>(32);
    mListenersToAdd = new List<IGirrafeQuadEventListener>(32);
    mListenersToRemove = new List<IGirrafeQuadEventListener>(32);
  }

  public void Awake()
  {
    mLayer = GetComponent<GiraffeLayer>();
  }

  public void Add(IGirrafeQuadEventListener eventListener)
  {
    mListenersToAdd.Add(eventListener);
  }

  public void Remove(IGirrafeQuadEventListener eventListener)
  {
    mListenersToRemove.Add(eventListener);
  }

  void FixedUpdate()
  {
    foreach (var girrafeLayerEventListener in mListenersToAdd)
    {
      mListeners.Add(girrafeLayerEventListener);
    }
    mListenersToAdd.Clear();

    foreach (var eventListener in mListenersToRemove)
    {
      mListeners.Remove(eventListener);
    }
    mListenersToRemove.Clear();

    int nbQuads = 0;

    foreach (var quad in mListeners)
    {
      nbQuads += quad.GetQuadCount();
    }

    mLayer.Begin(nbQuads);

    foreach (var eventListener in mListeners)
    {
      eventListener.DrawTo(mLayer);
    }

    mLayer.End();
  }
}

