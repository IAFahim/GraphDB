// ----- AUTO-GENERATED ECS AUTHORING + BAKER BY GraphToolGenerator.cs -----

using Unity.Entities;
using UnityEngine;
using GraphTookKitDB.Runtime.ECS;

namespace GraphTookKitDB.Runtime.ECS
{
    public sealed class GDBAuthoring : MonoBehaviour
    {
        public GDBAsset SourceAsset;
    }

    public sealed class GDBBaker : Baker<GDBAuthoring>
    {
        public override void Bake(GDBAuthoring authoring)
        {
            if (authoring.SourceAsset == null) return;
            var blob = authoring.SourceAsset.CreateBlobAsset();
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GDBComponent { Blob = blob });
        }
    }
}