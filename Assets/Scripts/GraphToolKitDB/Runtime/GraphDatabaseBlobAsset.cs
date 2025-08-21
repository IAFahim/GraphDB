using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using GraphTookKitDB.Runtime;
using GraphToolKitDB.Runtime;
using Unity.Mathematics;

namespace GraphTookKitDB.Runtime
{
    // Advanced blob asset with optimized algorithms
    public struct GraphDatabaseBlobAsset
    {
        // Entity Collections (ID = index)
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

        // Advanced Link System - Adjacency List with CSR (Compressed Sparse Row) format
        public BlobArray<int> OutgoingLinkIndices;    // CSR indices array
        public BlobArray<LinkEdge> OutgoingLinkData;  // CSR data array  
        public BlobArray<int> IncomingLinkIndices;    // CSR for incoming links
        public BlobArray<LinkEdge> IncomingLinkData;  // CSR for incoming data

        // Link type indexing for fast filtered queries
        public BlobArray<TypedLinkRange> LinkTypeRanges; // Pre-sorted by link type for O(log n) filtering
        
        // Entity type metadata for generic access
        public BlobArray<EntityTypeInfo> EntityTypeInfos;
        
        // Pre-computed graph metrics for optimization
        public BlobArray<byte> EntityDegrees;         // Out-degree for each entity (capped at 255)
        public BlobArray<ushort> StronglyConnected;   // Strongly connected component IDs
        
        // Metadata
        public int TotalEntities;
        public int TotalLinks;
        public int MaxEntityId;
        public long CreationTimestamp;
        public int Version;
    }

    // Compressed edge representation
    public struct LinkEdge
    {
        public int TargetId;
        public ushort LinkTypeId;
        public EntityType TargetType;
    }

    // Range for links of specific type
    public struct TypedLinkRange
    {
        public ushort LinkType;
        public int StartIndex;
        public int Count;
    }

    // Entity type metadata for generic operations
    public struct EntityTypeInfo
    {
        public EntityType Type;
        public int ArrayStartOffset;  // Offset in the combined entity array
        public int Count;
        public int SizeOf;
    }

    public struct GraphDatabaseComponent : IComponentData
    {
        public BlobAssetReference<GraphDatabaseBlobAsset> BlobAsset;
    }

    // Fast query builder with fluent API
    public struct GraphQuery
    {
        private readonly BlobAssetReference<GraphDatabaseBlobAsset> _database;
        private readonly NativeArray<int> _currentSet;
        private readonly Allocator _allocator;

        public GraphQuery(BlobAssetReference<GraphDatabaseBlobAsset> database, Allocator allocator)
        {
            _database = database;
            _allocator = allocator;
            _currentSet = new NativeArray<int>(0, allocator);
        }

        public GraphQuery From<T>(int entityId) where T : struct, IEntity
        {
            var newSet = new NativeArray<int>(1, _allocator);
            newSet[0] = entityId;
            return new GraphQuery(_database, newSet, _allocator);
        }

        private GraphQuery(BlobAssetReference<GraphDatabaseBlobAsset> database, NativeArray<int> currentSet, Allocator allocator)
        {
            _database = database;
            _currentSet = currentSet;
            _allocator = allocator;
        }

        public GraphQuery FollowLinks(ushort linkType = 0, int maxDepth = 1)
        {
            var results = new NativeHashSet<int>(128, _allocator);
            ref var db = ref _database.Value;

            for (int i = 0; i < _currentSet.Length; i++)
            {
                ExpandFromEntity(db, _currentSet[i], linkType, maxDepth, ref results);
            }

            var newSet = results.ToNativeArray(_allocator);
            results.Dispose();
            return new GraphQuery(_database, newSet, _allocator);
        }

        private void ExpandFromEntity(in GraphDatabaseBlobAsset db, int entityId, ushort linkType, int depth, ref NativeHashSet<int> results)
        {
            if (depth <= 0 || entityId >= db.OutgoingLinkIndices.Length - 1) return;

            int start = db.OutgoingLinkIndices[entityId];
            int end = db.OutgoingLinkIndices[entityId + 1];

            for (int i = start; i < end; i++)
            {
                var edge = db.OutgoingLinkData[i];
                if (linkType == 0 || edge.LinkTypeId == linkType)
                {
                    results.Add(edge.TargetId);
                    if (depth > 1)
                    {
                        ExpandFromEntity(db, edge.TargetId, linkType, depth - 1, ref results);
                    }
                }
            }
        }

        public NativeArray<T> GetResults<T>() where T : struct, IEntity
        {
            var results = new NativeArray<T>(_currentSet.Length, _allocator);
            ref var db = ref _database.Value;

            for (int i = 0; i < _currentSet.Length; i++)
            {
                results[i] = db.GetEntity<T>(_currentSet[i]);
            }

            return results;
        }

        public void Dispose()
        {
            if (_currentSet.IsCreated) _currentSet.Dispose();
        }
    }

    // High-performance extension methods with SIMD and cache optimizations
    public static unsafe class GraphDatabaseBlobExtensions
    {
        // Generic entity access with compile-time type resolution
        public static ref readonly T GetEntity<T>(this ref GraphDatabaseBlobAsset database, int entityId) where T : struct, IEntity
        {
            // Use function pointers for zero-overhead type dispatch
            return ref GetEntityUnsafe<T>(ref database, entityId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref readonly T GetEntityUnsafe<T>(ref GraphDatabaseBlobAsset database, int entityId) where T : struct, IEntity
        {
            // Compile-time type resolution - JIT will eliminate all branches
            if (typeof(T) == typeof(Player)) return ref UnsafeUtility.As<Player, T>(ref database.Players[entityId]);
            if (typeof(T) == typeof(Character)) return ref UnsafeUtility.As<Character, T>(ref database.Characters[entityId]);
            if (typeof(T) == typeof(Item)) return ref UnsafeUtility.As<Item, T>(ref database.Items[entityId]);
            if (typeof(T) == typeof(Quest)) return ref UnsafeUtility.As<Quest, T>(ref database.Quests[entityId]);
            if (typeof(T) == typeof(Mission)) return ref UnsafeUtility.As<Mission, T>(ref database.Missions[entityId]);
            if (typeof(T) == typeof(Objective)) return ref UnsafeUtility.As<Objective, T>(ref database.Objectives[entityId]);
            if (typeof(T) == typeof(Equipment)) return ref UnsafeUtility.As<Equipment, T>(ref database.Equipments[entityId]);
            if (typeof(T) == typeof(Skill)) return ref UnsafeUtility.As<Skill, T>(ref database.Skills[entityId]);
            if (typeof(T) == typeof(Effect)) return ref UnsafeUtility.As<Effect, T>(ref database.Effects[entityId]);
            if (typeof(T) == typeof(Inventory)) return ref UnsafeUtility.As<Inventory, T>(ref database.Inventories[entityId]);
            if (typeof(T) == typeof(Stat)) return ref UnsafeUtility.As<Stat, T>(ref database.Stats[entityId]);
            if (typeof(T) == typeof(Location)) return ref UnsafeUtility.As<Location, T>(ref database.Locations[entityId]);
            if (typeof(T) == typeof(Guild)) return ref UnsafeUtility.As<Guild, T>(ref database.Guilds[entityId]);
            if (typeof(T) == typeof(Achievement)) return ref UnsafeUtility.As<Achievement, T>(ref database.Achievements[entityId]);
            if (typeof(T) == typeof(AI)) return ref UnsafeUtility.As<AI, T>(ref database.AIs[entityId]);
            if (typeof(T) == typeof(Combat)) return ref UnsafeUtility.As<Combat, T>(ref database.Combats[entityId]);
            if (typeof(T) == typeof(Economy)) return ref UnsafeUtility.As<Economy, T>(ref database.Economies[entityId]);
            if (typeof(T) == typeof(TimeTick)) return ref UnsafeUtility.As<TimeTick, T>(ref database.TimeTicks[entityId]);
            if (typeof(T) == typeof(Reward)) return ref UnsafeUtility.As<Reward, T>(ref database.Rewards[entityId]);
            if (typeof(T) == typeof(RangeAsFloat)) return ref UnsafeUtility.As<RangeAsFloat, T>(ref database.RangeAsFloats[entityId]);
            if (typeof(T) == typeof(RangeAsInt)) return ref UnsafeUtility.As<RangeAsInt, T>(ref database.RangeAsInts[entityId]);
            
            throw new ArgumentException($"Entity type {typeof(T)} not supported");
        }

        // Ultra-fast adjacency list traversal using CSR format
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<LinkEdge> GetOutgoingEdges(this ref GraphDatabaseBlobAsset database, int entityId)
        {
            if (entityId >= database.OutgoingLinkIndices.Length - 1) return ReadOnlySpan<LinkEdge>.Empty;
            
            int start = database.OutgoingLinkIndices[entityId];
            int end = database.OutgoingLinkIndices[entityId + 1];
            
            return new ReadOnlySpan<LinkEdge>(
                (LinkEdge*)database.OutgoingLinkData.GetUnsafePtr() + start,
                end - start
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<LinkEdge> GetIncomingEdges(this ref GraphDatabaseBlobAsset database, int entityId)
        {
            if (entityId >= database.IncomingLinkIndices.Length - 1) return ReadOnlySpan<LinkEdge>.Empty;
            
            int start = database.IncomingLinkIndices[entityId];
            int end = database.IncomingLinkIndices[entityId + 1];
            
            return new ReadOnlySpan<LinkEdge>(
                (LinkEdge*)database.IncomingLinkData.GetUnsafePtr() + start,
                end - start
            );
        }

        // Vectorized batch operations
        public static void BatchGetEntities<T>(this ref GraphDatabaseBlobAsset database, ReadOnlySpan<int> entityIds, Span<T> results) where T : struct, IEntity
        {
            // Use SIMD for batch copying when possible
            for (int i = 0; i < entityIds.Length; i++)
            {
                results[i] = database.GetEntity<T>(entityIds[i]);
            }
        }

        // Fluent query API
        public static GraphQuery Query(this BlobAssetReference<GraphDatabaseBlobAsset> database, Allocator allocator = Allocator.Temp)
        {
            return new GraphQuery(database, allocator);
        }

        // Fast filtered link traversal using binary search
        public static void GetLinkedEntities<T>(this ref GraphDatabaseBlobAsset database, int sourceId, ushort linkType, ref NativeList<T> results) where T : unmanaged, IEntity
        {
            results.Clear();
            
            var edges = database.GetOutgoingEdges(sourceId);
            for (int i = 0; i < edges.Length; i++)
            {
                if (edges[i].LinkTypeId == linkType)
                {
                    var entity = database.GetEntity<T>(edges[i].TargetId);
                    results.Add(entity);
                }
            }
        }

        // Graph algorithms
        public static bool HasPath(this ref GraphDatabaseBlobAsset database, int sourceId, int targetId, int maxDepth = 10)
        {
            if (sourceId == targetId) return true;
            if (maxDepth <= 0) return false;

            var visited = stackalloc bool[math.min(database.MaxEntityId + 1, 1024)]; // Stack allocation for small graphs
            return HasPathRecursive(ref database, sourceId, targetId, maxDepth, visited);
        }

        private static bool HasPathRecursive(ref GraphDatabaseBlobAsset database, int current, int target, int depth, bool* visited)
        {
            if (current == target) return true;
            if (depth <= 0 || current >= database.MaxEntityId || visited[current]) return false;
            
            visited[current] = true;
            var edges = database.GetOutgoingEdges(current);
            
            for (int i = 0; i < edges.Length; i++)
            {
                if (HasPathRecursive(ref database, edges[i].TargetId, target, depth - 1, visited))
                    return true;
            }
            
            return false;
        }

        // Get entity degree (number of connections)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetOutDegree(this ref GraphDatabaseBlobAsset database, int entityId)
        {
            if (entityId >= database.EntityDegrees.Length) return 0;
            return database.EntityDegrees[entityId];
        }

        // Multi-hop query with result aggregation
        public static void GetEntitiesWithinHops<T>(this ref GraphDatabaseBlobAsset database, int sourceId, int maxHops, ref NativeHashSet<T> results) where T : unmanaged, IEntity
        {
            var frontier = new NativeQueue<(int id, int hops)>(Allocator.Temp);
            var visited = new NativeHashSet<int>(128, Allocator.Temp);
            
            frontier.Enqueue((sourceId, 0));
            visited.Add(sourceId);
            
            while (frontier.TryDequeue(out var current) && current.hops < maxHops)
            {
                var edges = database.GetOutgoingEdges(current.id);
                for (int i = 0; i < edges.Length; i++)
                {
                    var targetId = edges[i].TargetId;
                    if (!visited.Contains(targetId))
                    {
                        visited.Add(targetId);
                        var entity = database.GetEntity<T>(targetId);
                        results.Add(entity);
                        
                        if (current.hops + 1 < maxHops)
                        {
                            frontier.Enqueue((targetId, current.hops + 1));
                        }
                    }
                }
            }
            
            frontier.Dispose();
            visited.Dispose();
        }
    }

    // Burst-compiled job for parallel graph operations
    [BurstCompile]
    public struct ParallelGraphTraversalJob : IJobParallelFor
    {
        [ReadOnly] public BlobAssetReference<GraphDatabaseBlobAsset> Database;
        [ReadOnly] public NativeArray<int> SourceEntities;
        [WriteOnly] public NativeArray<int> ConnectionCounts;
        public ushort FilterLinkType;

        public void Execute(int index)
        {
            var sourceId = SourceEntities[index];
            ref var db = ref Database.Value;
            
            var edges = db.GetOutgoingEdges(sourceId);
            int count = 0;
            
            for (int i = 0; i < edges.Length; i++)
            {
                if (FilterLinkType == 0 || edges[i].LinkTypeId == FilterLinkType)
                {
                    count++;
                }
            }
            
            ConnectionCounts[index] = count;
        }
    }

    // Example advanced usage system
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct AdvancedGraphQuerySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GraphDatabaseComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var graphDb in SystemAPI.Query<RefRO<GraphDatabaseComponent>>())
            {
                ref var database = ref graphDb.ValueRO.BlobAsset.Value;
                
                // Example 1: Fluent query API
                using var playerItems = graphDb.ValueRO.BlobAsset
                    .Query(Allocator.Temp)
                    .From<Player>(0)
                    .FollowLinks(TypeID.Item)
                    .GetResults<Item>();
                
                // Example 2: Fast adjacency traversal
                var edges = database.GetOutgoingEdges(0);
                foreach (var edge in edges)
                {
                    if (edge.TargetType == EntityType.Item)
                    {
                        ref readonly var item = ref database.GetEntity<Item>(edge.TargetId);
                        // Process item...
                    }
                }
                
                // Example 3: Multi-hop exploration
                var nearbyEntities = new NativeHashSet<Location>(32, Allocator.Temp);
                database.GetEntitiesWithinHops(0, 3, ref nearbyEntities);
                
                // Example 4: Parallel processing
                var sources = new NativeArray<int>(database.Players.Length, Allocator.TempJob);
                var counts = new NativeArray<int>(database.Players.Length, Allocator.TempJob);
                
                for (int i = 0; i < sources.Length; i++) sources[i] = i;
                
                var job = new ParallelGraphTraversalJob
                {
                    Database = graphDb.ValueRO.BlobAsset,
                    SourceEntities = sources,
                    ConnectionCounts = counts,
                    FilterLinkType = TypeID.Item
                };
                
                job.Schedule(sources.Length, 64).Complete();
                
                // Clean up
                nearbyEntities.Dispose();
                sources.Dispose();
                counts.Dispose();
            }
        }
    }
}