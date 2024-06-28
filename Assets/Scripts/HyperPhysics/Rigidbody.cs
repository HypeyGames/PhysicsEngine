using UnityEngine;
using UnityEngine.Pool;
using Quaternion = HyperPhysics.MathH.Quaternion;
using Vector3 = HyperPhysics.MathH.Vector3;

namespace HyperPhysics
{
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
                [00] = 1 - 2 * (rotation.Y * rotation.Y + rotation.Z * rotation.Z),
                [04] = 2 * (rotation.X * rotation.Y + rotation.Z * rotation.W),
                [08] = 2 * (rotation.X * rotation.Z - rotation.Y * rotation.W),
                [12] = position.X,
                [01] = 2 * (rotation.X * rotation.Y - rotation.Z * rotation.W),
                [05] = 1 - 2 * (rotation.X * rotation.X + rotation.Z * rotation.Z),
                [09] = 2 * (rotation.Y * rotation.Z - rotation.X * rotation.W),
                [13] = position.Y,
                [02] = 2 * (rotation.X * rotation.Z + rotation.Y * rotation.W),
                [06] = 2 * (rotation.Y * rotation.Z - rotation.X * rotation.W),
                [10] = 1 - 2 * (rotation.X * rotation.X + rotation.Y * rotation.Y),
                [14] = position.Z,
                [15] = 1
            };
        }
    }
}