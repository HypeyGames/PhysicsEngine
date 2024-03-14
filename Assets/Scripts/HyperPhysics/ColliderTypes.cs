namespace HyperPhysics
{
    public enum ColliderTypes
    {
        Sphere,
        Cube,
        Capsule,
        Convex,
        Concave
    }

    public enum CollisionType
    {
        NotValid = 0,
        StaticDynamic = 1,
        DynamicStatic = 2,
        DynamicDynamic = 3
    }
}