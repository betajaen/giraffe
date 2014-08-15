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
    private int mEstimatedQuads;
    private int mPositionIterator, mIndexIterator, mIndex;
    private int mUpdateCount, mDrawCount;
    private Vector3 mTexelOffset;

    public Layer(Mesh mesh, Material material)
    {
      mMesh = mesh;
      mMaterial = material;
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
      BeginEstimation();
      AddToEstimation(1);
      EndEstimation();

      Begin();
      Quad(new Vector2(10.0f, 10.0f), new Vector2(32.0f, 32.0f), new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f));
      End();
    }

    public void Draw()
    {
      mDrawCount++;
      mMaterial.SetPass(0);
      Graphics.DrawMeshNow(mMesh, new Vector3(-Screen.width * 0.5f, Screen.height * 0.5f, 0.0f), Quaternion.identity);
    }

    public void BeginEstimation()
    {
      mEstimatedQuads = 0;
    }

    public void AddToEstimation(int nbQuads)
    {
      mEstimatedQuads += nbQuads;
    }

    public void EndEstimation()
    {
      const int verticesPerQuad = 6;
      const int indexesPerQuad = 6;

      int nbVertices = verticesPerQuad * mEstimatedQuads;
      int nbIndexes = indexesPerQuad * mEstimatedQuads;

      mBuffer.Reserve(nbVertices, nbIndexes);
    }

    public void Begin()
    {
      mPositionIterator = 0;
      mIndexIterator = 0;
      mIndex = 0;
    }

    private static Vector3 tP0, tP1, tP2, tP3;
    private static Vector2 tU0, tU1, tU2, tU3;

    public void Quad(Vector2 position, Vector2 size, Vector2 uvPosition, Vector2 uvSize)
    {

      // 0---1
      // |\  |
      // | \ |
      // 3--\2

      const float depth = 0.0f;
      const float yScale = -1.0f;

      tP0.x = position.x;
      tP0.y = yScale * position.y;
      tP0.z = depth;
      tU0.x = uvPosition.x;
      tU0.y = 1.0f - uvPosition.y;

      tP1.x = position.x + size.x;
      tP1.y = yScale * position.y;
      tP1.z = depth;
      tU1.x = uvPosition.x + uvSize.x;
      tU1.y = 1.0f - uvPosition.y;

      tP2.x = position.x + size.x;
      tP2.y = yScale * (position.y + size.y);
      tP2.z = depth;
      tU2.x = uvPosition.x + uvSize.x;
      tU2.y = 1.0f - (uvPosition.y + uvSize.y);

      tP3.x = position.x;
      tP3.y = yScale * (position.y + size.y);
      tP3.z = depth;
      tU3.x = uvPosition.x;
      tU3.y = 1.0f - (uvPosition.y + uvSize.y);

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
