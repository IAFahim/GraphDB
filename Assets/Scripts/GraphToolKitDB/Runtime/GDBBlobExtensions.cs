using System;
using System.Runtime.CompilerServices;
using GraphToolKitDB.Runtime;
using Unity.Collections;

namespace GraphTookKitDB.Runtime.ECS
{
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