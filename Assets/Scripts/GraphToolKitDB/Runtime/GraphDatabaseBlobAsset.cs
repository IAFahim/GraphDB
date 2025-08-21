using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using GraphToolKitDB.Runtime;

namespace GraphTookKitDB.Runtime
{
    // Ultimate performance blob asset - simplified and optimized
    public struct GraphDatabaseBlobAsset
    {
        // Entity Collections (ID = index) - Direct access
        public BlobArray<Achievement> Achievements;
        public BlobArray<AchievementState> AchievementStates;
        public BlobArray<AI> AIs;
        public BlobArray<AIState> AIStates;
        public BlobArray<Character> Characters;
        public BlobArray<CharacterState> CharacterStates;
        public BlobArray<Combat> Combats;
        public BlobArray<CombatState> CombatStates;
        public BlobArray<Description> Descriptions;
        public BlobArray<Economy> Economies;
        public BlobArray<EconomyState> EconomyStates;
        public BlobArray<Effect> Effects;
        public BlobArray<EffectState> EffectStates;
        public BlobArray<Equipment> Equipments;
        public BlobArray<Guild> Guilds;
        public BlobArray<GuildState> GuildStates;
        public BlobArray<Inventory> Inventories;
        public BlobArray<InventoryState> InventoryStates;
        public BlobArray<Item> Items;
        public BlobArray<Link> Links;
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

        // Optimized CSR (Compressed Sparse Row) Link System
        public BlobArray<int> OutgoingIndices; // CSR row pointers
        public BlobArray<LinkEdge> OutgoingEdges; // CSR column data
        public BlobArray<int> IncomingIndices; // Reverse CSR
        public BlobArray<LinkEdge> IncomingEdges; // Reverse data

        // Metadata
        public int MaxEntityId;
        public int TotalLinks;
        public long CreationTimestamp;
    }

    // Minimal edge representation for maximum cache efficiency
    public struct LinkEdge
    {
        public int TargetId;
        public EntityType TargetType;
    }

    public struct GraphDatabaseComponent : IComponentData
    {
        public BlobAssetReference<GraphDatabaseBlobAsset> BlobAssetRef;
    }

    // Ultra-high performance builder
    public static class GraphDatabaseBlobBuilder
    {
        public static BlobAssetReference<GraphDatabaseBlobAsset> CreateBlobAsset(this GDBAsset sourceAsset)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<GraphDatabaseBlobAsset>();

            // Direct copy all entity arrays
            CopyArray(builder, ref root.Achievements, sourceAsset.Achievements);
            CopyArray(builder, ref root.AchievementStates, sourceAsset.AchievementStates);
            CopyArray(builder, ref root.AIs, sourceAsset.AIs);
            CopyArray(builder, ref root.AIStates, sourceAsset.AIStates);
            CopyArray(builder, ref root.Characters, sourceAsset.Characters);
            CopyArray(builder, ref root.CharacterStates, sourceAsset.CharacterStates);
            CopyArray(builder, ref root.Combats, sourceAsset.Combats);
            CopyArray(builder, ref root.CombatStates, sourceAsset.CombatStates);
            CopyArray(builder, ref root.Descriptions, sourceAsset.Descriptions);
            CopyArray(builder, ref root.Economies, sourceAsset.Economys);
            CopyArray(builder, ref root.EconomyStates, sourceAsset.EconomyStates);
            CopyArray(builder, ref root.Effects, sourceAsset.Effects);
            CopyArray(builder, ref root.EffectStates, sourceAsset.EffectStates);
            CopyArray(builder, ref root.Equipments, sourceAsset.Equipments);
            CopyArray(builder, ref root.Guilds, sourceAsset.Guilds);
            CopyArray(builder, ref root.GuildStates, sourceAsset.GuildStates);
            CopyArray(builder, ref root.Inventories, sourceAsset.Inventorys);
            CopyArray(builder, ref root.InventoryStates, sourceAsset.InventoryStates);
            CopyArray(builder, ref root.Items, sourceAsset.Items);
            CopyArray(builder, ref root.Links, sourceAsset.Links);
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

            // Build optimized link system
            BuildCsrLinkSystem(builder, ref root, sourceAsset.Links);

            // Set metadata
            root.MaxEntityId = CalculateMaxEntityId(sourceAsset);
            root.TotalLinks = sourceAsset.Links.Count;
            root.CreationTimestamp = DateTime.Now.Ticks;

            return builder.CreateBlobAssetReference<GraphDatabaseBlobAsset>(Allocator.Persistent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyArray<T>(BlobBuilder builder, ref BlobArray<T> target,
            System.Collections.Generic.List<T> source) where T : struct
        {
            if (source.Count == 0)
            {
                builder.Allocate(ref target, 0);
                return;
            }

            var array = builder.Allocate(ref target, source.Count);
            for (int i = 0; i < source.Count; i++) array[i] = source[i];
        }

        private static void BuildCsrLinkSystem(BlobBuilder builder, ref GraphDatabaseBlobAsset root,
            System.Collections.Generic.List<Link> links)
        {
            if (links.Count == 0)
            {
                builder.Allocate(ref root.OutgoingIndices, 1)[0] = 0;
                builder.Allocate(ref root.OutgoingEdges, 0);
                builder.Allocate(ref root.IncomingIndices, 1)[0] = 0;
                builder.Allocate(ref root.IncomingEdges, 0);
                return;
            }

            int maxId = CalculateMaxEntityIdFromLinks(links);

            // Build outgoing CSR
            var outgoingGroups =
                new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<LinkEdge>>();
            foreach (var link in links)
            {
                if (!outgoingGroups.ContainsKey(link.SourceID))
                    outgoingGroups[link.SourceID] = new System.Collections.Generic.List<LinkEdge>();

                outgoingGroups[link.SourceID].Add(new LinkEdge
                {
                    TargetId = link.TargetID,
                    TargetType = link.TargetType
                });
            }

            // Create CSR arrays
            var outIndices = builder.Allocate(ref root.OutgoingIndices, maxId + 2);
            var outEdgesList = new System.Collections.Generic.List<LinkEdge>();

            outIndices[0] = 0;
            for (int i = 0; i <= maxId; i++)
            {
                if (outgoingGroups.TryGetValue(i, out var edges))
                {
                    outEdgesList.AddRange(edges);
                }

                outIndices[i + 1] = outEdgesList.Count;
            }

            var outEdges = builder.Allocate(ref root.OutgoingEdges, outEdgesList.Count);
            for (int i = 0; i < outEdgesList.Count; i++)
            {
                outEdges[i] = outEdgesList[i];
            }

            // Build incoming CSR (reverse graph)
            var incomingGroups =
                new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<LinkEdge>>();
            foreach (var link in links)
            {
                if (!incomingGroups.ContainsKey(link.TargetID))
                    incomingGroups[link.TargetID] = new System.Collections.Generic.List<LinkEdge>();

                incomingGroups[link.TargetID].Add(new LinkEdge
                {
                    TargetId = link.SourceID, // Reversed for incoming
                    TargetType = link.SourceType
                });
            }

            var inIndices = builder.Allocate(ref root.IncomingIndices, maxId + 2);
            var inEdgesList = new System.Collections.Generic.List<LinkEdge>();

            inIndices[0] = 0;
            for (int i = 0; i <= maxId; i++)
            {
                if (incomingGroups.TryGetValue(i, out var edges))
                {
                    inEdgesList.AddRange(edges);
                }

                inIndices[i + 1] = inEdgesList.Count;
            }

            var inEdges = builder.Allocate(ref root.IncomingEdges, inEdgesList.Count);
            for (int i = 0; i < inEdgesList.Count; i++)
            {
                inEdges[i] = inEdgesList[i];
            }
        }

        private static int CalculateMaxEntityId(GDBAsset asset)
        {
            int max = 0;
            if (asset.Players.Count > 0) max = math.max(max, asset.Players.Count - 1);
            if (asset.Characters.Count > 0) max = math.max(max, asset.Characters.Count - 1);
            if (asset.Items.Count > 0) max = math.max(max, asset.Items.Count - 1);
            if (asset.Quests.Count > 0) max = math.max(max, asset.Quests.Count - 1);
            if (asset.Missions.Count > 0) max = math.max(max, asset.Missions.Count - 1);
            // Add other arrays as needed...
            return max;
        }

        private static int CalculateMaxEntityIdFromLinks(System.Collections.Generic.List<Link> links)
        {
            int max = 0;
            foreach (var link in links)
            {
                if (link.SourceID > max) max = link.SourceID;
                if (link.TargetID > max) max = link.TargetID;
            }

            return max;
        }
    }

    // Blazing fast extension methods
    public static unsafe class GraphDatabaseExtensions
    {
        // Zero-overhead entity access with aggressive inlining
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T GetEntity<T>(this ref GraphDatabaseBlobAsset db, int id) where T : struct, IEntity
        {
            // JIT will eliminate all unused branches at compile time
            if (typeof(T) == typeof(Player)) return ref UnsafeUtility.As<Player, T>(ref db.Players[id]);
            if (typeof(T) == typeof(Character)) return ref UnsafeUtility.As<Character, T>(ref db.Characters[id]);
            if (typeof(T) == typeof(Item)) return ref UnsafeUtility.As<Item, T>(ref db.Items[id]);
            if (typeof(T) == typeof(Quest)) return ref UnsafeUtility.As<Quest, T>(ref db.Quests[id]);
            if (typeof(T) == typeof(Mission)) return ref UnsafeUtility.As<Mission, T>(ref db.Missions[id]);
            if (typeof(T) == typeof(Objective)) return ref UnsafeUtility.As<Objective, T>(ref db.Objectives[id]);
            if (typeof(T) == typeof(Equipment)) return ref UnsafeUtility.As<Equipment, T>(ref db.Equipments[id]);
            if (typeof(T) == typeof(Skill)) return ref UnsafeUtility.As<Skill, T>(ref db.Skills[id]);
            if (typeof(T) == typeof(Effect)) return ref UnsafeUtility.As<Effect, T>(ref db.Effects[id]);
            if (typeof(T) == typeof(Inventory)) return ref UnsafeUtility.As<Inventory, T>(ref db.Inventories[id]);
            if (typeof(T) == typeof(Stat)) return ref UnsafeUtility.As<Stat, T>(ref db.Stats[id]);
            if (typeof(T) == typeof(Location)) return ref UnsafeUtility.As<Location, T>(ref db.Locations[id]);
            if (typeof(T) == typeof(Guild)) return ref UnsafeUtility.As<Guild, T>(ref db.Guilds[id]);
            if (typeof(T) == typeof(Achievement)) return ref UnsafeUtility.As<Achievement, T>(ref db.Achievements[id]);
            if (typeof(T) == typeof(AI)) return ref UnsafeUtility.As<AI, T>(ref db.AIs[id]);
            if (typeof(T) == typeof(Combat)) return ref UnsafeUtility.As<Combat, T>(ref db.Combats[id]);
            if (typeof(T) == typeof(Economy)) return ref UnsafeUtility.As<Economy, T>(ref db.Economies[id]);
            if (typeof(T) == typeof(TimeTick)) return ref UnsafeUtility.As<TimeTick, T>(ref db.TimeTicks[id]);
            if (typeof(T) == typeof(Reward)) return ref UnsafeUtility.As<Reward, T>(ref db.Rewards[id]);
            if (typeof(T) == typeof(RangeAsFloat))
                return ref UnsafeUtility.As<RangeAsFloat, T>(ref db.RangeAsFloats[id]);
            if (typeof(T) == typeof(RangeAsInt)) return ref UnsafeUtility.As<RangeAsInt, T>(ref db.RangeAsInts[id]);
            if (typeof(T) == typeof(Name)) return ref UnsafeUtility.As<Name, T>(ref db.Names[id]);
            if (typeof(T) == typeof(Description)) return ref UnsafeUtility.As<Description, T>(ref db.Descriptions[id]);
            if (typeof(T) == typeof(Tag)) return ref UnsafeUtility.As<Tag, T>(ref db.Tags[id]);

            throw new ArgumentException($"Unsupported entity type: {typeof(T)}");
        }

        // Fastest possible graph traversal - direct CSR access
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<LinkEdge> GetOutgoingEdges(this ref GraphDatabaseBlobAsset db, int entityId)
        {
            if (entityId >= db.OutgoingIndices.Length - 1) return ReadOnlySpan<LinkEdge>.Empty;

            int start = db.OutgoingIndices[entityId];
            int end = db.OutgoingIndices[entityId + 1];
            int count = end - start;

            if (count <= 0) return ReadOnlySpan<LinkEdge>.Empty;

            return new ReadOnlySpan<LinkEdge>(
                (LinkEdge*)db.OutgoingEdges.GetUnsafePtr() + start,
                count
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<LinkEdge> GetIncomingEdges(this ref GraphDatabaseBlobAsset db, int entityId)
        {
            if (entityId >= db.IncomingIndices.Length - 1) return ReadOnlySpan<LinkEdge>.Empty;

            int start = db.IncomingIndices[entityId];
            int end = db.IncomingIndices[entityId + 1];
            int count = end - start;

            if (count <= 0) return ReadOnlySpan<LinkEdge>.Empty;

            return new ReadOnlySpan<LinkEdge>(
                (LinkEdge*)db.IncomingEdges.GetUnsafePtr() + start,
                count
            );
        }

        // Optimized filtered queries
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetLinkedEntities<T>(this ref GraphDatabaseBlobAsset db, int sourceId, EntityType targetType,
            ref NativeList<T> results) where T : unmanaged, IEntity
        {
            results.Clear();
            var edges = db.GetOutgoingEdges(sourceId);

            for (int i = 0; i < edges.Length; i++)
            {
                if (edges[i].TargetType == targetType) results.Add(db.GetEntity<T>(edges[i].TargetId));
            }
        }

        // High-performance path finding
        public static bool HasPath(this ref GraphDatabaseBlobAsset db, int sourceId, int targetId, int maxDepth = 6)
        {
            if (sourceId == targetId) return true;
            if (maxDepth <= 0 || sourceId > db.MaxEntityId || targetId > db.MaxEntityId) return false;

            // Use stack allocation for small searches
            const int MAX_STACK_SIZE = 512;
            if (db.MaxEntityId < MAX_STACK_SIZE)
            {
                var visited = stackalloc bool[db.MaxEntityId + 1];
                return HasPathRecursive(ref db, sourceId, targetId, maxDepth, visited);
            }
            else
            {
                // Fall back to heap allocation for large graphs
                var visited = new NativeArray<bool>(db.MaxEntityId + 1, Allocator.Temp);
                bool result = HasPathRecursiveHeap(ref db, sourceId, targetId, maxDepth, visited);
                visited.Dispose();
                return result;
            }
        }

        private static bool HasPathRecursive(ref GraphDatabaseBlobAsset db, int current, int target, int depth,
            bool* visited)
        {
            if (current == target) return true;
            if (depth <= 0 || visited[current]) return false;

            visited[current] = true;
            var edges = db.GetOutgoingEdges(current);

            for (int i = 0; i < edges.Length; i++)
            {
                if (HasPathRecursive(ref db, edges[i].TargetId, target, depth - 1, visited))
                    return true;
            }

            return false;
        }

        private static bool HasPathRecursiveHeap(ref GraphDatabaseBlobAsset db, int current, int target, int depth,
            NativeArray<bool> visited)
        {
            if (current == target) return true;
            if (depth <= 0 || visited[current]) return false;

            visited[current] = true;
            var edges = db.GetOutgoingEdges(current);

            for (int i = 0; i < edges.Length; i++)
            {
                if (HasPathRecursiveHeap(ref db, edges[i].TargetId, target, depth - 1, visited))
                    return true;
            }

            return false;
        }

        // BFS multi-hop search
        public static void GetEntitiesWithinHops<T>(this ref GraphDatabaseBlobAsset db, int sourceId, int maxHops,
            EntityType targetType, ref NativeHashSet<T> results)
            where T : unmanaged, IEntity, IEquatable<T>
        {
            results.Clear();

            var frontier = new NativeQueue<(int id, int hops)>(Allocator.Temp);
            var visited = new NativeHashSet<int>(64, Allocator.Temp);

            frontier.Enqueue((sourceId, 0));
            visited.Add(sourceId);

            while (frontier.TryDequeue(out var current) && current.hops < maxHops)
            {
                var edges = db.GetOutgoingEdges(current.id);
                for (int i = 0; i < edges.Length; i++)
                {
                    var edge = edges[i];
                    if (edge.TargetType == targetType && !visited.Contains(edge.TargetId))
                    {
                        visited.Add(edge.TargetId);
                        var entity = db.GetEntity<T>(edge.TargetId);
                        results.Add(entity);

                        if (current.hops + 1 < maxHops)
                        {
                            frontier.Enqueue((edge.TargetId, current.hops + 1));
                        }
                    }
                }
            }

            frontier.Dispose();
            visited.Dispose();
        }

        // Simple connection check
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConnectedTo(this ref GraphDatabaseBlobAsset db, int sourceId, int targetId,
            EntityType targetType = EntityType.None)
        {
            var edges = db.GetOutgoingEdges(sourceId);
            for (int i = 0; i < edges.Length; i++)
            {
                if (edges[i].TargetId == targetId &&
                    (targetType == EntityType.None || edges[i].TargetType == targetType))
                    return true;
            }

            return false;
        }
    }

    // Burst-optimized parallel job
    [BurstCompile]
    public struct ParallelGraphTraversalJob : IJobParallelFor
    {
        [ReadOnly] public BlobAssetReference<GraphDatabaseBlobAsset> Database;
        [ReadOnly] public NativeArray<int> SourceEntities;
        [WriteOnly] public NativeArray<int> OutgoingCounts;
        [WriteOnly] public NativeArray<int> IncomingCounts;

        public void Execute(int index)
        {
            var entityId = SourceEntities[index];
            ref var db = ref Database.Value;

            var outEdges = db.GetOutgoingEdges(entityId);
            var inEdges = db.GetIncomingEdges(entityId);

            OutgoingCounts[index] = outEdges.Length;
            IncomingCounts[index] = inEdges.Length;
        }
    }

    // Simple example system
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct GraphQueryExampleSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GraphDatabaseComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var graphDb in SystemAPI.Query<RefRO<GraphDatabaseComponent>>())
            {
                ref var db = ref graphDb.ValueRO.BlobAssetRef.Value;

                // Example 1: Direct entity access
                if (db.Players.Length > 0)
                {
                    ref readonly var player = ref db.GetEntity<Player>(0);

                    // Example 2: Get all connected items
                    var playerItems = new NativeList<Item>(16, Allocator.Temp);
                    db.GetLinkedEntities(player.Id, EntityType.Item, ref playerItems);

                    // Example 3: Check path existence
                    if (db.Players.Length > 1)
                    {
                        bool canReach = db.HasPath(0, 1, maxDepth: 5);
                    }

                    // Example 4: Multi-hop search
                    var nearbyLocations = new NativeHashSet<Location>(32, Allocator.Temp);
                    db.GetEntitiesWithinHops(player.Id, 3, EntityType.Location, ref nearbyLocations);

                    // Cleanup
                    playerItems.Dispose();
                    nearbyLocations.Dispose();
                }
            }
        }
    }
}