using Unity.Entities;
using UnityEngine;

namespace GraphTookKitDB.Runtime
{
    public class GraphDatabaseComponentAuthoring : MonoBehaviour
    {
        public GDBAsset sourceAsset;

        public class GraphDatabaseComponentBaker : Baker<GraphDatabaseComponentAuthoring>
        {
            public override void Bake(GraphDatabaseComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GraphDatabaseComponent
                {
                    BlobAssetRef = authoring.sourceAsset.CreateBlobAsset()
                });
            }
        }
    }
}