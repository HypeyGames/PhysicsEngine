using HyperPhysics;
using Unity.Collections;
using Unity.Entities;
using Vector3 = HyperPhysics.MathH.Vector3;

public struct ColliderEntity : IComponentData
{
    public ColliderTypes ColliderType;
    public Vector3 Position;
    public FixedList32Bytes<CollisionData> Collisions;
}

public struct CollisionData
{
    public struct Collision
    {
        public Vector3 Point1;
        public Vector3 Point2;
        public Vector3 Normal;

        public float Penetration;
        public float MassRatio21;
        public float MassRatio12;
        public float CollisionVelocity;

        public CollisionType CollisionType;
    }
}