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
            foreach (var indexToRemove in freeIndices)
            {
                // Move last element to this position if it's not the last element
                var lastStatIndex = statsBuffer.Length - 1;
                if (indexToRemove < lastStatIndex)
                {
                    statsBuffer[indexToRemove] = statsBuffer[lastStatIndex];

                    // Update any player that was referencing the moved element
                    UpdateMovedReference(ref state, lastStatIndex, indexToRemove);
                }

                // Remove the last element
                statsBuffer.RemoveAt(lastStatIndex);
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