using UnityEditor;
using UnityEngine;

namespace HyperPhysics
{
    public class SphereCollider : Collider
    {
        public override ColliderTypes ColliderType => ColliderTypes.Sphere;

        [field: SerializeField, Range(.01f, 1000)]
        public float Radius { get; private set; }

        public override AA3DBB AABB => new(Radius, Position);

        public override Collision CheckForCollision(Collider other)
        {
            Collision collision = default;
            switch (other.ColliderType)
            {
                case ColliderTypes.Sphere:

                    collision = base.CheckForCollision(other);

                    UpdatePenetration(ref collision);
                    
                    if (collision.Penetration < 0)
                    {
                        collision.CollisionType = CollisionType.NotValid;
                        return collision;
                    }

                    collision.Normal = (other.Position - Position);
                    collision.Normal = collision.Normal.normalized;
                    collision.Point1 = Position + collision.Normal * Radius;
                    collision.Point2 = other.Position - collision.Normal * (collision.Body2 as SphereCollider).Radius;

                    break;
            }

            return collision;
        }

        public override void UpdatePenetration(ref Collision collision)
        {
            var distance = Vector3.Distance(Position, collision.Body2.Position);
            var radius = (collision.Body2 as SphereCollider).Radius;
            collision.Penetration = (radius + Radius) - distance;
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