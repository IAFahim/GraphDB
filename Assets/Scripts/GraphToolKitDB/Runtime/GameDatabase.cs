using System;
using System.Collections.Generic;
using System.Linq;
using GraphTookKitDB.Runtime;
using Unity.Collections;
using UnityEngine;

namespace GraphToolKitDB.Runtime
{
    // Core Interface
    public interface IEntity
    {
        int ID { get; set; }
    }

    public interface IStateful<out T> where T : struct
    {
        T CreateState();
    }

// Core Entity Structs
    [Serializable]
    public struct Mission : IEntity, IStateful<MissionState>
        , IEquatable<Mission>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int Priority;
        public bool IsRepeatable;
        public int MaxAttempts;

        public MissionState CreateState() => new MissionState
            { ID = ID, Attempts = 0, IsActive = false, StartTime = DateTime.MinValue };

        public bool Equals(Mission other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Mission other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Quest : IEntity, IStateful<QuestState>
        , IEquatable<Quest>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int ChapterID;
        public int Difficulty;
        public bool IsMainQuest;
        public QuestState CreateState() => new QuestState { ID = ID, Progress = 0f, IsCompleted = false };

        public bool Equals(Quest other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Quest other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Objective : IEntity, IStateful<ObjectiveState>
        , IEquatable<Objective>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int TargetValue;
        public ushort ObjectiveType;
        public bool IsOptional;
        public ObjectiveState CreateState() => new ObjectiveState { ID = ID, CurrentValue = 0, IsCompleted = false };

        public bool Equals(Objective other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Objective other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Item : IEntity
        , IEquatable<Item>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public ushort ItemType;
        public int Rarity;
        public int MaxStack;
        public float Weight;
        public int Value;

        public bool Equals(Item other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Item other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Equipment : IEntity
        , IEquatable<Equipment>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public ushort EquipSlot;
        public int Level;
        public int Durability;
        public int MaxDurability;

        public bool Equals(Equipment other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Equipment other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Player : IEntity
        , IEquatable<Player>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int Level;
        public float Experience;
        public int PrestigeLevel;
        public DateTime CreatedAt;

        public bool Equals(Player other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Player other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Character : IEntity, IStateful<CharacterState>
        , IEquatable<Character>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public ushort CharacterClass;
        public int Level;
        public ushort Race;
        public ushort Gender;
        public CharacterState CreateState() => new CharacterState { ID = ID, Health = 100, Mana = 100, Stamina = 100 };

        public bool Equals(Character other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Character other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Stat : IEntity, IStateful<StatState>
        , IEquatable<Stat>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public ushort StatType;
        public float BaseValue;
        public float MinValue;
        public float MaxValue;
        public StatState CreateState() => new StatState { ID = ID, CurrentValue = BaseValue, Modifiers = 0 };

        public bool Equals(Stat other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Stat other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Location : IEntity, IEquatable<Location>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public Vector3 Position;
        public Vector3 Scale;
        public Quaternion Rotation;
        public int ParentZoneID;

        public bool Equals(Location other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Location other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct TimeTick : IEntity, IStateful<TimeState>
        , IEquatable<TimeTick>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public long StartTicks;
        public long DurationTicks;
        public bool IsRepeating;
        public long IntervalTicks;
        public TimeState CreateState() => new TimeState { ID = ID, ElapsedTicks = 0, IsActive = false, CycleCount = 0 };

        public bool Equals(TimeTick other) => Id == other.Id;
        public override bool Equals(object obj) => obj is TimeTick other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct RangeAsInt : IEntity
        , IEquatable<RangeAsInt>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int MinValue;
        public int MaxValue;
        public int StepSize;

        public bool Equals(RangeAsInt other) => Id == other.Id;
        public override bool Equals(object obj) => obj is RangeAsInt other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct RangeAsFloat : IEntity
        , IEquatable<RangeAsFloat>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public float MinValue;
        public float MaxValue;
        public float Precision;

        public bool Equals(RangeAsFloat other) => Id == other.Id;
        public override bool Equals(object obj) => obj is RangeAsFloat other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Reward : IEntity
        , IEquatable<Reward>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public ushort RewardType;
        public int Amount;
        public float Chance;
        public int ItemID;

        public bool Equals(Reward other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Reward other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Inventory : IEntity, IStateful<InventoryState>
        , IEquatable<Inventory>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int MaxSlots;
        public float MaxWeight;
        public ushort InventoryType;
        public InventoryState CreateState() => new InventoryState { ID = ID, UsedSlots = 0, CurrentWeight = 0 };

        public bool Equals(Inventory other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Inventory other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Skill : IEntity, IStateful<SkillState>
        , IEquatable<Skill>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public ushort SkillType;
        public int MaxLevel;
        public float BaseCooldown;
        public int ManaCost;

        public SkillState CreateState() => new SkillState
            { ID = ID, Level = 1, Experience = 0, LastUsed = DateTime.MinValue };

        public bool Equals(Skill other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Skill other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Effect : IEntity, IStateful<EffectState>
        , IEquatable<Effect>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public ushort EffectType;
        public float Magnitude;
        public long DurationTicks;
        public bool IsStackable;
        public EffectState CreateState() => new EffectState { ID = ID, Stacks = 1, RemainingTicks = DurationTicks };

        public bool Equals(Effect other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Effect other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Combat : IEntity, IStateful<CombatState>
        , IEquatable<Combat>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public float BaseDamage;
        public float AttackSpeed;
        public float CritChance;
        public float CritMultiplier;

        public CombatState CreateState() => new CombatState
            { ID = ID, IsInCombat = false, LastAttack = DateTime.MinValue };

        public bool Equals(Combat other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Combat other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Economy : IEntity, IStateful<EconomyState>
        , IEquatable<Economy>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int BasePrice;
        public float Inflation;
        public float Supply;
        public float Demand;

        public EconomyState CreateState() => new EconomyState
            { ID = ID, CurrentPrice = BasePrice, LastUpdate = DateTime.Now };

        public bool Equals(Economy other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Economy other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct AI : IEntity, IStateful<AIState>
        , IEquatable<AI>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public ushort BehaviorType;
        public float AggroRange;
        public float PatrolRadius;
        public int Priority;
        public AIState CreateState() => new AIState { ID = ID, CurrentTarget = 0, LastAction = DateTime.MinValue };

        public bool Equals(AI other) => Id == other.Id;
        public override bool Equals(object obj) => obj is AI other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Achievement : IEntity, IStateful<AchievementState>
        , IEquatable<Achievement>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public ushort AchievementType;
        public int Points;
        public bool IsHidden;
        public DateTime UnlockDate;
        public AchievementState CreateState() => new AchievementState { ID = ID, Progress = 0f, IsUnlocked = false };

        public bool Equals(Achievement other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Achievement other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Guild : IEntity, IStateful<GuildState>
        , IEquatable<Guild>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int MaxMembers;
        public int Level;
        public long Experience;
        public DateTime CreatedAt;
        public GuildState CreateState() => new GuildState { ID = ID, MemberCount = 0, Treasury = 0 };

        public bool Equals(Guild other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Guild other && Equals(other);
        public override int GetHashCode() => Id;
    }

// State Structs
    [Serializable]
    public struct MissionState : IEntity
        , IEquatable<MissionState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int Attempts;
        public bool IsActive;
        public DateTime StartTime;

        public bool Equals(MissionState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is MissionState other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct QuestState : IEntity
        , IEquatable<QuestState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public float Progress;
        public bool IsCompleted;

        public bool Equals(QuestState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is QuestState other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct ObjectiveState : IEntity
        , IEquatable<ObjectiveState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int CurrentValue;
        public bool IsCompleted;

        public bool Equals(ObjectiveState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is ObjectiveState other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct CharacterState : IEntity
        , IEquatable<CharacterState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public float Health;
        public float Mana;
        public float Stamina;

        public bool Equals(CharacterState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is CharacterState other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct StatState : IEntity
        , IEquatable<StatState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public float CurrentValue;
        public float Modifiers;

        public bool Equals(StatState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is StatState other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct TimeState : IEntity
        , IEquatable<TimeState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public long ElapsedTicks;
        public bool IsActive;
        public int CycleCount;

        public bool Equals(TimeState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is TimeState other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct InventoryState : IEntity
        , IEquatable<InventoryState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int UsedSlots;
        public float CurrentWeight;

        public bool Equals(InventoryState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is InventoryState other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct SkillState : IEntity
        , IEquatable<SkillState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int Level;
        public float Experience;
        public DateTime LastUsed;

        public bool Equals(SkillState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is SkillState other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct EffectState : IEntity
        , IEquatable<EffectState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int Stacks;
        public long RemainingTicks;

        public bool Equals(EffectState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is EffectState other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct CombatState : IEntity
        , IEquatable<CombatState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public bool IsInCombat;
        public DateTime LastAttack;

        public bool Equals(CombatState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is CombatState other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct EconomyState : IEntity
        , IEquatable<EconomyState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int CurrentPrice;
        public DateTime LastUpdate;

        public bool Equals(EconomyState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is EconomyState other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct AIState : IEntity
        , IEquatable<AIState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int CurrentTarget;
        public DateTime LastAction;

        public bool Equals(AIState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is AIState other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct AchievementState : IEntity
        , IEquatable<AchievementState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public float Progress;
        public bool IsUnlocked;

        public bool Equals(AchievementState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is AchievementState other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct GuildState : IEntity
        , IEquatable<GuildState>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int MemberCount;
        public long Treasury;

        public bool Equals(GuildState other) => Id == other.Id;
        public override bool Equals(object obj) => obj is GuildState other && Equals(other);
        public override int GetHashCode() => Id;
    }

// Meta Structs
    [Serializable]
    public struct Name : IEntity
        , IEquatable<Name>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public FixedString32Bytes Text;
        public ushort LanguageID;

        public bool Equals(Name other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Name other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Description : IEntity
        , IEquatable<Description>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public FixedString64Bytes Text;
        public ushort LanguageID;

        public bool Equals(Description other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Description other && Equals(other);
        public override int GetHashCode() => Id;
    }

    [Serializable]
    public struct Tag : IEntity
        , IEquatable<Tag>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public ushort TagType;
        public int Value;

        public bool Equals(Tag other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Tag other && Equals(other);
        public override int GetHashCode() => Id;
    }

// Link System
    [Serializable]
    public struct Link : IEntity
        , IEquatable<Link>
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public EntityType SourceType;
        public int SourceID;
        public EntityType TargetType;
        public int TargetID;

        public bool Equals(Link other) => Id == other.Id;
        public override bool Equals(object obj) => obj is Link other && Equals(other);
        public override int GetHashCode() => Id;
    }
}