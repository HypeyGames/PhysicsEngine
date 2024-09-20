using HyperPhysics.MathH;
using UnityEditor;
using UnityEngine;

namespace HyperPhysics
{
    public class BoxCollider : Collider
    {
        public override ColliderTypes ColliderType => ColliderTypes.Box;
        protected override AA3DBB AABB => new(Size, Position, Rotation);

        [field: SerializeField] public Vector3 Size { get; private set; }

        public override Collision CheckForCollision(Collider other)
        {
            Collision collision = default;
            collision = base.CheckForCollision(other);
            var hyperPhysics = HyperPhysics.Instance;

            switch (other.ColliderType)
            {
                case ColliderTypes.Sphere:
                    hyperPhysics.UpdatePenetration(ref collision, this, other as SphereCollider);
                    hyperPhysics.CalculateCollision(ref collision);
                    break;
                case ColliderTypes.Box:
                    UpdatePenetrationInternal(ref collision, other as BoxCollider);
                    break;
            }

            return collision;
        }

        public override void UpdatePenetration(ref Collision collision)
        {
            switch (collision.Body2.ColliderType)
            {
                case ColliderTypes.Sphere:
                    HyperPhysics.Instance.UpdatePenetration(ref collision, this, collision.Body2 as SphereCollider);
                    break;
                case ColliderTypes.Box:
                    UpdatePenetrationInternal(ref collision, collision.Body2 as BoxCollider);
                    break;
            }
        }

        private void UpdatePenetrationInternal(ref Collision collision, BoxCollider other)
        {
            CalculatePenetration(ref collision, this, other);
            if (collision.Penetration < 0)
            {
                CalculatePenetration(ref collision, other, this);
                collision.Normal = -collision.Normal;
            }

            HyperPhysics.Instance.CalculateCollision(ref collision);
        }

        private void CalculatePenetration(ref Collision collision, BoxCollider body1, BoxCollider other)
        {
            var minSize = Mathf.Min(other.Size.x, other.Size.y);
            minSize = Mathf.Min(other.Size.z, minSize);
            var distance = other.Position - (body1.Position);
            var localDistance = body1.transform.InverseTransformDirection(distance);
            var maxPenetration = collision.Penetration = HyperPhysics.Instance.FindPenetrationTroughBox(minSize / 2, body1.Size, localDistance, out var normal);
            collision.Normal = body1.transform.TransformDirection(normal);

            // If cubes behave wierd and incorrect collision check here first.
            if (maxPenetration > 0) return;

            for (byte i = 0; i < 8; i++)
            {
                var rotatedPoint = other.Rotation * MathExt.FindCubeVertex(i, other.Size) + other.Position;
                distance = rotatedPoint - (body1.Position);
                localDistance = body1.transform.InverseTransformDirection(distance);
                var penetration = HyperPhysics.Instance.FindPenetrationTroughBox(0, body1.Size, localDistance, out normal);
                if (penetration > maxPenetration)
                {
                    collision.Penetration = penetration;
                    maxPenetration = penetration;
                    collision.Normal = body1.transform.TransformDirection(normal);
                }
            }
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            var aabb = new AA3DBB(Size, transform.position, transform.rotation);
            DrawBoundingBox(aabb);
            DrawBoxCollider();
        }

        private void DrawBoxCollider()
        {
            Handles.matrix = transform.localToWorldMatrix;
            Handles.color = new Color(.5f, 1, .5f, 1);
            Handles.DrawWireCube(Vector3.zero, Size);
        }
#endif
    }
}