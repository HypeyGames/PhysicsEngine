using System;
using HyperPhysics.MathH;

namespace HyperPhysics
{
    [Serializable]
    public struct AA3DBB
    {
        public Bounds X;
        public Bounds Y;
        public Bounds Z;

        public Vector3 Center;
        public Vector3 Size;

        public AA3DBB(float radius, Vector3 center)
        {
            X = new Bounds(center.X - radius, center.X + radius);
            Y = new Bounds(center.Y - radius, center.Y + radius);
            Z = new Bounds(center.Z - radius, center.Z + radius);
            Center = center;
            Size = (2 * radius) * Vector3.One;
        }

        public AA3DBB(Vector3 extents, Vector3 center)
        {
            X = new Bounds(center.Z - extents.Z, center.Z + extents.Z);
            Y = new Bounds(center.Y - extents.Y, center.Y + extents.Y);
            Z = new Bounds(center.Z - extents.Z, center.Z + extents.Z);
            Center = center;
            Size = extents * 2;
        }

        public bool IsOverlapping(AA3DBB other)
        {
            if (Center.X < other.Center.X)
            {
                if (X.Max < other.X.Min) return false;
            }
            else
            {
                if (X.Min > other.X.Max) return false;
            }

            if (Center.Y < other.Center.Y)
            {
                if (Y.Max < other.Y.Min) return false;
            }
            else
            {
                if (Y.Min > other.Y.Max) return false;
            }

            if (Center.Z < other.Center.Z)
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