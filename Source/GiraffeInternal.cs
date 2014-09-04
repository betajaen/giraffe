using UnityEngine;

namespace GiraffeInternal
{

  class MeshBuffer
  {
    public Vector3[] position;
    public Vector2[] uv;
    public Color32[] colours;

    public int[] indexes;
    public int nbVertices;
    public int nbIndexes;

    public int bufferChanges;

    public MeshBuffer()
    {
      nbVertices = 0;
      nbIndexes = 0;
      bufferChanges = 0;
    }

    public void Delete()
    {
      position = null;
      uv = null;
      indexes = null;
    }

    public bool Reserve(int newNbVertices, int newNbIndexes)
    {
      bool clear = newNbVertices != nbVertices;

      if (newNbVertices > nbVertices || newNbIndexes > nbIndexes)
      {
        ReserveForce(newNbVertices, newNbIndexes);
      }
      else
      {
        for (int i = newNbIndexes; i < nbIndexes; i++)
          indexes[i] = 0;
      }

      return clear;
    }

    public void ReserveForce(int newNbVertices, int newNbIndexes)
    {
      nbVertices = newNbVertices;
      position = new Vector3[nbVertices];
      uv = new Vector2[nbVertices];
      colours = new Color32[nbVertices];

      nbIndexes = newNbIndexes;
      indexes = new int[nbIndexes];
    }

  }

  class Layer
  {

    private MeshBuffer mBuffer;
    private Mesh mMesh;
    private Material mMaterial;
    private int mPositionIterator, mIndexIterator, mIndex;
    private bool mClearThisTime;
    private Vector3 mTransformPosition;
    private Vector3 mTransformScale;
    private Matrix4x4 mTransform;

    public Color32 colour = new Color32(255, 255, 255, 255);
    public Vector2 quadOrigin = Vector2.zero;

    public Layer(Mesh mesh, Material material, int scale)
    {
      mMesh = mesh;
      mMaterial = material;
      mTransformScale = new Vector3(1.0f, -1.0f, 1.0f);

      mBuffer = new MeshBuffer();

      const float kDepth = 0.0f;
      tP0.z = kDepth;
      tP1.z = kDepth;
      tP2.z = kDepth;
      tP3.z = kDepth;

      mTransformPosition = new Vector3(-Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
      SetScale(scale);
    }

    public void SetScale(int scale)
    {
      int s = Mathf.Abs(scale);
      mTransformScale.x = s;
      mTransformScale.y = -s;
      mTransformScale.z = 1.0f;
      RefreshTransform();
    }

    void RefreshTransform()
    {
      mTransform = Matrix4x4.TRS(mTransformPosition, Quaternion.identity, mTransformScale);
    }

    public void Draw()
    {
      mMaterial.SetPass(0);
      Graphics.DrawMeshNow(mMesh, mTransform);
    }

    public void Begin(int nbQuads)
    {
      const int verticesPerQuad = 4;
      const int indexesPerQuad = 6;

      int nbVertices = verticesPerQuad * nbQuads;
      int nbIndexes = indexesPerQuad * nbQuads;

      mClearThisTime = mBuffer.Reserve(nbVertices, nbIndexes);

      mPositionIterator = 0;
      mIndexIterator = 0;
      mIndex = 0;

      colour = Color.white;
      quadOrigin = Vector2.zero;
    }

    private static Vector3 tP0, tP1, tP2, tP3;
    private static Vector2 tU0, tU1, tU2, tU3;



    public void Add(int x, int y, int w, int h, GiraffeSprite sprite)
    {


      // 0---1
      // |\  |
      // | \ |
      // 3--\2

      tP0.x = (x + quadOrigin.x);
      tP0.y = (y + quadOrigin.y);
      tU0.x = sprite.x0;
      tU0.y = sprite.y1;

      tP1.x = (x + w + quadOrigin.x);
      tP1.y = (y + quadOrigin.y);
      tU1.x = sprite.x1;
      tU1.y = sprite.y1;

      tP2.x = (x + w + quadOrigin.x);
      tP2.y = (y + h + quadOrigin.y);
      tU2.x = sprite.x1;
      tU2.y = sprite.y0;

      tP3.x = (x + quadOrigin.x);
      tP3.y = (y + h + quadOrigin.y);
      tU3.x = sprite.x0;
      tU3.y = sprite.y0;

      mBuffer.position[mPositionIterator] = tP0;
      mBuffer.uv[mPositionIterator] = tU0;
      mBuffer.colours[mPositionIterator++] = colour;

      mBuffer.position[mPositionIterator] = tP1;
      mBuffer.uv[mPositionIterator] = tU1;
      mBuffer.colours[mPositionIterator++] = colour;

      mBuffer.position[mPositionIterator] = tP2;
      mBuffer.uv[mPositionIterator] = tU2;
      mBuffer.colours[mPositionIterator++] = colour;

      mBuffer.position[mPositionIterator] = tP3;
      mBuffer.uv[mPositionIterator] = tU3;
      mBuffer.colours[mPositionIterator++] = colour;

      mBuffer.indexes[mIndexIterator++] = mIndex;
      mBuffer.indexes[mIndexIterator++] = mIndex + 1;
      mBuffer.indexes[mIndexIterator++] = mIndex + 2;

      mBuffer.indexes[mIndexIterator++] = mIndex;
      mBuffer.indexes[mIndexIterator++] = mIndex + 2;
      mBuffer.indexes[mIndexIterator++] = mIndex + 3;
      mIndex += 4;

    }

    public void Add(Matrix2D transform, GiraffeSprite sprite)
    {


      // 0---1
      // |\  |
      // | \ |
      // 3--\2

      transform.Transform(-0.5f, -0.5f, ref tP0.x, ref tP0.y);
      tP0.x += quadOrigin.x;
      tP0.y += quadOrigin.y;

      tU0.x = sprite.x0;
      tU0.y = sprite.y1;

      transform.Transform(0.5f, -0.5f, ref tP1.x, ref tP1.y);
      tP1.x += quadOrigin.x;
      tP1.y += quadOrigin.y;

      tU1.x = sprite.x1;
      tU1.y = sprite.y1;

      transform.Transform(0.5f, 0.5f, ref tP2.x, ref tP2.y);
      tP2.x += quadOrigin.x;
      tP2.y += quadOrigin.y;

      tU2.x = sprite.x1;
      tU2.y = sprite.y0;

      transform.Transform(-0.5f, 0.5f, ref tP3.x, ref tP3.y);
      tP3.x += quadOrigin.x;
      tP3.y += quadOrigin.y;

      tU3.x = sprite.x0;
      tU3.y = sprite.y0;

      mBuffer.position[mPositionIterator] = tP0;
      mBuffer.uv[mPositionIterator] = tU0;
      mBuffer.colours[mPositionIterator++] = colour;

      mBuffer.position[mPositionIterator] = tP1;
      mBuffer.uv[mPositionIterator] = tU1;
      mBuffer.colours[mPositionIterator++] = colour;

      mBuffer.position[mPositionIterator] = tP2;
      mBuffer.uv[mPositionIterator] = tU2;
      mBuffer.colours[mPositionIterator++] = colour;

      mBuffer.position[mPositionIterator] = tP3;
      mBuffer.uv[mPositionIterator] = tU3;
      mBuffer.colours[mPositionIterator++] = colour;

      mBuffer.indexes[mIndexIterator++] = mIndex;
      mBuffer.indexes[mIndexIterator++] = mIndex + 1;
      mBuffer.indexes[mIndexIterator++] = mIndex + 2;

      mBuffer.indexes[mIndexIterator++] = mIndex;
      mBuffer.indexes[mIndexIterator++] = mIndex + 2;
      mBuffer.indexes[mIndexIterator++] = mIndex + 3;
      mIndex += 4;

    }

    public void End()
    {

      mMesh.MarkDynamic();

      if (mClearThisTime)
      {
        mMesh.Clear();
      }

      mMesh.vertices = mBuffer.position;
      mMesh.uv = mBuffer.uv;
      mMesh.colors32 = mBuffer.colours;

      mMesh.SetIndices(mBuffer.indexes, MeshTopology.Triangles, 0);
    }

  }

  public static class GiraffeUtils
  {

    public static T FindRecursiveComponentBackwards<T>(Transform self) where T : MonoBehaviour
    {
      while (self != null)
      {
        T component = self.GetComponent<T>();
        if (component != null)
          return component;
        self = self.transform.parent;
      }
      return null;
    }
  }

}
