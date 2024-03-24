using System;
using UnityEngine;

namespace HyperPhysics
{
    public abstract class Collider : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody Rigidbody { get; protected set; }

        //    [SerializeField, Range(.01f, 2)] private float _bouciness;

        public bool Static => Rigidbody == null;

        public virtual AA3DBB AABB { get; protected set; }
        public virtual ColliderTypes ColliderType { get; }
        public Vector3 Acceleration { get; set; }
        public Vector3 Position => transform.position;


        // Note: Collision Normal is wrt 1st object for 2nd object(other) its negative.
        public virtual Collision CheckForCollision(Collider other)
        {
            var collision = new Collision();
            var collisionType = other.Static ? 0 : 1;
            collisionType += Static ? 0 : 2;
            collision.CollisionType = (CollisionType)collisionType;
            if (collisionType > 2)
            {
                collision.MassRatio21 = other.Rigidbody.Mass / (Rigidbody.Mass + other.Rigidbody.Mass);
                collision.MassRatio12 = 1 - collision.MassRatio21;
            }

            return collision;
        }

        public bool CheckForOverlap(Collider other)
        {
            return AABB.IsOverlapping(other.AABB);
        }

        public abstract void SetRigidBody(Rigidbody rigidbody);
    }

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
            X = new Bounds(center.x - radius, center.x + radius);
            Y = new Bounds(center.y - radius, center.y + radius);
            Z = new Bounds(center.z - radius, center.z + radius);
            Center = center;
            Size = (2 * radius) * Vector3.one;
        }

        public AA3DBB(Vector3 extents, Vector3 center)
        {
            X = new Bounds(center.z - extents.z, center.z + extents.z);
            Y = new Bounds(center.y - extents.y, center.y + extents.y);
            Z = new Bounds(center.z - extents.z, center.z + extents.z);
            Center = center;
            Size = extents * 2;
        }

        public bool IsOverlapping(AA3DBB other)
        {
            if (Center.x < other.Center.x)
            {
                if (X.Max < other.X.Min) return false;
            }
            else
            {
                if (X.Min > other.X.Max) return false;
            }

            if (Center.y < other.Center.y)
            {
                if (Y.Max < other.Y.Min) return false;
            }
            else
            {
                if (Y.Min > other.Y.Max) return false;
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

    public struct Bounds
    {
        public float Min;
        public float Max;

        public Bounds(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}