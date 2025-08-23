using System;
using System.Collections.Generic;
using System.Linq;
using GraphTookKitDB.Runtime;
using Unity.Collections;
using UnityEngine;

namespace GraphToolKitDB.Runtime
{
    public interface IStateful<out T> where T : struct
    {
        T CreateState();
    }

// Core Entity Structs
    [Serializable]
    public struct Mission : IStateful<MissionState>
    {
        public int Priority;
        public bool IsRepeatable;
        public int MaxAttempts;

        public MissionState CreateState() => new MissionState
            { Attempts = 0, IsActive = false, StartTime = DateTime.MinValue };
    }

    [Serializable]
    public struct Quest : IStateful<QuestState>
    {
        public int ChapterID;
        public int Difficulty;
        public bool IsMainQuest;
        public QuestState CreateState() => new QuestState { Progress = 0f, IsCompleted = false };
    }

    [Serializable]
    public struct Objective : IStateful<ObjectiveState>
    {
        public int TargetValue;
        public ushort ObjectiveType;
        public bool IsOptional;
        public ObjectiveState CreateState() => new ObjectiveState { CurrentValue = 0, IsCompleted = false };
    }

    [Serializable]
    public struct Item
    {
        public ushort ItemType;
        public int Rarity;
        public int MaxStack;
        public float Weight;
        public int Value;
    }

    [Serializable]
    public struct Equipment
    {
        public ushort EquipSlot;
        public int Level;
        public int Durability;
        public int MaxDurability;
    }

    [Serializable]
    public struct Player
    {
        public int Level;
        public float Experience;
        public int PrestigeLevel;
        public DateTime CreatedAt;
    }

    [Serializable]
    public struct Character : IStateful<CharacterState>
    {
        public ushort CharacterClass;
        public int Level;
        public ushort Race;
        public ushort Gender;
        public CharacterState CreateState() => new CharacterState { Health = 100, Mana = 100, Stamina = 100 };
    }

    [Serializable]
    public struct Stat : IStateful<StatState>
    {
        public ushort StatType;
        public float BaseValue;
        public float MinValue;
        public float MaxValue;
        public StatState CreateState() => new StatState { CurrentValue = BaseValue, Modifiers = 0 };
    }

    [Serializable]
    public struct Location
    {
        public Vector3 Position;
        public Vector3 Scale;
        public Quaternion Rotation;
        public int ParentZoneID;
    }

    [Serializable]
    public struct TimeTick : IStateful<TimeState>
    {
        public long StartTicks;
        public long DurationTicks;
        public bool IsRepeating;
        public long IntervalTicks;
        public TimeState CreateState() => new TimeState { ElapsedTicks = 0, IsActive = false, CycleCount = 0 };
    }

    [Serializable]
    public struct RangeAsInt
    {
        public int MinValue;
        public int MaxValue;
        public int StepSize;
    }

    [Serializable]
    public struct RangeAsFloat
    {
        public float MinValue;
        public float MaxValue;
        public float Precision;
    }

    [Serializable]
    public struct Reward
    {
        public ushort RewardType;
        public int Amount;
        public float Chance;
        public int ItemID;
    }

    [Serializable]
    public struct Inventory : IStateful<InventoryState>
    {
        public int MaxSlots;
        public float MaxWeight;
        public ushort InventoryType;
        public InventoryState CreateState() => new InventoryState { UsedSlots = 0, CurrentWeight = 0 };
    }

    [Serializable]
    public struct Skill : IStateful<SkillState>
    {
        public ushort SkillType;
        public int MaxLevel;
        public float BaseCooldown;
        public int ManaCost;

        public SkillState CreateState() => new SkillState
            { Level = 1, Experience = 0, LastUsed = DateTime.MinValue };
    }

    [Serializable]
    public struct Effect : IStateful<EffectState>
    {
        public ushort EffectType;
        public float Magnitude;
        public long DurationTicks;
        public bool IsStackable;
        public EffectState CreateState() => new EffectState { Stacks = 1, RemainingTicks = DurationTicks };
    }

    [Serializable]
    public struct Combat : IStateful<CombatState>
    {
        public float BaseDamage;
        public float AttackSpeed;
        public float CritChance;
        public float CritMultiplier;

        public CombatState CreateState() => new CombatState
            { IsInCombat = false, LastAttack = DateTime.MinValue };
    }

    [Serializable]
    public struct Economy : IStateful<EconomyState>
    {
        public int BasePrice;
        public float Inflation;
        public float Supply;
        public float Demand;

        public EconomyState CreateState() => new EconomyState
            { CurrentPrice = BasePrice, LastUpdate = DateTime.Now };
    }

    [Serializable]
    public struct AI : IStateful<AIState>
    {
        public ushort BehaviorType;
        public float AggroRange;
        public float PatrolRadius;
        public int Priority;
        public AIState CreateState() => new AIState { CurrentTarget = 0, LastAction = DateTime.MinValue };
    }

    [Serializable]
    public struct Achievement : IStateful<AchievementState>
    {
        public ushort AchievementType;
        public int Points;
        public bool IsHidden;
        public DateTime UnlockDate;
        public AchievementState CreateState() => new AchievementState { Progress = 0f, IsUnlocked = false };
    }

    [Serializable]
    public struct Guild : IStateful<GuildState>
    {
        public int MaxMembers;
        public int Level;
        public long Experience;
        public DateTime CreatedAt;
        public GuildState CreateState() => new GuildState { MemberCount = 0, Treasury = 0 };
    }

// State Structs
    [Serializable]
    public struct MissionState
    {
        public int Attempts;
        public bool IsActive;
        public DateTime StartTime;
    }

    [Serializable]
    public struct QuestState
    {
        public float Progress;
        public bool IsCompleted;
    }

    [Serializable]
    public struct ObjectiveState
    {
        public int CurrentValue;
        public bool IsCompleted;
    }

    [Serializable]
    public struct CharacterState
    {
        public float Health;
        public float Mana;
        public float Stamina;
    }

    [Serializable]
    public struct StatState
    {
        public float CurrentValue;
        public float Modifiers;
    }

    [Serializable]
    public struct TimeState
    {
        public long ElapsedTicks;
        public bool IsActive;
        public int CycleCount;
    }

    [Serializable]
    public struct InventoryState
    {
        public int UsedSlots;
        public float CurrentWeight;
    }

    [Serializable]
    public struct SkillState
    {
        public int Level;
        public float Experience;
        public DateTime LastUsed;
    }

    [Serializable]
    public struct EffectState
    {
        public int Stacks;
        public long RemainingTicks;
    }

    [Serializable]
    public struct CombatState
    {
        public bool IsInCombat;
        public DateTime LastAttack;
    }

    [Serializable]
    public struct EconomyState
    {
        public int CurrentPrice;
        public DateTime LastUpdate;
    }

    [Serializable]
    public struct AIState
    {
        public int CurrentTarget;
        public DateTime LastAction;
    }

    [Serializable]
    public struct AchievementState
    {
        public float Progress;
        public bool IsUnlocked;
    }

    [Serializable]
    public struct GuildState
    {
        public int MemberCount;
        public long Treasury;
    }

// Meta Structs
    [Serializable]
    public struct Name
    {
        public FixedString32Bytes Text;
        public ushort LanguageID;
    }

    [Serializable]
    public struct Description
    {
        public FixedString64Bytes Text;
        public ushort LanguageID;
    }

    [Serializable]
    public struct Tag
    {
        public ushort TagType;
        public int Value;
    }

// Link System
    [Serializable]
    public struct Link
    {
        public ushort Id;

        public ushort ID
        {
            get => Id;
            set => Id = value;
        }

        public EntityType SourceType;
        public ushort SourceID;
        public EntityType TargetType;
        public ushort TargetID;
    }
}