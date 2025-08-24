using System;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

// Component to mark stats for removal
public struct DestroyComponent : IComponentData, IEnableableComponent
{
    public bool Value;
}

// Component that references index in stats buffer
public struct StatRefComponent : IComponentData
{
    public int Index;
}

// Your stat data structure
[Serializable]
public struct PlayerStat : IBufferElementData
{
    public float Health;
    public float Mana;

    public float Damage;
    // ... other stat fields
}

// // System to handle buffer compaction
// [UpdateInGroup(typeof(SimulationSystemGroup))]
// [UpdateBefore(typeof(BeginSimulationEntityCommandBufferSystem))]
// public partial struct StatBufferCompactionSystem : ISystem
// {
//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
//             .CreateCommandBuffer(state.WorldUnmanaged);
//
//         // Get the singleton entity that holds the stats buffer
//         var statsEntity = SystemAPI.GetSingletonEntity<PlayerStat>();
//         var statsBuffer = SystemAPI.GetBuffer<PlayerStat>(statsEntity);
//
//         // Create a mapping from old indices to new indices
//         var indexMapping = new NativeArray<int>(statsBuffer.Length, Allocator.TempJob);
//         var compactedStats = new NativeList<PlayerStat>(statsBuffer.Length, Allocator.TempJob);
//
//         // Job to create the compacted buffer and index mapping
//         var compactionJob = new BufferCompactionJob
//         {
//             OriginalStats = statsBuffer.AsNativeArray(),
//             IndexMapping = indexMapping,
//             CompactedStats = compactedStats
//         };
//
//         var jobHandle = compactionJob.Schedule();
//         jobHandle.Complete();
//
//         // Update the stats buffer with compacted data
//         statsBuffer.Clear();
//         foreach (var stat in compactedStats.AsArray())
//         {
//             statsBuffer.Add(stat);
//         }
//
//         // Update all player references using the index mapping
//         foreach (var (statRef, entity) in SystemAPI.Query<RefRW<StatRefComponent>>().WithEntityAccess())
//         {
//             var oldIndex = statRef.ValueRO.Index;
//             var newIndex = indexMapping[oldIndex];
//
//             if (newIndex == -1)
//             {
//                 // This player's stats were removed, destroy the entity
//                 ecb.DestroyEntity(entity);
//             }
//             else
//             {
//                 // Update to new index
//                 statRef.ValueRW.Index = newIndex;
//             }
//         }
//
//         // Clean up
//         indexMapping.Dispose();
//         compactedStats.Dispose();
//     }
// }
//
// [BurstCompile]
// public struct BufferCompactionJob : IJob
// {
//     [ReadOnly] public NativeArray<PlayerStat> OriginalStats;
//     public NativeArray<int> IndexMapping;
//     public NativeList<PlayerStat> CompactedStats;
//
//     public void Execute()
//     {
//         int writeIndex = 0;
//
//         // Create mapping and compact the array
//         for (int readIndex = 0; readIndex < OriginalStats.Length; readIndex++)
//         {
//             // Check if this stat should be kept (you'd need to track which indices are marked for removal)
//             // For now, assuming you have a way to determine if index should be removed
//             if (ShouldKeepStat(readIndex))
//             {
//                 IndexMapping[readIndex] = writeIndex;
//                 CompactedStats.Add(OriginalStats[readIndex]);
//                 writeIndex++;
//             }
//             else
//             {
//                 IndexMapping[readIndex] = -1; // Mark as removed
//             }
//         }
//     }
//
//     private bool ShouldKeepStat(int index)
//     {
//         // This would need to be implemented based on your logic
//         // You might pass in a NativeArray<bool> marking which indices to remove
//         // or use another approach to determine if the stat should be kept
//         return true; // Placeholder
//     }
// }

// Alternative approach using a free list for better performance
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct StatBufferFreeListSystem : ISystem
{
    private NativeList<int> freeIndices;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        freeIndices = new NativeList<int>(Allocator.Persistent);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        if (freeIndices.IsCreated)
            freeIndices.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        var statsEntity = SystemAPI.GetSingletonEntity<PlayerStat>();
        var statsBuffer = SystemAPI.GetBuffer<PlayerStat>(statsEntity);

        // Collect indices to free from destroyed players
        foreach (var (statRef, destroy, entity) in
                 SystemAPI.Query<RefRO<StatRefComponent>, RefRO<DestroyComponent>>()
                     .WithEntityAccess())
        {
            if (destroy.ValueRO.Value)
            {
                // Add index to free list
                freeIndices.Add(statRef.ValueRO.Index);

                // Destroy the entity
                ecb.DestroyEntity(entity);
            }
        }

        // Sort free indices in descending order for efficient removal from end
        if (freeIndices.Length > 0)
        {
            var sortJob = new SortFreeIndicesJob { FreeIndices = freeIndices };
            var jobHandle = sortJob.Schedule();
            jobHandle.Complete();

            // Remove from buffer (from end to beginning to avoid index shifting issues)
            for (int i = 0; i < freeIndices.Length; i++)
            {
                int indexToRemove = freeIndices[i];

                // Move last element to this position if it's not the last element
                if (indexToRemove < statsBuffer.Length - 1)
                {
                    statsBuffer[indexToRemove] = statsBuffer[statsBuffer.Length - 1];

                    // Update any player that was referencing the moved element
                    UpdateMovedReference(ref state, statsBuffer.Length - 1, indexToRemove);
                }

                // Remove the last element
                statsBuffer.RemoveAt(statsBuffer.Length - 1);
            }

            freeIndices.Clear();
        }
    }

    private void UpdateMovedReference(ref SystemState state, int oldIndex, int newIndex)
    {
        foreach (var statRef in SystemAPI.Query<RefRW<StatRefComponent>>())
        {
            if (statRef.ValueRO.Index == oldIndex)
            {
                statRef.ValueRW.Index = newIndex;
                break; // Assuming only one player per stat index
            }
        }
    }
}

[BurstCompile]
public struct SortFreeIndicesJob : IJob
{
    public NativeList<int> FreeIndices;

    public void Execute()
    {
        // Simple selection sort for descending order
        for (int i = 0; i < FreeIndices.Length - 1; i++)
        {
            int maxIndex = i;
            for (int j = i + 1; j < FreeIndices.Length; j++)
            {
                if (FreeIndices[j] > FreeIndices[maxIndex])
                    maxIndex = j;
            }

            if (maxIndex != i)
            {
                (FreeIndices[i], FreeIndices[maxIndex]) = (FreeIndices[maxIndex], FreeIndices[i]);
            }
        }
    }
}