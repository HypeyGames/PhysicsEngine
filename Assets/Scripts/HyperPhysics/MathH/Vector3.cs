using System;

namespace HyperPhysics.MathH
{
    [Serializable]
    public struct Vector3
    {
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z == b.Z;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3 operator *(float a, Vector3 b)
        {
            return new Vector3(a * b.X, a * b.Y, a * b.Z);
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return b * a;
        }

        public static Vector3 operator /(Vector3 a, float b)
        {
            return (1 / b) * a;
        }

        public static Vector3 operator /(float a, Vector3 b)
        {
            return b / a;
        }

        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3((a.Y * b.Z) - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            var diff = a - b;
            return diff.Magnitude;
        }

        public float Magnitude => MathF.Sqrt(X * X + Y * Y + Z * Z);
        public static Vector3 One => new Vector3(1, 1, 1);
        public static Vector3 Zero => new Vector3(0, 0, 0);
        public float SqrMagnitude => X * X + Y * Y + Z * Z;
        public Vector3 Normalized => this / Magnitude;

        public float X;
        public float Y;
        public float Z;
    }

    public static class Vector3Ext
    {
        public static Vector3 ToVector3FromUnityVector3(this UnityEngine.Vector3 vector3)
        {
            return new Vector3(vector3.x, vector3.y, vector3.z);
        }

        public static UnityEngine.Vector3 ToUnityVector3FromVector3(this Vector3 vector3)
        {
            return new UnityEngine.Vector3(vector3.X, vector3.Y, vector3.Z);
        }
    }
}