// ----- AUTO-GENERATED FILE BY GraphToolGenerator.cs -----
// This file contains the complete GraphToolKit implementation for GameDatabase.
// Generated on: 8/20/2025 10:30:46 AM

using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;

// A dummy class to represent a linkable port in the graph.
public class Linkable
{
}

#region Runtime Asset

#endregion

#region Graph Definition

[Graph("gdb")]
[Serializable]
public class GameDatabaseGraph : Graph
{
    [MenuItem("Assets/Create/GameDatabase Graph")]
    private static void CreateAssetFile() =>
        GraphDatabase.PromptInProjectBrowserToCreateNewAsset<GameDatabaseGraph>("New GameDatabase");

    public static T GetPortValue<T>(IPort port)
    {
        if (port.isConnected)
        {
            var sourceNode = port.firstConnectedPort?.GetNode();
            if (sourceNode is IConstantNode c)
            {
                c.TryGetValue(out T v);
                return v;
            }

            if (sourceNode is IVariableNode vn)
            {
                vn.variable.TryGetDefaultValue(out T v);
                return v;
            }
        }
        else
        {
            port.TryGetValue(out T v);
            return v;
        }

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
    public const string PortId = "Id";
    public const string PortAchievementType = "AchievementType";
    public const string PortPoints = "Points";
    public const string PortIsHidden = "IsHidden";
    public const string PortUnlockDate = "UnlockDate";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortProgress = "Progress";
    public const string PortIsUnlocked = "IsUnlocked";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<float>(PortProgress).WithDisplayName("Progress").Build();
        context.AddInputPort<bool>(PortIsUnlocked).WithDisplayName("Is Unlocked").Build();
    }
}

[Serializable]
public class AIDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortBehaviorType = "BehaviorType";
    public const string PortAggroRange = "AggroRange";
    public const string PortPatrolRadius = "PatrolRadius";
    public const string PortPriority = "Priority";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortCurrentTarget = "CurrentTarget";
    public const string PortLastAction = "LastAction";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<int>(PortCurrentTarget).WithDisplayName("Current Target").Build();
        context.AddInputPort<System.DateTime>(PortLastAction).WithDisplayName("Last Action").Build();
    }
}

[Serializable]
public class CharacterDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortCharacterClass = "CharacterClass";
    public const string PortLevel = "Level";
    public const string PortRace = "Race";
    public const string PortGender = "Gender";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortHealth = "Health";
    public const string PortMana = "Mana";
    public const string PortStamina = "Stamina";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortBaseDamage = "BaseDamage";
    public const string PortAttackSpeed = "AttackSpeed";
    public const string PortCritChance = "CritChance";
    public const string PortCritMultiplier = "CritMultiplier";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortIsInCombat = "IsInCombat";
    public const string PortLastAttack = "LastAttack";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<bool>(PortIsInCombat).WithDisplayName("Is In Combat").Build();
        context.AddInputPort<System.DateTime>(PortLastAttack).WithDisplayName("Last Attack").Build();
    }
}

[Serializable]
public class DescriptionDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortText = "Text";
    public const string PortLanguageID = "LanguageID";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<string>(PortText).WithDisplayName("Text").Build();
        context.AddInputPort<ushort>(PortLanguageID).WithDisplayName("Language I D").Build();
    }
}

[Serializable]
public class EconomyDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortBasePrice = "BasePrice";
    public const string PortInflation = "Inflation";
    public const string PortSupply = "Supply";
    public const string PortDemand = "Demand";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortCurrentPrice = "CurrentPrice";
    public const string PortLastUpdate = "LastUpdate";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<int>(PortCurrentPrice).WithDisplayName("Current Price").Build();
        context.AddInputPort<System.DateTime>(PortLastUpdate).WithDisplayName("Last Update").Build();
    }
}

[Serializable]
public class EffectDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortEffectType = "EffectType";
    public const string PortMagnitude = "Magnitude";
    public const string PortDurationTicks = "DurationTicks";
    public const string PortIsStackable = "IsStackable";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortStacks = "Stacks";
    public const string PortRemainingTicks = "RemainingTicks";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<int>(PortStacks).WithDisplayName("Stacks").Build();
        context.AddInputPort<long>(PortRemainingTicks).WithDisplayName("Remaining Ticks").Build();
    }
}

[Serializable]
public class EquipmentDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortEquipSlot = "EquipSlot";
    public const string PortLevel = "Level";
    public const string PortDurability = "Durability";
    public const string PortMaxDurability = "MaxDurability";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortMaxMembers = "MaxMembers";
    public const string PortLevel = "Level";
    public const string PortExperience = "Experience";
    public const string PortCreatedAt = "CreatedAt";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortMemberCount = "MemberCount";
    public const string PortTreasury = "Treasury";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<int>(PortMemberCount).WithDisplayName("Member Count").Build();
        context.AddInputPort<long>(PortTreasury).WithDisplayName("Treasury").Build();
    }
}

[Serializable]
public class InventoryDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortMaxSlots = "MaxSlots";
    public const string PortMaxWeight = "MaxWeight";
    public const string PortInventoryType = "InventoryType";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortUsedSlots = "UsedSlots";
    public const string PortCurrentWeight = "CurrentWeight";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<int>(PortUsedSlots).WithDisplayName("Used Slots").Build();
        context.AddInputPort<float>(PortCurrentWeight).WithDisplayName("Current Weight").Build();
    }
}

[Serializable]
public class ItemDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortItemType = "ItemType";
    public const string PortRarity = "Rarity";
    public const string PortMaxStack = "MaxStack";
    public const string PortWeight = "Weight";
    public const string PortValue = "Value";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortPosition = "Position";
    public const string PortScale = "Scale";
    public const string PortRotation = "Rotation";
    public const string PortParentZoneID = "ParentZoneID";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortPriority = "Priority";
    public const string PortIsRepeatable = "IsRepeatable";
    public const string PortMaxAttempts = "MaxAttempts";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortAttempts = "Attempts";
    public const string PortIsActive = "IsActive";
    public const string PortStartTime = "StartTime";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortText = "Text";
    public const string PortLanguageID = "LanguageID";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<string>(PortText).WithDisplayName("Text").Build();
        context.AddInputPort<ushort>(PortLanguageID).WithDisplayName("Language I D").Build();
    }
}

[Serializable]
public class ObjectiveDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortTargetValue = "TargetValue";
    public const string PortObjectiveType = "ObjectiveType";
    public const string PortIsOptional = "IsOptional";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortCurrentValue = "CurrentValue";
    public const string PortIsCompleted = "IsCompleted";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<int>(PortCurrentValue).WithDisplayName("Current Value").Build();
        context.AddInputPort<bool>(PortIsCompleted).WithDisplayName("Is Completed").Build();
    }
}

[Serializable]
public class PlayerDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortLevel = "Level";
    public const string PortExperience = "Experience";
    public const string PortPrestigeLevel = "PrestigeLevel";
    public const string PortCreatedAt = "CreatedAt";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortChapterID = "ChapterID";
    public const string PortDifficulty = "Difficulty";
    public const string PortIsMainQuest = "IsMainQuest";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortProgress = "Progress";
    public const string PortIsCompleted = "IsCompleted";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<float>(PortProgress).WithDisplayName("Progress").Build();
        context.AddInputPort<bool>(PortIsCompleted).WithDisplayName("Is Completed").Build();
    }
}

[Serializable]
public class RangeFloatDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortMinValue = "MinValue";
    public const string PortMaxValue = "MaxValue";
    public const string PortPrecision = "Precision";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<float>(PortMinValue).WithDisplayName("Min Value").Build();
        context.AddInputPort<float>(PortMaxValue).WithDisplayName("Max Value").Build();
        context.AddInputPort<float>(PortPrecision).WithDisplayName("Precision").Build();
    }
}

[Serializable]
public class RangeIntDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortMinValue = "MinValue";
    public const string PortMaxValue = "MaxValue";
    public const string PortStepSize = "StepSize";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortRewardType = "RewardType";
    public const string PortAmount = "Amount";
    public const string PortChance = "Chance";
    public const string PortItemID = "ItemID";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortSkillType = "SkillType";
    public const string PortMaxLevel = "MaxLevel";
    public const string PortBaseCooldown = "BaseCooldown";
    public const string PortManaCost = "ManaCost";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortLevel = "Level";
    public const string PortExperience = "Experience";
    public const string PortLastUsed = "LastUsed";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortStatType = "StatType";
    public const string PortBaseValue = "BaseValue";
    public const string PortMinValue = "MinValue";
    public const string PortMaxValue = "MaxValue";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
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
    public const string PortId = "Id";
    public const string PortCurrentValue = "CurrentValue";
    public const string PortModifiers = "Modifiers";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<float>(PortCurrentValue).WithDisplayName("Current Value").Build();
        context.AddInputPort<float>(PortModifiers).WithDisplayName("Modifiers").Build();
    }
}

[Serializable]
public class TagDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortTagType = "TagType";
    public const string PortValue = "Value";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<ushort>(PortTagType).WithDisplayName("Tag Type").Build();
        context.AddInputPort<int>(PortValue).WithDisplayName("Value").Build();
    }
}

[Serializable]
public class TimeDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortStartTicks = "StartTicks";
    public const string PortDurationTicks = "DurationTicks";
    public const string PortIsRepeating = "IsRepeating";
    public const string PortIntervalTicks = "IntervalTicks";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<long>(PortStartTicks).WithDisplayName("Start Ticks").Build();
        context.AddInputPort<long>(PortDurationTicks).WithDisplayName("Duration Ticks").Build();
        context.AddInputPort<bool>(PortIsRepeating).WithDisplayName("Is Repeating").Build();
        context.AddInputPort<long>(PortIntervalTicks).WithDisplayName("Interval Ticks").Build();
    }
}

[Serializable]
public class TimeStateDefinitionNode : Node
{
    public const string PortInputLink = "InputLink";
    public const string PortOutputLink = "OutputLink";
    public const string PortId = "Id";
    public const string PortElapsedTicks = "ElapsedTicks";
    public const string PortIsActive = "IsActive";
    public const string PortCycleCount = "CycleCount";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<Linkable>(PortInputLink).WithDisplayName(" ").Build();
        context.AddOutputPort<Linkable>(PortOutputLink).WithDisplayName(" ").Build();

        context.AddInputPort<int>(PortId).WithDisplayName("Id").Build();
        context.AddInputPort<long>(PortElapsedTicks).WithDisplayName("Elapsed Ticks").Build();
        context.AddInputPort<bool>(PortIsActive).WithDisplayName("Is Active").Build();
        context.AddInputPort<int>(PortCycleCount).WithDisplayName("Cycle Count").Build();
    }
}

#endregion

#region Scripted Importer

#endregion