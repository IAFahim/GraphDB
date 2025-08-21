using GraphTookKitDB.Runtime;
using UnityEngine;

namespace GraphToolKitDB.Runtime
{
    [CreateAssetMenu(menuName = "Create/Create LinkTypeSo", fileName = "LinkTypeSo", order = 0)]
    public class LinkTypeSo : ScriptableObject, IEntity
    {
        public EntityType type;
        public int id;

        public int ID
        {
            get => id;
            set => id = value;
        }
    }
}