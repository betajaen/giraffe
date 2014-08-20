using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEngine;
using System.Collections;

namespace GiraffeInternal
{

  class MeshBuffer
  {
    public Vector3[] position;
    public Vector2[] uv;
    public int[] indexes;
    public int nbVertices;
    public int nbIndexes;

    public MeshBuffer(Mesh mesh)
    {
      mMesh = mesh;
    }

    public void Delete()
    {
      position = null;
    }

    public void Reserve(int vertexCount, int indexCount)
    {
      if (vertexCount != nbVertices)
      {
        Debug.Log("Resizing verts");
        nbVertices = vertexCount;
        position = new Vector3[nbVertices];
        uv = new Vector2[nbVertices];
      }

      if (indexCount != nbIndexes)
      {
        Debug.Log("Resizing indexes");
        nbIndexes = indexCount;
        indexes = new int[nbIndexes];
      }
    }

    private Mesh mMesh;
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
    private Vector3 mTexelOffset;

    public Layer(Mesh mesh, Material material)
    {
      mMesh = mesh;
      mMaterial = material;
      Texture texture = mMaterial.mainTexture;
      mInvTextureWidth = 1.0f / texture.width;
      mInvTextureHeight = 1.0f / texture.height;

      mBuffer = new MeshBuffer(mMesh);

      // Texel
      if (Application.platform == RuntimePlatform.WindowsPlayer ||
          Application.platform == RuntimePlatform.WindowsWebPlayer ||
          Application.platform == RuntimePlatform.WindowsEditor)
      {
        mTexelOffset = new Vector3(0.5f, -0.5f, 0.0f);
      }

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
      const int verticesPerQuad = 6;
      const int indexesPerQuad = 6;

      int nbVertices = verticesPerQuad * nbQuads;
      int nbIndexes = indexesPerQuad * nbQuads;

      mBuffer.Reserve(nbVertices, nbIndexes);

      mPositionIterator = 0;
      mIndexIterator = 0;
      mIndex = 0;
    }

    private static Vector3 tP0, tP1, tP2, tP3;
    private static Vector2 tU0, tU1, tU2, tU3;


    public void Quad(int x, int y, int w, int h, GiraffeSprite sprite)
    {


      // 0---1
      // |\  |
      // | \ |
      // 3--\2

      const float depth = 0.0f;
      const float yScale = -1.0f;

      tP0.x = x;
      tP0.y = yScale * y;
      tP0.z = depth;
      tU0.x = sprite.x0;
      tU0.y = sprite.y1;

      tP1.x = x + w;
      tP1.y = yScale * y;
      tP1.z = depth;
      tU1.x = sprite.x1;
      tU1.y = sprite.y1;

      tP2.x = x + w;
      tP2.y = yScale * (y + h);
      tP2.z = depth;
      tU2.x = sprite.x1;
      tU2.y = sprite.y0;

      tP3.x = x;
      tP3.y = yScale * (y + h);
      tP3.z = depth;
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
      mMesh.vertices = mBuffer.position;
      mMesh.uv = mBuffer.uv;
      mMesh.SetIndices(mBuffer.indexes, MeshTopology.Triangles, 0);
    }

  }

}
