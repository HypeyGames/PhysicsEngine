using System;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
#if Physics_Debug
using UnityEditor;
#endif

namespace HyperPhysics
{
    [BurstCompile, SelectionBase]
    public class Collider : MonoBehaviour
    {
        public bool Static;
        public IReadOnlyList<Collision> Collisions => _collisions;
        protected virtual AA3DBB AABB { get; set; }
        public virtual ColliderTypes ColliderType { get; }
        public Vector3 Position;

        [field: SerializeField] public Quaternion Rotation { get; set; }

        [field: SerializeField, Range(0, 1)] public float Bounciness { get; set; } = 1;

        [field: SerializeField] public Rigidbody Rigidbody { get; protected set; }

        private List<Collision> _collisions = new(32);

        private void OnEnable()
        {
            Static = Rigidbody == null;
            var colliderTransform = transform;
            Position = colliderTransform.position;
            Rotation = colliderTransform.rotation;
            HyperPhysics.Instance.AddCollider(this);
        }

        public void SetRigidBody(Rigidbody rigidbody)
        {
            Rigidbody = rigidbody;
        }

        private void OnDisable()
        {
            HyperPhysics.Instance.RemoveCollider(this);
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
#if UNITY_EDITOR

        protected void DrawBoundingBox(AA3DBB aa3Dbb)
        {
#if Physics_Debug
            Handles.color = Color.red;
            Handles.DrawWireCube(transform.position, new Vector3(aa3Dbb.X.Size, aa3Dbb.Y.Size, aa3Dbb.Z.Size));
#endif
        }

#endif
    }

    [Serializable]
    public struct Bounds
    {
        public float Min;
        public float Max;
        public float Center => (Min + Max) / 2;
        public float Size => Max - Min;


        public Bounds(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public void Encapsulate(Bounds bounds)
        {
            Min = Mathf.Min(bounds.Min, Min);
            Max = Mathf.Max(bounds.Max, Max);
        }
    }

    public static class CollisionExt
    {
    }
}