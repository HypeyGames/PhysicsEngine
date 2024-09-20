using UnityEditor;
using UnityEngine;

namespace HyperPhysics
{
    public class SphereCollider : Collider
    {
        public override ColliderTypes ColliderType => ColliderTypes.Sphere;

        [field: SerializeField, Range(.01f, 1000)]
        public float Radius { get; private set; }

        protected override AA3DBB AABB => new(Radius, Position);

        public override Collision CheckForCollision(Collider other)
        {
            Collision collision = default;
            collision = base.CheckForCollision(other);
            var hyperPhysics = HyperPhysics.Instance;

            switch (other.ColliderType)
            {
                case ColliderTypes.Sphere:
                    CalculateCollisionInternal(ref collision, this, other as SphereCollider);
                    break;
                case ColliderTypes.Box:
                    hyperPhysics.UpdatePenetration(ref collision, other as BoxCollider, this, false);
                    hyperPhysics.CalculateCollision(ref collision);
                    break;
            }

            return collision;
        }

        private void CalculateCollisionInternal(ref Collision collision, SphereCollider collider1, SphereCollider collider2)
        {
            UpdatePenetration(ref collision);
            if (collision.Penetration < 0)
            {
                collision.CollisionType = CollisionType.NotValid;
            }


            collision.Normal = (collider2.Position - collider1.Position);
            collision.Normal = collision.Normal.normalized;
            collision.Point1 = collider1.Position + collision.Normal * collider1.Radius;
            collision.Point2 = collider2.Position - collision.Normal * collider2.Radius;
        }

        public override void UpdatePenetration(ref Collision collision)
        {
            switch (collision.Body2.ColliderType)
            {
                case ColliderTypes.Sphere:
                    UpdatePenetrationInternal(ref collision, collision.Body2 as SphereCollider);
                    break;
                case ColliderTypes.Box:
                    HyperPhysics.Instance.UpdatePenetration(ref collision, collision.Body2 as BoxCollider, this, false);
                    break;
            }
        }

        private void UpdatePenetrationInternal(ref Collision collision, SphereCollider other)
        {
            var distance = Vector3.Distance(Position, collision.Body2.Position);
            var radius = other.Radius;
            collision.Penetration = (radius + Radius) - distance;
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var aabb = new AA3DBB(Radius, transform.position);
            DrawBoundingBox(aabb);
            DrawSphereCollider();
        }

        private void DrawSphereCollider()
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
#endif
    }
}