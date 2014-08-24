using System;
using UnityEngine;

public struct Matrix2D
{
  public float m00, m01, m02;
  public float m10, m11, m12;
  public float m20, m21, m22;

  public Matrix2D(float s = 1.0f)
  {
    m00 = s;
    m01 = 0.0f;
    m02 = 0.0f;

    m10 = 0.0f;
    m11 = s;
    m12 = 0.0f;

    m20 = 0.0f;
    m21 = 0.0f;
    m22 = s;
  }

  public Matrix2D(float e00, float e01, float e02, float e10, float e11, float e12, float e20, float e21, float e22)
  {
    m00 = e00;
    m01 = e01;
    m02 = e02;
    m10 = e10;
    m11 = e11;
    m12 = e12;
    m20 = e20;
    m21 = e21;
    m22 = e22;
  }

  public static Matrix2D operator *(Matrix2D lhs, Matrix2D rhs)
  {
    return new Matrix2D(
         lhs.m00 * rhs.m00 + lhs.m10 * rhs.m01 + lhs.m20 * rhs.m02,
         lhs.m00 * rhs.m10 + lhs.m10 * rhs.m11 + lhs.m20 * rhs.m12,
         lhs.m00 * rhs.m20 + lhs.m10 * rhs.m21 + lhs.m20 * rhs.m22,
         lhs.m01 * rhs.m00 + lhs.m11 * rhs.m01 + lhs.m21 * rhs.m02,
         lhs.m01 * rhs.m10 + lhs.m11 * rhs.m11 + lhs.m21 * rhs.m12,
         lhs.m01 * rhs.m20 + lhs.m11 * rhs.m21 + lhs.m21 * rhs.m22,
         lhs.m02 * rhs.m00 + lhs.m12 * rhs.m01 + lhs.m22 * rhs.m02,
         lhs.m02 * rhs.m10 + lhs.m12 * rhs.m11 + lhs.m22 * rhs.m12,
         lhs.m02 * rhs.m20 + lhs.m12 * rhs.m21 + lhs.m22 * rhs.m22
      );
  }

  public static Vector2 operator *(Matrix2D lhs, Vector2 v)
  {
    return new Vector2(
        v.x * lhs.m00 + v.y * lhs.m10 + lhs.m20,
        v.x * lhs.m01 + v.y * lhs.m11 + lhs.m21);
  }

  public void Transform(float ix, float iy, ref float ox, ref float oy)
  {
    ox = ix * m00 + iy * m10 + m20;
    oy = ix * m01 + iy * m11 + m21;
  }

  public static Matrix2D Translate(Matrix2D m, Vector2 v)
  {
    return m * new Matrix2D(
      1.0f, 0.0f, 0.0f,
      0.0f, 1.0f, 0.0f,
      v.x, v.y, 1.0f);
  }

  public static Matrix2D Scale(Matrix2D m, Vector2 v)
  {
    return m * new Matrix2D(
      v.x, 0.0f, 0.0f,
      0.0f, v.y, 0.0f,
      0.0f, 0.0f, 1.0f);
  }

  public static Matrix2D Rotate(Matrix2D m, float angle)
  {
    float r = angle;
    float c = Mathf.Cos(r);
    float s = Mathf.Sin(r);

    return m * new Matrix2D(
      c, s, 0.0f,
      -s, c, 0.0f,
      0.0f, 0.0f, 1.0f);
  }

  public static Matrix2D TRS(Vector2 position)
  {
    return new Matrix2D(
       1.0f, 0.0f, 0.0f,
       0.0f, 1.0f, 0.0f,
       position.x, position.y, 1.0f);
  }

  public static Matrix2D TRS(Vector2 position, float angle)
  {
    return TRS(position, angle, Vector2.one);
  }

  public static Matrix2D TRS(Vector2 position, float angle, Vector2 scale)
  {
    float c = Mathf.Cos(angle);
    float s = Mathf.Sin(angle);

    float a00 = scale.x;
    float a01 = 0.0f;
    float a10 = 0.0f;
    float a11 = scale.y;

    float b00 = c;
    float b01 = s;
    float b10 = -s;
    float b11 = c;

    // 00 01
    // 10 11

    // lhs.m00 * rhs.m00 + lhs.m10 * rhs.m01
    // lhs.m00 * rhs.m10 + lhs.m10 * rhs.m11

    // lhs.m01 * rhs.m00 + lhs.m11 * rhs.m01
    // lhs.m01 * rhs.m10 + lhs.m11 * rhs.m11

    float e00 = a00 * b00 + a10 * b01;
    float e01 = a00 * b10 + a10 * b11;
    float e10 = a01 * b00 + a11 * b01;
    float e11 = a01 * b10 + a11 * b11;

    return new Matrix2D(
       e00, e01, 0.0f,
       e10, e11, 0.0f,
       position.x, position.y, 1.0f);
  }

  public override string ToString()
  {
    return String.Format("[{0},{1},{2},  {3},{4},{5},  {6},{7},{8}]", m00, m01, m02, m10, m11, m12, m20, m21, m22);
  }
}

