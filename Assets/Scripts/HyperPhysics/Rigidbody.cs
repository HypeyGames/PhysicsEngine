using UnityEngine;
using UnityEngine.Pool;

namespace HyperPhysics
{
    public class Rigidbody : MonoBehaviour
    {
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