using System;
using UnityEngine;
using System.Collections.Generic;

public enum GiraffeAnimationMode
{
  Loop,
  Once,
  PingPong,
  RandomFrame
}

[Serializable]
public class GiraffeSpriteAnimation : ScriptableObject
{
  [SerializeField]
  public GiraffeAtlas atlas;

  [SerializeField]
  public List<String> frames = new List<String>(4)
  {
    "Giraffe/White"
  };

  [SerializeField]
  public List<int> offsetsX = new List<int>(4)
  {
    0
  };

  [SerializeField]
  public List<int> offsetsY = new List<int>(4)
  {
    0
  };

  [SerializeField]
  public float length = 1.0f;

  [SerializeField]
  public GiraffeAnimationMode mode;

  [NonSerialized]
  public GiraffeSprite[] frameSprites;

  private void OnEnable()
  {
    FetchSprites();
  }

  public void FetchSprites()
  {
    frameSprites = new GiraffeSprite[frames.Count];
    if (atlas == null)
    {
      for (int i = 0; i < frames.Count; i++)
      {
        frameSprites[i] = new GiraffeSprite();
      }
    }
    else
    {
      for (int i = 0; i < frames.Count; i++)
      {
        frameSprites[i] = atlas.GetSprite(frames[i]);
      }
    }
  }

  public static int Animate(GiraffeSpriteAnimation animation, float time, ref bool isPlaying)
  {
    switch (animation.mode)
    {
      case GiraffeAnimationMode.Loop:
      {
        isPlaying = true;
        if (Mathf.Approximately(time, animation.length))
        {
          return animation.frames.Count - 1;
        }
        time = time % animation.length;
        float frameRate = animation.frames.Count / animation.length;
        int frame = (int)(time * frameRate);
        return frame;
      }
      break;
      case GiraffeAnimationMode.Once:
      {
        isPlaying = true;
        if (time >= animation.length)
        {
          isPlaying = false;
          return animation.frames.Count - 1;
        }
        float frameRate = animation.frames.Count / animation.length;
        int frame = Mathf.Min((int)(time * frameRate), animation.frames.Count - 1);
        return frame;
      }
      break;
    }
    return 1;
  }

}

