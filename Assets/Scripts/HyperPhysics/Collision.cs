using System;
using System.Globalization;
using UnityEngine;

namespace HyperPhysics
{
    public struct Collision
    {
        public Vector3 Point1;
        public Vector3 Point2;
        public Vector3 Normal;
        public Collider Body1;
        public Collider Body2;
        public float Penetration;
        public float MassRatio21;
        public float MassRatio12;
        public float CollisionVelocity;

        public CollisionType CollisionType;

        public void SetCollisionVelocity()
        {
            switch (CollisionType)
            {
                case CollisionType.StaticDynamic:
                    CollisionVelocity = Vector3.Dot(Body2.Rigidbody.Velocity, Normal);
                    break;
                case CollisionType.DynamicStatic:
                    CollisionVelocity = Vector3.Dot(Body1.Rigidbody.Velocity, Normal);
                    break;
                case CollisionType.DynamicDynamic:
                    var relativeVelocity12 = Body1.Rigidbody.Velocity - Body2.Rigidbody.Velocity;
                    CollisionVelocity = Vector3.Dot(relativeVelocity12, Normal);
                    break;
            }
        }

        public void UpdatePenetration()
        {
           Body1.UpdatePenetration(ref this);
        }

        public override string ToString() => $"Normal{Normal.ToString()}::Point={Point1.ToString()},{Point2.ToString()}::Penetration{Penetration.ToString(CultureInfo.InvariantCulture)}";
    }
}