// ----- AUTO-GENERATED EDITOR FILE BY GraphToolGenerator.cs -----

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;
using GraphTookKitDB.Runtime;
using GraphToolKitDB.Runtime;
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using GraphTookKitDB.Runtime;

namespace GraphTookKitDB.Editor
{

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> m_Keys = new List<TKey>();
        [SerializeField] private List<TValue> m_Values = new List<TValue>();
        
        private Dictionary<TKey, TValue> m_Dictionary;
        private bool m_IsDeserialized;

        private void EnsureDeserialized()
        {
            if (!m_IsDeserialized)
            {
                m_Dictionary = new Dictionary<TKey, TValue>(m_Keys.Count);
                for (int i = 0; i < m_Keys.Count; i++)
                {
                    if (i < m_Values.Count)
                    {
                        m_Dictionary[m_Keys[i]] = m_Values[i];
                    }
                }
                m_IsDeserialized = true;
            }
        }

        public void OnBeforeSerialize()
        {
            if (m_Dictionary == null) return;
            m_Keys.Clear();
            m_Values.Clear();
            foreach (var kvp in m_Dictionary)
            {
                m_Keys.Add(kvp.Key);
                m_Values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            m_IsDeserialized = false;
        }

        public TValue this[TKey key] { get { EnsureDeserialized(); return m_Dictionary[key]; } set { EnsureDeserialized(); m_Dictionary[key] = value; } }
        public ICollection<TKey> Keys { get { EnsureDeserialized(); return m_Dictionary.Keys; } }
        public ICollection<TValue> Values { get { EnsureDeserialized(); return m_Dictionary.Values; } }
        public int Count { get { EnsureDeserialized(); return m_Dictionary.Count; } }
        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).IsReadOnly;
        public void Add(TKey key, TValue value) { EnsureDeserialized(); m_Dictionary.Add(key, value); }
        public void Add(KeyValuePair<TKey, TValue> item) { EnsureDeserialized(); ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).Add(item); }
        public void Clear() { EnsureDeserialized(); m_Dictionary.Clear(); }
        public bool Contains(KeyValuePair<TKey, TValue> item) { EnsureDeserialized(); return m_Dictionary.Contains(item); }
        public bool ContainsKey(TKey key) { EnsureDeserialized(); return m_Dictionary.ContainsKey(key); }
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { EnsureDeserialized(); ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).CopyTo(array, arrayIndex); }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { EnsureDeserialized(); return m_Dictionary.GetEnumerator(); }
        public bool Remove(TKey key) { EnsureDeserialized(); return m_Dictionary.Remove(key); }
        public bool Remove(KeyValuePair<TKey, TValue> item) { EnsureDeserialized(); return ((ICollection<KeyValuePair<TKey, TValue>>)m_Dictionary).Remove(item); }
        public bool TryGetValue(TKey key, out TValue value) { EnsureDeserialized(); return m_Dictionary.TryGetValue(key, out value); }
        IEnumerator IEnumerable.GetEnumerator() { EnsureDeserialized(); return m_Dictionary.GetEnumerator(); }
    }


    #region Graph Definition
    [Graph("gdb")]
    [Serializable]
    public class GDBGraph : Graph
    {
        [SerializeField]
        private SerializableDictionary<Type, int> nodeIdCounters = new SerializableDictionary<Type, int>();

        [MenuItem("Assets/Create/GDB Graph")]
        private static void CreateAssetFile() => GraphDatabase.PromptInProjectBrowserToCreateNewAsset<GDBGraph>("New GDB");

        public override void OnGraphChanged(GraphLogger infos)
        {
            base.OnGraphChanged(infos);
            foreach (var node in GetNodes().OfType<IDataNode>())
            {
                if (node.NodeID == 0)
                {
                    var nodeType = node.GetType();
                    nodeIdCounters.TryGetValue(nodeType, out int currentId);
                    currentId++;
                    node.NodeID = (ushort)currentId;
                    nodeIdCounters[nodeType] = currentId;
                }
            }
        }

        public static T GetPortValue<T>(IPort port) {
            if (port.isConnected) {
                var sourceNode = port.firstConnectedPort?.GetNode();
                if (sourceNode is IConstantNode c) { c.TryGetValue(out T v); return v; }
                if (sourceNode is IVariableNode vn) { vn.variable.TryGetDefaultValue(out T v); return v; }
            } else { port.TryGetValue(out T v); return v; }
            return default;
        }
    }
    #endregion

    #region Node Definitions

    public interface IDataNode : INode
    {
        ushort NodeID { get; set; }
        EntityType EntityType { get; }
    }

    [Serializable]
    public abstract class GraphDBNode : ContextNode, IDataNode
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        
        [SerializeField] private ushort nodeId;
        public ushort NodeID
        {
            get => nodeId;
            set => nodeId = value;
        }

        public abstract EntityType EntityType { get; }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<ushort>(PortInputLink).WithDisplayName("In").Build();
            context.AddOutputPort<ushort>(PortOutputLink).WithDisplayName("Out").Build();
        }
    }

   public class DataNode : IDataNode
    {
        public ushort NodeID { get; set; }
        public EntityType EntityType { get; set; }
    }
    [Serializable]
    public class AchievementDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Achievement;

        public const string PortAchievementType = "AchievementType";
        public const string PortPoints = "Points";
        public const string PortIsHidden = "IsHidden";
        public const string PortUnlockDate = "UnlockDate";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<ushort>(PortAchievementType).WithDisplayName("Achievement Type").Build();
            context.AddInputPort<int>(PortPoints).WithDisplayName("Points").Build();
            context.AddInputPort<bool>(PortIsHidden).WithDisplayName("Is Hidden").Build();
            context.AddInputPort<System.DateTime>(PortUnlockDate).WithDisplayName("Unlock Date").Build();
        }
    }

    [Serializable]
    public class AchievementStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.AchievementState;

        public const string PortProgress = "Progress";
        public const string PortIsUnlocked = "IsUnlocked";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<float>(PortProgress).WithDisplayName("Progress").Build();
            context.AddInputPort<bool>(PortIsUnlocked).WithDisplayName("Is Unlocked").Build();
        }
    }

    [Serializable]
    public class AIDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.AI;

        public const string PortBehaviorType = "BehaviorType";
        public const string PortAggroRange = "AggroRange";
        public const string PortPatrolRadius = "PatrolRadius";
        public const string PortPriority = "Priority";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<ushort>(PortBehaviorType).WithDisplayName("Behavior Type").Build();
            context.AddInputPort<float>(PortAggroRange).WithDisplayName("Aggro Range").Build();
            context.AddInputPort<float>(PortPatrolRadius).WithDisplayName("Patrol Radius").Build();
            context.AddInputPort<int>(PortPriority).WithDisplayName("Priority").Build();
        }
    }

    [Serializable]
    public class AIStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.AIState;

        public const string PortCurrentTarget = "CurrentTarget";
        public const string PortLastAction = "LastAction";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortCurrentTarget).WithDisplayName("Current Target").Build();
            context.AddInputPort<System.DateTime>(PortLastAction).WithDisplayName("Last Action").Build();
        }
    }

    [Serializable]
    public class CharacterDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Character;

        public const string PortCharacterClass = "CharacterClass";
        public const string PortLevel = "Level";
        public const string PortRace = "Race";
        public const string PortGender = "Gender";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<ushort>(PortCharacterClass).WithDisplayName("Character Class").Build();
            context.AddInputPort<int>(PortLevel).WithDisplayName("Level").Build();
            context.AddInputPort<ushort>(PortRace).WithDisplayName("Race").Build();
            context.AddInputPort<ushort>(PortGender).WithDisplayName("Gender").Build();
        }
    }

    [Serializable]
    public class CharacterStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.CharacterState;

        public const string PortHealth = "Health";
        public const string PortMana = "Mana";
        public const string PortStamina = "Stamina";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<float>(PortHealth).WithDisplayName("Health").Build();
            context.AddInputPort<float>(PortMana).WithDisplayName("Mana").Build();
            context.AddInputPort<float>(PortStamina).WithDisplayName("Stamina").Build();
        }
    }

    [Serializable]
    public class CombatDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Combat;

        public const string PortBaseDamage = "BaseDamage";
        public const string PortAttackSpeed = "AttackSpeed";
        public const string PortCritChance = "CritChance";
        public const string PortCritMultiplier = "CritMultiplier";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<float>(PortBaseDamage).WithDisplayName("Base Damage").Build();
            context.AddInputPort<float>(PortAttackSpeed).WithDisplayName("Attack Speed").Build();
            context.AddInputPort<float>(PortCritChance).WithDisplayName("Crit Chance").Build();
            context.AddInputPort<float>(PortCritMultiplier).WithDisplayName("Crit Multiplier").Build();
        }
    }

    [Serializable]
    public class CombatStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.CombatState;

        public const string PortIsInCombat = "IsInCombat";
        public const string PortLastAttack = "LastAttack";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<bool>(PortIsInCombat).WithDisplayName("Is In Combat").Build();
            context.AddInputPort<System.DateTime>(PortLastAttack).WithDisplayName("Last Attack").Build();
        }
    }

    [Serializable]
    public class DescriptionDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Description;

        public const string PortText = "Text";
        public const string PortLanguageID = "LanguageID";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<string>(PortText).WithDisplayName("Text").Build();
            context.AddInputPort<ushort>(PortLanguageID).WithDisplayName("Language I D").Build();
        }
    }

    [Serializable]
    public class EconomyDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Economy;

        public const string PortBasePrice = "BasePrice";
        public const string PortInflation = "Inflation";
        public const string PortSupply = "Supply";
        public const string PortDemand = "Demand";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortBasePrice).WithDisplayName("Base Price").Build();
            context.AddInputPort<float>(PortInflation).WithDisplayName("Inflation").Build();
            context.AddInputPort<float>(PortSupply).WithDisplayName("Supply").Build();
            context.AddInputPort<float>(PortDemand).WithDisplayName("Demand").Build();
        }
    }

    [Serializable]
    public class EconomyStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.EconomyState;

        public const string PortCurrentPrice = "CurrentPrice";
        public const string PortLastUpdate = "LastUpdate";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortCurrentPrice).WithDisplayName("Current Price").Build();
            context.AddInputPort<System.DateTime>(PortLastUpdate).WithDisplayName("Last Update").Build();
        }
    }

    [Serializable]
    public class EffectDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Effect;

        public const string PortEffectType = "EffectType";
        public const string PortMagnitude = "Magnitude";
        public const string PortDurationTicks = "DurationTicks";
        public const string PortIsStackable = "IsStackable";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<ushort>(PortEffectType).WithDisplayName("Effect Type").Build();
            context.AddInputPort<float>(PortMagnitude).WithDisplayName("Magnitude").Build();
            context.AddInputPort<long>(PortDurationTicks).WithDisplayName("Duration Ticks").Build();
            context.AddInputPort<bool>(PortIsStackable).WithDisplayName("Is Stackable").Build();
        }
    }

    [Serializable]
    public class EffectStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.EffectState;

        public const string PortStacks = "Stacks";
        public const string PortRemainingTicks = "RemainingTicks";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortStacks).WithDisplayName("Stacks").Build();
            context.AddInputPort<long>(PortRemainingTicks).WithDisplayName("Remaining Ticks").Build();
        }
    }

    [Serializable]
    public class EquipmentDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Equipment;

        public const string PortEquipSlot = "EquipSlot";
        public const string PortLevel = "Level";
        public const string PortDurability = "Durability";
        public const string PortMaxDurability = "MaxDurability";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<ushort>(PortEquipSlot).WithDisplayName("Equip Slot").Build();
            context.AddInputPort<int>(PortLevel).WithDisplayName("Level").Build();
            context.AddInputPort<int>(PortDurability).WithDisplayName("Durability").Build();
            context.AddInputPort<int>(PortMaxDurability).WithDisplayName("Max Durability").Build();
        }
    }

    [Serializable]
    public class GuildDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Guild;

        public const string PortMaxMembers = "MaxMembers";
        public const string PortLevel = "Level";
        public const string PortExperience = "Experience";
        public const string PortCreatedAt = "CreatedAt";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortMaxMembers).WithDisplayName("Max Members").Build();
            context.AddInputPort<int>(PortLevel).WithDisplayName("Level").Build();
            context.AddInputPort<long>(PortExperience).WithDisplayName("Experience").Build();
            context.AddInputPort<System.DateTime>(PortCreatedAt).WithDisplayName("Created At").Build();
        }
    }

    [Serializable]
    public class GuildStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.GuildState;

        public const string PortMemberCount = "MemberCount";
        public const string PortTreasury = "Treasury";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortMemberCount).WithDisplayName("Member Count").Build();
            context.AddInputPort<long>(PortTreasury).WithDisplayName("Treasury").Build();
        }
    }

    [Serializable]
    public class InventoryDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Inventory;

        public const string PortMaxSlots = "MaxSlots";
        public const string PortMaxWeight = "MaxWeight";
        public const string PortInventoryType = "InventoryType";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortMaxSlots).WithDisplayName("Max Slots").Build();
            context.AddInputPort<float>(PortMaxWeight).WithDisplayName("Max Weight").Build();
            context.AddInputPort<ushort>(PortInventoryType).WithDisplayName("Inventory Type").Build();
        }
    }

    [Serializable]
    public class InventoryStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.InventoryState;

        public const string PortUsedSlots = "UsedSlots";
        public const string PortCurrentWeight = "CurrentWeight";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortUsedSlots).WithDisplayName("Used Slots").Build();
            context.AddInputPort<float>(PortCurrentWeight).WithDisplayName("Current Weight").Build();
        }
    }

    [Serializable]
    public class ItemDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Item;

        public const string PortItemType = "ItemType";
        public const string PortRarity = "Rarity";
        public const string PortMaxStack = "MaxStack";
        public const string PortWeight = "Weight";
        public const string PortValue = "Value";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<ushort>(PortItemType).WithDisplayName("Item Type").Build();
            context.AddInputPort<int>(PortRarity).WithDisplayName("Rarity").Build();
            context.AddInputPort<int>(PortMaxStack).WithDisplayName("Max Stack").Build();
            context.AddInputPort<float>(PortWeight).WithDisplayName("Weight").Build();
            context.AddInputPort<int>(PortValue).WithDisplayName("Value").Build();
        }
    }

    [Serializable]
    public class LocationDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Location;

        public const string PortPosition = "Position";
        public const string PortScale = "Scale";
        public const string PortRotation = "Rotation";
        public const string PortParentZoneID = "ParentZoneID";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<UnityEngine.Vector3>(PortPosition).WithDisplayName("Position").Build();
            context.AddInputPort<UnityEngine.Vector3>(PortScale).WithDisplayName("Scale").Build();
            context.AddInputPort<UnityEngine.Quaternion>(PortRotation).WithDisplayName("Rotation").Build();
            context.AddInputPort<int>(PortParentZoneID).WithDisplayName("Parent Zone I D").Build();
        }
    }

    [Serializable]
    public class MissionDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Mission;

        public const string PortPriority = "Priority";
        public const string PortIsRepeatable = "IsRepeatable";
        public const string PortMaxAttempts = "MaxAttempts";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortPriority).WithDisplayName("Priority").Build();
            context.AddInputPort<bool>(PortIsRepeatable).WithDisplayName("Is Repeatable").Build();
            context.AddInputPort<int>(PortMaxAttempts).WithDisplayName("Max Attempts").Build();
        }
    }

    [Serializable]
    public class MissionStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.MissionState;

        public const string PortAttempts = "Attempts";
        public const string PortIsActive = "IsActive";
        public const string PortStartTime = "StartTime";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortAttempts).WithDisplayName("Attempts").Build();
            context.AddInputPort<bool>(PortIsActive).WithDisplayName("Is Active").Build();
            context.AddInputPort<System.DateTime>(PortStartTime).WithDisplayName("Start Time").Build();
        }
    }

    [Serializable]
    public class NameDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Name;

        public const string PortText = "Text";
        public const string PortLanguageID = "LanguageID";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<string>(PortText).WithDisplayName("Text").Build();
            context.AddInputPort<ushort>(PortLanguageID).WithDisplayName("Language I D").Build();
        }
    }

    [Serializable]
    public class ObjectiveDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Objective;

        public const string PortTargetValue = "TargetValue";
        public const string PortObjectiveType = "ObjectiveType";
        public const string PortIsOptional = "IsOptional";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortTargetValue).WithDisplayName("Target Value").Build();
            context.AddInputPort<ushort>(PortObjectiveType).WithDisplayName("Objective Type").Build();
            context.AddInputPort<bool>(PortIsOptional).WithDisplayName("Is Optional").Build();
        }
    }

    [Serializable]
    public class ObjectiveStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.ObjectiveState;

        public const string PortCurrentValue = "CurrentValue";
        public const string PortIsCompleted = "IsCompleted";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortCurrentValue).WithDisplayName("Current Value").Build();
            context.AddInputPort<bool>(PortIsCompleted).WithDisplayName("Is Completed").Build();
        }
    }

    [Serializable]
    public class PlayerDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Player;

        public const string PortLevel = "Level";
        public const string PortExperience = "Experience";
        public const string PortPrestigeLevel = "PrestigeLevel";
        public const string PortCreatedAt = "CreatedAt";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortLevel).WithDisplayName("Level").Build();
            context.AddInputPort<float>(PortExperience).WithDisplayName("Experience").Build();
            context.AddInputPort<int>(PortPrestigeLevel).WithDisplayName("Prestige Level").Build();
            context.AddInputPort<System.DateTime>(PortCreatedAt).WithDisplayName("Created At").Build();
        }
    }

    [Serializable]
    public class QuestDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Quest;

        public const string PortChapterID = "ChapterID";
        public const string PortDifficulty = "Difficulty";
        public const string PortIsMainQuest = "IsMainQuest";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortChapterID).WithDisplayName("Chapter I D").Build();
            context.AddInputPort<int>(PortDifficulty).WithDisplayName("Difficulty").Build();
            context.AddInputPort<bool>(PortIsMainQuest).WithDisplayName("Is Main Quest").Build();
        }
    }

    [Serializable]
    public class QuestStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.QuestState;

        public const string PortProgress = "Progress";
        public const string PortIsCompleted = "IsCompleted";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<float>(PortProgress).WithDisplayName("Progress").Build();
            context.AddInputPort<bool>(PortIsCompleted).WithDisplayName("Is Completed").Build();
        }
    }

    [Serializable]
    public class RangeAsFloatDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.RangeAsFloat;

        public const string PortMinValue = "MinValue";
        public const string PortMaxValue = "MaxValue";
        public const string PortPrecision = "Precision";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<float>(PortMinValue).WithDisplayName("Min Value").Build();
            context.AddInputPort<float>(PortMaxValue).WithDisplayName("Max Value").Build();
            context.AddInputPort<float>(PortPrecision).WithDisplayName("Precision").Build();
        }
    }

    [Serializable]
    public class RangeAsIntDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.RangeAsInt;

        public const string PortMinValue = "MinValue";
        public const string PortMaxValue = "MaxValue";
        public const string PortStepSize = "StepSize";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortMinValue).WithDisplayName("Min Value").Build();
            context.AddInputPort<int>(PortMaxValue).WithDisplayName("Max Value").Build();
            context.AddInputPort<int>(PortStepSize).WithDisplayName("Step Size").Build();
        }
    }

    [Serializable]
    public class RewardDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Reward;

        public const string PortRewardType = "RewardType";
        public const string PortAmount = "Amount";
        public const string PortChance = "Chance";
        public const string PortItemID = "ItemID";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<ushort>(PortRewardType).WithDisplayName("Reward Type").Build();
            context.AddInputPort<int>(PortAmount).WithDisplayName("Amount").Build();
            context.AddInputPort<float>(PortChance).WithDisplayName("Chance").Build();
            context.AddInputPort<int>(PortItemID).WithDisplayName("Item I D").Build();
        }
    }

    [Serializable]
    public class SkillDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Skill;

        public const string PortSkillType = "SkillType";
        public const string PortMaxLevel = "MaxLevel";
        public const string PortBaseCooldown = "BaseCooldown";
        public const string PortManaCost = "ManaCost";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<ushort>(PortSkillType).WithDisplayName("Skill Type").Build();
            context.AddInputPort<int>(PortMaxLevel).WithDisplayName("Max Level").Build();
            context.AddInputPort<float>(PortBaseCooldown).WithDisplayName("Base Cooldown").Build();
            context.AddInputPort<int>(PortManaCost).WithDisplayName("Mana Cost").Build();
        }
    }

    [Serializable]
    public class SkillStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.SkillState;

        public const string PortLevel = "Level";
        public const string PortExperience = "Experience";
        public const string PortLastUsed = "LastUsed";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<int>(PortLevel).WithDisplayName("Level").Build();
            context.AddInputPort<float>(PortExperience).WithDisplayName("Experience").Build();
            context.AddInputPort<System.DateTime>(PortLastUsed).WithDisplayName("Last Used").Build();
        }
    }

    [Serializable]
    public class StatDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Stat;

        public const string PortStatType = "StatType";
        public const string PortBaseValue = "BaseValue";
        public const string PortMinValue = "MinValue";
        public const string PortMaxValue = "MaxValue";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<ushort>(PortStatType).WithDisplayName("Stat Type").Build();
            context.AddInputPort<float>(PortBaseValue).WithDisplayName("Base Value").Build();
            context.AddInputPort<float>(PortMinValue).WithDisplayName("Min Value").Build();
            context.AddInputPort<float>(PortMaxValue).WithDisplayName("Max Value").Build();
        }
    }

    [Serializable]
    public class StatStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.StatState;

        public const string PortCurrentValue = "CurrentValue";
        public const string PortModifiers = "Modifiers";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<float>(PortCurrentValue).WithDisplayName("Current Value").Build();
            context.AddInputPort<float>(PortModifiers).WithDisplayName("Modifiers").Build();
        }
    }

    [Serializable]
    public class TagDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.Tag;

        public const string PortTagType = "TagType";
        public const string PortValue = "Value";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<ushort>(PortTagType).WithDisplayName("Tag Type").Build();
            context.AddInputPort<int>(PortValue).WithDisplayName("Value").Build();
        }
    }

    [Serializable]
    public class TimeStateDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.TimeState;

        public const string PortElapsedTicks = "ElapsedTicks";
        public const string PortIsActive = "IsActive";
        public const string PortCycleCount = "CycleCount";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<long>(PortElapsedTicks).WithDisplayName("Elapsed Ticks").Build();
            context.AddInputPort<bool>(PortIsActive).WithDisplayName("Is Active").Build();
            context.AddInputPort<int>(PortCycleCount).WithDisplayName("Cycle Count").Build();
        }
    }

    [Serializable]
    public class TimeTickDefinitionNode : GraphDBNode, IDataNode
    {
        public override EntityType EntityType => EntityType.TimeTick;

        public const string PortStartTicks = "StartTicks";
        public const string PortDurationTicks = "DurationTicks";
        public const string PortIsRepeating = "IsRepeating";
        public const string PortIntervalTicks = "IntervalTicks";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort<long>(PortStartTicks).WithDisplayName("Start Ticks").Build();
            context.AddInputPort<long>(PortDurationTicks).WithDisplayName("Duration Ticks").Build();
            context.AddInputPort<bool>(PortIsRepeating).WithDisplayName("Is Repeating").Build();
            context.AddInputPort<long>(PortIntervalTicks).WithDisplayName("Interval Ticks").Build();
        }
    }

    #endregion
}
