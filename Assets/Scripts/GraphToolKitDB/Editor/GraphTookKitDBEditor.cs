// ----- AUTO-GENERATED EDITOR FILE BY GraphToolGenerator.cs -----

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using GraphTookKitDB.Runtime;
using GraphToolKitDB.Runtime; // Import runtime namespace

namespace GraphTookKitDB.Runtime
{
    public class Linkable {}

    #region Graph Definition

    [Graph("gdb")]
    [Serializable]
    public class GraphTookKitDBGraph : Graph
    {
        [MenuItem("Assets/Create/GraphTookKitDB Graph")]
        private static void CreateAssetFile() => GraphDatabase.PromptInProjectBrowserToCreateNewAsset<GraphTookKitDBGraph>("New GraphTookKitDB");

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

    [Serializable]
    public class AchievementDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortAchievementType = "AchievementType";
        public const string PortPoints = "Points";
        public const string PortIsHidden = "IsHidden";
        public const string PortUnlockDate = "UnlockDate";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<ushort>(PortAchievementType).WithDisplayName("Achievement Type").Build();
            context.AddInputPort<int>(PortPoints).WithDisplayName("Points").Build();
            context.AddInputPort<bool>(PortIsHidden).WithDisplayName("Is Hidden").Build();
            context.AddInputPort<System.DateTime>(PortUnlockDate).WithDisplayName("Unlock Date").Build();
        }
    }

    [Serializable]
    public class AchievementStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortProgress = "Progress";
        public const string PortIsUnlocked = "IsUnlocked";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<float>(PortProgress).WithDisplayName("Progress").Build();
            context.AddInputPort<bool>(PortIsUnlocked).WithDisplayName("Is Unlocked").Build();
        }
    }

    [Serializable]
    public class AIDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortBehaviorType = "BehaviorType";
        public const string PortAggroRange = "AggroRange";
        public const string PortPatrolRadius = "PatrolRadius";
        public const string PortPriority = "Priority";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<ushort>(PortBehaviorType).WithDisplayName("Behavior Type").Build();
            context.AddInputPort<float>(PortAggroRange).WithDisplayName("Aggro Range").Build();
            context.AddInputPort<float>(PortPatrolRadius).WithDisplayName("Patrol Radius").Build();
            context.AddInputPort<int>(PortPriority).WithDisplayName("Priority").Build();
        }
    }

    [Serializable]
    public class AIStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortCurrentTarget = "CurrentTarget";
        public const string PortLastAction = "LastAction";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortCurrentTarget).WithDisplayName("Current Target").Build();
            context.AddInputPort<System.DateTime>(PortLastAction).WithDisplayName("Last Action").Build();
        }
    }

    [Serializable]
    public class CharacterDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortCharacterClass = "CharacterClass";
        public const string PortLevel = "Level";
        public const string PortRace = "Race";
        public const string PortGender = "Gender";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<ushort>(PortCharacterClass).WithDisplayName("Character Class").Build();
            context.AddInputPort<int>(PortLevel).WithDisplayName("Level").Build();
            context.AddInputPort<ushort>(PortRace).WithDisplayName("Race").Build();
            context.AddInputPort<ushort>(PortGender).WithDisplayName("Gender").Build();
        }
    }

    [Serializable]
    public class CharacterStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortHealth = "Health";
        public const string PortMana = "Mana";
        public const string PortStamina = "Stamina";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<float>(PortHealth).WithDisplayName("Health").Build();
            context.AddInputPort<float>(PortMana).WithDisplayName("Mana").Build();
            context.AddInputPort<float>(PortStamina).WithDisplayName("Stamina").Build();
        }
    }

    [Serializable]
    public class CombatDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortBaseDamage = "BaseDamage";
        public const string PortAttackSpeed = "AttackSpeed";
        public const string PortCritChance = "CritChance";
        public const string PortCritMultiplier = "CritMultiplier";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<float>(PortBaseDamage).WithDisplayName("Base Damage").Build();
            context.AddInputPort<float>(PortAttackSpeed).WithDisplayName("Attack Speed").Build();
            context.AddInputPort<float>(PortCritChance).WithDisplayName("Crit Chance").Build();
            context.AddInputPort<float>(PortCritMultiplier).WithDisplayName("Crit Multiplier").Build();
        }
    }

    [Serializable]
    public class CombatStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortIsInCombat = "IsInCombat";
        public const string PortLastAttack = "LastAttack";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<bool>(PortIsInCombat).WithDisplayName("Is In Combat").Build();
            context.AddInputPort<System.DateTime>(PortLastAttack).WithDisplayName("Last Attack").Build();
        }
    }

    [Serializable]
    public class DescriptionDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortText = "Text";
        public const string PortLanguageID = "LanguageID";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<string>(PortText).WithDisplayName("Text").Build();
            context.AddInputPort<ushort>(PortLanguageID).WithDisplayName("Language I D").Build();
        }
    }

    [Serializable]
    public class EconomyDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortBasePrice = "BasePrice";
        public const string PortInflation = "Inflation";
        public const string PortSupply = "Supply";
        public const string PortDemand = "Demand";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortBasePrice).WithDisplayName("Base Price").Build();
            context.AddInputPort<float>(PortInflation).WithDisplayName("Inflation").Build();
            context.AddInputPort<float>(PortSupply).WithDisplayName("Supply").Build();
            context.AddInputPort<float>(PortDemand).WithDisplayName("Demand").Build();
        }
    }

    [Serializable]
    public class EconomyStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortCurrentPrice = "CurrentPrice";
        public const string PortLastUpdate = "LastUpdate";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortCurrentPrice).WithDisplayName("Current Price").Build();
            context.AddInputPort<System.DateTime>(PortLastUpdate).WithDisplayName("Last Update").Build();
        }
    }

    [Serializable]
    public class EffectDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortEffectType = "EffectType";
        public const string PortMagnitude = "Magnitude";
        public const string PortDurationTicks = "DurationTicks";
        public const string PortIsStackable = "IsStackable";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<ushort>(PortEffectType).WithDisplayName("Effect Type").Build();
            context.AddInputPort<float>(PortMagnitude).WithDisplayName("Magnitude").Build();
            context.AddInputPort<long>(PortDurationTicks).WithDisplayName("Duration Ticks").Build();
            context.AddInputPort<bool>(PortIsStackable).WithDisplayName("Is Stackable").Build();
        }
    }

    [Serializable]
    public class EffectStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortStacks = "Stacks";
        public const string PortRemainingTicks = "RemainingTicks";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortStacks).WithDisplayName("Stacks").Build();
            context.AddInputPort<long>(PortRemainingTicks).WithDisplayName("Remaining Ticks").Build();
        }
    }

    [Serializable]
    public class EquipmentDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortEquipSlot = "EquipSlot";
        public const string PortLevel = "Level";
        public const string PortDurability = "Durability";
        public const string PortMaxDurability = "MaxDurability";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<ushort>(PortEquipSlot).WithDisplayName("Equip Slot").Build();
            context.AddInputPort<int>(PortLevel).WithDisplayName("Level").Build();
            context.AddInputPort<int>(PortDurability).WithDisplayName("Durability").Build();
            context.AddInputPort<int>(PortMaxDurability).WithDisplayName("Max Durability").Build();
        }
    }

    [Serializable]
    public class GuildDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortMaxMembers = "MaxMembers";
        public const string PortLevel = "Level";
        public const string PortExperience = "Experience";
        public const string PortCreatedAt = "CreatedAt";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortMaxMembers).WithDisplayName("Max Members").Build();
            context.AddInputPort<int>(PortLevel).WithDisplayName("Level").Build();
            context.AddInputPort<long>(PortExperience).WithDisplayName("Experience").Build();
            context.AddInputPort<System.DateTime>(PortCreatedAt).WithDisplayName("Created At").Build();
        }
    }

    [Serializable]
    public class GuildStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortMemberCount = "MemberCount";
        public const string PortTreasury = "Treasury";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortMemberCount).WithDisplayName("Member Count").Build();
            context.AddInputPort<long>(PortTreasury).WithDisplayName("Treasury").Build();
        }
    }

    [Serializable]
    public class InventoryDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortMaxSlots = "MaxSlots";
        public const string PortMaxWeight = "MaxWeight";
        public const string PortInventoryType = "InventoryType";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortMaxSlots).WithDisplayName("Max Slots").Build();
            context.AddInputPort<float>(PortMaxWeight).WithDisplayName("Max Weight").Build();
            context.AddInputPort<ushort>(PortInventoryType).WithDisplayName("Inventory Type").Build();
        }
    }

    [Serializable]
    public class InventoryStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortUsedSlots = "UsedSlots";
        public const string PortCurrentWeight = "CurrentWeight";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortUsedSlots).WithDisplayName("Used Slots").Build();
            context.AddInputPort<float>(PortCurrentWeight).WithDisplayName("Current Weight").Build();
        }
    }

    [Serializable]
    public class ItemDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortItemType = "ItemType";
        public const string PortRarity = "Rarity";
        public const string PortMaxStack = "MaxStack";
        public const string PortWeight = "Weight";
        public const string PortValue = "Value";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<ushort>(PortItemType).WithDisplayName("Item Type").Build();
            context.AddInputPort<int>(PortRarity).WithDisplayName("Rarity").Build();
            context.AddInputPort<int>(PortMaxStack).WithDisplayName("Max Stack").Build();
            context.AddInputPort<float>(PortWeight).WithDisplayName("Weight").Build();
            context.AddInputPort<int>(PortValue).WithDisplayName("Value").Build();
        }
    }

    [Serializable]
    public class LocationDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortPosition = "Position";
        public const string PortScale = "Scale";
        public const string PortRotation = "Rotation";
        public const string PortParentZoneID = "ParentZoneID";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<UnityEngine.Vector3>(PortPosition).WithDisplayName("Position").Build();
            context.AddInputPort<UnityEngine.Vector3>(PortScale).WithDisplayName("Scale").Build();
            context.AddInputPort<UnityEngine.Quaternion>(PortRotation).WithDisplayName("Rotation").Build();
            context.AddInputPort<int>(PortParentZoneID).WithDisplayName("Parent Zone I D").Build();
        }
    }

    [Serializable]
    public class MissionDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortPriority = "Priority";
        public const string PortIsRepeatable = "IsRepeatable";
        public const string PortMaxAttempts = "MaxAttempts";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortPriority).WithDisplayName("Priority").Build();
            context.AddInputPort<bool>(PortIsRepeatable).WithDisplayName("Is Repeatable").Build();
            context.AddInputPort<int>(PortMaxAttempts).WithDisplayName("Max Attempts").Build();
        }
    }

    [Serializable]
    public class MissionStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortAttempts = "Attempts";
        public const string PortIsActive = "IsActive";
        public const string PortStartTime = "StartTime";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortAttempts).WithDisplayName("Attempts").Build();
            context.AddInputPort<bool>(PortIsActive).WithDisplayName("Is Active").Build();
            context.AddInputPort<System.DateTime>(PortStartTime).WithDisplayName("Start Time").Build();
        }
    }

    [Serializable]
    public class NameDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortText = "Text";
        public const string PortLanguageID = "LanguageID";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<string>(PortText).WithDisplayName("Text").Build();
            context.AddInputPort<ushort>(PortLanguageID).WithDisplayName("Language I D").Build();
        }
    }

    [Serializable]
    public class ObjectiveDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortTargetValue = "TargetValue";
        public const string PortObjectiveType = "ObjectiveType";
        public const string PortIsOptional = "IsOptional";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortTargetValue).WithDisplayName("Target Value").Build();
            context.AddInputPort<ushort>(PortObjectiveType).WithDisplayName("Objective Type").Build();
            context.AddInputPort<bool>(PortIsOptional).WithDisplayName("Is Optional").Build();
        }
    }

    [Serializable]
    public class ObjectiveStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortCurrentValue = "CurrentValue";
        public const string PortIsCompleted = "IsCompleted";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortCurrentValue).WithDisplayName("Current Value").Build();
            context.AddInputPort<bool>(PortIsCompleted).WithDisplayName("Is Completed").Build();
        }
    }

    [Serializable]
    public class PlayerDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortLevel = "Level";
        public const string PortExperience = "Experience";
        public const string PortPrestigeLevel = "PrestigeLevel";
        public const string PortCreatedAt = "CreatedAt";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortLevel).WithDisplayName("Level").Build();
            context.AddInputPort<float>(PortExperience).WithDisplayName("Experience").Build();
            context.AddInputPort<int>(PortPrestigeLevel).WithDisplayName("Prestige Level").Build();
            context.AddInputPort<System.DateTime>(PortCreatedAt).WithDisplayName("Created At").Build();
        }
    }

    [Serializable]
    public class QuestDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortChapterID = "ChapterID";
        public const string PortDifficulty = "Difficulty";
        public const string PortIsMainQuest = "IsMainQuest";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortChapterID).WithDisplayName("Chapter I D").Build();
            context.AddInputPort<int>(PortDifficulty).WithDisplayName("Difficulty").Build();
            context.AddInputPort<bool>(PortIsMainQuest).WithDisplayName("Is Main Quest").Build();
        }
    }

    [Serializable]
    public class QuestStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortProgress = "Progress";
        public const string PortIsCompleted = "IsCompleted";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<float>(PortProgress).WithDisplayName("Progress").Build();
            context.AddInputPort<bool>(PortIsCompleted).WithDisplayName("Is Completed").Build();
        }
    }

    [Serializable]
    public class RangeAsFloatDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortMinValue = "MinValue";
        public const string PortMaxValue = "MaxValue";
        public const string PortPrecision = "Precision";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<float>(PortMinValue).WithDisplayName("Min Value").Build();
            context.AddInputPort<float>(PortMaxValue).WithDisplayName("Max Value").Build();
            context.AddInputPort<float>(PortPrecision).WithDisplayName("Precision").Build();
        }
    }

    [Serializable]
    public class RangeAsIntDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortMinValue = "MinValue";
        public const string PortMaxValue = "MaxValue";
        public const string PortStepSize = "StepSize";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortMinValue).WithDisplayName("Min Value").Build();
            context.AddInputPort<int>(PortMaxValue).WithDisplayName("Max Value").Build();
            context.AddInputPort<int>(PortStepSize).WithDisplayName("Step Size").Build();
        }
    }

    [Serializable]
    public class RewardDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortRewardType = "RewardType";
        public const string PortAmount = "Amount";
        public const string PortChance = "Chance";
        public const string PortItemID = "ItemID";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<ushort>(PortRewardType).WithDisplayName("Reward Type").Build();
            context.AddInputPort<int>(PortAmount).WithDisplayName("Amount").Build();
            context.AddInputPort<float>(PortChance).WithDisplayName("Chance").Build();
            context.AddInputPort<int>(PortItemID).WithDisplayName("Item I D").Build();
        }
    }

    [Serializable]
    public class SkillDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortSkillType = "SkillType";
        public const string PortMaxLevel = "MaxLevel";
        public const string PortBaseCooldown = "BaseCooldown";
        public const string PortManaCost = "ManaCost";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<ushort>(PortSkillType).WithDisplayName("Skill Type").Build();
            context.AddInputPort<int>(PortMaxLevel).WithDisplayName("Max Level").Build();
            context.AddInputPort<float>(PortBaseCooldown).WithDisplayName("Base Cooldown").Build();
            context.AddInputPort<int>(PortManaCost).WithDisplayName("Mana Cost").Build();
        }
    }

    [Serializable]
    public class SkillStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortLevel = "Level";
        public const string PortExperience = "Experience";
        public const string PortLastUsed = "LastUsed";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<int>(PortLevel).WithDisplayName("Level").Build();
            context.AddInputPort<float>(PortExperience).WithDisplayName("Experience").Build();
            context.AddInputPort<System.DateTime>(PortLastUsed).WithDisplayName("Last Used").Build();
        }
    }

    [Serializable]
    public class StatDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortStatType = "StatType";
        public const string PortBaseValue = "BaseValue";
        public const string PortMinValue = "MinValue";
        public const string PortMaxValue = "MaxValue";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<ushort>(PortStatType).WithDisplayName("Stat Type").Build();
            context.AddInputPort<float>(PortBaseValue).WithDisplayName("Base Value").Build();
            context.AddInputPort<float>(PortMinValue).WithDisplayName("Min Value").Build();
            context.AddInputPort<float>(PortMaxValue).WithDisplayName("Max Value").Build();
        }
    }

    [Serializable]
    public class StatStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortCurrentValue = "CurrentValue";
        public const string PortModifiers = "Modifiers";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<float>(PortCurrentValue).WithDisplayName("Current Value").Build();
            context.AddInputPort<float>(PortModifiers).WithDisplayName("Modifiers").Build();
        }
    }

    [Serializable]
    public class TagDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortTagType = "TagType";
        public const string PortValue = "Value";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<ushort>(PortTagType).WithDisplayName("Tag Type").Build();
            context.AddInputPort<int>(PortValue).WithDisplayName("Value").Build();
        }
    }

    [Serializable]
    public class TimeStateDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortElapsedTicks = "ElapsedTicks";
        public const string PortIsActive = "IsActive";
        public const string PortCycleCount = "CycleCount";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<long>(PortElapsedTicks).WithDisplayName("Elapsed Ticks").Build();
            context.AddInputPort<bool>(PortIsActive).WithDisplayName("Is Active").Build();
            context.AddInputPort<int>(PortCycleCount).WithDisplayName("Cycle Count").Build();
        }
    }

    [Serializable]
    public class TimeTickDefinitionNode : Node
    {
        public const string PortInputLink = "InputLink";
        public const string PortOutputLink = "OutputLink";
        public const string PortStartTicks = "StartTicks";
        public const string PortDurationTicks = "DurationTicks";
        public const string PortIsRepeating = "IsRepeating";
        public const string PortIntervalTicks = "IntervalTicks";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
            context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

            context.AddInputPort<long>(PortStartTicks).WithDisplayName("Start Ticks").Build();
            context.AddInputPort<long>(PortDurationTicks).WithDisplayName("Duration Ticks").Build();
            context.AddInputPort<bool>(PortIsRepeating).WithDisplayName("Is Repeating").Build();
            context.AddInputPort<long>(PortIntervalTicks).WithDisplayName("Interval Ticks").Build();
        }
    }

    #endregion

    #region Scripted Importer

    [ScriptedImporter(1, "gdb")]
    public class GraphTookKitDBImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var graph = GraphDatabase.LoadGraphForImporter<GraphTookKitDBGraph>(ctx.assetPath);
            if (graph == null) return;

            var runtimeAsset = ScriptableObject.CreateInstance<GraphTookKitDBAsset>();
            var nodeToIdMap = new Dictionary<INode, int>();
            var typeToIdMap = new Dictionary<Type, ushort>();

            // Pre-populate type IDs for linking
            typeToIdMap[typeof(Achievement)] = 1;
            typeToIdMap[typeof(AchievementState)] = 2;
            typeToIdMap[typeof(AI)] = 3;
            typeToIdMap[typeof(AIState)] = 4;
            typeToIdMap[typeof(Character)] = 5;
            typeToIdMap[typeof(CharacterState)] = 6;
            typeToIdMap[typeof(Combat)] = 7;
            typeToIdMap[typeof(CombatState)] = 8;
            typeToIdMap[typeof(Description)] = 9;
            typeToIdMap[typeof(Economy)] = 10;
            typeToIdMap[typeof(EconomyState)] = 11;
            typeToIdMap[typeof(Effect)] = 12;
            typeToIdMap[typeof(EffectState)] = 13;
            typeToIdMap[typeof(Equipment)] = 14;
            typeToIdMap[typeof(Guild)] = 15;
            typeToIdMap[typeof(GuildState)] = 16;
            typeToIdMap[typeof(Inventory)] = 17;
            typeToIdMap[typeof(InventoryState)] = 18;
            typeToIdMap[typeof(Item)] = 19;
            typeToIdMap[typeof(Link)] = 20;
            typeToIdMap[typeof(Location)] = 21;
            typeToIdMap[typeof(Mission)] = 22;
            typeToIdMap[typeof(MissionState)] = 23;
            typeToIdMap[typeof(Name)] = 24;
            typeToIdMap[typeof(Objective)] = 25;
            typeToIdMap[typeof(ObjectiveState)] = 26;
            typeToIdMap[typeof(Player)] = 27;
            typeToIdMap[typeof(Quest)] = 28;
            typeToIdMap[typeof(QuestState)] = 29;
            typeToIdMap[typeof(RangeAsFloat)] = 30;
            typeToIdMap[typeof(RangeAsInt)] = 31;
            typeToIdMap[typeof(Reward)] = 32;
            typeToIdMap[typeof(Skill)] = 33;
            typeToIdMap[typeof(SkillState)] = 34;
            typeToIdMap[typeof(Stat)] = 35;
            typeToIdMap[typeof(StatState)] = 36;
            typeToIdMap[typeof(Tag)] = 37;
            typeToIdMap[typeof(TimeState)] = 38;
            typeToIdMap[typeof(TimeTick)] = 39;

            // --- PASS 1: Create all data entries and map nodes to their index-based ID ---
            var achievementNodes = graph.GetNodes().OfType<AchievementDefinitionNode>().ToList();
            for(int i = 0; i < achievementNodes.Count; i++)
            {
                var node = achievementNodes[i];
                var data = new Achievement();
                data.AchievementType = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(AchievementDefinitionNode.PortAchievementType));
                data.Points = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(AchievementDefinitionNode.PortPoints));
                data.IsHidden = GraphTookKitDBGraph.GetPortValue<bool>(node.GetInputPortByName(AchievementDefinitionNode.PortIsHidden));
                data.UnlockDate = GraphTookKitDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(AchievementDefinitionNode.PortUnlockDate));
                data.ID = i;
                runtimeAsset.Achievements.Add(data);
                nodeToIdMap[node] = i;
            }
            var achievementstateNodes = graph.GetNodes().OfType<AchievementStateDefinitionNode>().ToList();
            for(int i = 0; i < achievementstateNodes.Count; i++)
            {
                var node = achievementstateNodes[i];
                var data = new AchievementState();
                data.Progress = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(AchievementStateDefinitionNode.PortProgress));
                data.IsUnlocked = GraphTookKitDBGraph.GetPortValue<bool>(node.GetInputPortByName(AchievementStateDefinitionNode.PortIsUnlocked));
                data.ID = i;
                runtimeAsset.AchievementStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var aiNodes = graph.GetNodes().OfType<AIDefinitionNode>().ToList();
            for(int i = 0; i < aiNodes.Count; i++)
            {
                var node = aiNodes[i];
                var data = new AI();
                data.BehaviorType = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(AIDefinitionNode.PortBehaviorType));
                data.AggroRange = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(AIDefinitionNode.PortAggroRange));
                data.PatrolRadius = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(AIDefinitionNode.PortPatrolRadius));
                data.Priority = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(AIDefinitionNode.PortPriority));
                data.ID = i;
                runtimeAsset.AIs.Add(data);
                nodeToIdMap[node] = i;
            }
            var aistateNodes = graph.GetNodes().OfType<AIStateDefinitionNode>().ToList();
            for(int i = 0; i < aistateNodes.Count; i++)
            {
                var node = aistateNodes[i];
                var data = new AIState();
                data.CurrentTarget = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(AIStateDefinitionNode.PortCurrentTarget));
                data.LastAction = GraphTookKitDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(AIStateDefinitionNode.PortLastAction));
                data.ID = i;
                runtimeAsset.AIStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var characterNodes = graph.GetNodes().OfType<CharacterDefinitionNode>().ToList();
            for(int i = 0; i < characterNodes.Count; i++)
            {
                var node = characterNodes[i];
                var data = new Character();
                data.CharacterClass = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(CharacterDefinitionNode.PortCharacterClass));
                data.Level = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(CharacterDefinitionNode.PortLevel));
                data.Race = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(CharacterDefinitionNode.PortRace));
                data.Gender = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(CharacterDefinitionNode.PortGender));
                data.ID = i;
                runtimeAsset.Characters.Add(data);
                nodeToIdMap[node] = i;
            }
            var characterstateNodes = graph.GetNodes().OfType<CharacterStateDefinitionNode>().ToList();
            for(int i = 0; i < characterstateNodes.Count; i++)
            {
                var node = characterstateNodes[i];
                var data = new CharacterState();
                data.Health = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(CharacterStateDefinitionNode.PortHealth));
                data.Mana = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(CharacterStateDefinitionNode.PortMana));
                data.Stamina = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(CharacterStateDefinitionNode.PortStamina));
                data.ID = i;
                runtimeAsset.CharacterStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var combatNodes = graph.GetNodes().OfType<CombatDefinitionNode>().ToList();
            for(int i = 0; i < combatNodes.Count; i++)
            {
                var node = combatNodes[i];
                var data = new Combat();
                data.BaseDamage = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(CombatDefinitionNode.PortBaseDamage));
                data.AttackSpeed = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(CombatDefinitionNode.PortAttackSpeed));
                data.CritChance = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(CombatDefinitionNode.PortCritChance));
                data.CritMultiplier = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(CombatDefinitionNode.PortCritMultiplier));
                data.ID = i;
                runtimeAsset.Combats.Add(data);
                nodeToIdMap[node] = i;
            }
            var combatstateNodes = graph.GetNodes().OfType<CombatStateDefinitionNode>().ToList();
            for(int i = 0; i < combatstateNodes.Count; i++)
            {
                var node = combatstateNodes[i];
                var data = new CombatState();
                data.IsInCombat = GraphTookKitDBGraph.GetPortValue<bool>(node.GetInputPortByName(CombatStateDefinitionNode.PortIsInCombat));
                data.LastAttack = GraphTookKitDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(CombatStateDefinitionNode.PortLastAttack));
                data.ID = i;
                runtimeAsset.CombatStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var descriptionNodes = graph.GetNodes().OfType<DescriptionDefinitionNode>().ToList();
            for(int i = 0; i < descriptionNodes.Count; i++)
            {
                var node = descriptionNodes[i];
                var data = new Description();
                data.Text = GraphTookKitDBGraph.GetPortValue<string>(node.GetInputPortByName(DescriptionDefinitionNode.PortText));
                data.LanguageID = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(DescriptionDefinitionNode.PortLanguageID));
                data.ID = i;
                runtimeAsset.Descriptions.Add(data);
                nodeToIdMap[node] = i;
            }
            var economyNodes = graph.GetNodes().OfType<EconomyDefinitionNode>().ToList();
            for(int i = 0; i < economyNodes.Count; i++)
            {
                var node = economyNodes[i];
                var data = new Economy();
                data.BasePrice = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(EconomyDefinitionNode.PortBasePrice));
                data.Inflation = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(EconomyDefinitionNode.PortInflation));
                data.Supply = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(EconomyDefinitionNode.PortSupply));
                data.Demand = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(EconomyDefinitionNode.PortDemand));
                data.ID = i;
                runtimeAsset.Economys.Add(data);
                nodeToIdMap[node] = i;
            }
            var economystateNodes = graph.GetNodes().OfType<EconomyStateDefinitionNode>().ToList();
            for(int i = 0; i < economystateNodes.Count; i++)
            {
                var node = economystateNodes[i];
                var data = new EconomyState();
                data.CurrentPrice = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(EconomyStateDefinitionNode.PortCurrentPrice));
                data.LastUpdate = GraphTookKitDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(EconomyStateDefinitionNode.PortLastUpdate));
                data.ID = i;
                runtimeAsset.EconomyStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var effectNodes = graph.GetNodes().OfType<EffectDefinitionNode>().ToList();
            for(int i = 0; i < effectNodes.Count; i++)
            {
                var node = effectNodes[i];
                var data = new Effect();
                data.EffectType = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(EffectDefinitionNode.PortEffectType));
                data.Magnitude = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(EffectDefinitionNode.PortMagnitude));
                data.DurationTicks = GraphTookKitDBGraph.GetPortValue<long>(node.GetInputPortByName(EffectDefinitionNode.PortDurationTicks));
                data.IsStackable = GraphTookKitDBGraph.GetPortValue<bool>(node.GetInputPortByName(EffectDefinitionNode.PortIsStackable));
                data.ID = i;
                runtimeAsset.Effects.Add(data);
                nodeToIdMap[node] = i;
            }
            var effectstateNodes = graph.GetNodes().OfType<EffectStateDefinitionNode>().ToList();
            for(int i = 0; i < effectstateNodes.Count; i++)
            {
                var node = effectstateNodes[i];
                var data = new EffectState();
                data.Stacks = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(EffectStateDefinitionNode.PortStacks));
                data.RemainingTicks = GraphTookKitDBGraph.GetPortValue<long>(node.GetInputPortByName(EffectStateDefinitionNode.PortRemainingTicks));
                data.ID = i;
                runtimeAsset.EffectStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var equipmentNodes = graph.GetNodes().OfType<EquipmentDefinitionNode>().ToList();
            for(int i = 0; i < equipmentNodes.Count; i++)
            {
                var node = equipmentNodes[i];
                var data = new Equipment();
                data.EquipSlot = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(EquipmentDefinitionNode.PortEquipSlot));
                data.Level = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(EquipmentDefinitionNode.PortLevel));
                data.Durability = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(EquipmentDefinitionNode.PortDurability));
                data.MaxDurability = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(EquipmentDefinitionNode.PortMaxDurability));
                data.ID = i;
                runtimeAsset.Equipments.Add(data);
                nodeToIdMap[node] = i;
            }
            var guildNodes = graph.GetNodes().OfType<GuildDefinitionNode>().ToList();
            for(int i = 0; i < guildNodes.Count; i++)
            {
                var node = guildNodes[i];
                var data = new Guild();
                data.MaxMembers = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(GuildDefinitionNode.PortMaxMembers));
                data.Level = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(GuildDefinitionNode.PortLevel));
                data.Experience = GraphTookKitDBGraph.GetPortValue<long>(node.GetInputPortByName(GuildDefinitionNode.PortExperience));
                data.CreatedAt = GraphTookKitDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(GuildDefinitionNode.PortCreatedAt));
                data.ID = i;
                runtimeAsset.Guilds.Add(data);
                nodeToIdMap[node] = i;
            }
            var guildstateNodes = graph.GetNodes().OfType<GuildStateDefinitionNode>().ToList();
            for(int i = 0; i < guildstateNodes.Count; i++)
            {
                var node = guildstateNodes[i];
                var data = new GuildState();
                data.MemberCount = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(GuildStateDefinitionNode.PortMemberCount));
                data.Treasury = GraphTookKitDBGraph.GetPortValue<long>(node.GetInputPortByName(GuildStateDefinitionNode.PortTreasury));
                data.ID = i;
                runtimeAsset.GuildStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var inventoryNodes = graph.GetNodes().OfType<InventoryDefinitionNode>().ToList();
            for(int i = 0; i < inventoryNodes.Count; i++)
            {
                var node = inventoryNodes[i];
                var data = new Inventory();
                data.MaxSlots = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(InventoryDefinitionNode.PortMaxSlots));
                data.MaxWeight = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(InventoryDefinitionNode.PortMaxWeight));
                data.InventoryType = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(InventoryDefinitionNode.PortInventoryType));
                data.ID = i;
                runtimeAsset.Inventorys.Add(data);
                nodeToIdMap[node] = i;
            }
            var inventorystateNodes = graph.GetNodes().OfType<InventoryStateDefinitionNode>().ToList();
            for(int i = 0; i < inventorystateNodes.Count; i++)
            {
                var node = inventorystateNodes[i];
                var data = new InventoryState();
                data.UsedSlots = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(InventoryStateDefinitionNode.PortUsedSlots));
                data.CurrentWeight = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(InventoryStateDefinitionNode.PortCurrentWeight));
                data.ID = i;
                runtimeAsset.InventoryStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var itemNodes = graph.GetNodes().OfType<ItemDefinitionNode>().ToList();
            for(int i = 0; i < itemNodes.Count; i++)
            {
                var node = itemNodes[i];
                var data = new Item();
                data.ItemType = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(ItemDefinitionNode.PortItemType));
                data.Rarity = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(ItemDefinitionNode.PortRarity));
                data.MaxStack = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(ItemDefinitionNode.PortMaxStack));
                data.Weight = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(ItemDefinitionNode.PortWeight));
                data.Value = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(ItemDefinitionNode.PortValue));
                data.ID = i;
                runtimeAsset.Items.Add(data);
                nodeToIdMap[node] = i;
            }
            var locationNodes = graph.GetNodes().OfType<LocationDefinitionNode>().ToList();
            for(int i = 0; i < locationNodes.Count; i++)
            {
                var node = locationNodes[i];
                var data = new Location();
                data.Position = GraphTookKitDBGraph.GetPortValue<UnityEngine.Vector3>(node.GetInputPortByName(LocationDefinitionNode.PortPosition));
                data.Scale = GraphTookKitDBGraph.GetPortValue<UnityEngine.Vector3>(node.GetInputPortByName(LocationDefinitionNode.PortScale));
                data.Rotation = GraphTookKitDBGraph.GetPortValue<UnityEngine.Quaternion>(node.GetInputPortByName(LocationDefinitionNode.PortRotation));
                data.ParentZoneID = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(LocationDefinitionNode.PortParentZoneID));
                data.ID = i;
                runtimeAsset.Locations.Add(data);
                nodeToIdMap[node] = i;
            }
            var missionNodes = graph.GetNodes().OfType<MissionDefinitionNode>().ToList();
            for(int i = 0; i < missionNodes.Count; i++)
            {
                var node = missionNodes[i];
                var data = new Mission();
                data.Priority = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(MissionDefinitionNode.PortPriority));
                data.IsRepeatable = GraphTookKitDBGraph.GetPortValue<bool>(node.GetInputPortByName(MissionDefinitionNode.PortIsRepeatable));
                data.MaxAttempts = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(MissionDefinitionNode.PortMaxAttempts));
                data.ID = i;
                runtimeAsset.Missions.Add(data);
                nodeToIdMap[node] = i;
            }
            var missionstateNodes = graph.GetNodes().OfType<MissionStateDefinitionNode>().ToList();
            for(int i = 0; i < missionstateNodes.Count; i++)
            {
                var node = missionstateNodes[i];
                var data = new MissionState();
                data.Attempts = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(MissionStateDefinitionNode.PortAttempts));
                data.IsActive = GraphTookKitDBGraph.GetPortValue<bool>(node.GetInputPortByName(MissionStateDefinitionNode.PortIsActive));
                data.StartTime = GraphTookKitDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(MissionStateDefinitionNode.PortStartTime));
                data.ID = i;
                runtimeAsset.MissionStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var nameNodes = graph.GetNodes().OfType<NameDefinitionNode>().ToList();
            for(int i = 0; i < nameNodes.Count; i++)
            {
                var node = nameNodes[i];
                var data = new Name();
                data.Text = GraphTookKitDBGraph.GetPortValue<string>(node.GetInputPortByName(NameDefinitionNode.PortText));
                data.LanguageID = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(NameDefinitionNode.PortLanguageID));
                data.ID = i;
                runtimeAsset.Names.Add(data);
                nodeToIdMap[node] = i;
            }
            var objectiveNodes = graph.GetNodes().OfType<ObjectiveDefinitionNode>().ToList();
            for(int i = 0; i < objectiveNodes.Count; i++)
            {
                var node = objectiveNodes[i];
                var data = new Objective();
                data.TargetValue = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(ObjectiveDefinitionNode.PortTargetValue));
                data.ObjectiveType = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(ObjectiveDefinitionNode.PortObjectiveType));
                data.IsOptional = GraphTookKitDBGraph.GetPortValue<bool>(node.GetInputPortByName(ObjectiveDefinitionNode.PortIsOptional));
                data.ID = i;
                runtimeAsset.Objectives.Add(data);
                nodeToIdMap[node] = i;
            }
            var objectivestateNodes = graph.GetNodes().OfType<ObjectiveStateDefinitionNode>().ToList();
            for(int i = 0; i < objectivestateNodes.Count; i++)
            {
                var node = objectivestateNodes[i];
                var data = new ObjectiveState();
                data.CurrentValue = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(ObjectiveStateDefinitionNode.PortCurrentValue));
                data.IsCompleted = GraphTookKitDBGraph.GetPortValue<bool>(node.GetInputPortByName(ObjectiveStateDefinitionNode.PortIsCompleted));
                data.ID = i;
                runtimeAsset.ObjectiveStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var playerNodes = graph.GetNodes().OfType<PlayerDefinitionNode>().ToList();
            for(int i = 0; i < playerNodes.Count; i++)
            {
                var node = playerNodes[i];
                var data = new Player();
                data.Level = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(PlayerDefinitionNode.PortLevel));
                data.Experience = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(PlayerDefinitionNode.PortExperience));
                data.PrestigeLevel = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(PlayerDefinitionNode.PortPrestigeLevel));
                data.CreatedAt = GraphTookKitDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(PlayerDefinitionNode.PortCreatedAt));
                data.ID = i;
                runtimeAsset.Players.Add(data);
                nodeToIdMap[node] = i;
            }
            var questNodes = graph.GetNodes().OfType<QuestDefinitionNode>().ToList();
            for(int i = 0; i < questNodes.Count; i++)
            {
                var node = questNodes[i];
                var data = new Quest();
                data.ChapterID = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(QuestDefinitionNode.PortChapterID));
                data.Difficulty = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(QuestDefinitionNode.PortDifficulty));
                data.IsMainQuest = GraphTookKitDBGraph.GetPortValue<bool>(node.GetInputPortByName(QuestDefinitionNode.PortIsMainQuest));
                data.ID = i;
                runtimeAsset.Quests.Add(data);
                nodeToIdMap[node] = i;
            }
            var queststateNodes = graph.GetNodes().OfType<QuestStateDefinitionNode>().ToList();
            for(int i = 0; i < queststateNodes.Count; i++)
            {
                var node = queststateNodes[i];
                var data = new QuestState();
                data.Progress = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(QuestStateDefinitionNode.PortProgress));
                data.IsCompleted = GraphTookKitDBGraph.GetPortValue<bool>(node.GetInputPortByName(QuestStateDefinitionNode.PortIsCompleted));
                data.ID = i;
                runtimeAsset.QuestStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var rangeasfloatNodes = graph.GetNodes().OfType<RangeAsFloatDefinitionNode>().ToList();
            for(int i = 0; i < rangeasfloatNodes.Count; i++)
            {
                var node = rangeasfloatNodes[i];
                var data = new RangeAsFloat();
                data.MinValue = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(RangeAsFloatDefinitionNode.PortMinValue));
                data.MaxValue = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(RangeAsFloatDefinitionNode.PortMaxValue));
                data.Precision = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(RangeAsFloatDefinitionNode.PortPrecision));
                data.ID = i;
                runtimeAsset.RangeAsFloats.Add(data);
                nodeToIdMap[node] = i;
            }
            var rangeasintNodes = graph.GetNodes().OfType<RangeAsIntDefinitionNode>().ToList();
            for(int i = 0; i < rangeasintNodes.Count; i++)
            {
                var node = rangeasintNodes[i];
                var data = new RangeAsInt();
                data.MinValue = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(RangeAsIntDefinitionNode.PortMinValue));
                data.MaxValue = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(RangeAsIntDefinitionNode.PortMaxValue));
                data.StepSize = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(RangeAsIntDefinitionNode.PortStepSize));
                data.ID = i;
                runtimeAsset.RangeAsInts.Add(data);
                nodeToIdMap[node] = i;
            }
            var rewardNodes = graph.GetNodes().OfType<RewardDefinitionNode>().ToList();
            for(int i = 0; i < rewardNodes.Count; i++)
            {
                var node = rewardNodes[i];
                var data = new Reward();
                data.RewardType = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(RewardDefinitionNode.PortRewardType));
                data.Amount = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(RewardDefinitionNode.PortAmount));
                data.Chance = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(RewardDefinitionNode.PortChance));
                data.ItemID = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(RewardDefinitionNode.PortItemID));
                data.ID = i;
                runtimeAsset.Rewards.Add(data);
                nodeToIdMap[node] = i;
            }
            var skillNodes = graph.GetNodes().OfType<SkillDefinitionNode>().ToList();
            for(int i = 0; i < skillNodes.Count; i++)
            {
                var node = skillNodes[i];
                var data = new Skill();
                data.SkillType = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(SkillDefinitionNode.PortSkillType));
                data.MaxLevel = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(SkillDefinitionNode.PortMaxLevel));
                data.BaseCooldown = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(SkillDefinitionNode.PortBaseCooldown));
                data.ManaCost = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(SkillDefinitionNode.PortManaCost));
                data.ID = i;
                runtimeAsset.Skills.Add(data);
                nodeToIdMap[node] = i;
            }
            var skillstateNodes = graph.GetNodes().OfType<SkillStateDefinitionNode>().ToList();
            for(int i = 0; i < skillstateNodes.Count; i++)
            {
                var node = skillstateNodes[i];
                var data = new SkillState();
                data.Level = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(SkillStateDefinitionNode.PortLevel));
                data.Experience = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(SkillStateDefinitionNode.PortExperience));
                data.LastUsed = GraphTookKitDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(SkillStateDefinitionNode.PortLastUsed));
                data.ID = i;
                runtimeAsset.SkillStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var statNodes = graph.GetNodes().OfType<StatDefinitionNode>().ToList();
            for(int i = 0; i < statNodes.Count; i++)
            {
                var node = statNodes[i];
                var data = new Stat();
                data.StatType = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(StatDefinitionNode.PortStatType));
                data.BaseValue = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(StatDefinitionNode.PortBaseValue));
                data.MinValue = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(StatDefinitionNode.PortMinValue));
                data.MaxValue = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(StatDefinitionNode.PortMaxValue));
                data.ID = i;
                runtimeAsset.Stats.Add(data);
                nodeToIdMap[node] = i;
            }
            var statstateNodes = graph.GetNodes().OfType<StatStateDefinitionNode>().ToList();
            for(int i = 0; i < statstateNodes.Count; i++)
            {
                var node = statstateNodes[i];
                var data = new StatState();
                data.CurrentValue = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(StatStateDefinitionNode.PortCurrentValue));
                data.Modifiers = GraphTookKitDBGraph.GetPortValue<float>(node.GetInputPortByName(StatStateDefinitionNode.PortModifiers));
                data.ID = i;
                runtimeAsset.StatStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var tagNodes = graph.GetNodes().OfType<TagDefinitionNode>().ToList();
            for(int i = 0; i < tagNodes.Count; i++)
            {
                var node = tagNodes[i];
                var data = new Tag();
                data.TagType = GraphTookKitDBGraph.GetPortValue<ushort>(node.GetInputPortByName(TagDefinitionNode.PortTagType));
                data.Value = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(TagDefinitionNode.PortValue));
                data.ID = i;
                runtimeAsset.Tags.Add(data);
                nodeToIdMap[node] = i;
            }
            var timestateNodes = graph.GetNodes().OfType<TimeStateDefinitionNode>().ToList();
            for(int i = 0; i < timestateNodes.Count; i++)
            {
                var node = timestateNodes[i];
                var data = new TimeState();
                data.ElapsedTicks = GraphTookKitDBGraph.GetPortValue<long>(node.GetInputPortByName(TimeStateDefinitionNode.PortElapsedTicks));
                data.IsActive = GraphTookKitDBGraph.GetPortValue<bool>(node.GetInputPortByName(TimeStateDefinitionNode.PortIsActive));
                data.CycleCount = GraphTookKitDBGraph.GetPortValue<int>(node.GetInputPortByName(TimeStateDefinitionNode.PortCycleCount));
                data.ID = i;
                runtimeAsset.TimeStates.Add(data);
                nodeToIdMap[node] = i;
            }
            var timetickNodes = graph.GetNodes().OfType<TimeTickDefinitionNode>().ToList();
            for(int i = 0; i < timetickNodes.Count; i++)
            {
                var node = timetickNodes[i];
                var data = new TimeTick();
                data.StartTicks = GraphTookKitDBGraph.GetPortValue<long>(node.GetInputPortByName(TimeTickDefinitionNode.PortStartTicks));
                data.DurationTicks = GraphTookKitDBGraph.GetPortValue<long>(node.GetInputPortByName(TimeTickDefinitionNode.PortDurationTicks));
                data.IsRepeating = GraphTookKitDBGraph.GetPortValue<bool>(node.GetInputPortByName(TimeTickDefinitionNode.PortIsRepeating));
                data.IntervalTicks = GraphTookKitDBGraph.GetPortValue<long>(node.GetInputPortByName(TimeTickDefinitionNode.PortIntervalTicks));
                data.ID = i;
                runtimeAsset.TimeTicks.Add(data);
                nodeToIdMap[node] = i;
            }

            // --- PASS 2: Create links by traversing graph connections ---
            foreach (var sourceNode in graph.GetNodes())
            {
                var outputPort = sourceNode.GetOutputPorts().FirstOrDefault(p => p.name == "OutputLink");
                if (outputPort == null || !outputPort.isConnected) continue;
                if (!nodeToIdMap.TryGetValue(sourceNode, out int sourceId)) continue;

                var sourceDataType = GetNodeType(sourceNode);
                ushort sourceTypeId = typeToIdMap.GetValueOrDefault(sourceDataType, (ushort)0);

                var connectedPorts = new List<IPort>();
                outputPort.GetConnectedPorts(connectedPorts);
                foreach (var connectedPort in connectedPorts.Where(p => p.name == "InputLink"))
                {
                    var targetNode = connectedPort.GetNode();
                    if (!nodeToIdMap.TryGetValue(targetNode, out int targetId)) continue;

                    var targetDataType = GetNodeType(targetNode);
                    ushort targetTypeId = typeToIdMap.GetValueOrDefault(targetDataType, (ushort)0);

                    var link = new Link { SourceType = sourceTypeId, SourceID = sourceId, TargetType = targetTypeId, TargetID = targetId, LinkTypeID = 0 };
                    runtimeAsset.Links.Add(link);
                }
            }

            ctx.AddObjectToAsset("GraphTookKitDBGraph", runtimeAsset);
            ctx.SetMainObject(runtimeAsset);
        }

        private Type GetNodeType(INode node)
        {
            if (node is AchievementDefinitionNode) return typeof(Achievement);
            if (node is AchievementStateDefinitionNode) return typeof(AchievementState);
            if (node is AIDefinitionNode) return typeof(AI);
            if (node is AIStateDefinitionNode) return typeof(AIState);
            if (node is CharacterDefinitionNode) return typeof(Character);
            if (node is CharacterStateDefinitionNode) return typeof(CharacterState);
            if (node is CombatDefinitionNode) return typeof(Combat);
            if (node is CombatStateDefinitionNode) return typeof(CombatState);
            if (node is DescriptionDefinitionNode) return typeof(Description);
            if (node is EconomyDefinitionNode) return typeof(Economy);
            if (node is EconomyStateDefinitionNode) return typeof(EconomyState);
            if (node is EffectDefinitionNode) return typeof(Effect);
            if (node is EffectStateDefinitionNode) return typeof(EffectState);
            if (node is EquipmentDefinitionNode) return typeof(Equipment);
            if (node is GuildDefinitionNode) return typeof(Guild);
            if (node is GuildStateDefinitionNode) return typeof(GuildState);
            if (node is InventoryDefinitionNode) return typeof(Inventory);
            if (node is InventoryStateDefinitionNode) return typeof(InventoryState);
            if (node is ItemDefinitionNode) return typeof(Item);
            if (node is LocationDefinitionNode) return typeof(Location);
            if (node is MissionDefinitionNode) return typeof(Mission);
            if (node is MissionStateDefinitionNode) return typeof(MissionState);
            if (node is NameDefinitionNode) return typeof(Name);
            if (node is ObjectiveDefinitionNode) return typeof(Objective);
            if (node is ObjectiveStateDefinitionNode) return typeof(ObjectiveState);
            if (node is PlayerDefinitionNode) return typeof(Player);
            if (node is QuestDefinitionNode) return typeof(Quest);
            if (node is QuestStateDefinitionNode) return typeof(QuestState);
            if (node is RangeAsFloatDefinitionNode) return typeof(RangeAsFloat);
            if (node is RangeAsIntDefinitionNode) return typeof(RangeAsInt);
            if (node is RewardDefinitionNode) return typeof(Reward);
            if (node is SkillDefinitionNode) return typeof(Skill);
            if (node is SkillStateDefinitionNode) return typeof(SkillState);
            if (node is StatDefinitionNode) return typeof(Stat);
            if (node is StatStateDefinitionNode) return typeof(StatState);
            if (node is TagDefinitionNode) return typeof(Tag);
            if (node is TimeStateDefinitionNode) return typeof(TimeState);
            if (node is TimeTickDefinitionNode) return typeof(TimeTick);
            return null;
        }
    }

    #endregion
}
