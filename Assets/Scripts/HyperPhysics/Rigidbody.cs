using UnityEngine;
using UnityEngine.Pool;

namespace HyperPhysics
{
    public class Rigidbody : MonoBehaviour
    {
        [field: SerializeField] public float Mass { get; set; } = 1;
        [field: SerializeField] public Vector3 Velocity { get; set; }
        [field: SerializeField] public Vector3 Acceleration { get; set; }
        [field: SerializeField] public float Damping { get; set; }
        [field: SerializeField] public bool Gravity { get; set; }

        private void OnValidate()
        {
            ListPool<Collider>.Get(out var colliders);
            GetComponentsInChildren(colliders);
            foreach (var collider in colliders)
            {
                collider.SetRigidBody(this);
            }
        }
    }
}