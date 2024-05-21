using System;
using System.Collections.Generic;
using UnityEngine;

namespace HyperPhysics
{
    public abstract class Collider : MonoBehaviour
    {
        public bool Static;
        public IReadOnlyList<Collision> Collisions => _collisions;
        protected virtual AA3DBB AABB { get; set; }
        public virtual ColliderTypes ColliderType { get; }
        public Vector3 Position { get; set; }

        [field: SerializeField] public Rigidbody Rigidbody { get; protected set; }

        private List<Collision> _collisions = new List<Collision>();
        private bool _intialized;

        private void OnEnable()
        {
            Static = Rigidbody == null;
            if (_intialized)
            {
                Initialize();
            }
        }


        private void Start()
        {
            Initialize();
            _intialized = true;
        }

        private void Initialize()
        {
            Position = transform.position;
            PhysicsManager.Instance.AddCollider(this);
        }

        private void OnDisable()
        {
            PhysicsManager.Instance.RemoveCollider(this);
        }

        // Note: Collision Normal is wrt 1st object for 2nd object(other) its negative.
        public virtual Collision CheckForCollision(Collider other)
        {
            var collision = new Collision();
            var collisionType = 0;
            if (other.Rigidbody != Rigidbody)
            {
                collisionType = other.Static ? 0 : 1;
                collisionType += Static ? 0 : 2;
            }

            collision.Body1 = this;
            collision.Body2 = other;
            if (collisionType > 2)
            {
                collision.MassRatio21 = other.Rigidbody.Mass / (Rigidbody.Mass + other.Rigidbody.Mass);
                collision.MassRatio12 = 1 - collision.MassRatio21;
            }
               
            collision.CollisionType = (CollisionType)collisionType;
            return collision;
        }

        public virtual void UpdatePenetration(ref Collision collision)
        {
        }

        public void OnPostCollision(Collider other, Collision collision)
        {
            _collisions.Add(collision);
            other._collisions.Add(collision);
        }

        public bool CheckForOverlap(Collider other)
        {
            return AABB.IsOverlapping(other.AABB);
        }

        public void ResetCollider()
        {
            _collisions.Clear();
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