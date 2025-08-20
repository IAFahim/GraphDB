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
            var nodeToIdMap = new Dictionary<INode, int>();

            // --- PASS 1: Create all data entries and map nodes to their index-based ID ---
            var achievementNodes = graph.GetNodes().OfType<AchievementDefinitionNode>().ToList();
            for (int i = 0; i < achievementNodes.Count; i++)
            {
                var node = achievementNodes[i];
                var data = new Achievement();
                data.AchievementType =
                    GDBGraph.GetPortValue<ushort>(
                        node.GetInputPortByName(AchievementDefinitionNode.PortAchievementType));
                data.Points = GDBGraph.GetPortValue<int>(node.GetInputPortByName(AchievementDefinitionNode.PortPoints));
                data.IsHidden =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(AchievementDefinitionNode.PortIsHidden));
                data.UnlockDate =
                    GDBGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(AchievementDefinitionNode.PortUnlockDate));
                data.ID = i;
                runtimeAsset.Achievements.Add(data);
                nodeToIdMap[node] = i;
            }

            var achievementstateNodes = graph.GetNodes().OfType<AchievementStateDefinitionNode>().ToList();
            for (int i = 0; i < achievementstateNodes.Count; i++)
            {
                var node = achievementstateNodes[i];
                var data = new AchievementState();
                data.Progress =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(AchievementStateDefinitionNode.PortProgress));
                data.IsUnlocked =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(AchievementStateDefinitionNode.PortIsUnlocked));
                data.ID = i;
                runtimeAsset.AchievementStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var aiNodes = graph.GetNodes().OfType<AIDefinitionNode>().ToList();
            for (int i = 0; i < aiNodes.Count; i++)
            {
                var node = aiNodes[i];
                var data = new AI();
                data.BehaviorType =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(AIDefinitionNode.PortBehaviorType));
                data.AggroRange =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(AIDefinitionNode.PortAggroRange));
                data.PatrolRadius =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(AIDefinitionNode.PortPatrolRadius));
                data.Priority = GDBGraph.GetPortValue<int>(node.GetInputPortByName(AIDefinitionNode.PortPriority));
                data.ID = i;
                runtimeAsset.AIs.Add(data);
                nodeToIdMap[node] = i;
            }

            var aistateNodes = graph.GetNodes().OfType<AIStateDefinitionNode>().ToList();
            for (int i = 0; i < aistateNodes.Count; i++)
            {
                var node = aistateNodes[i];
                var data = new AIState();
                data.CurrentTarget =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(AIStateDefinitionNode.PortCurrentTarget));
                data.LastAction =
                    GDBGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(AIStateDefinitionNode.PortLastAction));
                data.ID = i;
                runtimeAsset.AIStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var characterNodes = graph.GetNodes().OfType<CharacterDefinitionNode>().ToList();
            for (int i = 0; i < characterNodes.Count; i++)
            {
                var node = characterNodes[i];
                var data = new Character();
                data.CharacterClass =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(CharacterDefinitionNode.PortCharacterClass));
                data.Level = GDBGraph.GetPortValue<int>(node.GetInputPortByName(CharacterDefinitionNode.PortLevel));
                data.Race = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(CharacterDefinitionNode.PortRace));
                data.Gender =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(CharacterDefinitionNode.PortGender));
                data.ID = i;
                runtimeAsset.Characters.Add(data);
                nodeToIdMap[node] = i;
            }

            var characterstateNodes = graph.GetNodes().OfType<CharacterStateDefinitionNode>().ToList();
            for (int i = 0; i < characterstateNodes.Count; i++)
            {
                var node = characterstateNodes[i];
                var data = new CharacterState();
                data.Health =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(CharacterStateDefinitionNode.PortHealth));
                data.Mana = GDBGraph.GetPortValue<float>(
                    node.GetInputPortByName(CharacterStateDefinitionNode.PortMana));
                data.Stamina =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(CharacterStateDefinitionNode.PortStamina));
                data.ID = i;
                runtimeAsset.CharacterStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var combatNodes = graph.GetNodes().OfType<CombatDefinitionNode>().ToList();
            for (int i = 0; i < combatNodes.Count; i++)
            {
                var node = combatNodes[i];
                var data = new Combat();
                data.BaseDamage =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(CombatDefinitionNode.PortBaseDamage));
                data.AttackSpeed =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(CombatDefinitionNode.PortAttackSpeed));
                data.CritChance =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(CombatDefinitionNode.PortCritChance));
                data.CritMultiplier =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(CombatDefinitionNode.PortCritMultiplier));
                data.ID = i;
                runtimeAsset.Combats.Add(data);
                nodeToIdMap[node] = i;
            }

            var combatstateNodes = graph.GetNodes().OfType<CombatStateDefinitionNode>().ToList();
            for (int i = 0; i < combatstateNodes.Count; i++)
            {
                var node = combatstateNodes[i];
                var data = new CombatState();
                data.IsInCombat =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(CombatStateDefinitionNode.PortIsInCombat));
                data.LastAttack =
                    GDBGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(CombatStateDefinitionNode.PortLastAttack));
                data.ID = i;
                runtimeAsset.CombatStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var descriptionNodes = graph.GetNodes().OfType<DescriptionDefinitionNode>().ToList();
            for (int i = 0; i < descriptionNodes.Count; i++)
            {
                var node = descriptionNodes[i];
                var data = new Description();
                data.Text = GDBGraph.GetPortValue<string>(node.GetInputPortByName(DescriptionDefinitionNode.PortText));
                data.LanguageID =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(DescriptionDefinitionNode.PortLanguageID));
                data.ID = i;
                runtimeAsset.Descriptions.Add(data);
                nodeToIdMap[node] = i;
            }

            var economyNodes = graph.GetNodes().OfType<EconomyDefinitionNode>().ToList();
            for (int i = 0; i < economyNodes.Count; i++)
            {
                var node = economyNodes[i];
                var data = new Economy();
                data.BasePrice =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(EconomyDefinitionNode.PortBasePrice));
                data.Inflation =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(EconomyDefinitionNode.PortInflation));
                data.Supply = GDBGraph.GetPortValue<float>(node.GetInputPortByName(EconomyDefinitionNode.PortSupply));
                data.Demand = GDBGraph.GetPortValue<float>(node.GetInputPortByName(EconomyDefinitionNode.PortDemand));
                data.ID = i;
                runtimeAsset.Economys.Add(data);
                nodeToIdMap[node] = i;
            }

            var economystateNodes = graph.GetNodes().OfType<EconomyStateDefinitionNode>().ToList();
            for (int i = 0; i < economystateNodes.Count; i++)
            {
                var node = economystateNodes[i];
                var data = new EconomyState();
                data.CurrentPrice =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(EconomyStateDefinitionNode.PortCurrentPrice));
                data.LastUpdate =
                    GDBGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(EconomyStateDefinitionNode.PortLastUpdate));
                data.ID = i;
                runtimeAsset.EconomyStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var effectNodes = graph.GetNodes().OfType<EffectDefinitionNode>().ToList();
            for (int i = 0; i < effectNodes.Count; i++)
            {
                var node = effectNodes[i];
                var data = new Effect();
                data.EffectType =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(EffectDefinitionNode.PortEffectType));
                data.Magnitude =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(EffectDefinitionNode.PortMagnitude));
                data.DurationTicks =
                    GDBGraph.GetPortValue<long>(node.GetInputPortByName(EffectDefinitionNode.PortDurationTicks));
                data.IsStackable =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(EffectDefinitionNode.PortIsStackable));
                data.ID = i;
                runtimeAsset.Effects.Add(data);
                nodeToIdMap[node] = i;
            }

            var effectstateNodes = graph.GetNodes().OfType<EffectStateDefinitionNode>().ToList();
            for (int i = 0; i < effectstateNodes.Count; i++)
            {
                var node = effectstateNodes[i];
                var data = new EffectState();
                data.Stacks = GDBGraph.GetPortValue<int>(node.GetInputPortByName(EffectStateDefinitionNode.PortStacks));
                data.RemainingTicks =
                    GDBGraph.GetPortValue<long>(node.GetInputPortByName(EffectStateDefinitionNode.PortRemainingTicks));
                data.ID = i;
                runtimeAsset.EffectStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var equipmentNodes = graph.GetNodes().OfType<EquipmentDefinitionNode>().ToList();
            for (int i = 0; i < equipmentNodes.Count; i++)
            {
                var node = equipmentNodes[i];
                var data = new Equipment();
                data.EquipSlot =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(EquipmentDefinitionNode.PortEquipSlot));
                data.Level = GDBGraph.GetPortValue<int>(node.GetInputPortByName(EquipmentDefinitionNode.PortLevel));
                data.Durability =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(EquipmentDefinitionNode.PortDurability));
                data.MaxDurability =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(EquipmentDefinitionNode.PortMaxDurability));
                data.ID = i;
                runtimeAsset.Equipments.Add(data);
                nodeToIdMap[node] = i;
            }

            var guildNodes = graph.GetNodes().OfType<GuildDefinitionNode>().ToList();
            for (int i = 0; i < guildNodes.Count; i++)
            {
                var node = guildNodes[i];
                var data = new Guild();
                data.MaxMembers =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(GuildDefinitionNode.PortMaxMembers));
                data.Level = GDBGraph.GetPortValue<int>(node.GetInputPortByName(GuildDefinitionNode.PortLevel));
                data.Experience =
                    GDBGraph.GetPortValue<long>(node.GetInputPortByName(GuildDefinitionNode.PortExperience));
                data.CreatedAt =
                    GDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(GuildDefinitionNode.PortCreatedAt));
                data.ID = i;
                runtimeAsset.Guilds.Add(data);
                nodeToIdMap[node] = i;
            }

            var guildstateNodes = graph.GetNodes().OfType<GuildStateDefinitionNode>().ToList();
            for (int i = 0; i < guildstateNodes.Count; i++)
            {
                var node = guildstateNodes[i];
                var data = new GuildState();
                data.MemberCount =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(GuildStateDefinitionNode.PortMemberCount));
                data.Treasury =
                    GDBGraph.GetPortValue<long>(node.GetInputPortByName(GuildStateDefinitionNode.PortTreasury));
                data.ID = i;
                runtimeAsset.GuildStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var inventoryNodes = graph.GetNodes().OfType<InventoryDefinitionNode>().ToList();
            for (int i = 0; i < inventoryNodes.Count; i++)
            {
                var node = inventoryNodes[i];
                var data = new Inventory();
                data.MaxSlots =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(InventoryDefinitionNode.PortMaxSlots));
                data.MaxWeight =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(InventoryDefinitionNode.PortMaxWeight));
                data.InventoryType =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(InventoryDefinitionNode.PortInventoryType));
                data.ID = i;
                runtimeAsset.Inventorys.Add(data);
                nodeToIdMap[node] = i;
            }

            var inventorystateNodes = graph.GetNodes().OfType<InventoryStateDefinitionNode>().ToList();
            for (int i = 0; i < inventorystateNodes.Count; i++)
            {
                var node = inventorystateNodes[i];
                var data = new InventoryState();
                data.UsedSlots =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(InventoryStateDefinitionNode.PortUsedSlots));
                data.CurrentWeight =
                    GDBGraph.GetPortValue<float>(
                        node.GetInputPortByName(InventoryStateDefinitionNode.PortCurrentWeight));
                data.ID = i;
                runtimeAsset.InventoryStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var itemNodes = graph.GetNodes().OfType<ItemDefinitionNode>().ToList();
            for (int i = 0; i < itemNodes.Count; i++)
            {
                var node = itemNodes[i];
                var data = new Item();
                data.ItemType = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(ItemDefinitionNode.PortItemType));
                data.Rarity = GDBGraph.GetPortValue<int>(node.GetInputPortByName(ItemDefinitionNode.PortRarity));
                data.MaxStack = GDBGraph.GetPortValue<int>(node.GetInputPortByName(ItemDefinitionNode.PortMaxStack));
                data.Weight = GDBGraph.GetPortValue<float>(node.GetInputPortByName(ItemDefinitionNode.PortWeight));
                data.Value = GDBGraph.GetPortValue<int>(node.GetInputPortByName(ItemDefinitionNode.PortValue));
                data.ID = i;
                runtimeAsset.Items.Add(data);
                nodeToIdMap[node] = i;
            }

            var locationNodes = graph.GetNodes().OfType<LocationDefinitionNode>().ToList();
            for (int i = 0; i < locationNodes.Count; i++)
            {
                var node = locationNodes[i];
                var data = new Location();
                data.Position =
                    GDBGraph.GetPortValue<UnityEngine.Vector3>(
                        node.GetInputPortByName(LocationDefinitionNode.PortPosition));
                data.Scale =
                    GDBGraph.GetPortValue<UnityEngine.Vector3>(
                        node.GetInputPortByName(LocationDefinitionNode.PortScale));
                data.Rotation =
                    GDBGraph.GetPortValue<UnityEngine.Quaternion>(
                        node.GetInputPortByName(LocationDefinitionNode.PortRotation));
                data.ParentZoneID =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(LocationDefinitionNode.PortParentZoneID));
                data.ID = i;
                runtimeAsset.Locations.Add(data);
                nodeToIdMap[node] = i;
            }

            var missionNodes = graph.GetNodes().OfType<MissionDefinitionNode>().ToList();
            for (int i = 0; i < missionNodes.Count; i++)
            {
                var node = missionNodes[i];
                var data = new Mission();
                data.Priority = GDBGraph.GetPortValue<int>(node.GetInputPortByName(MissionDefinitionNode.PortPriority));
                data.IsRepeatable =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(MissionDefinitionNode.PortIsRepeatable));
                data.MaxAttempts =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(MissionDefinitionNode.PortMaxAttempts));
                data.ID = i;
                runtimeAsset.Missions.Add(data);
                nodeToIdMap[node] = i;
            }

            var missionstateNodes = graph.GetNodes().OfType<MissionStateDefinitionNode>().ToList();
            for (int i = 0; i < missionstateNodes.Count; i++)
            {
                var node = missionstateNodes[i];
                var data = new MissionState();
                data.Attempts =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(MissionStateDefinitionNode.PortAttempts));
                data.IsActive =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(MissionStateDefinitionNode.PortIsActive));
                data.StartTime =
                    GDBGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(MissionStateDefinitionNode.PortStartTime));
                data.ID = i;
                runtimeAsset.MissionStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var nameNodes = graph.GetNodes().OfType<NameDefinitionNode>().ToList();
            for (int i = 0; i < nameNodes.Count; i++)
            {
                var node = nameNodes[i];
                var data = new Name();
                data.Text = GDBGraph.GetPortValue<string>(node.GetInputPortByName(NameDefinitionNode.PortText));
                data.LanguageID =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(NameDefinitionNode.PortLanguageID));
                data.ID = i;
                runtimeAsset.Names.Add(data);
                nodeToIdMap[node] = i;
            }

            var objectiveNodes = graph.GetNodes().OfType<ObjectiveDefinitionNode>().ToList();
            for (int i = 0; i < objectiveNodes.Count; i++)
            {
                var node = objectiveNodes[i];
                var data = new Objective();
                data.TargetValue =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(ObjectiveDefinitionNode.PortTargetValue));
                data.ObjectiveType =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(ObjectiveDefinitionNode.PortObjectiveType));
                data.IsOptional =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(ObjectiveDefinitionNode.PortIsOptional));
                data.ID = i;
                runtimeAsset.Objectives.Add(data);
                nodeToIdMap[node] = i;
            }

            var objectivestateNodes = graph.GetNodes().OfType<ObjectiveStateDefinitionNode>().ToList();
            for (int i = 0; i < objectivestateNodes.Count; i++)
            {
                var node = objectivestateNodes[i];
                var data = new ObjectiveState();
                data.CurrentValue =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(ObjectiveStateDefinitionNode.PortCurrentValue));
                data.IsCompleted =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(ObjectiveStateDefinitionNode.PortIsCompleted));
                data.ID = i;
                runtimeAsset.ObjectiveStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var playerNodes = graph.GetNodes().OfType<PlayerDefinitionNode>().ToList();
            for (int i = 0; i < playerNodes.Count; i++)
            {
                var node = playerNodes[i];
                var data = new Player();
                data.Level = GDBGraph.GetPortValue<int>(node.GetInputPortByName(PlayerDefinitionNode.PortLevel));
                data.Experience =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(PlayerDefinitionNode.PortExperience));
                data.PrestigeLevel =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(PlayerDefinitionNode.PortPrestigeLevel));
                data.CreatedAt =
                    GDBGraph.GetPortValue<System.DateTime>(node.GetInputPortByName(PlayerDefinitionNode.PortCreatedAt));
                data.ID = i;
                runtimeAsset.Players.Add(data);
                nodeToIdMap[node] = i;
            }

            var questNodes = graph.GetNodes().OfType<QuestDefinitionNode>().ToList();
            for (int i = 0; i < questNodes.Count; i++)
            {
                var node = questNodes[i];
                var data = new Quest();
                data.ChapterID = GDBGraph.GetPortValue<int>(node.GetInputPortByName(QuestDefinitionNode.PortChapterID));
                data.Difficulty =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(QuestDefinitionNode.PortDifficulty));
                data.IsMainQuest =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(QuestDefinitionNode.PortIsMainQuest));
                data.ID = i;
                runtimeAsset.Quests.Add(data);
                nodeToIdMap[node] = i;
            }

            var queststateNodes = graph.GetNodes().OfType<QuestStateDefinitionNode>().ToList();
            for (int i = 0; i < queststateNodes.Count; i++)
            {
                var node = queststateNodes[i];
                var data = new QuestState();
                data.Progress =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(QuestStateDefinitionNode.PortProgress));
                data.IsCompleted =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(QuestStateDefinitionNode.PortIsCompleted));
                data.ID = i;
                runtimeAsset.QuestStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var rangeasfloatNodes = graph.GetNodes().OfType<RangeAsFloatDefinitionNode>().ToList();
            for (int i = 0; i < rangeasfloatNodes.Count; i++)
            {
                var node = rangeasfloatNodes[i];
                var data = new RangeAsFloat();
                data.MinValue =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(RangeAsFloatDefinitionNode.PortMinValue));
                data.MaxValue =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(RangeAsFloatDefinitionNode.PortMaxValue));
                data.Precision =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(RangeAsFloatDefinitionNode.PortPrecision));
                data.ID = i;
                runtimeAsset.RangeAsFloats.Add(data);
                nodeToIdMap[node] = i;
            }

            var rangeasintNodes = graph.GetNodes().OfType<RangeAsIntDefinitionNode>().ToList();
            for (int i = 0; i < rangeasintNodes.Count; i++)
            {
                var node = rangeasintNodes[i];
                var data = new RangeAsInt();
                data.MinValue =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(RangeAsIntDefinitionNode.PortMinValue));
                data.MaxValue =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(RangeAsIntDefinitionNode.PortMaxValue));
                data.StepSize =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(RangeAsIntDefinitionNode.PortStepSize));
                data.ID = i;
                runtimeAsset.RangeAsInts.Add(data);
                nodeToIdMap[node] = i;
            }

            var rewardNodes = graph.GetNodes().OfType<RewardDefinitionNode>().ToList();
            for (int i = 0; i < rewardNodes.Count; i++)
            {
                var node = rewardNodes[i];
                var data = new Reward();
                data.RewardType =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(RewardDefinitionNode.PortRewardType));
                data.Amount = GDBGraph.GetPortValue<int>(node.GetInputPortByName(RewardDefinitionNode.PortAmount));
                data.Chance = GDBGraph.GetPortValue<float>(node.GetInputPortByName(RewardDefinitionNode.PortChance));
                data.ItemID = GDBGraph.GetPortValue<int>(node.GetInputPortByName(RewardDefinitionNode.PortItemID));
                data.ID = i;
                runtimeAsset.Rewards.Add(data);
                nodeToIdMap[node] = i;
            }

            var skillNodes = graph.GetNodes().OfType<SkillDefinitionNode>().ToList();
            for (int i = 0; i < skillNodes.Count; i++)
            {
                var node = skillNodes[i];
                var data = new Skill();
                data.SkillType =
                    GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(SkillDefinitionNode.PortSkillType));
                data.MaxLevel = GDBGraph.GetPortValue<int>(node.GetInputPortByName(SkillDefinitionNode.PortMaxLevel));
                data.BaseCooldown =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(SkillDefinitionNode.PortBaseCooldown));
                data.ManaCost = GDBGraph.GetPortValue<int>(node.GetInputPortByName(SkillDefinitionNode.PortManaCost));
                data.ID = i;
                runtimeAsset.Skills.Add(data);
                nodeToIdMap[node] = i;
            }

            var skillstateNodes = graph.GetNodes().OfType<SkillStateDefinitionNode>().ToList();
            for (int i = 0; i < skillstateNodes.Count; i++)
            {
                var node = skillstateNodes[i];
                var data = new SkillState();
                data.Level = GDBGraph.GetPortValue<int>(node.GetInputPortByName(SkillStateDefinitionNode.PortLevel));
                data.Experience =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(SkillStateDefinitionNode.PortExperience));
                data.LastUsed =
                    GDBGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(SkillStateDefinitionNode.PortLastUsed));
                data.ID = i;
                runtimeAsset.SkillStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var statNodes = graph.GetNodes().OfType<StatDefinitionNode>().ToList();
            for (int i = 0; i < statNodes.Count; i++)
            {
                var node = statNodes[i];
                var data = new Stat();
                data.StatType = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(StatDefinitionNode.PortStatType));
                data.BaseValue =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(StatDefinitionNode.PortBaseValue));
                data.MinValue = GDBGraph.GetPortValue<float>(node.GetInputPortByName(StatDefinitionNode.PortMinValue));
                data.MaxValue = GDBGraph.GetPortValue<float>(node.GetInputPortByName(StatDefinitionNode.PortMaxValue));
                data.ID = i;
                runtimeAsset.Stats.Add(data);
                nodeToIdMap[node] = i;
            }

            var statstateNodes = graph.GetNodes().OfType<StatStateDefinitionNode>().ToList();
            for (int i = 0; i < statstateNodes.Count; i++)
            {
                var node = statstateNodes[i];
                var data = new StatState();
                data.CurrentValue =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(StatStateDefinitionNode.PortCurrentValue));
                data.Modifiers =
                    GDBGraph.GetPortValue<float>(node.GetInputPortByName(StatStateDefinitionNode.PortModifiers));
                data.ID = i;
                runtimeAsset.StatStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var tagNodes = graph.GetNodes().OfType<TagDefinitionNode>().ToList();
            for (int i = 0; i < tagNodes.Count; i++)
            {
                var node = tagNodes[i];
                var data = new Tag();
                data.TagType = GDBGraph.GetPortValue<ushort>(node.GetInputPortByName(TagDefinitionNode.PortTagType));
                data.Value = GDBGraph.GetPortValue<int>(node.GetInputPortByName(TagDefinitionNode.PortValue));
                data.ID = i;
                runtimeAsset.Tags.Add(data);
                nodeToIdMap[node] = i;
            }

            var timestateNodes = graph.GetNodes().OfType<TimeStateDefinitionNode>().ToList();
            for (int i = 0; i < timestateNodes.Count; i++)
            {
                var node = timestateNodes[i];
                var data = new TimeState();
                data.ElapsedTicks =
                    GDBGraph.GetPortValue<long>(node.GetInputPortByName(TimeStateDefinitionNode.PortElapsedTicks));
                data.IsActive =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(TimeStateDefinitionNode.PortIsActive));
                data.CycleCount =
                    GDBGraph.GetPortValue<int>(node.GetInputPortByName(TimeStateDefinitionNode.PortCycleCount));
                data.ID = i;
                runtimeAsset.TimeStates.Add(data);
                nodeToIdMap[node] = i;
            }

            var timetickNodes = graph.GetNodes().OfType<TimeTickDefinitionNode>().ToList();
            for (int i = 0; i < timetickNodes.Count; i++)
            {
                var node = timetickNodes[i];
                var data = new TimeTick();
                data.StartTicks =
                    GDBGraph.GetPortValue<long>(node.GetInputPortByName(TimeTickDefinitionNode.PortStartTicks));
                data.DurationTicks =
                    GDBGraph.GetPortValue<long>(node.GetInputPortByName(TimeTickDefinitionNode.PortDurationTicks));
                data.IsRepeating =
                    GDBGraph.GetPortValue<bool>(node.GetInputPortByName(TimeTickDefinitionNode.PortIsRepeating));
                data.IntervalTicks =
                    GDBGraph.GetPortValue<long>(node.GetInputPortByName(TimeTickDefinitionNode.PortIntervalTicks));
                data.ID = i;
                runtimeAsset.TimeTicks.Add(data);
                nodeToIdMap[node] = i;
            }

            // --- PASS 2: Create links by traversing graph connections ---
            int linkId = 0;
            foreach (var sourceNode in graph.GetNodes())
            {
                var outputPort = sourceNode.GetOutputPorts().FirstOrDefault(p => p.name == "OutputLink");
                if (outputPort == null || !outputPort.isConnected) continue;
                if (!nodeToIdMap.TryGetValue(sourceNode, out int sourceId)) continue;

                var sourceDataType = GetNodeType(sourceNode);
                if (sourceDataType == null) continue;

                var connectedPorts = new List<IPort>();
                outputPort.GetConnectedPorts(connectedPorts);
                foreach (var connectedPort in connectedPorts.Where(p => p.name == "InputLink"))
                {
                    var targetNode = connectedPort.GetNode();
                    if (!nodeToIdMap.TryGetValue(targetNode, out int targetId)) continue;

                    var targetDataType = GetNodeType(targetNode);
                    if (targetDataType == null) continue;

                    var link = new Link
                    {
                        ID = linkId++, SourceType = (EntityType)Enum.Parse(typeof(EntityType), sourceDataType.Name),
                        SourceID = sourceId,
                        TargetType = (EntityType)Enum.Parse(typeof(EntityType), targetDataType.Name),
                        TargetID = targetId, LinkTypeID = 0
                    };
                    runtimeAsset.Links.Add(link);
                }
            }

            ctx.AddObjectToAsset("GDB", runtimeAsset);
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