using UnityEngine;

namespace HyperPhysics
{
    public abstract class Collider : MonoBehaviour
    {
        [field: SerializeField] protected Rigidbody Rigidbody { get; set; }

        public virtual ColliderTypes ColliderType { get; }
        public Vector3 Position => transform.position;

        public abstract float CheckForCollision(Collider other);
        public abstract void SetRigidBody(Rigidbody rigidbody);
    }
}