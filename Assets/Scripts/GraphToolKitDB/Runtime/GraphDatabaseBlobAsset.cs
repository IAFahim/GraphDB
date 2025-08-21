// ----- AUTO-GENERATED ECS/DOTS BLOB FILE BY GraphToolGenerator.cs -----

using System;
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

    public static unsafe class GDBBlobExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T GetEntity<T>(this ref GDBBlobAsset db, int id) where T : struct, IEntity
        {
            if (typeof(T) == typeof(Achievement)) { ref var r = ref db.Achievements[id]; return ref Unsafe.As<Achievement, T>(ref r); }
            if (typeof(T) == typeof(AchievementState)) { ref var r = ref db.AchievementStates[id]; return ref Unsafe.As<AchievementState, T>(ref r); }
            if (typeof(T) == typeof(AI)) { ref var r = ref db.AIs[id]; return ref Unsafe.As<AI, T>(ref r); }
            if (typeof(T) == typeof(AIState)) { ref var r = ref db.AIStates[id]; return ref Unsafe.As<AIState, T>(ref r); }
            if (typeof(T) == typeof(Character)) { ref var r = ref db.Characters[id]; return ref Unsafe.As<Character, T>(ref r); }
            if (typeof(T) == typeof(CharacterState)) { ref var r = ref db.CharacterStates[id]; return ref Unsafe.As<CharacterState, T>(ref r); }
            if (typeof(T) == typeof(Combat)) { ref var r = ref db.Combats[id]; return ref Unsafe.As<Combat, T>(ref r); }
            if (typeof(T) == typeof(CombatState)) { ref var r = ref db.CombatStates[id]; return ref Unsafe.As<CombatState, T>(ref r); }
            if (typeof(T) == typeof(Description)) { ref var r = ref db.Descriptions[id]; return ref Unsafe.As<Description, T>(ref r); }
            if (typeof(T) == typeof(Economy)) { ref var r = ref db.Economys[id]; return ref Unsafe.As<Economy, T>(ref r); }
            if (typeof(T) == typeof(EconomyState)) { ref var r = ref db.EconomyStates[id]; return ref Unsafe.As<EconomyState, T>(ref r); }
            if (typeof(T) == typeof(Effect)) { ref var r = ref db.Effects[id]; return ref Unsafe.As<Effect, T>(ref r); }
            if (typeof(T) == typeof(EffectState)) { ref var r = ref db.EffectStates[id]; return ref Unsafe.As<EffectState, T>(ref r); }
            if (typeof(T) == typeof(Equipment)) { ref var r = ref db.Equipments[id]; return ref Unsafe.As<Equipment, T>(ref r); }
            if (typeof(T) == typeof(Guild)) { ref var r = ref db.Guilds[id]; return ref Unsafe.As<Guild, T>(ref r); }
            if (typeof(T) == typeof(GuildState)) { ref var r = ref db.GuildStates[id]; return ref Unsafe.As<GuildState, T>(ref r); }
            if (typeof(T) == typeof(Inventory)) { ref var r = ref db.Inventorys[id]; return ref Unsafe.As<Inventory, T>(ref r); }
            if (typeof(T) == typeof(InventoryState)) { ref var r = ref db.InventoryStates[id]; return ref Unsafe.As<InventoryState, T>(ref r); }
            if (typeof(T) == typeof(Item)) { ref var r = ref db.Items[id]; return ref Unsafe.As<Item, T>(ref r); }
            if (typeof(T) == typeof(Location)) { ref var r = ref db.Locations[id]; return ref Unsafe.As<Location, T>(ref r); }
            if (typeof(T) == typeof(Mission)) { ref var r = ref db.Missions[id]; return ref Unsafe.As<Mission, T>(ref r); }
            if (typeof(T) == typeof(MissionState)) { ref var r = ref db.MissionStates[id]; return ref Unsafe.As<MissionState, T>(ref r); }
            if (typeof(T) == typeof(Name)) { ref var r = ref db.Names[id]; return ref Unsafe.As<Name, T>(ref r); }
            if (typeof(T) == typeof(Objective)) { ref var r = ref db.Objectives[id]; return ref Unsafe.As<Objective, T>(ref r); }
            if (typeof(T) == typeof(ObjectiveState)) { ref var r = ref db.ObjectiveStates[id]; return ref Unsafe.As<ObjectiveState, T>(ref r); }
            if (typeof(T) == typeof(Player)) { ref var r = ref db.Players[id]; return ref Unsafe.As<Player, T>(ref r); }
            if (typeof(T) == typeof(Quest)) { ref var r = ref db.Quests[id]; return ref Unsafe.As<Quest, T>(ref r); }
            if (typeof(T) == typeof(QuestState)) { ref var r = ref db.QuestStates[id]; return ref Unsafe.As<QuestState, T>(ref r); }
            if (typeof(T) == typeof(RangeAsFloat)) { ref var r = ref db.RangeAsFloats[id]; return ref Unsafe.As<RangeAsFloat, T>(ref r); }
            if (typeof(T) == typeof(RangeAsInt)) { ref var r = ref db.RangeAsInts[id]; return ref Unsafe.As<RangeAsInt, T>(ref r); }
            if (typeof(T) == typeof(Reward)) { ref var r = ref db.Rewards[id]; return ref Unsafe.As<Reward, T>(ref r); }
            if (typeof(T) == typeof(Skill)) { ref var r = ref db.Skills[id]; return ref Unsafe.As<Skill, T>(ref r); }
            if (typeof(T) == typeof(SkillState)) { ref var r = ref db.SkillStates[id]; return ref Unsafe.As<SkillState, T>(ref r); }
            if (typeof(T) == typeof(Stat)) { ref var r = ref db.Stats[id]; return ref Unsafe.As<Stat, T>(ref r); }
            if (typeof(T) == typeof(StatState)) { ref var r = ref db.StatStates[id]; return ref Unsafe.As<StatState, T>(ref r); }
            if (typeof(T) == typeof(Tag)) { ref var r = ref db.Tags[id]; return ref Unsafe.As<Tag, T>(ref r); }
            if (typeof(T) == typeof(TimeState)) { ref var r = ref db.TimeStates[id]; return ref Unsafe.As<TimeState, T>(ref r); }
            if (typeof(T) == typeof(TimeTick)) { ref var r = ref db.TimeTicks[id]; return ref Unsafe.As<TimeTick, T>(ref r); }
            throw new ArgumentException($"Unsupported entity type: {typeof(T)}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<LinkEdge> GetOutgoingEdges(this ref GDBBlobAsset db, int entityId)
        {
            if (entityId >= db.OutgoingIndices.Length - 1) return ReadOnlySpan<LinkEdge>.Empty;
            int start = db.OutgoingIndices[entityId];
            int end = db.OutgoingIndices[entityId + 1];
            int count = end - start;
            if (count <= 0) return ReadOnlySpan<LinkEdge>.Empty;
            return new ReadOnlySpan<LinkEdge>((LinkEdge*)db.OutgoingEdges.GetUnsafePtr() + start, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<LinkEdge> GetIncomingEdges(this ref GDBBlobAsset db, int entityId)
        {
            if (entityId >= db.IncomingIndices.Length - 1) return ReadOnlySpan<LinkEdge>.Empty;
            int start = db.IncomingIndices[entityId];
            int end = db.IncomingIndices[entityId + 1];
            int count = end - start;
            if (count <= 0) return ReadOnlySpan<LinkEdge>.Empty;
            return new ReadOnlySpan<LinkEdge>((LinkEdge*)db.IncomingEdges.GetUnsafePtr() + start, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetLinkedEntities<T>(this ref GDBBlobAsset db, int sourceId, EntityType targetType, ref NativeList<T> results) where T : unmanaged, IEntity
        {
            results.Clear();
            var edges = db.GetOutgoingEdges(sourceId);
            for (int i = 0; i < edges.Length; i++)
                if (edges[i].TargetType == targetType) results.Add(db.GetEntity<T>(edges[i].TargetId));
        }

        public static bool HasPath(this ref GDBBlobAsset db, int sourceId, int targetId, int maxDepth = 6)
        {
            if (sourceId == targetId) return true;
            if (maxDepth <= 0 || sourceId > db.MaxEntityId || targetId > db.MaxEntityId) return false;
            const int MAX_STACK_SIZE = 512;
            if (db.MaxEntityId < MAX_STACK_SIZE)
            {
                var visited = stackalloc bool[db.MaxEntityId + 1];
                return HasPathRecursive(ref db, sourceId, targetId, maxDepth, visited);
            }
            else
            {
                var visited = new NativeArray<bool>(db.MaxEntityId + 1, Allocator.Temp);
                bool result = HasPathRecursiveHeap(ref db, sourceId, targetId, maxDepth, visited);
                visited.Dispose();
                return result;
            }
        }

        private static bool HasPathRecursive(ref GDBBlobAsset db, int current, int target, int depth, bool* visited)
        {
            if (current == target) return true;
            if (depth <= 0 || visited[current]) return false;
            visited[current] = true;
            var edges = db.GetOutgoingEdges(current);
            for (int i = 0; i < edges.Length; i++)
                if (HasPathRecursive(ref db, edges[i].TargetId, target, depth - 1, visited)) return true;
            return false;
        }

        private static bool HasPathRecursiveHeap(ref GDBBlobAsset db, int current, int target, int depth, NativeArray<bool> visited)
        {
            if (current == target) return true;
            if (depth <= 0 || visited[current]) return false;
            visited[current] = true;
            var edges = db.GetOutgoingEdges(current);
            for (int i = 0; i < edges.Length; i++)
                if (HasPathRecursiveHeap(ref db, edges[i].TargetId, target, depth - 1, visited)) return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConnectedTo(this ref GDBBlobAsset db, int sourceId, int targetId, EntityType targetType = EntityType.None)
        {
            var edges = db.GetOutgoingEdges(sourceId);
            for (int i = 0; i < edges.Length; i++)
                if (edges[i].TargetId == targetId && (targetType == EntityType.None || edges[i].TargetType == targetType)) return true;
            return false;
        }
    }
}
