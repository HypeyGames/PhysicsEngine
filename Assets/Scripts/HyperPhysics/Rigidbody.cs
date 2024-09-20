using Unity.Burst;
using UnityEngine;
using UnityEngine.Pool;

namespace HyperPhysics
{
    [BurstCompile]
    public class Rigidbody : MonoBehaviour
    {
        [field: SerializeField] public float Mass { get; set; } = 1;
        [field: SerializeField] public Vector3 Velocity { get; set; }
        [field: SerializeField] public Vector3 AngularVelocity { get; set; }
        [field: SerializeField] public Vector3 Acceleration { get; set; }
        [field: SerializeField] public Vector3 AngularAcceleration { get; set; }
        [field: SerializeField] public float Damping { get; set; }
        [field: SerializeField] public bool Gravity { get; set; }

        [field: SerializeField] public Vector3 Force { get; set; }
        [field: SerializeField] public Vector3 Torque { get; set; }


        private void OnValidate()
        {
            ListPool<Collider>.Get(out var colliders);
            GetComponentsInChildren(colliders);
            foreach (var collider in colliders)
            {
                collider.SetRigidBody(this);
            }
        }

        private void CalculateDerivedData(Quaternion rotation, Vector3 position)
        {
            Matrix4x4 matrix4X4 = new Matrix4x4
            {
                [00] = 1 - 2 * (rotation.y * rotation.y + rotation.z * rotation.z),
                [04] = 2 * (rotation.x * rotation.y + rotation.z * rotation.w),
                [08] = 2 * (rotation.x * rotation.z - rotation.y * rotation.w),
                [12] = position.x,
                [01] = 2 * (rotation.x * rotation.y - rotation.z * rotation.w),
                [05] = 1 - 2 * (rotation.x * rotation.x + rotation.z * rotation.z),
                [09] = 2 * (rotation.y * rotation.z - rotation.x * rotation.w),
                [13] = position.y,
                [02] = 2 * (rotation.x * rotation.z + rotation.y * rotation.w),
                [06] = 2 * (rotation.y * rotation.z - rotation.x * rotation.w),
                [10] = 1 - 2 * (rotation.x* rotation.x + rotation.y * rotation.y),
                [14] = position.z,
                [15] = 1
            };
        }
    }
}