using Unity.Entities;
using UnityEngine;

public class DestroyComponentAuthoring : MonoBehaviour
{
    public bool DestroyComponent;

    public class DestroyComponentBaker : Baker<DestroyComponentAuthoring>
    {
        public override void Bake(DestroyComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DestroyComponent { Value = authoring.DestroyComponent });
        }
    }
}