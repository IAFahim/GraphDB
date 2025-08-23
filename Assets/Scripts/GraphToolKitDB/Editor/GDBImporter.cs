// ----- AUTO-GENERATED IMPORTER FILE BY GraphToolGenerator.cs -----

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
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
    #region Scripted Importer

    [ScriptedImporter(1, "gdb")]
    public class GDBImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var graph = GraphDatabase.LoadGraphForImporter<GDBGraph>(ctx.assetPath);
            if (graph == null) return;

            var runtimeAsset = ScriptableObject.CreateInstance<GDBAsset>();
            var nodeToIdMap = new Dictionary<INode, GraphDBNode>();

            // --- PASS 1: Create all data entries and map nodes to their index-based ID ---
            var achievementNodes = graph.GetNodes().OfType<AchievementNode>().ToList();
            for (ushort i = 0; i < achievementNodes.Count; i++)
            {
                var node = achievementNodes[i];
                var data = new Achievement();
                data.AchievementType =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(AchievementNode.PortAchievementType));
                data.Points = GDBGraph.GetPortValue<int>(node.GetInputPortByName(AchievementNode.PortPoints));
                data.IsHidden = GDBGraph.GetPortValue<bool>(node.GetInputPortByName(AchievementNode.PortIsHidden));
                data.UnlockDate =
                    GDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(AchievementNode.PortUnlockDate));
                runtimeAsset.Achievements.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var achievementstateNodes = graph.GetNodes().OfType<AchievementStateNode>().ToList();
            for (ushort i = 0; i < achievementstateNodes.Count; i++)
            {
                var node = achievementstateNodes[i];
                var data = new AchievementState();
                data.Progress =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(AchievementStateNode.PortProgress));
                data.IsUnlocked =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(AchievementStateNode.PortIsUnlocked));
                runtimeAsset.AchievementStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var aiNodes = graph.GetNodes().OfType<AINode>().ToList();
            for (ushort i = 0; i < aiNodes.Count; i++)
            {
                var node = aiNodes[i];
                var data = new AI();
                data.BehaviorType = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(AINode.PortBehaviorType));
                data.AggroRange = GDBGraph.GetPortValue<float>(node.GetInputPortByName(AINode.PortAggroRange));
                data.PatrolRadius = GDBGraph.GetPortValue<float>(node.GetInputPortByName(AINode.PortPatrolRadius));
                data.Priority = GDBGraph.GetPortValue<int>(node.GetInputPortByName(AINode.PortPriority));
                runtimeAsset.AIs.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var aistateNodes = graph.GetNodes().OfType<AIStateNode>().ToList();
            for (ushort i = 0; i < aistateNodes.Count; i++)
            {
                var node = aistateNodes[i];
                var data = new AIState();
                data.CurrentTarget = GDBGraph.GetPortValue<int>(node.GetInputPortByName(AIStateNode.PortCurrentTarget));
                data.LastAction =
                    GDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(AIStateNode.PortLastAction));
                runtimeAsset.AIStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var characterNodes = graph.GetNodes().OfType<CharacterNode>().ToList();
            for (ushort i = 0; i < characterNodes.Count; i++)
            {
                var node = characterNodes[i];
                var data = new Character();
                data.CharacterClass =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(CharacterNode.PortCharacterClass));
                data.Level = GDBGraph.GetPortValue<int>(node.GetInputPortByName(CharacterNode.PortLevel));
                data.Race = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(CharacterNode.PortRace));
                data.Gender = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(CharacterNode.PortGender));
                runtimeAsset.Characters.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var characterstateNodes = graph.GetNodes().OfType<CharacterStateNode>().ToList();
            for (ushort i = 0; i < characterstateNodes.Count; i++)
            {
                var node = characterstateNodes[i];
                var data = new CharacterState();
                data.Health = GDBGraph.GetPortValue<float>(node.GetInputPortByName(CharacterStateNode.PortHealth));
                data.Mana = GDBGraph.GetPortValue<float>(node.GetInputPortByName(CharacterStateNode.PortMana));
                data.Stamina = GDBGraph.GetPortValue<float>(node.GetInputPortByName(CharacterStateNode.PortStamina));
                runtimeAsset.CharacterStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var combatNodes = graph.GetNodes().OfType<CombatNode>().ToList();
            for (ushort i = 0; i < combatNodes.Count; i++)
            {
                var node = combatNodes[i];
                var data = new Combat();
                data.BaseDamage = GDBGraph.GetPortValue<float>(node.GetInputPortByName(CombatNode.PortBaseDamage));
                data.AttackSpeed = GDBGraph.GetPortValue<float>(node.GetInputPortByName(CombatNode.PortAttackSpeed));
                data.CritChance = GDBGraph.GetPortValue<float>(node.GetInputPortByName(CombatNode.PortCritChance));
                data.CritMultiplier =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(CombatNode.PortCritMultiplier));
                runtimeAsset.Combats.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var combatstateNodes = graph.GetNodes().OfType<CombatStateNode>().ToList();
            for (ushort i = 0; i < combatstateNodes.Count; i++)
            {
                var node = combatstateNodes[i];
                var data = new CombatState();
                data.IsInCombat = GDBGraph.GetPortValue<bool>(node.GetInputPortByName(CombatStateNode.PortIsInCombat));
                data.LastAttack =
                    GDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(CombatStateNode.PortLastAttack));
                runtimeAsset.CombatStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var descriptionNodes = graph.GetNodes().OfType<DescriptionNode>().ToList();
            for (ushort i = 0; i < descriptionNodes.Count; i++)
            {
                var node = descriptionNodes[i];
                var data = new Description();
                data.Text = GDBGraph.GetPortValue<string>(node.GetInputPortByName(DescriptionNode.PortText));
                data.LanguageID =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(DescriptionNode.PortLanguageID));
                runtimeAsset.Descriptions.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var economyNodes = graph.GetNodes().OfType<EconomyNode>().ToList();
            for (ushort i = 0; i < economyNodes.Count; i++)
            {
                var node = economyNodes[i];
                var data = new Economy();
                data.BasePrice = GDBGraph.GetPortValue<int>(node.GetInputPortByName(EconomyNode.PortBasePrice));
                data.Inflation = GDBGraph.GetPortValue<float>(node.GetInputPortByName(EconomyNode.PortInflation));
                data.Supply = GDBGraph.GetPortValue<float>(node.GetInputPortByName(EconomyNode.PortSupply));
                data.Demand = GDBGraph.GetPortValue<float>(node.GetInputPortByName(EconomyNode.PortDemand));
                runtimeAsset.Economys.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var economystateNodes = graph.GetNodes().OfType<EconomyStateNode>().ToList();
            for (ushort i = 0; i < economystateNodes.Count; i++)
            {
                var node = economystateNodes[i];
                var data = new EconomyState();
                data.CurrentPrice =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(EconomyStateNode.PortCurrentPrice));
                data.LastUpdate =
                    GDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(EconomyStateNode.PortLastUpdate));
                runtimeAsset.EconomyStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var effectNodes = graph.GetNodes().OfType<EffectNode>().ToList();
            for (ushort i = 0; i < effectNodes.Count; i++)
            {
                var node = effectNodes[i];
                var data = new Effect();
                data.EffectType = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(EffectNode.PortEffectType));
                data.Magnitude = GDBGraph.GetPortValue<float>(node.GetInputPortByName(EffectNode.PortMagnitude));
                data.DurationTicks = GDBGraph.GetPortValue<long>(node.GetInputPortByName(EffectNode.PortDurationTicks));
                data.IsStackable = GDBGraph.GetPortValue<bool>(node.GetInputPortByName(EffectNode.PortIsStackable));
                runtimeAsset.Effects.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var effectstateNodes = graph.GetNodes().OfType<EffectStateNode>().ToList();
            for (ushort i = 0; i < effectstateNodes.Count; i++)
            {
                var node = effectstateNodes[i];
                var data = new EffectState();
                data.Stacks = GDBGraph.GetPortValue<int>(node.GetInputPortByName(EffectStateNode.PortStacks));
                data.RemainingTicks =
                    GDBGraph.GetPortValue<long>(node.GetInputPortByName(EffectStateNode.PortRemainingTicks));
                runtimeAsset.EffectStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var equipmentNodes = graph.GetNodes().OfType<EquipmentNode>().ToList();
            for (ushort i = 0; i < equipmentNodes.Count; i++)
            {
                var node = equipmentNodes[i];
                var data = new Equipment();
                data.EquipSlot = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(EquipmentNode.PortEquipSlot));
                data.Level = GDBGraph.GetPortValue<int>(node.GetInputPortByName(EquipmentNode.PortLevel));
                data.Durability = GDBGraph.GetPortValue<int>(node.GetInputPortByName(EquipmentNode.PortDurability));
                data.MaxDurability =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(EquipmentNode.PortMaxDurability));
                runtimeAsset.Equipments.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var guildNodes = graph.GetNodes().OfType<GuildNode>().ToList();
            for (ushort i = 0; i < guildNodes.Count; i++)
            {
                var node = guildNodes[i];
                var data = new Guild();
                data.MaxMembers = GDBGraph.GetPortValue<int>(node.GetInputPortByName(GuildNode.PortMaxMembers));
                data.Level = GDBGraph.GetPortValue<int>(node.GetInputPortByName(GuildNode.PortLevel));
                data.Experience = GDBGraph.GetPortValue<long>(node.GetInputPortByName(GuildNode.PortExperience));
                data.CreatedAt =
                    GDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(GuildNode.PortCreatedAt));
                runtimeAsset.Guilds.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var guildstateNodes = graph.GetNodes().OfType<GuildStateNode>().ToList();
            for (ushort i = 0; i < guildstateNodes.Count; i++)
            {
                var node = guildstateNodes[i];
                var data = new GuildState();
                data.MemberCount = GDBGraph.GetPortValue<int>(node.GetInputPortByName(GuildStateNode.PortMemberCount));
                data.Treasury = GDBGraph.GetPortValue<long>(node.GetInputPortByName(GuildStateNode.PortTreasury));
                runtimeAsset.GuildStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var inventoryNodes = graph.GetNodes().OfType<InventoryNode>().ToList();
            for (ushort i = 0; i < inventoryNodes.Count; i++)
            {
                var node = inventoryNodes[i];
                var data = new Inventory();
                data.MaxSlots = GDBGraph.GetPortValue<int>(node.GetInputPortByName(InventoryNode.PortMaxSlots));
                data.MaxWeight = GDBGraph.GetPortValue<float>(node.GetInputPortByName(InventoryNode.PortMaxWeight));
                data.InventoryType =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(InventoryNode.PortInventoryType));
                runtimeAsset.Inventorys.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var inventorystateNodes = graph.GetNodes().OfType<InventoryStateNode>().ToList();
            for (ushort i = 0; i < inventorystateNodes.Count; i++)
            {
                var node = inventorystateNodes[i];
                var data = new InventoryState();
                data.UsedSlots = GDBGraph.GetPortValue<int>(node.GetInputPortByName(InventoryStateNode.PortUsedSlots));
                data.CurrentWeight =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(InventoryStateNode.PortCurrentWeight));
                runtimeAsset.InventoryStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var itemNodes = graph.GetNodes().OfType<ItemNode>().ToList();
            for (ushort i = 0; i < itemNodes.Count; i++)
            {
                var node = itemNodes[i];
                var data = new Item();
                data.ItemType = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(ItemNode.PortItemType));
                data.Rarity = GDBGraph.GetPortValue<int>(node.GetInputPortByName(ItemNode.PortRarity));
                data.MaxStack = GDBGraph.GetPortValue<int>(node.GetInputPortByName(ItemNode.PortMaxStack));
                data.Weight = GDBGraph.GetPortValue<float>(node.GetInputPortByName(ItemNode.PortWeight));
                data.Value = GDBGraph.GetPortValue<int>(node.GetInputPortByName(ItemNode.PortValue));
                runtimeAsset.Items.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var locationNodes = graph.GetNodes().OfType<LocationNode>().ToList();
            for (ushort i = 0; i < locationNodes.Count; i++)
            {
                var node = locationNodes[i];
                var data = new Location();
                data.Position =
                    GDBGraph.GetPortValue<UnityEngine.Vector3>(node.GetInputPortByName(LocationNode.PortPosition));
                data.Scale =
                    GDBGraph.GetPortValue<UnityEngine.Vector3>(node.GetInputPortByName(LocationNode.PortScale));
                data.Rotation =
                    GDBGraph.GetPortValue<UnityEngine.Quaternion>(node.GetInputPortByName(LocationNode.PortRotation));
                data.ParentZoneID = GDBGraph.GetPortValue<int>(node.GetInputPortByName(LocationNode.PortParentZoneID));
                runtimeAsset.Locations.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var missionNodes = graph.GetNodes().OfType<MissionNode>().ToList();
            for (ushort i = 0; i < missionNodes.Count; i++)
            {
                var node = missionNodes[i];
                var data = new Mission();
                data.Priority = GDBGraph.GetPortValue<int>(node.GetInputPortByName(MissionNode.PortPriority));
                data.IsRepeatable = GDBGraph.GetPortValue<bool>(node.GetInputPortByName(MissionNode.PortIsRepeatable));
                data.MaxAttempts = GDBGraph.GetPortValue<int>(node.GetInputPortByName(MissionNode.PortMaxAttempts));
                runtimeAsset.Missions.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var missionstateNodes = graph.GetNodes().OfType<MissionStateNode>().ToList();
            for (ushort i = 0; i < missionstateNodes.Count; i++)
            {
                var node = missionstateNodes[i];
                var data = new MissionState();
                data.Attempts = GDBGraph.GetPortValue<int>(node.GetInputPortByName(MissionStateNode.PortAttempts));
                data.IsActive = GDBGraph.GetPortValue<bool>(node.GetInputPortByName(MissionStateNode.PortIsActive));
                data.StartTime =
                    GDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(MissionStateNode.PortStartTime));
                runtimeAsset.MissionStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var nameNodes = graph.GetNodes().OfType<NameNode>().ToList();
            for (ushort i = 0; i < nameNodes.Count; i++)
            {
                var node = nameNodes[i];
                var data = new Name();
                data.Text = GDBGraph.GetPortValue<string>(node.GetInputPortByName(NameNode.PortText));
                data.LanguageID = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(NameNode.PortLanguageID));
                runtimeAsset.Names.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var objectiveNodes = graph.GetNodes().OfType<ObjectiveNode>().ToList();
            for (ushort i = 0; i < objectiveNodes.Count; i++)
            {
                var node = objectiveNodes[i];
                var data = new Objective();
                data.TargetValue = GDBGraph.GetPortValue<int>(node.GetInputPortByName(ObjectiveNode.PortTargetValue));
                data.ObjectiveType =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(ObjectiveNode.PortObjectiveType));
                data.IsOptional = GDBGraph.GetPortValue<bool>(node.GetInputPortByName(ObjectiveNode.PortIsOptional));
                runtimeAsset.Objectives.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var objectivestateNodes = graph.GetNodes().OfType<ObjectiveStateNode>().ToList();
            for (ushort i = 0; i < objectivestateNodes.Count; i++)
            {
                var node = objectivestateNodes[i];
                var data = new ObjectiveState();
                data.CurrentValue =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(ObjectiveStateNode.PortCurrentValue));
                data.IsCompleted =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(ObjectiveStateNode.PortIsCompleted));
                runtimeAsset.ObjectiveStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var playerNodes = graph.GetNodes().OfType<PlayerNode>().ToList();
            for (ushort i = 0; i < playerNodes.Count; i++)
            {
                var node = playerNodes[i];
                var data = new Player();
                data.Level = GDBGraph.GetPortValue<int>(node.GetInputPortByName(PlayerNode.PortLevel));
                data.Experience = GDBGraph.GetPortValue<float>(node.GetInputPortByName(PlayerNode.PortExperience));
                data.PrestigeLevel = GDBGraph.GetPortValue<int>(node.GetInputPortByName(PlayerNode.PortPrestigeLevel));
                data.CreatedAt =
                    GDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(PlayerNode.PortCreatedAt));
                runtimeAsset.Players.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var questNodes = graph.GetNodes().OfType<QuestNode>().ToList();
            for (ushort i = 0; i < questNodes.Count; i++)
            {
                var node = questNodes[i];
                var data = new Quest();
                data.ChapterID = GDBGraph.GetPortValue<int>(node.GetInputPortByName(QuestNode.PortChapterID));
                data.Difficulty = GDBGraph.GetPortValue<int>(node.GetInputPortByName(QuestNode.PortDifficulty));
                data.IsMainQuest = GDBGraph.GetPortValue<bool>(node.GetInputPortByName(QuestNode.PortIsMainQuest));
                runtimeAsset.Quests.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var queststateNodes = graph.GetNodes().OfType<QuestStateNode>().ToList();
            for (ushort i = 0; i < queststateNodes.Count; i++)
            {
                var node = queststateNodes[i];
                var data = new QuestState();
                data.Progress = GDBGraph.GetPortValue<float>(node.GetInputPortByName(QuestStateNode.PortProgress));
                data.IsCompleted = GDBGraph.GetPortValue<bool>(node.GetInputPortByName(QuestStateNode.PortIsCompleted));
                runtimeAsset.QuestStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var rangeasfloatNodes = graph.GetNodes().OfType<RangeAsFloatNode>().ToList();
            for (ushort i = 0; i < rangeasfloatNodes.Count; i++)
            {
                var node = rangeasfloatNodes[i];
                var data = new RangeAsFloat();
                data.MinValue = GDBGraph.GetPortValue<float>(node.GetInputPortByName(RangeAsFloatNode.PortMinValue));
                data.MaxValue = GDBGraph.GetPortValue<float>(node.GetInputPortByName(RangeAsFloatNode.PortMaxValue));
                data.Precision = GDBGraph.GetPortValue<float>(node.GetInputPortByName(RangeAsFloatNode.PortPrecision));
                runtimeAsset.RangeAsFloats.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var rangeasintNodes = graph.GetNodes().OfType<RangeAsIntNode>().ToList();
            for (ushort i = 0; i < rangeasintNodes.Count; i++)
            {
                var node = rangeasintNodes[i];
                var data = new RangeAsInt();
                data.MinValue = GDBGraph.GetPortValue<int>(node.GetInputPortByName(RangeAsIntNode.PortMinValue));
                data.MaxValue = GDBGraph.GetPortValue<int>(node.GetInputPortByName(RangeAsIntNode.PortMaxValue));
                data.StepSize = GDBGraph.GetPortValue<int>(node.GetInputPortByName(RangeAsIntNode.PortStepSize));
                runtimeAsset.RangeAsInts.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var rewardNodes = graph.GetNodes().OfType<RewardNode>().ToList();
            for (ushort i = 0; i < rewardNodes.Count; i++)
            {
                var node = rewardNodes[i];
                var data = new Reward();
                data.RewardType = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(RewardNode.PortRewardType));
                data.Amount = GDBGraph.GetPortValue<int>(node.GetInputPortByName(RewardNode.PortAmount));
                data.Chance = GDBGraph.GetPortValue<float>(node.GetInputPortByName(RewardNode.PortChance));
                data.ItemID = GDBGraph.GetPortValue<int>(node.GetInputPortByName(RewardNode.PortItemID));
                runtimeAsset.Rewards.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var skillNodes = graph.GetNodes().OfType<SkillNode>().ToList();
            for (ushort i = 0; i < skillNodes.Count; i++)
            {
                var node = skillNodes[i];
                var data = new Skill();
                data.SkillType = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(SkillNode.PortSkillType));
                data.MaxLevel = GDBGraph.GetPortValue<int>(node.GetInputPortByName(SkillNode.PortMaxLevel));
                data.BaseCooldown = GDBGraph.GetPortValue<float>(node.GetInputPortByName(SkillNode.PortBaseCooldown));
                data.ManaCost = GDBGraph.GetPortValue<int>(node.GetInputPortByName(SkillNode.PortManaCost));
                runtimeAsset.Skills.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var skillstateNodes = graph.GetNodes().OfType<SkillStateNode>().ToList();
            for (ushort i = 0; i < skillstateNodes.Count; i++)
            {
                var node = skillstateNodes[i];
                var data = new SkillState();
                data.Level = GDBGraph.GetPortValue<int>(node.GetInputPortByName(SkillStateNode.PortLevel));
                data.Experience = GDBGraph.GetPortValue<float>(node.GetInputPortByName(SkillStateNode.PortExperience));
                data.LastUsed =
                    GDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(SkillStateNode.PortLastUsed));
                runtimeAsset.SkillStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var statNodes = graph.GetNodes().OfType<StatNode>().ToList();
            for (ushort i = 0; i < statNodes.Count; i++)
            {
                var node = statNodes[i];
                var data = new Stat();
                data.StatType = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(StatNode.PortStatType));
                data.BaseValue = GDBGraph.GetPortValue<float>(node.GetInputPortByName(StatNode.PortBaseValue));
                data.MinValue = GDBGraph.GetPortValue<float>(node.GetInputPortByName(StatNode.PortMinValue));
                data.MaxValue = GDBGraph.GetPortValue<float>(node.GetInputPortByName(StatNode.PortMaxValue));
                runtimeAsset.Stats.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var statstateNodes = graph.GetNodes().OfType<StatStateNode>().ToList();
            for (ushort i = 0; i < statstateNodes.Count; i++)
            {
                var node = statstateNodes[i];
                var data = new StatState();
                data.CurrentValue =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(StatStateNode.PortCurrentValue));
                data.Modifiers = GDBGraph.GetPortValue<float>(node.GetInputPortByName(StatStateNode.PortModifiers));
                runtimeAsset.StatStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var tagNodes = graph.GetNodes().OfType<TagNode>().ToList();
            for (ushort i = 0; i < tagNodes.Count; i++)
            {
                var node = tagNodes[i];
                var data = new Tag();
                data.TagType = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(TagNode.PortTagType));
                data.Value = GDBGraph.GetPortValue<int>(node.GetInputPortByName(TagNode.PortValue));
                runtimeAsset.Tags.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var timestateNodes = graph.GetNodes().OfType<TimeStateNode>().ToList();
            for (ushort i = 0; i < timestateNodes.Count; i++)
            {
                var node = timestateNodes[i];
                var data = new TimeState();
                data.ElapsedTicks =
                    GDBGraph.GetPortValue<long>(node.GetInputPortByName(TimeStateNode.PortElapsedTicks));
                data.IsActive = GDBGraph.GetPortValue<bool>(node.GetInputPortByName(TimeStateNode.PortIsActive));
                data.CycleCount = GDBGraph.GetPortValue<int>(node.GetInputPortByName(TimeStateNode.PortCycleCount));
                runtimeAsset.TimeStates.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            var timetickNodes = graph.GetNodes().OfType<TimeTickNode>().ToList();
            for (ushort i = 0; i < timetickNodes.Count; i++)
            {
                var node = timetickNodes[i];
                var data = new TimeTick();
                data.StartTicks = GDBGraph.GetPortValue<long>(node.GetInputPortByName(TimeTickNode.PortStartTicks));
                data.DurationTicks =
                    GDBGraph.GetPortValue<long>(node.GetInputPortByName(TimeTickNode.PortDurationTicks));
                data.IsRepeating = GDBGraph.GetPortValue<bool>(node.GetInputPortByName(TimeTickNode.PortIsRepeating));
                data.IntervalTicks =
                    GDBGraph.GetPortValue<long>(node.GetInputPortByName(TimeTickNode.PortIntervalTicks));
                runtimeAsset.TimeTicks.Add(data);
                node.NodeID = i;
                nodeToIdMap[node] = node;
            }

            PopulateLink(graph, nodeToIdMap, runtimeAsset);
            ctx.AddObjectToAsset("GDB", runtimeAsset);
            ctx.SetMainObject(runtimeAsset);
        }

        private static void PopulateLink(GDBGraph graph, Dictionary<INode, GraphDBNode> nodeToIdMap,
            GDBAsset runtimeAsset)
        {
            ushort linkId = 0;
            foreach (var sourceNode in graph.GetNodes().OfType<GraphDBNode>())
            {
                var outputPort = sourceNode.GetOutputPorts().FirstOrDefault(p => p.name == GraphDBNode.PortOutputLink);
                foreach (var linkBlock in sourceNode.blockNodes)
                {
                    var nodeOption = linkBlock.GetNodeOptionByName(EntryIDBlock.OptionLink);
                    if (!nodeOption.TryGetValue(out EntryIDSchema entryIDSchema)) continue;
                    if (entryIDSchema == null) continue;
                    entryIDSchema.Id = sourceNode.NodeID;
                    entryIDSchema.Type = sourceNode.EntityType;
                }

                if (outputPort == null || !outputPort.isConnected) continue;


                var connectedPorts = new List<IPort>();
                outputPort.GetConnectedPorts(connectedPorts);
                foreach (var connectedPort in connectedPorts.Where(p => p.name == GraphDBNode.PortInputLink))
                {
                    var targetNode = connectedPort.GetNode();
                    if (!nodeToIdMap.TryGetValue(targetNode, out var targetData)) continue;
                    var link = new Link
                    {
                        ID = linkId++,
                        SourceType = sourceNode.EntityType,
                        SourceID = sourceNode.NodeID,
                        TargetType = targetData.EntityType,
                        TargetID = targetData.NodeID,
                    };
                    runtimeAsset.Links.Add(link);
                }
            }
        }
    }

    #endregion
}