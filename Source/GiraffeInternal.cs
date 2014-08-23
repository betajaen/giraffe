using UnityEngine;

namespace GiraffeInternal
{

  class MeshBuffer
  {
    public Vector3[] position;
    public Vector2[] uv;
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

      nbIndexes = newNbIndexes;
      indexes = new int[nbIndexes];
    }

  }

  class Layer
  {

    private MeshBuffer mBuffer;
    private Mesh mMesh;
    private Material mMaterial;
    private float mInvTextureWidth, mInvTextureHeight;
    private int mEstimatedQuads;
    private int mPositionIterator, mIndexIterator, mIndex;
    private int mUpdateCount, mDrawCount;
    private bool mClearThisTime;
    private Vector3 mTexelOffset;

    public Layer(Mesh mesh, Material material)
    {
      mMesh = mesh;
      mMaterial = material;
      Texture texture = mMaterial.mainTexture;
      mInvTextureWidth = 1.0f / texture.width;
      mInvTextureHeight = 1.0f / texture.height;

      mBuffer = new MeshBuffer();

      // Texel
      if (Application.platform == RuntimePlatform.WindowsPlayer ||
          Application.platform == RuntimePlatform.WindowsWebPlayer ||
          Application.platform == RuntimePlatform.WindowsEditor)
      {
        mTexelOffset = new Vector3(0.5f, -0.5f, 0.0f);
      }

      const float depth = 0.0f;
      tP0.z = depth;
      tP1.z = depth;
      tP2.z = depth;
      tP3.z = depth;
    }

    void Delete()
    {
    }

    public void Update()
    {
      mUpdateCount++;
    }

    public void Draw()
    {
      mDrawCount++;
      mMaterial.SetPass(0);
      Graphics.DrawMeshNow(mMesh, new Vector3(-Screen.width * 0.5f, Screen.height * 0.5f, 0.0f), Quaternion.identity);
    }

    public void Begin(int nbQuads)
    {
      const int verticesPerQuad = 4;
      const int indexesPerQuad = 6;

      int nbVertices = verticesPerQuad * nbQuads;
      int nbIndexes = indexesPerQuad * nbQuads;

      int changes = mBuffer.bufferChanges;
      mClearThisTime = mBuffer.Reserve(nbVertices, nbIndexes);


      mPositionIterator = 0;
      mIndexIterator = 0;
      mIndex = 0;

    }

    private static Vector3 tP0, tP1, tP2, tP3;
    private static Vector2 tU0, tU1, tU2, tU3;


    public void Add(int x, int y, int w, int h, GiraffeSprite sprite)
    {


      // 0---1
      // |\  |
      // | \ |
      // 3--\2

      const float yScale = -1.0f;

      tP0.x = x;
      tP0.y = yScale * y;
      tU0.x = sprite.x0;
      tU0.y = sprite.y1;

      tP1.x = x + w;
      tP1.y = yScale * y;
      tU1.x = sprite.x1;
      tU1.y = sprite.y1;

      tP2.x = x + w;
      tP2.y = yScale * (y + h);
      tU2.x = sprite.x1;
      tU2.y = sprite.y0;

      tP3.x = x;
      tP3.y = yScale * (y + h);
      tU3.x = sprite.x0;
      tU3.y = sprite.y0;

      mBuffer.position[mPositionIterator] = tP0;
      mBuffer.uv[mPositionIterator++] = tU0;

      mBuffer.position[mPositionIterator] = tP1;
      mBuffer.uv[mPositionIterator++] = tU1;

      mBuffer.position[mPositionIterator] = tP2;
      mBuffer.uv[mPositionIterator++] = tU2;

      mBuffer.position[mPositionIterator] = tP3;
      mBuffer.uv[mPositionIterator++] = tU3;

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

      const float yScale = -1.0f;

      transform.Transform(-0.5f, -0.5f, ref tP0.x, ref tP0.y);
      tP0.y *= yScale;
      tU0.x = sprite.x0;
      tU0.y = sprite.y1;

      transform.Transform(0.5f, -0.5f, ref tP1.x, ref tP1.y);
      tP1.y *= yScale;
      tU1.x = sprite.x1;
      tU1.y = sprite.y1;

      transform.Transform(0.5f, 0.5f, ref tP2.x, ref tP2.y);
      tP2.y *= yScale;
      tU2.x = sprite.x1;
      tU2.y = sprite.y0;

      transform.Transform(-0.5f, 0.5f, ref tP3.x, ref tP3.y);
      tP3.y *= yScale;
      tU3.x = sprite.x0;
      tU3.y = sprite.y0;

      mBuffer.position[mPositionIterator] = tP0;
      mBuffer.uv[mPositionIterator++] = tU0;

      mBuffer.position[mPositionIterator] = tP1;
      mBuffer.uv[mPositionIterator++] = tU1;

      mBuffer.position[mPositionIterator] = tP2;
      mBuffer.uv[mPositionIterator++] = tU2;

      mBuffer.position[mPositionIterator] = tP3;
      mBuffer.uv[mPositionIterator++] = tU3;

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

      mMesh.SetIndices(mBuffer.indexes, MeshTopology.Triangles, 0);
    }

  }

}
