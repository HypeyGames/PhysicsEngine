using System.Collections.Generic;
using HyperPhysics.MathH;
using UnityEngine;
using Quaternion = HyperPhysics.MathH.Quaternion;
using Vector3 = HyperPhysics.MathH.Vector3;

namespace HyperPhysics
{
    public abstract class Collider : MonoBehaviour
    {
        public bool Static;
        public IReadOnlyList<Collision> Collisions => _collisions;
        protected virtual AA3DBB AABB { get; set; }
        public virtual ColliderTypes ColliderType { get; }
        public Vector3 Position { get; set; }
        [field: SerializeField] public Quaternion Rotation { get; set; }

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
            Position = transform.position.FromUnityVector3();
            Rotation = transform.rotation.FromUnityQuaternion();
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