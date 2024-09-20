using UnityEngine;

namespace HyperPhysics
{
    public class BoxCollider : Collider
    {
        public override ColliderTypes ColliderType => ColliderTypes.Cube;

        [field: SerializeField] public Vector3 Size { get; private set; }
        [field: SerializeField] public Vector3 Center { get; private set; }
    }
}