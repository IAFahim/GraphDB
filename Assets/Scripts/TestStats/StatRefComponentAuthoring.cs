using Unity.Entities;
using UnityEngine;

public class StatRefComponentAuthoring : MonoBehaviour
{
    public int Index;

    public class StatRefComponentBaker : Baker<StatRefComponentAuthoring>
    {
        public override void Bake(StatRefComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new StatRefComponent { Index = authoring.Index });
        }
    }
}