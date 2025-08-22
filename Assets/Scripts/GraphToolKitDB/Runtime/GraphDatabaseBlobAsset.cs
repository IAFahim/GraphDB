// ----- AUTO-GENERATED ECS/DOTS BLOB FILE BY GraphToolGenerator.cs -----

using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using GraphTookKitDB.Runtime;
using GraphToolKitDB.Runtime;
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using GraphTookKitDB.Runtime;

namespace GraphTookKitDB.Runtime.ECS
{
    // Blob asset for ultra-fast runtime queries
    public struct GDBBlobAsset
    {
        public BlobArray<Achievement> Achievements;
        public BlobArray<AchievementState> AchievementStates;
        public BlobArray<AI> AIs;
        public BlobArray<AIState> AIStates;
        public BlobArray<Character> Characters;
        public BlobArray<CharacterState> CharacterStates;
        public BlobArray<Combat> Combats;
        public BlobArray<CombatState> CombatStates;
        public BlobArray<Description> Descriptions;
        public BlobArray<Economy> Economys;
        public BlobArray<EconomyState> EconomyStates;
        public BlobArray<Effect> Effects;
        public BlobArray<EffectState> EffectStates;
        public BlobArray<Equipment> Equipments;
        public BlobArray<Guild> Guilds;
        public BlobArray<GuildState> GuildStates;
        public BlobArray<Inventory> Inventorys;
        public BlobArray<InventoryState> InventoryStates;
        public BlobArray<Item> Items;
        public BlobArray<Location> Locations;
        public BlobArray<Mission> Missions;
        public BlobArray<MissionState> MissionStates;
        public BlobArray<Name> Names;
        public BlobArray<Objective> Objectives;
        public BlobArray<ObjectiveState> ObjectiveStates;
        public BlobArray<Player> Players;
        public BlobArray<Quest> Quests;
        public BlobArray<QuestState> QuestStates;
        public BlobArray<RangeAsFloat> RangeAsFloats;
        public BlobArray<RangeAsInt> RangeAsInts;
        public BlobArray<Reward> Rewards;
        public BlobArray<Skill> Skills;
        public BlobArray<SkillState> SkillStates;
        public BlobArray<Stat> Stats;
        public BlobArray<StatState> StatStates;
        public BlobArray<Tag> Tags;
        public BlobArray<TimeState> TimeStates;
        public BlobArray<TimeTick> TimeTicks;
        public BlobArray<Link> Links;
        public BlobArray<int> OutgoingIndices;
        public BlobArray<LinkEdge> OutgoingEdges;
        public BlobArray<int> IncomingIndices;
        public BlobArray<LinkEdge> IncomingEdges;
        public int MaxEntityId;
    }

    // Edge data for CSR graph traversal
    public struct LinkEdge
    {
        public int TargetId;
        public EntityType TargetType;
    }

    public struct GDBComponent : IComponentData
    {
        public BlobAssetReference<GDBBlobAsset> Blob;
    }

    public static class GDBBlobBuilder
    {
        public static BlobAssetReference<GDBBlobAsset> CreateBlobAsset(this GDBAsset sourceAsset)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<GDBBlobAsset>();

            CopyArray(builder, ref root.Achievements, sourceAsset.Achievements);
            CopyArray(builder, ref root.AchievementStates, sourceAsset.AchievementStates);
            CopyArray(builder, ref root.AIs, sourceAsset.AIs);
            CopyArray(builder, ref root.AIStates, sourceAsset.AIStates);
            CopyArray(builder, ref root.Characters, sourceAsset.Characters);
            CopyArray(builder, ref root.CharacterStates, sourceAsset.CharacterStates);
            CopyArray(builder, ref root.Combats, sourceAsset.Combats);
            CopyArray(builder, ref root.CombatStates, sourceAsset.CombatStates);
            CopyArray(builder, ref root.Descriptions, sourceAsset.Descriptions);
            CopyArray(builder, ref root.Economys, sourceAsset.Economys);
            CopyArray(builder, ref root.EconomyStates, sourceAsset.EconomyStates);
            CopyArray(builder, ref root.Effects, sourceAsset.Effects);
            CopyArray(builder, ref root.EffectStates, sourceAsset.EffectStates);
            CopyArray(builder, ref root.Equipments, sourceAsset.Equipments);
            CopyArray(builder, ref root.Guilds, sourceAsset.Guilds);
            CopyArray(builder, ref root.GuildStates, sourceAsset.GuildStates);
            CopyArray(builder, ref root.Inventorys, sourceAsset.Inventorys);
            CopyArray(builder, ref root.InventoryStates, sourceAsset.InventoryStates);
            CopyArray(builder, ref root.Items, sourceAsset.Items);
            CopyArray(builder, ref root.Locations, sourceAsset.Locations);
            CopyArray(builder, ref root.Missions, sourceAsset.Missions);
            CopyArray(builder, ref root.MissionStates, sourceAsset.MissionStates);
            CopyArray(builder, ref root.Names, sourceAsset.Names);
            CopyArray(builder, ref root.Objectives, sourceAsset.Objectives);
            CopyArray(builder, ref root.ObjectiveStates, sourceAsset.ObjectiveStates);
            CopyArray(builder, ref root.Players, sourceAsset.Players);
            CopyArray(builder, ref root.Quests, sourceAsset.Quests);
            CopyArray(builder, ref root.QuestStates, sourceAsset.QuestStates);
            CopyArray(builder, ref root.RangeAsFloats, sourceAsset.RangeAsFloats);
            CopyArray(builder, ref root.RangeAsInts, sourceAsset.RangeAsInts);
            CopyArray(builder, ref root.Rewards, sourceAsset.Rewards);
            CopyArray(builder, ref root.Skills, sourceAsset.Skills);
            CopyArray(builder, ref root.SkillStates, sourceAsset.SkillStates);
            CopyArray(builder, ref root.Stats, sourceAsset.Stats);
            CopyArray(builder, ref root.StatStates, sourceAsset.StatStates);
            CopyArray(builder, ref root.Tags, sourceAsset.Tags);
            CopyArray(builder, ref root.TimeStates, sourceAsset.TimeStates);
            CopyArray(builder, ref root.TimeTicks, sourceAsset.TimeTicks);
            CopyArray(builder, ref root.Links, sourceAsset.Links);
            BuildCsrLinkSystem(builder, ref root, sourceAsset.Links);
            root.MaxEntityId = CalculateMaxEntityId(sourceAsset);

            return builder.CreateBlobAssetReference<GDBBlobAsset>(Allocator.Persistent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyArray<T>(BlobBuilder builder, ref BlobArray<T> target, System.Collections.Generic.List<T> source) where T : struct
        {
            var array = builder.Allocate(ref target, source != null ? source.Count : 0);
            if (source == null || source.Count == 0) return;
            for (int i = 0; i < source.Count; i++) array[i] = source[i];
        }

        private static void BuildCsrLinkSystem(BlobBuilder builder, ref GDBBlobAsset root, System.Collections.Generic.List<Link> links)
        {
            if (links == null || links.Count == 0)
            {
                builder.Allocate(ref root.OutgoingIndices, 1)[0] = 0;
                builder.Allocate(ref root.OutgoingEdges, 0);
                builder.Allocate(ref root.IncomingIndices, 1)[0] = 0;
                builder.Allocate(ref root.IncomingEdges, 0);
                return;
            }

            int maxId = CalculateMaxEntityIdFromLinks(links);
            var outgoingGroups = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<LinkEdge>>();
            foreach (var link in links)
            {
                if (!outgoingGroups.TryGetValue(link.SourceID, out var list))
                    outgoingGroups[link.SourceID] = list = new System.Collections.Generic.List<LinkEdge>();
                list.Add(new LinkEdge { TargetId = link.TargetID, TargetType = link.TargetType });
            }

            var outIndices = builder.Allocate(ref root.OutgoingIndices, maxId + 2);
            var outEdgesList = new System.Collections.Generic.List<LinkEdge>();
            outIndices[0] = 0;
            for (int i = 0; i <= maxId; i++)
            {
                if (outgoingGroups.TryGetValue(i, out var edges)) outEdgesList.AddRange(edges);
                outIndices[i + 1] = outEdgesList.Count;
            }
            var outEdges = builder.Allocate(ref root.OutgoingEdges, outEdgesList.Count);
            for (int i = 0; i < outEdgesList.Count; i++) outEdges[i] = outEdgesList[i];

            var incomingGroups = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<LinkEdge>>();
            foreach (var link in links)
            {
                if (!incomingGroups.TryGetValue(link.TargetID, out var list))
                    incomingGroups[link.TargetID] = list = new System.Collections.Generic.List<LinkEdge>();
                list.Add(new LinkEdge { TargetId = link.SourceID, TargetType = link.SourceType });
            }
            var inIndices = builder.Allocate(ref root.IncomingIndices, maxId + 2);
            var inEdgesList = new System.Collections.Generic.List<LinkEdge>();
            inIndices[0] = 0;
            for (int i = 0; i <= maxId; i++)
            {
                if (incomingGroups.TryGetValue(i, out var edges)) inEdgesList.AddRange(edges);
                inIndices[i + 1] = inEdgesList.Count;
            }
            var inEdges = builder.Allocate(ref root.IncomingEdges, inEdgesList.Count);
            for (int i = 0; i < inEdgesList.Count; i++) inEdges[i] = inEdgesList[i];
        }

        private static int CalculateMaxEntityId(GDBAsset asset)
        {
            int max = 0;
            if (asset.Achievements != null && asset.Achievements.Count > 0) max = math.max(max, asset.Achievements.Count - 1);
            if (asset.AchievementStates != null && asset.AchievementStates.Count > 0) max = math.max(max, asset.AchievementStates.Count - 1);
            if (asset.AIs != null && asset.AIs.Count > 0) max = math.max(max, asset.AIs.Count - 1);
            if (asset.AIStates != null && asset.AIStates.Count > 0) max = math.max(max, asset.AIStates.Count - 1);
            if (asset.Characters != null && asset.Characters.Count > 0) max = math.max(max, asset.Characters.Count - 1);
            if (asset.CharacterStates != null && asset.CharacterStates.Count > 0) max = math.max(max, asset.CharacterStates.Count - 1);
            if (asset.Combats != null && asset.Combats.Count > 0) max = math.max(max, asset.Combats.Count - 1);
            if (asset.CombatStates != null && asset.CombatStates.Count > 0) max = math.max(max, asset.CombatStates.Count - 1);
            if (asset.Descriptions != null && asset.Descriptions.Count > 0) max = math.max(max, asset.Descriptions.Count - 1);
            if (asset.Economys != null && asset.Economys.Count > 0) max = math.max(max, asset.Economys.Count - 1);
            if (asset.EconomyStates != null && asset.EconomyStates.Count > 0) max = math.max(max, asset.EconomyStates.Count - 1);
            if (asset.Effects != null && asset.Effects.Count > 0) max = math.max(max, asset.Effects.Count - 1);
            if (asset.EffectStates != null && asset.EffectStates.Count > 0) max = math.max(max, asset.EffectStates.Count - 1);
            if (asset.Equipments != null && asset.Equipments.Count > 0) max = math.max(max, asset.Equipments.Count - 1);
            if (asset.Guilds != null && asset.Guilds.Count > 0) max = math.max(max, asset.Guilds.Count - 1);
            if (asset.GuildStates != null && asset.GuildStates.Count > 0) max = math.max(max, asset.GuildStates.Count - 1);
            if (asset.Inventorys != null && asset.Inventorys.Count > 0) max = math.max(max, asset.Inventorys.Count - 1);
            if (asset.InventoryStates != null && asset.InventoryStates.Count > 0) max = math.max(max, asset.InventoryStates.Count - 1);
            if (asset.Items != null && asset.Items.Count > 0) max = math.max(max, asset.Items.Count - 1);
            if (asset.Locations != null && asset.Locations.Count > 0) max = math.max(max, asset.Locations.Count - 1);
            if (asset.Missions != null && asset.Missions.Count > 0) max = math.max(max, asset.Missions.Count - 1);
            if (asset.MissionStates != null && asset.MissionStates.Count > 0) max = math.max(max, asset.MissionStates.Count - 1);
            if (asset.Names != null && asset.Names.Count > 0) max = math.max(max, asset.Names.Count - 1);
            if (asset.Objectives != null && asset.Objectives.Count > 0) max = math.max(max, asset.Objectives.Count - 1);
            if (asset.ObjectiveStates != null && asset.ObjectiveStates.Count > 0) max = math.max(max, asset.ObjectiveStates.Count - 1);
            if (asset.Players != null && asset.Players.Count > 0) max = math.max(max, asset.Players.Count - 1);
            if (asset.Quests != null && asset.Quests.Count > 0) max = math.max(max, asset.Quests.Count - 1);
            if (asset.QuestStates != null && asset.QuestStates.Count > 0) max = math.max(max, asset.QuestStates.Count - 1);
            if (asset.RangeAsFloats != null && asset.RangeAsFloats.Count > 0) max = math.max(max, asset.RangeAsFloats.Count - 1);
            if (asset.RangeAsInts != null && asset.RangeAsInts.Count > 0) max = math.max(max, asset.RangeAsInts.Count - 1);
            if (asset.Rewards != null && asset.Rewards.Count > 0) max = math.max(max, asset.Rewards.Count - 1);
            if (asset.Skills != null && asset.Skills.Count > 0) max = math.max(max, asset.Skills.Count - 1);
            if (asset.SkillStates != null && asset.SkillStates.Count > 0) max = math.max(max, asset.SkillStates.Count - 1);
            if (asset.Stats != null && asset.Stats.Count > 0) max = math.max(max, asset.Stats.Count - 1);
            if (asset.StatStates != null && asset.StatStates.Count > 0) max = math.max(max, asset.StatStates.Count - 1);
            if (asset.Tags != null && asset.Tags.Count > 0) max = math.max(max, asset.Tags.Count - 1);
            if (asset.TimeStates != null && asset.TimeStates.Count > 0) max = math.max(max, asset.TimeStates.Count - 1);
            if (asset.TimeTicks != null && asset.TimeTicks.Count > 0) max = math.max(max, asset.TimeTicks.Count - 1);
            return max;
        }

        private static int CalculateMaxEntityIdFromLinks(System.Collections.Generic.List<Link> links)
        {
            int max = 0;
            for (int i = 0; i < links.Count; i++)
            {
                var l = links[i];
                if (l.SourceID > max) max = l.SourceID;
                if (l.TargetID > max) max = l.TargetID;
            }
            return max;
        }
    }
}
