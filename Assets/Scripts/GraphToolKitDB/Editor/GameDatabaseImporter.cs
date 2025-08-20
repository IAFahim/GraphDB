using System;
using System.Collections.Generic;
using System.Linq;
using GraphToolKitDB.Runtime;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using RangeInt = GraphToolKitDB.Runtime.RangeInt;
using Time = GraphToolKitDB.Runtime.Time;

namespace GraphToolKitDB.Editor
{
    [ScriptedImporter(1, "gdb")]
    internal class GameDatabaseImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            Debug.Log("WTF");
            var graph = GraphDatabase.LoadGraphForImporter<GameDatabaseGraph>(ctx.assetPath);
            if (graph == null) return;

            var runtimeAsset = ScriptableObject.CreateInstance<GameDatabaseAsset>();
            var nodeToIdMap = new Dictionary<INode, int>();
            var typeToIdMap = new Dictionary<Type, ushort>();
            int nextId = 1;

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
            typeToIdMap[typeof(Location)] = 20;
            typeToIdMap[typeof(Mission)] = 21;
            typeToIdMap[typeof(MissionState)] = 22;
            typeToIdMap[typeof(Name)] = 23;
            typeToIdMap[typeof(Objective)] = 24;
            typeToIdMap[typeof(ObjectiveState)] = 25;
            typeToIdMap[typeof(Player)] = 26;
            typeToIdMap[typeof(IPrimaryKey)] = 27;
            typeToIdMap[typeof(Quest)] = 28;
            typeToIdMap[typeof(QuestState)] = 29;
            typeToIdMap[typeof(RangeFloat)] = 30;
            typeToIdMap[typeof(RangeInt)] = 31;
            typeToIdMap[typeof(Reward)] = 32;
            typeToIdMap[typeof(Skill)] = 33;
            typeToIdMap[typeof(SkillState)] = 34;
            typeToIdMap[typeof(Stat)] = 35;
            typeToIdMap[typeof(StatState)] = 36;
            typeToIdMap[typeof(Tag)] = 37;
            typeToIdMap[typeof(Time)] = 38;
            typeToIdMap[typeof(TimeState)] = 39;

            // --- PASS 1: Create all data entries and map nodes to a new, unique ID ---
            var achievementsNodes = graph.GetNodes().OfType<AchievementDefinitionNode>();
            foreach (var node in achievementsNodes)
            {
                var data = new Achievement();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(AchievementDefinitionNode.PortId));
                data.AchievementType =
                    GameDatabaseGraph.GetPortValue<ushort>(
                        node.GetInputPortByName(AchievementDefinitionNode.PortAchievementType));
                data.Points =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(AchievementDefinitionNode.PortPoints));
                data.IsHidden =
                    GameDatabaseGraph.GetPortValue<bool>(node.GetInputPortByName(AchievementDefinitionNode.PortIsHidden));
                data.UnlockDate =
                    GameDatabaseGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(AchievementDefinitionNode.PortUnlockDate));
                data.ID = nextId;
                runtimeAsset.Achievements.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var achievementStatesNodes = graph.GetNodes().OfType<AchievementStateDefinitionNode>();
            foreach (var node in achievementStatesNodes)
            {
                var data = new AchievementState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(
                    node.GetInputPortByName(AchievementStateDefinitionNode.PortId));
                data.Progress =
                    GameDatabaseGraph.GetPortValue<float>(
                        node.GetInputPortByName(AchievementStateDefinitionNode.PortProgress));
                data.IsUnlocked =
                    GameDatabaseGraph.GetPortValue<bool>(
                        node.GetInputPortByName(AchievementStateDefinitionNode.PortIsUnlocked));
                data.ID = nextId;
                runtimeAsset.AchievementStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var aIsNodes = graph.GetNodes().OfType<AIDefinitionNode>();
            foreach (var node in aIsNodes)
            {
                var data = new AI();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(AIDefinitionNode.PortId));
                data.BehaviorType =
                    GameDatabaseGraph.GetPortValue<ushort>(node.GetInputPortByName(AIDefinitionNode.PortBehaviorType));
                data.AggroRange =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(AIDefinitionNode.PortAggroRange));
                data.PatrolRadius =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(AIDefinitionNode.PortPatrolRadius));
                data.Priority = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(AIDefinitionNode.PortPriority));
                data.ID = nextId;
                runtimeAsset.AIs.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var aIStatesNodes = graph.GetNodes().OfType<AIStateDefinitionNode>();
            foreach (var node in aIStatesNodes)
            {
                var data = new AIState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(AIStateDefinitionNode.PortId));
                data.CurrentTarget =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(AIStateDefinitionNode.PortCurrentTarget));
                data.LastAction =
                    GameDatabaseGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(AIStateDefinitionNode.PortLastAction));
                data.ID = nextId;
                runtimeAsset.AIStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var charactersNodes = graph.GetNodes().OfType<CharacterDefinitionNode>();
            foreach (var node in charactersNodes)
            {
                var data = new Character();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(CharacterDefinitionNode.PortId));
                data.CharacterClass =
                    GameDatabaseGraph.GetPortValue<ushort>(
                        node.GetInputPortByName(CharacterDefinitionNode.PortCharacterClass));
                data.Level =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(CharacterDefinitionNode.PortLevel));
                data.Race = GameDatabaseGraph.GetPortValue<ushort>(
                    node.GetInputPortByName(CharacterDefinitionNode.PortRace));
                data.Gender =
                    GameDatabaseGraph.GetPortValue<ushort>(node.GetInputPortByName(CharacterDefinitionNode.PortGender));
                data.ID = nextId;
                runtimeAsset.Characters.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var characterStatesNodes = graph.GetNodes().OfType<CharacterStateDefinitionNode>();
            foreach (var node in characterStatesNodes)
            {
                var data = new CharacterState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(CharacterStateDefinitionNode.PortId));
                data.Health =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(CharacterStateDefinitionNode.PortHealth));
                data.Mana = GameDatabaseGraph.GetPortValue<float>(
                    node.GetInputPortByName(CharacterStateDefinitionNode.PortMana));
                data.Stamina =
                    GameDatabaseGraph.GetPortValue<float>(
                        node.GetInputPortByName(CharacterStateDefinitionNode.PortStamina));
                data.ID = nextId;
                runtimeAsset.CharacterStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var combatsNodes = graph.GetNodes().OfType<CombatDefinitionNode>();
            foreach (var node in combatsNodes)
            {
                var data = new Combat();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(CombatDefinitionNode.PortId));
                data.BaseDamage =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(CombatDefinitionNode.PortBaseDamage));
                data.AttackSpeed =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(CombatDefinitionNode.PortAttackSpeed));
                data.CritChance =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(CombatDefinitionNode.PortCritChance));
                data.CritMultiplier =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(CombatDefinitionNode.PortCritMultiplier));
                data.ID = nextId;
                runtimeAsset.Combats.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var combatStatesNodes = graph.GetNodes().OfType<CombatStateDefinitionNode>();
            foreach (var node in combatStatesNodes)
            {
                var data = new CombatState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(CombatStateDefinitionNode.PortId));
                data.IsInCombat =
                    GameDatabaseGraph.GetPortValue<bool>(node.GetInputPortByName(CombatStateDefinitionNode.PortIsInCombat));
                data.LastAttack =
                    GameDatabaseGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(CombatStateDefinitionNode.PortLastAttack));
                data.ID = nextId;
                runtimeAsset.CombatStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var descriptionsNodes = graph.GetNodes().OfType<DescriptionDefinitionNode>();
            foreach (var node in descriptionsNodes)
            {
                var data = new Description();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(DescriptionDefinitionNode.PortId));
                data.Text = GameDatabaseGraph.GetPortValue<string>(
                    node.GetInputPortByName(DescriptionDefinitionNode.PortText));
                data.LanguageID =
                    GameDatabaseGraph.GetPortValue<ushort>(
                        node.GetInputPortByName(DescriptionDefinitionNode.PortLanguageID));
                data.ID = nextId;
                runtimeAsset.Descriptions.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var economysNodes = graph.GetNodes().OfType<EconomyDefinitionNode>();
            foreach (var node in economysNodes)
            {
                var data = new Economy();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(EconomyDefinitionNode.PortId));
                data.BasePrice =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(EconomyDefinitionNode.PortBasePrice));
                data.Inflation =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(EconomyDefinitionNode.PortInflation));
                data.Supply =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(EconomyDefinitionNode.PortSupply));
                data.Demand =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(EconomyDefinitionNode.PortDemand));
                data.ID = nextId;
                runtimeAsset.Economys.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var economyStatesNodes = graph.GetNodes().OfType<EconomyStateDefinitionNode>();
            foreach (var node in economyStatesNodes)
            {
                var data = new EconomyState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(EconomyStateDefinitionNode.PortId));
                data.CurrentPrice =
                    GameDatabaseGraph.GetPortValue<int>(
                        node.GetInputPortByName(EconomyStateDefinitionNode.PortCurrentPrice));
                data.LastUpdate =
                    GameDatabaseGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(EconomyStateDefinitionNode.PortLastUpdate));
                data.ID = nextId;
                runtimeAsset.EconomyStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var effectsNodes = graph.GetNodes().OfType<EffectDefinitionNode>();
            foreach (var node in effectsNodes)
            {
                var data = new Effect();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(EffectDefinitionNode.PortId));
                data.EffectType =
                    GameDatabaseGraph.GetPortValue<ushort>(node.GetInputPortByName(EffectDefinitionNode.PortEffectType));
                data.Magnitude =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(EffectDefinitionNode.PortMagnitude));
                data.DurationTicks =
                    GameDatabaseGraph.GetPortValue<long>(node.GetInputPortByName(EffectDefinitionNode.PortDurationTicks));
                data.IsStackable =
                    GameDatabaseGraph.GetPortValue<bool>(node.GetInputPortByName(EffectDefinitionNode.PortIsStackable));
                data.ID = nextId;
                runtimeAsset.Effects.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var effectStatesNodes = graph.GetNodes().OfType<EffectStateDefinitionNode>();
            foreach (var node in effectStatesNodes)
            {
                var data = new EffectState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(EffectStateDefinitionNode.PortId));
                data.Stacks =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(EffectStateDefinitionNode.PortStacks));
                data.RemainingTicks =
                    GameDatabaseGraph.GetPortValue<long>(
                        node.GetInputPortByName(EffectStateDefinitionNode.PortRemainingTicks));
                data.ID = nextId;
                runtimeAsset.EffectStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var equipmentsNodes = graph.GetNodes().OfType<EquipmentDefinitionNode>();
            foreach (var node in equipmentsNodes)
            {
                var data = new Equipment();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(EquipmentDefinitionNode.PortId));
                data.EquipSlot =
                    GameDatabaseGraph.GetPortValue<ushort>(node.GetInputPortByName(EquipmentDefinitionNode.PortEquipSlot));
                data.Level =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(EquipmentDefinitionNode.PortLevel));
                data.Durability =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(EquipmentDefinitionNode.PortDurability));
                data.MaxDurability =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(EquipmentDefinitionNode.PortMaxDurability));
                data.ID = nextId;
                runtimeAsset.Equipments.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var guildsNodes = graph.GetNodes().OfType<GuildDefinitionNode>();
            foreach (var node in guildsNodes)
            {
                var data = new Guild();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(GuildDefinitionNode.PortId));
                data.MaxMembers =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(GuildDefinitionNode.PortMaxMembers));
                data.Level = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(GuildDefinitionNode.PortLevel));
                data.Experience =
                    GameDatabaseGraph.GetPortValue<long>(node.GetInputPortByName(GuildDefinitionNode.PortExperience));
                data.CreatedAt =
                    GameDatabaseGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(GuildDefinitionNode.PortCreatedAt));
                data.ID = nextId;
                runtimeAsset.Guilds.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var guildStatesNodes = graph.GetNodes().OfType<GuildStateDefinitionNode>();
            foreach (var node in guildStatesNodes)
            {
                var data = new GuildState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(GuildStateDefinitionNode.PortId));
                data.MemberCount =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(GuildStateDefinitionNode.PortMemberCount));
                data.Treasury =
                    GameDatabaseGraph.GetPortValue<long>(node.GetInputPortByName(GuildStateDefinitionNode.PortTreasury));
                data.ID = nextId;
                runtimeAsset.GuildStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var inventorysNodes = graph.GetNodes().OfType<InventoryDefinitionNode>();
            foreach (var node in inventorysNodes)
            {
                var data = new Inventory();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(InventoryDefinitionNode.PortId));
                data.MaxSlots =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(InventoryDefinitionNode.PortMaxSlots));
                data.MaxWeight =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(InventoryDefinitionNode.PortMaxWeight));
                data.InventoryType =
                    GameDatabaseGraph.GetPortValue<ushort>(
                        node.GetInputPortByName(InventoryDefinitionNode.PortInventoryType));
                data.ID = nextId;
                runtimeAsset.Inventorys.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var inventoryStatesNodes = graph.GetNodes().OfType<InventoryStateDefinitionNode>();
            foreach (var node in inventoryStatesNodes)
            {
                var data = new InventoryState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(InventoryStateDefinitionNode.PortId));
                data.UsedSlots =
                    GameDatabaseGraph.GetPortValue<int>(
                        node.GetInputPortByName(InventoryStateDefinitionNode.PortUsedSlots));
                data.CurrentWeight =
                    GameDatabaseGraph.GetPortValue<float>(
                        node.GetInputPortByName(InventoryStateDefinitionNode.PortCurrentWeight));
                data.ID = nextId;
                runtimeAsset.InventoryStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var itemsNodes = graph.GetNodes().OfType<ItemDefinitionNode>();
            foreach (var node in itemsNodes)
            {
                var data = new Item();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(ItemDefinitionNode.PortId));
                data.ItemType =
                    GameDatabaseGraph.GetPortValue<ushort>(node.GetInputPortByName(ItemDefinitionNode.PortItemType));
                data.Rarity = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(ItemDefinitionNode.PortRarity));
                data.MaxStack =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(ItemDefinitionNode.PortMaxStack));
                data.Weight = GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(ItemDefinitionNode.PortWeight));
                data.Value = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(ItemDefinitionNode.PortValue));
                data.ID = nextId;
                runtimeAsset.Items.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var locationsNodes = graph.GetNodes().OfType<LocationDefinitionNode>();
            foreach (var node in locationsNodes)
            {
                var data = new Location();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(LocationDefinitionNode.PortId));
                data.Position =
                    GameDatabaseGraph.GetPortValue<UnityEngine.Vector3>(
                        node.GetInputPortByName(LocationDefinitionNode.PortPosition));
                data.Scale =
                    GameDatabaseGraph.GetPortValue<UnityEngine.Vector3>(
                        node.GetInputPortByName(LocationDefinitionNode.PortScale));
                data.Rotation =
                    GameDatabaseGraph.GetPortValue<UnityEngine.Quaternion>(
                        node.GetInputPortByName(LocationDefinitionNode.PortRotation));
                data.ParentZoneID =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(LocationDefinitionNode.PortParentZoneID));
                data.ID = nextId;
                runtimeAsset.Locations.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var missionsNodes = graph.GetNodes().OfType<MissionDefinitionNode>();
            foreach (var node in missionsNodes)
            {
                var data = new Mission();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(MissionDefinitionNode.PortId));
                data.Priority =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(MissionDefinitionNode.PortPriority));
                data.IsRepeatable =
                    GameDatabaseGraph.GetPortValue<bool>(node.GetInputPortByName(MissionDefinitionNode.PortIsRepeatable));
                data.MaxAttempts =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(MissionDefinitionNode.PortMaxAttempts));
                data.ID = nextId;
                runtimeAsset.Missions.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var missionStatesNodes = graph.GetNodes().OfType<MissionStateDefinitionNode>();
            foreach (var node in missionStatesNodes)
            {
                var data = new MissionState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(MissionStateDefinitionNode.PortId));
                data.Attempts =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(MissionStateDefinitionNode.PortAttempts));
                data.IsActive =
                    GameDatabaseGraph.GetPortValue<bool>(node.GetInputPortByName(MissionStateDefinitionNode.PortIsActive));
                data.StartTime =
                    GameDatabaseGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(MissionStateDefinitionNode.PortStartTime));
                data.ID = nextId;
                runtimeAsset.MissionStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var namesNodes = graph.GetNodes().OfType<NameDefinitionNode>();
            foreach (var node in namesNodes)
            {
                var data = new Name();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(NameDefinitionNode.PortId));
                data.Text = GameDatabaseGraph.GetPortValue<string>(node.GetInputPortByName(NameDefinitionNode.PortText));
                data.LanguageID =
                    GameDatabaseGraph.GetPortValue<ushort>(node.GetInputPortByName(NameDefinitionNode.PortLanguageID));
                data.ID = nextId;
                runtimeAsset.Names.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var objectivesNodes = graph.GetNodes().OfType<ObjectiveDefinitionNode>();
            foreach (var node in objectivesNodes)
            {
                var data = new Objective();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(ObjectiveDefinitionNode.PortId));
                data.TargetValue =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(ObjectiveDefinitionNode.PortTargetValue));
                data.ObjectiveType =
                    GameDatabaseGraph.GetPortValue<ushort>(
                        node.GetInputPortByName(ObjectiveDefinitionNode.PortObjectiveType));
                data.IsOptional =
                    GameDatabaseGraph.GetPortValue<bool>(node.GetInputPortByName(ObjectiveDefinitionNode.PortIsOptional));
                data.ID = nextId;
                runtimeAsset.Objectives.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var objectiveStatesNodes = graph.GetNodes().OfType<ObjectiveStateDefinitionNode>();
            foreach (var node in objectiveStatesNodes)
            {
                var data = new ObjectiveState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(ObjectiveStateDefinitionNode.PortId));
                data.CurrentValue =
                    GameDatabaseGraph.GetPortValue<int>(
                        node.GetInputPortByName(ObjectiveStateDefinitionNode.PortCurrentValue));
                data.IsCompleted =
                    GameDatabaseGraph.GetPortValue<bool>(
                        node.GetInputPortByName(ObjectiveStateDefinitionNode.PortIsCompleted));
                data.ID = nextId;
                runtimeAsset.ObjectiveStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var playersNodes = graph.GetNodes().OfType<PlayerDefinitionNode>();
            foreach (var node in playersNodes)
            {
                var data = new Player();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(PlayerDefinitionNode.PortId));
                data.Level = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(PlayerDefinitionNode.PortLevel));
                data.Experience =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(PlayerDefinitionNode.PortExperience));
                data.PrestigeLevel =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(PlayerDefinitionNode.PortPrestigeLevel));
                data.CreatedAt =
                    GameDatabaseGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(PlayerDefinitionNode.PortCreatedAt));
                data.ID = nextId;
                runtimeAsset.Players.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var questsNodes = graph.GetNodes().OfType<QuestDefinitionNode>();
            foreach (var node in questsNodes)
            {
                var data = new Quest();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(QuestDefinitionNode.PortId));
                data.ChapterID =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(QuestDefinitionNode.PortChapterID));
                data.Difficulty =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(QuestDefinitionNode.PortDifficulty));
                data.IsMainQuest =
                    GameDatabaseGraph.GetPortValue<bool>(node.GetInputPortByName(QuestDefinitionNode.PortIsMainQuest));
                data.ID = nextId;
                runtimeAsset.Quests.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var questStatesNodes = graph.GetNodes().OfType<QuestStateDefinitionNode>();
            foreach (var node in questStatesNodes)
            {
                var data = new QuestState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(QuestStateDefinitionNode.PortId));
                data.Progress =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(QuestStateDefinitionNode.PortProgress));
                data.IsCompleted =
                    GameDatabaseGraph.GetPortValue<bool>(node.GetInputPortByName(QuestStateDefinitionNode.PortIsCompleted));
                data.ID = nextId;
                runtimeAsset.QuestStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var rangeFloatsNodes = graph.GetNodes().OfType<RangeFloatDefinitionNode>();
            foreach (var node in rangeFloatsNodes)
            {
                var data = new RangeFloat();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(RangeFloatDefinitionNode.PortId));
                data.MinValue =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(RangeFloatDefinitionNode.PortMinValue));
                data.MaxValue =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(RangeFloatDefinitionNode.PortMaxValue));
                data.Precision =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(RangeFloatDefinitionNode.PortPrecision));
                data.ID = nextId;
                runtimeAsset.RangeFloats.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var rangeIntsNodes = graph.GetNodes().OfType<RangeIntDefinitionNode>();
            foreach (var node in rangeIntsNodes)
            {
                var data = new RangeInt();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(RangeIntDefinitionNode.PortId));
                data.MinValue =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(RangeIntDefinitionNode.PortMinValue));
                data.MaxValue =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(RangeIntDefinitionNode.PortMaxValue));
                data.StepSize =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(RangeIntDefinitionNode.PortStepSize));
                data.ID = nextId;
                runtimeAsset.RangeInts.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var rewardsNodes = graph.GetNodes().OfType<RewardDefinitionNode>();
            foreach (var node in rewardsNodes)
            {
                var data = new Reward();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(RewardDefinitionNode.PortId));
                data.RewardType =
                    GameDatabaseGraph.GetPortValue<ushort>(node.GetInputPortByName(RewardDefinitionNode.PortRewardType));
                data.Amount = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(RewardDefinitionNode.PortAmount));
                data.Chance =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(RewardDefinitionNode.PortChance));
                data.ItemID = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(RewardDefinitionNode.PortItemID));
                data.ID = nextId;
                runtimeAsset.Rewards.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var skillsNodes = graph.GetNodes().OfType<SkillDefinitionNode>();
            foreach (var node in skillsNodes)
            {
                var data = new Skill();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(SkillDefinitionNode.PortId));
                data.SkillType =
                    GameDatabaseGraph.GetPortValue<ushort>(node.GetInputPortByName(SkillDefinitionNode.PortSkillType));
                data.MaxLevel =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(SkillDefinitionNode.PortMaxLevel));
                data.BaseCooldown =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(SkillDefinitionNode.PortBaseCooldown));
                data.ManaCost =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(SkillDefinitionNode.PortManaCost));
                data.ID = nextId;
                runtimeAsset.Skills.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var skillStatesNodes = graph.GetNodes().OfType<SkillStateDefinitionNode>();
            foreach (var node in skillStatesNodes)
            {
                var data = new SkillState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(SkillStateDefinitionNode.PortId));
                data.Level =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(SkillStateDefinitionNode.PortLevel));
                data.Experience =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(SkillStateDefinitionNode.PortExperience));
                data.LastUsed =
                    GameDatabaseGraph.GetPortValue<System.DateTime>(
                        node.GetInputPortByName(SkillStateDefinitionNode.PortLastUsed));
                data.ID = nextId;
                runtimeAsset.SkillStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var statsNodes = graph.GetNodes().OfType<StatDefinitionNode>();
            foreach (var node in statsNodes)
            {
                var data = new Stat();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(StatDefinitionNode.PortId));
                data.StatType =
                    GameDatabaseGraph.GetPortValue<ushort>(node.GetInputPortByName(StatDefinitionNode.PortStatType));
                data.BaseValue =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(StatDefinitionNode.PortBaseValue));
                data.MinValue =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(StatDefinitionNode.PortMinValue));
                data.MaxValue =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(StatDefinitionNode.PortMaxValue));
                data.ID = nextId;
                runtimeAsset.Stats.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var statStatesNodes = graph.GetNodes().OfType<StatStateDefinitionNode>();
            foreach (var node in statStatesNodes)
            {
                var data = new StatState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(StatStateDefinitionNode.PortId));
                data.CurrentValue =
                    GameDatabaseGraph.GetPortValue<float>(
                        node.GetInputPortByName(StatStateDefinitionNode.PortCurrentValue));
                data.Modifiers =
                    GameDatabaseGraph.GetPortValue<float>(node.GetInputPortByName(StatStateDefinitionNode.PortModifiers));
                data.ID = nextId;
                runtimeAsset.StatStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var tagsNodes = graph.GetNodes().OfType<TagDefinitionNode>();
            foreach (var node in tagsNodes)
            {
                var data = new Tag();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(TagDefinitionNode.PortId));
                data.TagType =
                    GameDatabaseGraph.GetPortValue<ushort>(node.GetInputPortByName(TagDefinitionNode.PortTagType));
                data.Value = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(TagDefinitionNode.PortValue));
                data.ID = nextId;
                runtimeAsset.Tags.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var timesNodes = graph.GetNodes().OfType<TimeDefinitionNode>();
            foreach (var node in timesNodes)
            {
                var data = new Time();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(TimeDefinitionNode.PortId));
                data.StartTicks =
                    GameDatabaseGraph.GetPortValue<long>(node.GetInputPortByName(TimeDefinitionNode.PortStartTicks));
                data.DurationTicks =
                    GameDatabaseGraph.GetPortValue<long>(node.GetInputPortByName(TimeDefinitionNode.PortDurationTicks));
                data.IsRepeating =
                    GameDatabaseGraph.GetPortValue<bool>(node.GetInputPortByName(TimeDefinitionNode.PortIsRepeating));
                data.IntervalTicks =
                    GameDatabaseGraph.GetPortValue<long>(node.GetInputPortByName(TimeDefinitionNode.PortIntervalTicks));
                data.ID = nextId;
                runtimeAsset.Times.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
            }

            var timeStatesNodes = graph.GetNodes().OfType<TimeStateDefinitionNode>();
            foreach (var node in timeStatesNodes)
            {
                var data = new TimeState();
                data.Id = GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(TimeStateDefinitionNode.PortId));
                data.ElapsedTicks =
                    GameDatabaseGraph.GetPortValue<long>(node.GetInputPortByName(TimeStateDefinitionNode.PortElapsedTicks));
                data.IsActive =
                    GameDatabaseGraph.GetPortValue<bool>(node.GetInputPortByName(TimeStateDefinitionNode.PortIsActive));
                data.CycleCount =
                    GameDatabaseGraph.GetPortValue<int>(node.GetInputPortByName(TimeStateDefinitionNode.PortCycleCount));
                data.ID = nextId;
                runtimeAsset.TimeStates.Add(data);
                nodeToIdMap[node] = nextId;
                nextId++;
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

                    Link link = new Link()
                    {
                        SourceType = sourceTypeId, SourceID = sourceId, TargetType = targetTypeId, TargetID = targetId,
                        LinkTypeID = 0
                    };
                    runtimeAsset.Links.Add(link);
                }
            }

            // ctx.AddObjectToAsset("GameDatabaseGraph", runtimeAsset);
            // ctx.SetMainObject(runtimeAsset);
            string path = "Assets/MyData.asset";

            // Save the asset to the Assets folder
            AssetDatabase.CreateAsset(runtimeAsset, path);
            // Refresh the AssetDatabase to show the new a

            // Optional: Select the newly created asset
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = runtimeAsset;
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
            if (node is RangeFloatDefinitionNode) return typeof(RangeFloat);
            if (node is RangeIntDefinitionNode) return typeof(RangeInt);
            if (node is RewardDefinitionNode) return typeof(Reward);
            if (node is SkillDefinitionNode) return typeof(Skill);
            if (node is SkillStateDefinitionNode) return typeof(SkillState);
            if (node is StatDefinitionNode) return typeof(Stat);
            if (node is StatStateDefinitionNode) return typeof(StatState);
            if (node is TagDefinitionNode) return typeof(Tag);
            if (node is TimeDefinitionNode) return typeof(Time);
            if (node is TimeStateDefinitionNode) return typeof(TimeState);
            return null;
        }
    }
}