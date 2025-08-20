// using System;
// using Unity.Collections;
// using Unity.Entities;
// using UnityEngine;
// using GraphTookKitDB.Runtime;
// using GraphToolKitDB.Runtime;
//
// namespace GraphTookKitDB.Runtime
// {
//     // Blob asset structure for the entire graph database
//     public struct GraphDatabaseBlobAsset
//     {
//         // Entity Collections (ID = index in these arrays)
//         public BlobArray<Achievement> Achievements;
//         public BlobArray<AchievementState> AchievementStates;
//         public BlobArray<AI> AIs;
//         public BlobArray<AIState> AIStates;
//         public BlobArray<Character> Characters;
//         public BlobArray<CharacterState> CharacterStates;
//         public BlobArray<Combat> Combats;
//         public BlobArray<CombatState> CombatStates;
//         public BlobArray<Description> Descriptions;
//         public BlobArray<Economy> Economies;
//         public BlobArray<EconomyState> EconomyStates;
//         public BlobArray<Effect> Effects;
//         public BlobArray<EffectState> EffectStates;
//         public BlobArray<Equipment> Equipments;
//         public BlobArray<Guild> Guilds;
//         public BlobArray<GuildState> GuildStates;
//         public BlobArray<Inventory> Inventories;
//         public BlobArray<InventoryState> InventoryStates;
//         public BlobArray<Item> Items;
//         public BlobArray<Link> Links;
//         public BlobArray<Location> Locations;
//         public BlobArray<Mission> Missions;
//         public BlobArray<MissionState> MissionStates;
//         public BlobArray<Name> Names;
//         public BlobArray<Objective> Objectives;
//         public BlobArray<ObjectiveState> ObjectiveStates;
//         public BlobArray<Player> Players;
//         public BlobArray<Quest> Quests;
//         public BlobArray<QuestState> QuestStates;
//         public BlobArray<RangeAsFloat> RangeAsFloats;
//         public BlobArray<RangeAsInt> RangeAsInts;
//         public BlobArray<Reward> Rewards;
//         public BlobArray<Skill> Skills;
//         public BlobArray<SkillState> SkillStates;
//         public BlobArray<Stat> Stats;
//         public BlobArray<StatState> StatStates;
//         public BlobArray<Tag> Tags;
//         public BlobArray<TimeState> TimeStates;
//         public BlobArray<TimeTick> TimeTicks;
//
//         // Link indexing for graph traversal
//         public BlobArray<int> LinksBySourceId;     // Links sorted by source entity ID
//         public BlobArray<int> LinksByTargetId;     // Links sorted by target entity ID
//         public BlobArray<LinkRange> SourceRanges;  // Range info for each source ID
//         public BlobArray<LinkRange> TargetRanges;  // Range info for each target ID
//
//         // Metadata
//         public int TotalEntities;
//         public int TotalLinks;
//         public long CreationTimestamp;
//         public int Version;
//     }
//
//     // Helper struct for link ranges
//     public struct LinkRange
//     {
//         public int StartIndex; // Start index in the link array
//         public int Count;      // Number of links for this entity
//     }
//
//     // Component to reference the blob asset in ECS
//     public struct GraphDatabaseComponent : IComponentData
//     {
//         public BlobAssetReference<GraphDatabaseBlobAsset> BlobAsset;
//     }
//
//     // Builder class to create the blob asset from GDBAsset
//     public static class GraphDatabaseBlobBuilder
//     {
//         public static BlobAssetReference<GraphDatabaseBlobAsset> CreateBlobAsset(GDBAsset sourceAsset)
//         {
//             using var builder = new BlobBuilder(Allocator.Temp);
//             ref var root = ref builder.ConstructRoot<GraphDatabaseBlobAsset>();
//
//             // Build entity arrays (straightforward copy since ID = index)
//             BuildEntityArray(builder, ref root.Achievements, sourceAsset.Achievements);
//             BuildEntityArray(builder, ref root.AchievementStates, sourceAsset.AchievementStates);
//             BuildEntityArray(builder, ref root.AIs, sourceAsset.AIs);
//             BuildEntityArray(builder, ref root.AIStates, sourceAsset.AIStates);
//             BuildEntityArray(builder, ref root.Characters, sourceAsset.Characters);
//             BuildEntityArray(builder, ref root.CharacterStates, sourceAsset.CharacterStates);
//             BuildEntityArray(builder, ref root.Combats, sourceAsset.Combats);
//             BuildEntityArray(builder, ref root.CombatStates, sourceAsset.CombatStates);
//             BuildEntityArray(builder, ref root.Descriptions, sourceAsset.Descriptions);
//             BuildEntityArray(builder, ref root.Economies, sourceAsset.Economys);
//             BuildEntityArray(builder, ref root.EconomyStates, sourceAsset.EconomyStates);
//             BuildEntityArray(builder, ref root.Effects, sourceAsset.Effects);
//             BuildEntityArray(builder, ref root.EffectStates, sourceAsset.EffectStates);
//             BuildEntityArray(builder, ref root.Equipments, sourceAsset.Equipments);
//             BuildEntityArray(builder, ref root.Guilds, sourceAsset.Guilds);
//             BuildEntityArray(builder, ref root.GuildStates, sourceAsset.GuildStates);
//             BuildEntityArray(builder, ref root.Inventories, sourceAsset.Inventorys);
//             BuildEntityArray(builder, ref root.InventoryStates, sourceAsset.InventoryStates);
//             BuildEntityArray(builder, ref root.Items, sourceAsset.Items);
//             BuildEntityArray(builder, ref root.Links, sourceAsset.Links);
//             BuildEntityArray(builder, ref root.Locations, sourceAsset.Locations);
//             BuildEntityArray(builder, ref root.Missions, sourceAsset.Missions);
//             BuildEntityArray(builder, ref root.MissionStates, sourceAsset.MissionStates);
//             BuildEntityArray(builder, ref root.Names, sourceAsset.Names);
//             BuildEntityArray(builder, ref root.Objectives, sourceAsset.Objectives);
//             BuildEntityArray(builder, ref root.ObjectiveStates, sourceAsset.ObjectiveStates);
//             BuildEntityArray(builder, ref root.Players, sourceAsset.Players);
//             BuildEntityArray(builder, ref root.Quests, sourceAsset.Quests);
//             BuildEntityArray(builder, ref root.QuestStates, sourceAsset.QuestStates);
//             BuildEntityArray(builder, ref root.RangeAsFloats, sourceAsset.RangeAsFloats);
//             BuildEntityArray(builder, ref root.RangeAsInts, sourceAsset.RangeAsInts);
//             BuildEntityArray(builder, ref root.Rewards, sourceAsset.Rewards);
//             BuildEntityArray(builder, ref root.Skills, sourceAsset.Skills);
//             BuildEntityArray(builder, ref root.SkillStates, sourceAsset.SkillStates);
//             BuildEntityArray(builder, ref root.Stats, sourceAsset.Stats);
//             BuildEntityArray(builder, ref root.StatStates, sourceAsset.StatStates);
//             BuildEntityArray(builder, ref root.Tags, sourceAsset.Tags);
//             BuildEntityArray(builder, ref root.TimeStates, sourceAsset.TimeStates);
//             BuildEntityArray(builder, ref root.TimeTicks, sourceAsset.TimeTicks);
//
//             // Build link indexing for graph traversal
//             BuildLinkIndexing(builder, ref root, sourceAsset.Links);
//
//             // Set metadata
//             root.TotalEntities = CalculateTotalEntities(sourceAsset);
//             root.TotalLinks = sourceAsset.Links.Count;
//             root.CreationTimestamp = DateTime.Now.Ticks;
//             root.Version = 1;
//
//             return builder.CreateBlobAssetReference<GraphDatabaseBlobAsset>(Allocator.Persistent);
//         }
//
//         private static void BuildEntityArray<T>(BlobBuilder builder, ref BlobArray<T> blobArray, System.Collections.Generic.List<T> sourceList) where T : struct
//         {
//             var arrayBuilder = builder.Allocate(ref blobArray, sourceList.Count);
//             for (int i = 0; i < sourceList.Count; i++)
//             {
//                 arrayBuilder[i] = sourceList[i];
//             }
//         }
//
//         private static void BuildLinkIndexing(BlobBuilder builder, ref GraphDatabaseBlobAsset root, System.Collections.Generic.List<Link> links)
//         {
//             if (links.Count == 0)
//             {
//                 builder.Allocate(ref root.LinksBySourceId, 0);
//                 builder.Allocate(ref root.LinksByTargetId, 0);
//                 builder.Allocate(ref root.SourceRanges, 0);
//                 builder.Allocate(ref root.TargetRanges, 0);
//                 return;
//             }
//
//             // Find max entity IDs to determine range array sizes
//             int maxSourceId = 0, maxTargetId = 0;
//             foreach (var link in links)
//             {
//                 if (link.SourceID > maxSourceId) maxSourceId = link.SourceID;
//                 if (link.TargetID > maxTargetId) maxTargetId = link.TargetID;
//             }
//
//             // Build source-based indexing
//             var sourceGroups = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>();
//             for (int i = 0; i < links.Count; i++)
//             {
//                 var sourceId = links[i].SourceID;
//                 if (!sourceGroups.ContainsKey(sourceId))
//                     sourceGroups[sourceId] = new System.Collections.Generic.List<int>();
//                 sourceGroups[sourceId].Add(i); // Store link indices
//             }
//
//             // Build target-based indexing
//             var targetGroups = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>();
//             for (int i = 0; i < links.Count; i++)
//             {
//                 var targetId = links[i].TargetID;
//                 if (!targetGroups.ContainsKey(targetId))
//                     targetGroups[targetId] = new System.Collections.Generic.List<int>();
//                 targetGroups[targetId].Add(i); // Store link indices
//             }
//
//             // Create source link index array and ranges
//             var sourceLinkIndices = new System.Collections.Generic.List<int>();
//             var sourceRanges = builder.Allocate(ref root.SourceRanges, maxSourceId + 1);
//             
//             for (int entityId = 0; entityId <= maxSourceId; entityId++)
//             {
//                 if (sourceGroups.TryGetValue(entityId, out var linkIndices))
//                 {
//                     sourceRanges[entityId] = new LinkRange 
//                     { 
//                         StartIndex = sourceLinkIndices.Count, 
//                         Count = linkIndices.Count 
//                     };
//                     sourceLinkIndices.AddRange(linkIndices);
//                 }
//                 else
//                 {
//                     sourceRanges[entityId] = new LinkRange { StartIndex = -1, Count = 0 };
//                 }
//             }
//
//             // Create target link index array and ranges
//             var targetLinkIndices = new System.Collections.Generic.List<int>();
//             var targetRanges = builder.Allocate(ref root.TargetRanges, maxTargetId + 1);
//             
//             for (int entityId = 0; entityId <= maxTargetId; entityId++)
//             {
//                 if (targetGroups.TryGetValue(entityId, out var linkIndices))
//                 {
//                     targetRanges[entityId] = new LinkRange 
//                     { 
//                         StartIndex = targetLinkIndices.Count, 
//                         Count = linkIndices.Count 
//                     };
//                     targetLinkIndices.AddRange(linkIndices);
//                 }
//                 else
//                 {
//                     targetRanges[entityId] = new LinkRange { StartIndex = -1, Count = 0 };
//                 }
//             }
//
//             // Build the blob arrays
//             var sourceArray = builder.Allocate(ref root.LinksBySourceId, sourceLinkIndices.Count);
//             for (int i = 0; i < sourceLinkIndices.Count; i++)
//             {
//                 sourceArray[i] = sourceLinkIndices[i];
//             }
//
//             var targetArray = builder.Allocate(ref root.LinksByTargetId, targetLinkIndices.Count);
//             for (int i = 0; i < targetLinkIndices.Count; i++)
//             {
//                 targetArray[i] = targetLinkIndices[i];
//             }
//         }
//
//         private static int CalculateTotalEntities(GDBAsset asset)
//         {
//             return asset.Achievements.Count + asset.AchievementStates.Count + asset.AIs.Count +
//                    asset.AIStates.Count + asset.Characters.Count + asset.CharacterStates.Count +
//                    asset.Combats.Count + asset.CombatStates.Count + asset.Descriptions.Count +
//                    asset.Economys.Count + asset.EconomyStates.Count + asset.Effects.Count +
//                    asset.EffectStates.Count + asset.Equipments.Count + asset.Guilds.Count +
//                    asset.GuildStates.Count + asset.Inventorys.Count + asset.InventoryStates.Count +
//                    asset.Items.Count + asset.Links.Count + asset.Locations.Count +
//                    asset.Missions.Count + asset.MissionStates.Count + asset.Names.Count +
//                    asset.Objectives.Count + asset.ObjectiveStates.Count + asset.Players.Count +
//                    asset.Quests.Count + asset.QuestStates.Count + asset.RangeAsFloats.Count +
//                    asset.RangeAsInts.Count + asset.Rewards.Count + asset.Skills.Count +
//                    asset.SkillStates.Count + asset.Stats.Count + asset.StatStates.Count +
//                    asset.Tags.Count + asset.TimeStates.Count + asset.TimeTicks.Count;
//         }
//     }
//
//     // Utility extension methods for easy access
//     public static class GraphDatabaseBlobExtensions
//     {
//         // Direct entity access by ID (since ID = index)
//         public static ref readonly T GetEntity<T>(this ref GraphDatabaseBlobAsset database, int entityId) where T : struct
//         {
//             // You'll need to implement type-specific access based on T
//             // This is a template - implement for each entity type
//             if (typeof(T) == typeof(Player))
//                 return ref (T)(object)database.Players[entityId];
//             if (typeof(T) == typeof(Character))
//                 return ref (T)(object)database.Characters[entityId];
//             if (typeof(T) == typeof(Item))
//                 return ref (T)(object)database.Items[entityId];
//             // Add other entity types...
//             
//             throw new System.ArgumentException($"Entity type {typeof(T)} not supported");
//         }
//
//         // Get all outgoing links for an entity
//         public static void GetOutgoingLinks(this ref GraphDatabaseBlobAsset database, int sourceEntityId, ref NativeList<Link> results)
//         {
//             results.Clear();
//             
//             if (sourceEntityId >= database.SourceRanges.Length) return;
//             
//             var range = database.SourceRanges[sourceEntityId];
//             if (range.Count == 0) return;
//
//             for (int i = 0; i < range.Count; i++)
//             {
//                 var linkIndex = database.LinksBySourceId[range.StartIndex + i];
//                 results.Add(database.Links[linkIndex]);
//             }
//         }
//
//         // Get all incoming links for an entity
//         public static void GetIncomingLinks(this ref GraphDatabaseBlobAsset database, int targetEntityId, ref NativeList<Link> results)
//         {
//             results.Clear();
//             
//             if (targetEntityId >= database.TargetRanges.Length) return;
//             
//             var range = database.TargetRanges[targetEntityId];
//             if (range.Count == 0) return;
//
//             for (int i = 0; i < range.Count; i++)
//             {
//                 var linkIndex = database.LinksByTargetId[range.StartIndex + i];
//                 results.Add(database.Links[linkIndex]);
//             }
//         }
//
//         // Check if two entities are connected
//         public static bool AreConnected(this ref GraphDatabaseBlobAsset database, int sourceId, int targetId, ushort linkType = 0)
//         {
//             if (sourceId >= database.SourceRanges.Length) return false;
//             
//             var range = database.SourceRanges[sourceId];
//             if (range.Count == 0) return false;
//
//             for (int i = 0; i < range.Count; i++)
//             {
//                 var linkIndex = database.LinksBySourceId[range.StartIndex + i];
//                 var link = database.Links[linkIndex];
//                 
//                 if (link.TargetID == targetId && (linkType == 0 || link.LinkTypeID == linkType))
//                     return true;
//             }
//             
//             return false;
//         }
//
//         // Get connected entities of specific type
//         public static void GetConnectedEntities<T>(this ref GraphDatabaseBlobAsset database, int sourceEntityId, EntityType targetType, ref NativeList<T> results) where T : struct, IEntity
//         {
//             results.Clear();
//             
//             var tempLinks = new NativeList<Link>(16, Allocator.Temp);
//             database.GetOutgoingLinks(sourceEntityId, ref tempLinks);
//             
//             foreach (var link in tempLinks)
//             {
//                 if (link.TargetType == targetType)
//                 {
//                     var entity = database.GetEntity<T>(link.TargetID);
//                     results.Add(entity);
//                 }
//             }
//             
//             tempLinks.Dispose();
//         }
//     }
//
//     // Example ISystem that uses the blob asset
//     [UpdateInGroup(typeof(SimulationSystemGroup))]
//     public partial struct GraphDatabaseQuerySystem : ISystem
//     {
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<GraphDatabaseComponent>();
//         }
//
//         public void OnUpdate(ref SystemState state)
//         {
//             foreach (var graphDb in SystemAPI.Query<RefRO<GraphDatabaseComponent>>())
//             {
//                 ref var database = ref graphDb.ValueRO.BlobAsset.Value;
//                 
//                 // Example: Access player by ID (direct index access)
//                 if (database.Players.Length > 0)
//                 {
//                     ref readonly var player = ref database.Players[0]; // Player with ID 0
//                     Debug.Log($"Player Level: {player.Level}");
//                     
//                     // Find all items linked to this player
//                     var links = new NativeList<Link>(16, Allocator.Temp);
//                     database.GetOutgoingLinks(player.ID, ref links);
//                     
//                     foreach (var link in links)
//                     {
//                         if (link.TargetType == EntityType.Item)
//                         {
//                             ref readonly var item = ref database.Items[link.TargetID];
//                             Debug.Log($"Player has item: {item.ItemType}");
//                         }
//                     }
//                     
//                     links.Dispose();
//                 }
//             }
//         }
//     }
// }