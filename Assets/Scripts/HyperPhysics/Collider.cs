using UnityEngine;

namespace HyperPhysics
{
    public abstract class Collider : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody Rigidbody { get; protected set; }
        public bool Static => Rigidbody == null;

        public virtual ColliderTypes ColliderType { get; }
        public Vector3 Position => transform.position;

        public abstract Collision CheckForCollision(Collider other);
        public abstract void SetRigidBody(Rigidbody rigidbody);
    }
}