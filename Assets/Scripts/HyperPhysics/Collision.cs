using System.Globalization;
using UnityEngine;

namespace HyperPhysics
{
    public struct Collision
    {
        public Vector3 Point1;
        public Vector3 Point2;
        public Vector3 Normal;
        public float Penetration;
        public float MassRatio21;
        public float MassRatio12;

        public CollisionType CollisionType;

        public override string ToString() => $"Normal{Normal.ToString()}::Point={Point1.ToString()},{Point2.ToString()}::Penetration{Penetration.ToString(CultureInfo.InvariantCulture)}";
    }
}