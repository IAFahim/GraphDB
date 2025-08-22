using GraphTookKitDB.Runtime;
using UnityEngine;

namespace GraphToolKitDB.Runtime
{
    [CreateAssetMenu(menuName = "Create/Create LinkTypeSo", fileName = "LinkTypeSo", order = 0)]
    public class LinkTypeSo : ScriptableObject, IEntity
    {
        public Link link;
        public int ID
        {
            get => link.ID;
            set => link.ID = value;
        }
    }
}