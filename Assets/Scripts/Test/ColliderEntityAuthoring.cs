using Unity.Entities;
using UnityEngine;
using Collider = HyperPhysics.Collider;

public class ColliderEntityAuthoring : MonoBehaviour
{
   
    public Collider Collider;
    
    private class ColliderBaker: Baker<ColliderEntityAuthoring>
    {
        public override void Bake(ColliderEntityAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,new ColliderEntity()
            {
                ColliderType = authoring.Collider.ColliderType,
            });
        }
    }
}