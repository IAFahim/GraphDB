using GraphTookKitDB.Runtime;
using UnityEngine;

namespace GraphToolKitDB.Runtime
{
    [CreateAssetMenu(menuName = "Create/Create EntryID Schema", fileName = "EntryIDSchema", order = 0)]
    public class EntryIDSchema : ScriptableObject
    {
        public EntityType Type;
        public ushort Id;

        public ushort ID
        {
            get => Id;
            set => Id = value;
        }
    }
}