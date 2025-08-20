using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Core Interface
public interface IPrimaryKey
{
    int ID { get; }
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
public struct Mission : IPrimaryKey, IStateful<MissionState>
{
    public int id;
    public int ID => id;
    public int Priority;
    public bool IsRepeatable;
    public int MaxAttempts;

    public MissionState CreateState() => new MissionState
        { id = id, Attempts = 0, IsActive = false, StartTime = DateTime.MinValue };
}

[Serializable]
public struct Quest : IPrimaryKey, IStateful<QuestState>
{
    public int id;
    public int ID => id;
    public int ChapterID;
    public int Difficulty;
    public bool IsMainQuest;
    public QuestState CreateState() => new QuestState { id = id, Progress = 0f, IsCompleted = false };
}

[Serializable]
public struct Objective : IPrimaryKey, IStateful<ObjectiveState>
{
    public int id;
    public int ID => id;
    public int TargetValue;
    public ushort ObjectiveType;
    public bool IsOptional;
    public ObjectiveState CreateState() => new ObjectiveState { id = id, CurrentValue = 0, IsCompleted = false };
}

[Serializable]
public struct Item : IPrimaryKey
{
    public int id;
    public int ID => id;
    public ushort ItemType;
    public int Rarity;
    public int MaxStack;
    public float Weight;
    public int Value;
}

[Serializable]
public struct Equipment : IPrimaryKey
{
    public int id;
    public int ID => id;
    public ushort EquipSlot;
    public int Level;
    public int Durability;
    public int MaxDurability;
}

[Serializable]
public struct Player : IPrimaryKey
{
    public int id;
    public int ID => id;
    public int Level;
    public float Experience;
    public int PrestigeLevel;
    public DateTime CreatedAt;
}

[Serializable]
public struct Character : IPrimaryKey, IStateful<CharacterState>
{
    public int id;
    public int ID => id;
    public ushort CharacterClass;
    public int Level;
    public ushort Race;
    public ushort Gender;
    public CharacterState CreateState() => new CharacterState { id = id, Health = 100, Mana = 100, Stamina = 100 };
}

[Serializable]
public struct Stat : IPrimaryKey, IStateful<StatState>
{
    public int id;
    public int ID => id;
    public ushort StatType;
    public float BaseValue;
    public float MinValue;
    public float MaxValue;
    public StatState CreateState() => new StatState { id = id, CurrentValue = BaseValue, Modifiers = 0 };
}

[Serializable]
public struct Location : IPrimaryKey
{
    public int id;
    public int ID => id;
    public Vector3 Position;
    public Vector3 Scale;
    public Quaternion Rotation;
    public int ParentZoneID;
}

[Serializable]
public struct Time : IPrimaryKey, IStateful<TimeState>
{
    public int id;
    public int ID => id;
    public long StartTicks;
    public long DurationTicks;
    public bool IsRepeating;
    public long IntervalTicks;
    public TimeState CreateState() => new TimeState { id = id, ElapsedTicks = 0, IsActive = false, CycleCount = 0 };
}

[Serializable]
public struct RangeInt : IPrimaryKey
{
    public int id;
    public int ID => id;
    public int MinValue;
    public int MaxValue;
    public int StepSize;
}

[Serializable]
public struct RangeFloat : IPrimaryKey
{
    public int id;
    public int ID => id;
    public float MinValue;
    public float MaxValue;
    public float Precision;
}

[Serializable]
public struct Reward : IPrimaryKey
{
    public int id;
    public int ID => id;
    public ushort RewardType;
    public int Amount;
    public float Chance;
    public int ItemID;
}

[Serializable]
public struct Inventory : IPrimaryKey, IStateful<InventoryState>
{
    public int id;
    public int ID => id;
    public int MaxSlots;
    public float MaxWeight;
    public ushort InventoryType;
    public InventoryState CreateState() => new InventoryState { id = id, UsedSlots = 0, CurrentWeight = 0 };
}

[Serializable]
public struct Skill : IPrimaryKey, IStateful<SkillState>
{
    public int id;
    public int ID => id;
    public ushort SkillType;
    public int MaxLevel;
    public float BaseCooldown;
    public int ManaCost;

    public SkillState CreateState() => new SkillState
        { id = id, Level = 1, Experience = 0, LastUsed = DateTime.MinValue };
}

[Serializable]
public struct Effect : IPrimaryKey, IStateful<EffectState>
{
    public int id;
    public int ID => id;
    public ushort EffectType;
    public float Magnitude;
    public long DurationTicks;
    public bool IsStackable;
    public EffectState CreateState() => new EffectState { id = id, Stacks = 1, RemainingTicks = DurationTicks };
}

[Serializable]
public struct Combat : IPrimaryKey, IStateful<CombatState>
{
    public int id;
    public int ID => id;
    public float BaseDamage;
    public float AttackSpeed;
    public float CritChance;
    public float CritMultiplier;
    public CombatState CreateState() => new CombatState { id = id, IsInCombat = false, LastAttack = DateTime.MinValue };
}

[Serializable]
public struct Economy : IPrimaryKey, IStateful<EconomyState>
{
    public int id;
    public int ID => id;
    public int BasePrice;
    public float Inflation;
    public float Supply;
    public float Demand;

    public EconomyState CreateState() => new EconomyState
        { id = id, CurrentPrice = BasePrice, LastUpdate = DateTime.Now };
}

[Serializable]
public struct AI : IPrimaryKey, IStateful<AIState>
{
    public int id;
    public int ID => id;
    public ushort BehaviorType;
    public float AggroRange;
    public float PatrolRadius;
    public int Priority;
    public AIState CreateState() => new AIState { id = id, CurrentTarget = 0, LastAction = DateTime.MinValue };
}

[Serializable]
public struct Achievement : IPrimaryKey, IStateful<AchievementState>
{
    public int id;
    public int ID => id;
    public ushort AchievementType;
    public int Points;
    public bool IsHidden;
    public DateTime UnlockDate;
    public AchievementState CreateState() => new AchievementState { id = id, Progress = 0f, IsUnlocked = false };
}

[Serializable]
public struct Guild : IPrimaryKey, IStateful<GuildState>
{
    public int id;
    public int ID => id;
    public int MaxMembers;
    public int Level;
    public long Experience;
    public DateTime CreatedAt;
    public GuildState CreateState() => new GuildState { id = id, MemberCount = 0, Treasury = 0 };
}

// State Structs
[Serializable]
public struct MissionState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public int Attempts;
    public bool IsActive;
    public DateTime StartTime;
}

[Serializable]
public struct QuestState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public float Progress;
    public bool IsCompleted;
}

[Serializable]
public struct ObjectiveState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public int CurrentValue;
    public bool IsCompleted;
}

[Serializable]
public struct CharacterState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public float Health;
    public float Mana;
    public float Stamina;
}

[Serializable]
public struct StatState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public float CurrentValue;
    public float Modifiers;
}

[Serializable]
public struct TimeState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public long ElapsedTicks;
    public bool IsActive;
    public int CycleCount;
}

[Serializable]
public struct InventoryState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public int UsedSlots;
    public float CurrentWeight;
}

[Serializable]
public struct SkillState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public int Level;
    public float Experience;
    public DateTime LastUsed;
}

[Serializable]
public struct EffectState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public int Stacks;
    public long RemainingTicks;
}

[Serializable]
public struct CombatState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public bool IsInCombat;
    public DateTime LastAttack;
}

[Serializable]
public struct EconomyState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public int CurrentPrice;
    public DateTime LastUpdate;
}

[Serializable]
public struct AIState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public int CurrentTarget;
    public DateTime LastAction;
}

[Serializable]
public struct AchievementState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public float Progress;
    public bool IsUnlocked;
}

[Serializable]
public struct GuildState : IPrimaryKey
{
    public int id;

    public int ID => id;

    public int MemberCount;
    public long Treasury;
}

// Meta Structs
[Serializable]
public struct Name : IPrimaryKey
{
    public int id;
    public int ID => id;
    public string Text;
    public ushort LanguageID;
}

[Serializable]
public struct Description : IPrimaryKey
{
    public int id;
    public int ID => id;
    public string Text;
    public ushort LanguageID;
}

[Serializable]
public struct Tag : IPrimaryKey
{
    public int id;
    public int ID => id;
    public ushort TagType;
    public int Value;
}

// Link System
[Serializable]
public struct PrimaryKey : IPrimaryKey
{
    public int id;
    public int ID => id;
    public ushort SourceType;
    public int SourceID;
    public ushort TargetType;
    public int TargetID;
    public ushort LinkTypeID;
}

// Database Manager
public class GameDatabase : MonoBehaviour
{
    // Entity Tables
    [SerializeField] public List<Mission> Missions = new();
    [SerializeField] public List<Quest> Quests = new();
    [SerializeField] public List<Objective> Objectives = new();
    [SerializeField] public List<Item> Items = new();
    [SerializeField] public List<Equipment> Equipments = new();
    [SerializeField] public List<Player> Players = new();
    [SerializeField] public List<Character> Characters = new();
    [SerializeField] public List<Stat> Stats = new();
    [SerializeField] public List<Location> Locations = new();
    [SerializeField] public List<Time> Times = new();
    [SerializeField] public List<RangeInt> RangeInts = new();
    [SerializeField] public List<RangeFloat> RangeFloats = new();
    [SerializeField] public List<Reward> Rewards = new();
    [SerializeField] public List<Inventory> Inventories = new();
    [SerializeField] public List<Skill> Skills = new();
    [SerializeField] public List<Effect> Effects = new();
    [SerializeField] public List<Combat> Combats = new();
    [SerializeField] public List<Economy> Economies = new();
    [SerializeField] public List<AI> AIs = new();
    [SerializeField] public List<Achievement> Achievements = new();
    [SerializeField] public List<Guild> Guilds = new();

    // State Tables  
    [SerializeField] public List<MissionState> MissionStates = new();
    [SerializeField] public List<QuestState> QuestStates = new();
    [SerializeField] public List<ObjectiveState> ObjectiveStates = new();
    [SerializeField] public List<CharacterState> CharacterStates = new();
    [SerializeField] public List<StatState> StatStates = new();
    [SerializeField] public List<TimeState> TimeStates = new();
    [SerializeField] public List<InventoryState> InventoryStates = new();
    [SerializeField] public List<SkillState> SkillStates = new();
    [SerializeField] public List<EffectState> EffectStates = new();
    [SerializeField] public List<CombatState> CombatStates = new();
    [SerializeField] public List<EconomyState> EconomyStates = new();
    [SerializeField] public List<AIState> AIStates = new();
    [SerializeField] public List<AchievementState> AchievementStates = new();
    [SerializeField] public List<GuildState> GuildStates = new();

    // Meta Tables
    [SerializeField] public List<Name> Names = new();
    [SerializeField] public List<Description> Descriptions = new();
    [SerializeField] public List<Tag> Tags = new();
    [SerializeField] public List<PrimaryKey> Links = new();

    // Generic CRUD
    public T Create<T>(T entity) where T : struct, IPrimaryKey
    {
        GetTable<T>().Add(entity);
        return entity;
    }

    public T? GetByID<T>(int id) where T : struct, IPrimaryKey => GetTable<T>().Cast<T?>().FirstOrDefault(e => e?.ID == id);
    public List<T> GetAll<T>() where T : struct, IPrimaryKey => GetTable<T>();

    public bool TryUpdate<T>(T entity) where T : struct, IPrimaryKey
    {
        var table = GetTable<T>();
        for (int i = 0; i < table.Count; i++)
            if (table[i].ID == entity.ID)
            {
                table[i] = entity;
                return true;
            }

        return false;
    }

    public bool Delete<T>(int id) where T : struct, IPrimaryKey
    {
        var table = GetTable<T>();
        for (int i = 0; i < table.Count; i++)
            if (table[i].ID == id)
            {
                table.RemoveAt(i);
                return true;
            }

        return false;
    }

    // Link Operations
    public PrimaryKey CreateLink(ushort sourceType, int sourceID, ushort targetType, int targetID, ushort linkTypeID) =>
        Create(new PrimaryKey
        {
            SourceType = sourceType, SourceID = sourceID, TargetType = targetType, TargetID = targetID,
            LinkTypeID = linkTypeID
        });

    public List<T> GetLinked<T>(ushort sourceType, int sourceID, ushort targetType, ushort linkTypeID)
        where T : struct, IPrimaryKey => GetTable<T>().Where(e => Links.Any(l =>
        l.SourceType == sourceType && l.SourceID == sourceID && l.TargetType == targetType && l.TargetID == e.ID &&
        l.LinkTypeID == linkTypeID)).ToList();

    public List<PrimaryKey> GetLinks(ushort sourceType, int sourceID, ushort linkTypeID = 0) => Links.Where(l =>
            l.SourceType == sourceType && l.SourceID == sourceID && (linkTypeID == 0 || l.LinkTypeID == linkTypeID))
        .ToList();

    // State Management
    public TState GetState<TEntity, TState>(int entityID)
        where TEntity : struct, IPrimaryKey, IStateful<TState> where TState : struct, IPrimaryKey =>
        GetTable<TState>().FirstOrDefault(s => s.ID == entityID);

    public TState CreateState<TEntity, TState>(TEntity entity) where TEntity : struct, IPrimaryKey, IStateful<TState>
        where TState : struct, IPrimaryKey
    {
        var state = entity.CreateState();
        Create(state);
        return state;
    }

    // Meta Helpers
    public string GetName(int entityID) => Names.FirstOrDefault(n => n.ID == entityID).Text ?? "";

    public Name CreateName(int entityID, string text, ushort languageID = 0) =>
        Create(new Name { Text = text, LanguageID = languageID });

    // Table Resolver
    private List<T> GetTable<T>() where T : struct, IPrimaryKey => typeof(T).Name switch
    {
        nameof(Mission) => Missions as List<T>, nameof(Quest) => Quests as List<T>,
        nameof(Objective) => Objectives as List<T>,
        nameof(Item) => Items as List<T>, nameof(Equipment) => Equipments as List<T>,
        nameof(Player) => Players as List<T>,
        nameof(Character) => Characters as List<T>, nameof(Stat) => Stats as List<T>,
        nameof(Location) => Locations as List<T>,
        nameof(Time) => Times as List<T>, nameof(RangeInt) => RangeInts as List<T>,
        nameof(RangeFloat) => RangeFloats as List<T>,
        nameof(Reward) => Rewards as List<T>, nameof(Inventory) => Inventories as List<T>,
        nameof(Skill) => Skills as List<T>,
        nameof(Effect) => Effects as List<T>, nameof(Combat) => Combats as List<T>,
        nameof(Economy) => Economies as List<T>,
        nameof(AI) => AIs as List<T>, nameof(Achievement) => Achievements as List<T>,
        nameof(Guild) => Guilds as List<T>,
        nameof(MissionState) => MissionStates as List<T>, nameof(QuestState) => QuestStates as List<T>,
        nameof(ObjectiveState) => ObjectiveStates as List<T>,
        nameof(CharacterState) => CharacterStates as List<T>, nameof(StatState) => StatStates as List<T>,
        nameof(TimeState) => TimeStates as List<T>,
        nameof(InventoryState) => InventoryStates as List<T>, nameof(SkillState) => SkillStates as List<T>,
        nameof(EffectState) => EffectStates as List<T>,
        nameof(CombatState) => CombatStates as List<T>, nameof(EconomyState) => EconomyStates as List<T>,
        nameof(AIState) => AIStates as List<T>,
        nameof(AchievementState) => AchievementStates as List<T>, nameof(GuildState) => GuildStates as List<T>,
        nameof(Name) => Names as List<T>, nameof(Description) => Descriptions as List<T>,
        nameof(Tag) => Tags as List<T>, nameof(PrimaryKey) => Links as List<T>,
        _ => throw new ArgumentException($"Unknown type: {typeof(T).Name}")
    };

    private void Start()
    {
        Mission mission = Create(new Mission { id = 1, Priority = 10, IsRepeatable = true, MaxAttempts = 3 });
        Quest quest = Create(new Quest { id = 2, ChapterID = 1, Difficulty = 5, IsMainQuest = true });
        Objective objective = Create(new Objective { id = 3, TargetValue = 10, ObjectiveType = 1, IsOptional = false });
        Item item = Create(new Item { id = 4, ItemType = 20, Rarity = 3, MaxStack = 99, Weight = 1.5f, Value = 100 });
        Equipment equipment = Create(new Equipment
            { id = 5, EquipSlot = 21, Level = 10, Durability = 100, MaxDurability = 100 });
        Player player = Create(new Player
            { id = 6, Level = 1, Experience = 0f, PrestigeLevel = 0, CreatedAt = DateTime.Now });
        Character character = Create(new Character { id = 7, CharacterClass = 32, Level = 1, Race = 1, Gender = 1 });
        Stat stat = Create(new Stat { id = 8, StatType = 40, BaseValue = 10f, MinValue = 0f, MaxValue = 100f });
        Location location = Create(new Location
            { id = 9, Position = Vector3.zero, Scale = Vector3.one, Rotation = Quaternion.identity, ParentZoneID = 0 });
        Time time = Create(new Time
            { id = 10, StartTicks = 0, DurationTicks = 3600, IsRepeating = true, IntervalTicks = 86400 });
        RangeInt rangeInt = Create(new RangeInt { id = 11, MinValue = 1, MaxValue = 10, StepSize = 1 });
        RangeFloat rangeFloat = Create(new RangeFloat { id = 12, MinValue = 0.5f, MaxValue = 5.0f, Precision = 0.1f });
        Reward reward = Create(new Reward { id = 13, RewardType = 70, Amount = 50, Chance = 0.75f, ItemID = 4 });
        Inventory inventory = Create(new Inventory { id = 14, MaxSlots = 20, MaxWeight = 50f, InventoryType = 90 });
        Skill skill = Create(new Skill { id = 15, SkillType = 42, MaxLevel = 10, BaseCooldown = 30f, ManaCost = 20 });
        Effect effect = Create(new Effect
            { id = 16, EffectType = 46, Magnitude = 1.5f, DurationTicks = 300, IsStackable = true });
        Combat combat = Create(new Combat
            { id = 17, BaseDamage = 10f, AttackSpeed = 1.5f, CritChance = 0.1f, CritMultiplier = 2f });
        Economy economy = Create(new Economy
            { id = 18, BasePrice = 100, Inflation = 0.05f, Supply = 1000f, Demand = 500f });
        AI ai = Create(new AI { id = 19, BehaviorType = 120, AggroRange = 10f, PatrolRadius = 20f, Priority = 5 });
        Achievement achievement = Create(new Achievement
            { id = 20, AchievementType = 14, Points = 100, IsHidden = false, UnlockDate = DateTime.MinValue });
    }
}