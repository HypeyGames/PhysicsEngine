using System;
using HyperPhysics.MathH;
using UnityEngine;

namespace HyperPhysics
{
    [Serializable]
    public struct AA3DBB
    {
        public Bounds X;
        public Bounds Y;
        public Bounds Z;

        public Vector3 Center;


        public AA3DBB(float radius, Vector3 center)
        {
            X = new Bounds(center.x - radius, center.x + radius);
            Y = new Bounds(center.y - radius, center.y + radius);
            Z = new Bounds(center.z - radius, center.z + radius);
            Center = center;
        }

        public AA3DBB(Vector3 size, Vector3 center, Quaternion rotation)
        {
            Vector3 max = Vector3.zero;
            Vector3 min = Vector3.zero;
            for (byte i = 0; i < 8; i++)
            {
                var rotatedPoint = rotation * MathExt.FindCubeVertex(i, size);
                max = new Vector3(Mathf.Max(rotatedPoint.x, max.x), Mathf.Max(rotatedPoint.y, max.y), Mathf.Max(rotatedPoint.z, max.z));
                min = new Vector3(Mathf.Min(rotatedPoint.x, min.x), Mathf.Min(rotatedPoint.y, min.y), Mathf.Min(rotatedPoint.z, min.z));
            }

            X = new Bounds(min.x + center.x, max.x + center.x);
            Y = new Bounds(min.y + center.y, max.y + center.y);
            Z = new Bounds(min.z + center.z, max.z + center.z);

            Center = center;
        }

        public bool IsOverlapping(AA3DBB other)
        {
            if (Center.y < other.Center.y)
            {
                if (Y.Max < other.Y.Min) return false;
            }
            else
            {
                if (Y.Min > other.Y.Max) return false;
            }

            if (Center.x < other.Center.x)
            {
                if (X.Max < other.X.Min) return false;
            }
            else
            {
                if (X.Min > other.X.Max) return false;
            }

            if (Center.z < other.Center.z)
            {
                if (Z.Max < other.Z.Min) return false;
            }
            else
            {
                if (Z.Min > other.Z.Max) return false;
            }

            return true;
        }
    }
}