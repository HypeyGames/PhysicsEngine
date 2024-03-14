using System;
using UnityEditor;
using UnityEngine;

namespace HyperPhysics
{
    public class SphereCollider : Collider
    {
        public override ColliderTypes ColliderType => ColliderTypes.Sphere;

        [field: SerializeField] public float Radius { get; set; }

        public override float CheckForCollision(Collider other)
        {
            switch (other.ColliderType)
            {
                case ColliderTypes.Sphere:
                    var radius = (other as SphereCollider).Radius;
                    var distance = Vector3.Distance(Position, other.Position);
                    return distance - (radius + Radius);
            }

            return Mathf.Infinity;
        }

        public override void SetRigidBody(Rigidbody rigidbody)
        {
            Rigidbody = rigidbody;
        }

        private void OnDrawGizmosSelected()
        {
            Handles.matrix = transform.localToWorldMatrix;
            Handles.color = new Color(.5f, 1, .5f, 0.5f);
            Vector3 position = Vector3.zero;

            Handles.DrawWireDisc(position, Vector3.right, Radius);
            Handles.DrawWireDisc(position, Vector3.up, Radius);
            Handles.DrawWireDisc(position, Vector3.forward, Radius);
            Handles.color = new Color(.5f, 2, .5f, 1);
            if (Camera.current.orthographic)
            {
                Vector3 normal = position - Handles.inverseMatrix.MultiplyVector(Camera.current.transform.forward);
                float sqrMagnitude = normal.sqrMagnitude;
                float num0 = Radius * Radius;
                Handles.DrawWireDisc(position - num0 * normal / sqrMagnitude, normal, Radius);
            }
            else
            {
                Vector3 normal = position - Handles.inverseMatrix.MultiplyPoint(Camera.current.transform.position);
                float sqrMagnitude = normal.sqrMagnitude;
                float num0 = Radius * Radius;
                float num1 = num0 * num0 / sqrMagnitude;
                float num2 = Mathf.Sqrt(num0 - num1);
                Handles.DrawWireDisc(position - num0 * normal / sqrMagnitude, normal, num2);
            }
        }
    }
}