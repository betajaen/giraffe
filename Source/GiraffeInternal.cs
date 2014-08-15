using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEngine;
using System.Collections;

namespace GiraffeInternal
{

  static class Common
  {
    static void ConfigureCamera(Camera camera)
    {
      camera.cullingMask = 0;
      camera.rect = new Rect(0, 0, 1, 1);
    }
  }

  class MeshBuffer
  {
    public Vector3[] position;
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
    private int mEstimatedQuads;
    private int mPositionIterator, mIndexIterator, mIndex;
    private int mUpdateCount, mDrawCount;

    public Layer(Mesh mesh)
    {
      mMesh = mesh;
      mBuffer = new MeshBuffer(mMesh);
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
      Quad(new Vector2(10.0f, 10.0f), new Vector2(100.0f, 100.0f));
      End();

    }

    public void Draw()
    {
      mDrawCount++;
      Graphics.DrawMeshNow(mMesh, new Vector3(-Screen.width * 0.5f, Screen.height * 0.5f, 0f), Quaternion.identity);
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

    static Vector3 tQ0, tQ1, tQ2, tQ3;

    public void Quad(Vector2 position, Vector2 size)
    {

      // 0---1
      // |\  |
      // | \ |
      // 3--\2

      const float depth = 1.0f;
      const float yScale = -1.0f;

      tQ0.x = position.x;
      tQ0.y = yScale * position.y;
      tQ0.z = depth;

      tQ1.x = position.x + size.x;
      tQ1.y = yScale * position.y;
      tQ1.z = depth;

      tQ2.x = position.x + size.x;
      tQ2.y = yScale * (position.y + size.y);
      tQ2.z = depth;

      tQ3.x = position.x;
      tQ3.y = yScale * (position.y + size.y);
      tQ3.z = depth;

      mBuffer.position[mPositionIterator++] = tQ0;
      mBuffer.position[mPositionIterator++] = tQ1;
      mBuffer.position[mPositionIterator++] = tQ2;
      mBuffer.position[mPositionIterator++] = tQ3;

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
      mMesh.SetIndices(mBuffer.indexes, MeshTopology.Triangles, 0);
    }

  }

}
