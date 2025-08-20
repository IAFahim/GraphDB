using System;
using System.Collections.Generic;
using System.Linq;
using GraphTookKitDB.Runtime;
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

// Type IDs for linking
    public static class TypeID
    {
        public const ushort Entity = 1, Component = 2, System = 3, Event = 4, State = 5, Rule = 6;
        public const ushort Mission = 10, Quest = 11, Objective = 12, Task = 13, Achievement = 14, Challenge = 15;

        public const ushort Item = 20,
            Equipment = 21,
            Consumable = 22,
            Currency = 23,
            Resource = 24,
            Blueprint = 25,
            Recipe = 26;

        public const ushort Player = 30,
            NPC = 31,
            Character = 32,
            Unit = 33,
            Squad = 34,
            Guild = 35,
            Faction = 36,
            Team = 37;

        public const ushort Stat = 40,
            Attribute = 41,
            Skill = 42,
            Ability = 43,
            Buff = 44,
            Debuff = 45,
            Effect = 46,
            Modifier = 47;

        public const ushort Location = 50,
            Zone = 51,
            Region = 52,
            Dungeon = 53,
            Room = 54,
            Portal = 55,
            Spawn = 56,
            Waypoint = 57;

        public const ushort Time = 60, Schedule = 61, Cooldown = 62, Duration = 63, Timer = 64, Calendar = 65, Season = 66;
        public const ushort Reward = 70, Loot = 71, Drop = 72, Prize = 73, Bonus = 74, Penalty = 75;
        public const ushort RangeInt = 80, RangeFloat = 81, Condition = 82, Requirement = 83, Trigger = 84, Validator = 85;
        public const ushort Inventory = 90, Container = 91, Storage = 92, Slot = 93, Stack = 94;
        public const ushort Economy = 100, Market = 101, Trade = 102, Shop = 103, Auction = 104, Price = 105;
        public const ushort Combat = 110, Damage = 111, Heal = 112, Shield = 113, Armor = 114, Weapon = 115;
        public const ushort AI = 120, Behavior = 121, Decision = 122, Goal = 123, Action = 124, Pathfinding = 125;
        public const ushort UI = 130, Menu = 131, Dialog = 132, Notification = 133, HUD = 134;
        public const ushort Audio = 140, Music = 141, SFX = 142, Voice = 143, Ambient = 144;
        public const ushort Visual = 150, Animation = 151, Particle = 152, Light = 153, Material = 154;
        public const ushort Physics = 160, Collision = 161, Force = 162, Constraint = 163;
        public const ushort Network = 170, Session = 171, Message = 172, Sync = 173;
        public const ushort Meta = 180, Name = 181, Description = 182, Tag = 183, Category = 184, Version = 185;
    }

// Core Entity Structs
    [Serializable]
    public struct Mission : IEntity, IStateful<MissionState>
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
    }

    [Serializable]
    public struct Quest : IEntity, IStateful<QuestState>
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
    }

    [Serializable]
    public struct Objective : IEntity, IStateful<ObjectiveState>
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
    }

    [Serializable]
    public struct Item : IEntity
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
    }

    [Serializable]
    public struct Equipment : IEntity
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
    }

    [Serializable]
    public struct Player : IEntity
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
    }

    [Serializable]
    public struct Character : IEntity, IStateful<CharacterState>
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
    }

    [Serializable]
    public struct Stat : IEntity, IStateful<StatState>
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
    }

    [Serializable]
    public struct Location : IEntity
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
    }

    [Serializable]
    public struct TimeTick : IEntity, IStateful<TimeState>
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
    }

    [Serializable]
    public struct RangeAsInt : IEntity
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
    }

    [Serializable]
    public struct RangeAsFloat : IEntity
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
    }

    [Serializable]
    public struct Reward : IEntity
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
    }

    [Serializable]
    public struct Inventory : IEntity, IStateful<InventoryState>
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
    }

    [Serializable]
    public struct Skill : IEntity, IStateful<SkillState>
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
    }

    [Serializable]
    public struct Effect : IEntity, IStateful<EffectState>
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
    }

    [Serializable]
    public struct Combat : IEntity, IStateful<CombatState>
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
        public CombatState CreateState() => new CombatState { ID = ID, IsInCombat = false, LastAttack = DateTime.MinValue };
    }

    [Serializable]
    public struct Economy : IEntity, IStateful<EconomyState>
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
    }

    [Serializable]
    public struct AI : IEntity, IStateful<AIState>
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
    }

    [Serializable]
    public struct Achievement : IEntity, IStateful<AchievementState>
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
    }

    [Serializable]
    public struct Guild : IEntity, IStateful<GuildState>
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
    }

// State Structs
    [Serializable]
    public struct MissionState : IEntity
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
    }

    [Serializable]
    public struct QuestState : IEntity
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public float Progress;
        public bool IsCompleted;
    }

    [Serializable]
    public struct ObjectiveState : IEntity
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int CurrentValue;
        public bool IsCompleted;
    }

    [Serializable]
    public struct CharacterState : IEntity
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
    }

    [Serializable]
    public struct StatState : IEntity
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public float CurrentValue;
        public float Modifiers;
    }

    [Serializable]
    public struct TimeState : IEntity
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
    }

    [Serializable]
    public struct InventoryState : IEntity
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int UsedSlots;
        public float CurrentWeight;
    }

    [Serializable]
    public struct SkillState : IEntity
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
    }

    [Serializable]
    public struct EffectState : IEntity
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int Stacks;
        public long RemainingTicks;
    }

    [Serializable]
    public struct CombatState : IEntity
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public bool IsInCombat;
        public DateTime LastAttack;
    }

    [Serializable]
    public struct EconomyState : IEntity
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int CurrentPrice;
        public DateTime LastUpdate;
    }

    [Serializable]
    public struct AIState : IEntity
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int CurrentTarget;
        public DateTime LastAction;
    }

    [Serializable]
    public struct AchievementState : IEntity
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public float Progress;
        public bool IsUnlocked;
    }

    [Serializable]
    public struct GuildState : IEntity
    {
        public int Id;

        public int ID
        {
            get => Id;
            set => Id = value;
        }

        public int MemberCount;
        public long Treasury;
    }

// Meta Structs
    [Serializable]
    public struct Name : IEntity
    {
        public int Id;
        public int ID
        {
            get => Id;
            set => Id = value;
        }
        public string Text;
        public ushort LanguageID;
    }

    [Serializable]
    public struct Description : IEntity
    {
        public int Id;
        public int ID
        {
            get => Id;
            set => Id = value;
        }
        public string Text;
        public ushort LanguageID;
    }

    [Serializable]
    public struct Tag : IEntity
    {
        public int Id;
        public int ID
        {
            get => Id;
            set => Id = value;
        }
        public ushort TagType;
        public int Value;
    }

// Link System
    [Serializable]
    public struct Link : IEntity
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
    }
}