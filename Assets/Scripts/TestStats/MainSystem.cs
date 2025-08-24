using System;
using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using UnityEngine;

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

    public override string ToString()
    {
        return $"Health: {Health:F1}, Mana: {Mana:F1}, Damage: {Damage:F1}";
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
[BurstCompile]
public partial struct OptimizedStatBufferSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var statsBuffer = SystemAPI.GetSingletonBuffer<PlayerStat>();
        var statsBufferLength = statsBuffer.Length;
        if (statsBufferLength == 0) return;

        // Step 1: Collect indices to remove in a single job
        var safeLength = statsBufferLength < 4 ? statsBufferLength : statsBufferLength / 4;
        var indicesToRemove = new NativeList<int>(safeLength, Allocator.TempJob);
        var entitiesToDestroy = new NativeList<Entity>(safeLength, Allocator.TempJob);

        var collectJob = new CollectDestroyedJob
        {
            IndicesToRemove = indicesToRemove.AsParallelWriter(),
            EntitiesToDestroy = entitiesToDestroy.AsParallelWriter()
        };
        var collectJobHandle = collectJob.ScheduleParallel(state.Dependency);
        collectJobHandle.Complete();

        if (indicesToRemove.Length == 0)
        {
            indicesToRemove.Dispose();
            entitiesToDestroy.Dispose();
            return;
        }

        // Step 2: Create O(1) lookup table for index remapping
        var indexRemap = new NativeArray<int>(statsBufferLength, Allocator.TempJob);
        var writeIndex = MapIndices(ref statsBuffer, ref indexRemap);
        statsBuffer.ResizeUninitialized(writeIndex);

        // Step 4: Update all player indices with O(1) lookup
        var updateJob = new UpdateIndicesJob { IndexRemap = indexRemap };
        updateJob.ScheduleParallel();

        // Step 5: Destroy entities
        state.EntityManager.DestroyEntity(entitiesToDestroy.AsArray());

        // Cleanup
        entitiesToDestroy.Dispose();
        indexRemap.Dispose();
    }

    [BurstCompile]
    partial struct CollectDestroyedJob : IJobEntity
    {
        [WriteOnly] public NativeList<int>.ParallelWriter IndicesToRemove;
        [WriteOnly] public NativeList<Entity>.ParallelWriter EntitiesToDestroy;

        void Execute(Entity entity, in StatRefComponent statRef, in DestroyComponent destroy)
        {
            if (destroy.Value)
            {
                IndicesToRemove.AddNoResize(statRef.Index);
                EntitiesToDestroy.AddNoResize(entity);
            }
        }
    }

    private static int MapIndices(
        ref DynamicBuffer<PlayerStat> statsBuffer,
        ref NativeArray<int> indexRemap
    )
    {
        var removalFlags = new NativeBitArray(statsBuffer.Length, Allocator.Temp);
        // Initialize with identity mapping and mark removals
        for (int i = 0; i < statsBuffer.Length; i++) indexRemap[i] = i;

        // Step 3: Compact buffer and update lookup table in single pass
        int writeIndex = 0;
        for (int readIndex = 0; readIndex < statsBuffer.Length; readIndex++)
        {
            if (removalFlags.IsSet(readIndex))
            {
                indexRemap[readIndex] = -1; // Mark as removed
                continue;
            }

            if (writeIndex != readIndex) statsBuffer[writeIndex] = statsBuffer[readIndex];
            indexRemap[readIndex] = writeIndex;
            writeIndex++;
        }

        removalFlags.Dispose();
        return writeIndex;
    }
}


[BurstCompile]
partial struct UpdateIndicesJob : IJobEntity
{
    [ReadOnly] public NativeArray<int> IndexRemap;

    void Execute(ref StatRefComponent statRef)
    {
        var oldIndex = statRef.Index;
        if (oldIndex >= 0 && oldIndex < IndexRemap.Length)
        {
            var newIndex = IndexRemap[oldIndex];
            if (newIndex >= 0) // -1 means removed
                statRef.Index = newIndex;
        }
    }
}


// Debug system (unchanged)
[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct DebugStatLoggingSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (UnityEngine.Time.frameCount % 60 != 0) return;

        var statsBuffer = SystemAPI.GetSingletonBuffer<PlayerStat>();

        foreach (var (statRef, entity) in SystemAPI.Query<RefRO<StatRefComponent>>().WithEntityAccess())
        {
            if (statRef.ValueRO.Index >= 0 && statRef.ValueRO.Index < statsBuffer.Length)
            {
                var stats = statsBuffer[statRef.ValueRO.Index];
                Debug.Log($"Entity {entity}: {stats}");
            }
        }
    }
}