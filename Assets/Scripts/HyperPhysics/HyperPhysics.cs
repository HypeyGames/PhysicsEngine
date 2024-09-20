using System;
using System.Collections.Generic;
using UnityEngine;

namespace HyperPhysics
{
    public class HyperPhysics
    {
        public static readonly HyperPhysics Instance = new HyperPhysics();
        private static List<Collider> _colliders = new(512);

        internal void AddCollider(Collider collider)
        {
            _colliders.Add(collider);
        }

        internal void RemoveCollider(Collider collider)
        {
            _colliders.Remove(collider);
        }

        internal List<Collider> GetCollider()
        {
            return _colliders;
        }

        internal void UpdatePenetration(ref Collision collision, BoxCollider body1, SphereCollider other, bool forward = true)
        {
            var distance = other.Position - body1.Position;
            var localDistance = body1.transform.InverseTransformDirection(distance);
            collision.Penetration = FindPenetrationTroughBox(other.Radius, body1.Size, localDistance, out var normal);
            normal = body1.transform.TransformDirection(normal);
            collision.Normal = forward ? normal : -normal;
        }

        internal float FindPenetrationTroughBox(float pointRadius, Vector3 boxSize, Vector3 localDistance, out Vector3 normal)
        {
            normal = Vector3.up;

            var maxPenetration = Single.PositiveInfinity;
            var penetrationX = (pointRadius + boxSize.x / 2) - Mathf.Abs(localDistance.x);
            if (penetrationX < maxPenetration)
            {
                maxPenetration = penetrationX;
                normal = localDistance.x > 0 ? Vector3.right : Vector3.left;
            }

            var penetrationY = (pointRadius + boxSize.y / 2) - Mathf.Abs(localDistance.y);
            if (penetrationY < maxPenetration)
            {
                maxPenetration = penetrationY;
                normal = localDistance.y > 0 ? Vector3.up : Vector3.down;
            }

            var penetrationZ = (pointRadius + boxSize.z / 2) - Mathf.Abs(localDistance.z);
            if (penetrationZ < maxPenetration)
            {
                maxPenetration = penetrationZ;
                normal = localDistance.z > 0 ? Vector3.forward : Vector3.back;
            }

            return maxPenetration;
        }

        public void CalculateCollision(ref Collision collision)
        {
            if (collision.Penetration < 0)
            {
                collision.CollisionType = CollisionType.NotValid;
            }

            //     collision.Point1 = collider1.Position + collision.Normal * collider1.Radius;
            // collision.Point2 = collider2.Position - collision.Normal * collider2.Radius;
        }
    }
}