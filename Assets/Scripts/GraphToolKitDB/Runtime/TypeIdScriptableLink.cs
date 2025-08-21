using GraphTookKitDB.Runtime;
using UnityEngine;

namespace GraphToolKitDB.Runtime
{
    public class TypeIdScriptableLink : ScriptableObject, IEntity
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