
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api
{
    public static class ApiVersion
    {
        public const UInt64 VERSION = 19;
    }
    public enum Upgrade
    {
        U0 = 0,
        U1 = 1000000,
        U2 = 2000000,
        U3 = 3000000,
    }

    public class CardIdConverter : JsonConverter<CardId>
    {
        public override bool CanConvert(Type t) { return t == typeof(CardId); }
        public override CardId? Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options) { var v = reader.GetUInt32(); return new CardId(v); }
        public override void Write(Utf8JsonWriter writer, CardId value, JsonSerializerOptions options) { writer.WriteNumberValue(value.V); }
    }
    /// <summary>
    ///  ID of the card resource
    /// </summary>
    [JsonConverter(typeof(CardIdConverter))]
    public record CardId(UInt32 V);

    public class SquadIdConverter : JsonConverter<SquadId>
    {
        public override bool CanConvert(Type t) { return t == typeof(SquadId); }
        public override SquadId? Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options) { var v = reader.GetUInt32(); return new SquadId(v); }
        public override void Write(Utf8JsonWriter writer, SquadId value, JsonSerializerOptions options) { writer.WriteNumberValue(value.V); }
    }
    /// <summary>
    ///  ID of squad resource
    /// </summary>
    [JsonConverter(typeof(SquadIdConverter))]
    public record SquadId(UInt32 V);

    public class BuildingIdConverter : JsonConverter<BuildingId>
    {
        public override bool CanConvert(Type t) { return t == typeof(BuildingId); }
        public override BuildingId? Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options) { var v = reader.GetUInt32(); return new BuildingId(v); }
        public override void Write(Utf8JsonWriter writer, BuildingId value, JsonSerializerOptions options) { writer.WriteNumberValue(value.V); }
    }
    /// <summary>
    ///  ID of building resource
    /// </summary>
    [JsonConverter(typeof(BuildingIdConverter))]
    public record BuildingId(UInt32 V);

    public class SpellIdConverter : JsonConverter<SpellId>
    {
        public override bool CanConvert(Type t) { return t == typeof(SpellId); }
        public override SpellId? Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options) { var v = reader.GetUInt32(); return new SpellId(v); }
        public override void Write(Utf8JsonWriter writer, SpellId value, JsonSerializerOptions options) { writer.WriteNumberValue(value.V); }
    }
    /// <summary>
    ///  ID of spell resource
    /// </summary>
    [JsonConverter(typeof(SpellIdConverter))]
    public record SpellId(UInt32 V);

    public class AbilityIdConverter : JsonConverter<AbilityId>
    {
        public override bool CanConvert(Type t) { return t == typeof(AbilityId); }
        public override AbilityId? Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options) { var v = reader.GetUInt32(); return new AbilityId(v); }
        public override void Write(Utf8JsonWriter writer, AbilityId value, JsonSerializerOptions options) { writer.WriteNumberValue(value.V); }
    }
    /// <summary>
    ///  ID of ability resource
    /// </summary>
    [JsonConverter(typeof(AbilityIdConverter))]
    public record AbilityId(UInt32 V);

    public class ModeIdConverter : JsonConverter<ModeId>
    {
        public override bool CanConvert(Type t) { return t == typeof(ModeId); }
        public override ModeId? Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options) { var v = reader.GetUInt32(); return new ModeId(v); }
        public override void Write(Utf8JsonWriter writer, ModeId value, JsonSerializerOptions options) { writer.WriteNumberValue(value.V); }
    }
    /// <summary>
    ///  ID of mode resource
    /// </summary>
    [JsonConverter(typeof(ModeIdConverter))]
    public record ModeId(UInt32 V);

    public class EntityIdConverter : JsonConverter<EntityId>
    {
        public override bool CanConvert(Type t) { return t == typeof(EntityId); }
        public override EntityId? Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options) { var v = reader.GetUInt32(); return new EntityId(v); }
        public override void Write(Utf8JsonWriter writer, EntityId value, JsonSerializerOptions options) { writer.WriteNumberValue(value.V); }
    }
    /// <summary>
    ///  ID of an entity present in the match unique to that match
    ///  First entity have ID 1, next 2, ...
    ///  Ids are never reused
    /// </summary>
    [JsonConverter(typeof(EntityIdConverter))]
    public record EntityId(UInt32 V);

    public class TickConverter : JsonConverter<Tick>
    {
        public override bool CanConvert(Type t) { return t == typeof(Tick); }
        public override Tick? Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options) { var v = reader.GetUInt32(); return new Tick(v); }
        public override void Write(Utf8JsonWriter writer, Tick value, JsonSerializerOptions options) { writer.WriteNumberValue(value.V); }
    }
    /// <summary>
    ///  Time information 1 tick = 0.1s = 100 ms
    /// </summary>
    [JsonConverter(typeof(TickConverter))]
    public record Tick(UInt32 V);

    public class TickCountConverter : JsonConverter<TickCount>
    {
        public override bool CanConvert(Type t) { return t == typeof(TickCount); }
        public override TickCount? Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options) { var v = reader.GetUInt32(); return new TickCount(v); }
        public override void Write(Utf8JsonWriter writer, TickCount value, JsonSerializerOptions options) { writer.WriteNumberValue(value.V); }
    }
    /// <summary>
    ///  Difference between two `Tick` (points in times, remaining time, ...)
    /// </summary>
    [JsonConverter(typeof(TickCountConverter))]
    public record TickCount(UInt32 V);

    /// <summary>
    ///  `x` and `z` are coordinates on the 2D map.
    /// </summary>
    public class Position
    {
        [JsonPropertyName("x")]
        public required float X { get; set; }
        /// <summary>
        ///  Also known as height.
        /// </summary>
        [JsonPropertyName("y")]
        public required float Y { get; set; }
        [JsonPropertyName("z")]
        public required float Z { get; set; }
    }

    public class Position2D
    {
        [JsonPropertyName("x")]
        public required float X { get; set; }
        [JsonPropertyName("y")]
        public required float Y { get; set; }
    }

    public class Position2DWithOrientation
    {
        [JsonPropertyName("x")]
        public required float X { get; set; }
        [JsonPropertyName("y")]
        public required float Y { get; set; }
        /// <summary>
        ///  in default camera orientation
        ///  0 = down, π/2 = right, π = up, π3/2 = left
        /// </summary>
        [JsonPropertyName("orientation")]
        public required float Orientation { get; set; }
    }

    /// <summary>
    ///  Color of an orb.
    /// </summary>
    public enum OrbColor
    {
        White = 0,
        Shadow = 1,
        Nature = 2,
        Frost = 3,
        Fire = 4,
        Starting = 5,
        All = 7,
    }

    /// <summary>
    ///  Subset of `OrbColor`, because creating the other colors does not make sense.
    /// </summary>
    public enum CreateOrbColor
    {
        Shadow = 1,
        Nature = 2,
        Frost = 3,
        Fire = 4,
    }

    /// <summary>
    ///  When targeting you can target either entity, or ground coordinates.
    /// </summary>
    public class SingleTargetHolder
    {
        /// <summary>
        ///  Target entity
        /// </summary>
        [JsonPropertyName("SingleEntity")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SingleTargetSingleEntity? SingleEntity { get; set; }
        /// <summary>
        ///  Target location on the ground
        /// </summary>
        [JsonPropertyName("Location")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SingleTargetLocation? Location { get; set; }

        public SingleTarget Get()
        {
            if (SingleEntity != null) { return SingleEntity; }
            if (Location != null) { return Location; }
            else { throw new InvalidOperationException("Impossible, because all cases was handled above"); }
        }
        public SingleTargetHolder() { }
        public SingleTargetHolder(SingleTargetSingleEntity v) { SingleEntity = v; }
        public SingleTargetHolder(SingleTargetLocation v) { Location = v; }
        public SingleTargetHolder(SingleTarget v)
        {
            switch (v)
            {
                case SingleTargetSingleEntity s:
                    SingleEntity = s; break;
                case SingleTargetLocation s:
                    Location = s; break;
            }
        }
    }
    [JsonDerivedType(typeof(SingleTargetSingleEntity))]
    [JsonDerivedType(typeof(SingleTargetLocation))]
    public /*abstract*/ class SingleTarget { }

    /// <summary>
    ///  Target entity
    /// </summary>
    public sealed class SingleTargetSingleEntity : SingleTarget
    {
        [JsonPropertyName("id")]
        public required EntityId Id { get; set; }
    }

    /// <summary>
    ///  Target location on the ground
    /// </summary>
    public sealed class SingleTargetLocation : SingleTarget
    {
        [JsonPropertyName("xy")]
        public required Position2D Xy { get; set; }
    }


    public class TargetHolder
    {
        [JsonPropertyName("Single")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TargetSingle? Single { get; set; }
        [JsonPropertyName("Multi")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TargetMulti? Multi { get; set; }

        public Target Get()
        {
            if (Single != null) { return Single; }
            if (Multi != null) { return Multi; }
            else { throw new InvalidOperationException("Impossible, because all cases was handled above"); }
        }
        public TargetHolder() { }
        public TargetHolder(TargetSingle v) { Single = v; }
        public TargetHolder(TargetMulti v) { Multi = v; }
        public TargetHolder(Target v)
        {
            switch (v)
            {
                case TargetSingle s:
                    Single = s; break;
                case TargetMulti s:
                    Multi = s; break;
            }
        }
    }
    [JsonDerivedType(typeof(TargetSingle))]
    [JsonDerivedType(typeof(TargetMulti))]
    public /*abstract*/ class Target { }

    public sealed class TargetSingle : Target
    {
        [JsonPropertyName("single")]
        public required SingleTargetHolder Single { get; set; }
    }

    public sealed class TargetMulti : Target
    {
        [JsonPropertyName("xy_begin")]
        public required Position2D XyBegin { get; set; }
        [JsonPropertyName("xy_end")]
        public required Position2D XyEnd { get; set; }
    }


    public enum WalkMode
    {
        PartialForce = 1,
        Force = 2,
        /// <summary>
        ///  Also called by players "Attack move", or "Q move"
        /// </summary>
        Normal = 4,
        Crusade = 5,
        Scout = 6,
        Patrol = 7,
    }

    public class CommunityMapInfo
    {
        /// <summary>
        ///  Name of the map.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        /// <summary>
        ///  Checksum of them map.
        /// </summary>
        [JsonPropertyName("crc")]
        public required UInt64 Crc { get; set; }
    }

    /// <summary>
    ///  Official spectator maps are normal maps (have unique id) so only `map` field is needed.
    /// </summary>
    public class MapInfo
    {
        /// <summary>
        ///  Represents the map, unfortunately EA decided, it will be harder for community maps.
        /// </summary>
        [JsonPropertyName("map")]
        public required Maps Map { get; set; }
        /// <summary>
        ///  Is relevant only for community maps.
        /// </summary>
        [JsonPropertyName("community_map_details")]
        public CommunityMapInfo? CommunityMapDetails { get; set; }
    }

    public class Deck
    {
        /// <summary>
        ///  Name of the deck, must be unique across decks used by bot, but different bots can have same deck names.
        ///  Must not contain spaces, to be addable in game.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        /// <summary>
        ///  Index of a card that will be deck icon 0 to 19 inclusive
        /// </summary>
        [JsonPropertyName("cover_card_index")]
        public required byte CoverCardIndex { get; set; }
        /// <summary>
        ///  List of 20 cards in deck.
        ///  Fill empty spaces with `NotACard`.
        /// </summary>
        [JsonPropertyName("cards")]
        public required CardId[/*size=20*/] Cards { get; set; }
    }

    public enum AbilityLine
    {
        _EAsBug_betterSafeThanSorry = 0,
        ModifyWalkSpeed = 1,
        UnControllable = 4,
        UnKillable = 5,
        UnAttackable = 9,
        HitMultiple = 10,
        _ACModifier = 14,
        DamageOverTime = 15,
        DamageBuff = 21,
        PowerOutputModifier = 23,
        MoveSpeedOverwrite = 24,
        HitMultipleRanged = 25,
        HPModifier = 26,
        Aura = 27,
        PreventCardPlay = 29,
        _SpreadFire = 31,
        _SquadSpawnZone = 32,
        HitMultipleProjectile = 33,
        _MarkedTargetDamageMultiplier = 36,
        _MarkedTargetDamage = 37,
        _AttackPauseDelay = 39,
        _MarkedForTeleport = 40,
        _RangeModifier = 41,
        ForceAttack = 42,
        OnEntityDie = 44,
        _FireLanceAbility = 47,
        Collector = 50,
        _ChangeTargetAggro = 51,
        _FireLanceBurstCollector = 53,
        RegenerationOld = 54,
        _TimedSpell = 57,
        Scatter = 58,
        _DamageOverTimeNoCombat = 59,
        LifeStealer = 60,
        BarrierGate = 61,
        GlobalRevive = 62,
        TrampleResistance = 64,
        TrampleOverwrite = 65,
        PushbackResistance = 66,
        MeleePushbackOverride = 67,
        _FanCollector = 69,
        MeleeFightSpeedModifier = 71,
        _SpellRangeModifierIncoming = 72,
        SpellRangeModifierOutgoing = 73,
        _FanCollectorBurst = 74,
        RangedFightSpeedModifier = 75,
        _SquadRestore = 76,
        DamagePowerTransfer = 79,
        TimedSpell = 80,
        TrampleRevengeDamage = 81,
        LinkedFire = 83,
        DamageBuffAgainst = 84,
        IncomingDamageModifier = 85,
        GeneratorPower = 86,
        IceShield = 87,
        DoTRefresh = 88,
        EnrageThreshold = 89,
        Immunity = 90,
        _UnitSpawnZone = 91,
        Rage = 92,
        _PassiveCharge = 93,
        MeleeHitSpell = 95,
        _FireDebuff = 97,
        FrostDebuff = 98,
        SpellBlocker = 100,
        ShadowDebuff = 102,
        SuicidalBomb = 103,
        GrantToken = 110,
        TurretCannon = 112,
        SpellOnSelfCast = 113,
        AbilityOnSelfResolve = 114,
        SuppressUserCommand = 118,
        LineCast = 120,
        NoCheer = 132,
        UnitShredderJobCondition = 133,
        DamageRadialArea = 134,
        _DamageConeArea = 137,
        DamageConeCutArea = 138,
        ConstructionRepairModifier = 139,
        Portal = 140,
        Tunnel = 141,
        ModeConditionDelay = 142,
        HealAreaRadial = 144,
        _145LeftoverDoesNotReallyExistButIsUsed = 145,
        _146LeftoverDoesNotReallyExistButIsUsed = 146,
        OverrideWeaponType = 151,
        DamageRadialAreaUsingCorpse = 153,
        HealAreaRadialInstantContinues = 154,
        ChargeableBombController = 155,
        ChargeAttack = 156,
        ChargeableBomb = 157,
        ModifyRotationSpeed = 159,
        ModifyAcceleration = 160,
        FormationOverwrite = 161,
        EffectHolder = 162,
        WhiteRangersHomeDefenseTrigger = 163,
        _167LeftoverDoesNotReallyExistButIsUsed = 167,
        _168LeftoverDoesNotReallyExistButIsUsed = 168,
        HealReservoirUsingCorpse = 170,
        ModeChangeBlocker = 171,
        BarrierModuleEnterBlock = 172,
        ProduceAmmoUsingCorpseInjurity = 173,
        IncomingDamageSpreadOnTargetAlignmentArea1 = 174,
        DamageSelfOnMeleeHit = 175,
        HealthCapCurrent = 176,
        ConstructionUnCrushable = 179,
        ProduceAmmoOverTime = 180,
        BarrierSetBuildDelay = 181,
        ChannelTimedSpell = 183,
        AuraOnEnter = 184,
        ParalyzeAbility = 185,
        IgnoreSummoningSickness = 186,
        BlockRepair = 187,
        Corruption = 188,
        UnHealable = 189,
        Immobile = 190,
        ModifyHealing = 191,
        IgnoreInCardCondition = 192,
        MovementMode = 193,
        ConsumeAmmoHealSelf = 195,
        ConsumeAmmoHealAreaRadial = 196,
        CorpseGather = 197,
        AbilityNearEntity = 198,
        ModifyIceShieldDecayRate = 200,
        ModifyDamageIncomingAuraContingentSelfDamage = 201,
        ModifyDamageIncomingAuraContingentSelfDamageTargetAbility = 202,
        ConvertCorpseToPower = 203,
        EraseOverTime = 204,
        FireStreamChannel = 205,
        DisableMeleeAttack = 206,
        AbilityOnPlayer = 207,
        GlobalAbilityOnEntity = 208,
        AuraModifyCardCost = 209,
        AuraModifyBuildTime = 210,
        GlobalRotTimeModifier = 211,
        _212LeftoverDoesNotReallyExistButIsUsed = 212,
        MindControl = 213,
        SpellOnEntityNearby = 214,
        AmmoConsumeModifyIncomingDamage = 216,
        AmmoConsumeModifyOutgoingDamage = 217,
        GlobalSuppressRefund = 219,
        DirectRefundOnDie = 220,
        OutgoingDamageDependendSpell = 221,
        DeathCounter = 222,
        DeathCounterController = 223,
        DamageRadialAreaUsingGraveyard = 224,
        MovingIntervalCast = 225,
        BarrierGateDelay = 226,
        EffectHolderAmmo = 227,
        FightDependentAbility = 228,
        GlobalIgnoreCardPlayConditions = 229,
        WormMovement = 230,
        DamageRectAreaAligned = 231,
        GlobalRefundOnEntityDie = 232,
        GlobalDamageAbsorption = 233,
        GlobalPowerRecovermentModifier = 234,
        GlobalDamageAbsorptionTargetAbility = 235,
        OverwriteVisRange = 236,
        DamageOverTimeCastDepending = 237,
        ModifyDamageIncomingAuraContingentSelfRadialAreaDamage = 238,
        SuperWeaponShadow = 239,
        NoMeleeAgainstAir = 240,
        _SuperWeaponShadowDamage = 242,
        NoCardPlay = 243,
        NoClaim = 244,
        DamageRadialAreaAmmo = 246,
        PathLayerOverride = 247,
        ChannelBlock = 248,
        Polymorph = 249,
        Delay = 250,
        ModifyDamageIncomingOnFigure = 251,
        ImmobileRoot = 252,
        GlobalModifyCorpseGather = 253,
        AbilityDependentAbility = 254,
        CorpseManager = 255,
        DisableToken = 256,
        Piercing = 258,
        ReceiveMeleeAttacks = 259,
        BuildBlock = 260,
        PreventCardPlayAuraBuilding = 262,
        GraveyardDependentRecast = 263,
        ClaimBlock = 264,
        AmmoStartup = 265,
        DamageDistribution = 266,
        SwapSquadNightGuard = 267,
        Revive = 268,
        Amok = 269,
        NoCombat = 270,
        SlowDownDisabled = 271,
        CrowdControlTimeModifier = 272,
        DamageOnMeleeHit = 273,
        IgnoreIncomingDamageModifier = 275,
        BlockRevive = 278,
        GlobalMorphState = 279,
        SpecialOnTarget = 280,
        FleshBenderBugSwitch = 281,
        TimedMorph = 282,
        GlobalBuildTimeModifier = 283,
        CardBlock = 285,
        IceShieldRegeneration = 286,
        HealOverTime = 287,
        IceShieldTimerOffset = 288,
        SpellOnVanish = 289,
        GlobalVoidAbsorption = 290,
        VoidContainer = 291,
        ConvertCorpseToHealing = 292,
        OnEntitySpawn = 293,
        OnMorph = 294,
        Sprint = 295,
    }

    public class AreaShapeHolder
    {
        [JsonPropertyName("Circle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AreaShapeCircle? Circle { get; set; }
        [JsonPropertyName("Cone")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AreaShapeCone? Cone { get; set; }
        [JsonPropertyName("ConeCut")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AreaShapeConeCut? ConeCut { get; set; }
        [JsonPropertyName("WideLine")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AreaShapeWideLine? WideLine { get; set; }

        public AreaShape Get()
        {
            if (Circle != null) { return Circle; }
            if (Cone != null) { return Cone; }
            if (ConeCut != null) { return ConeCut; }
            if (WideLine != null) { return WideLine; }
            else { throw new InvalidOperationException("Impossible, because all cases was handled above"); }
        }
        public AreaShapeHolder() { }
        public AreaShapeHolder(AreaShapeCircle v) { Circle = v; }
        public AreaShapeHolder(AreaShapeCone v) { Cone = v; }
        public AreaShapeHolder(AreaShapeConeCut v) { ConeCut = v; }
        public AreaShapeHolder(AreaShapeWideLine v) { WideLine = v; }
        public AreaShapeHolder(AreaShape v)
        {
            switch (v)
            {
                case AreaShapeCircle s:
                    Circle = s; break;
                case AreaShapeCone s:
                    Cone = s; break;
                case AreaShapeConeCut s:
                    ConeCut = s; break;
                case AreaShapeWideLine s:
                    WideLine = s; break;
            }
        }
    }
    [JsonDerivedType(typeof(AreaShapeCircle))]
    [JsonDerivedType(typeof(AreaShapeCone))]
    [JsonDerivedType(typeof(AreaShapeConeCut))]
    [JsonDerivedType(typeof(AreaShapeWideLine))]
    public /*abstract*/ class AreaShape { }

    public sealed class AreaShapeCircle : AreaShape
    {
        [JsonPropertyName("center")]
        public required Position2D Center { get; set; }
        [JsonPropertyName("radius")]
        public required float Radius { get; set; }
    }

    public sealed class AreaShapeCone : AreaShape
    {
        [JsonPropertyName("base")]
        public required Position2D Base { get; set; }
        [JsonPropertyName("radius")]
        public required float Radius { get; set; }
        [JsonPropertyName("angle")]
        public required float Angle { get; set; }
    }

    public sealed class AreaShapeConeCut : AreaShape
    {
        [JsonPropertyName("start")]
        public required Position2D Start { get; set; }
        /// <summary>
        ///  or maybe direction (normalized to length 1), I did not quickly find example to check this on
        /// </summary>
        [JsonPropertyName("end")]
        public required Position2D End { get; set; }
        [JsonPropertyName("radius")]
        public required float Radius { get; set; }
        [JsonPropertyName("width_near")]
        public required float WidthNear { get; set; }
        [JsonPropertyName("width_far")]
        public required float WidthFar { get; set; }
    }

    public sealed class AreaShapeWideLine : AreaShape
    {
        [JsonPropertyName("start")]
        public required Position2D Start { get; set; }
        [JsonPropertyName("end")]
        public required Position2D End { get; set; }
        [JsonPropertyName("width")]
        public required float Width { get; set; }
    }


    public class AbilityEffectSpecificHolder
    {
        [JsonPropertyName("DamageArea")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AbilityEffectSpecificDamageArea? DamageArea { get; set; }
        [JsonPropertyName("DamageOverTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AbilityEffectSpecificDamageOverTime? DamageOverTime { get; set; }
        [JsonPropertyName("LinkedFire")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AbilityEffectSpecificLinkedFire? LinkedFire { get; set; }
        [JsonPropertyName("SpellOnEntityNearby")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AbilityEffectSpecificSpellOnEntityNearby? SpellOnEntityNearby { get; set; }
        [JsonPropertyName("TimedSpell")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AbilityEffectSpecificTimedSpell? TimedSpell { get; set; }
        [JsonPropertyName("Collector")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AbilityEffectSpecificCollector? Collector { get; set; }
        [JsonPropertyName("Aura")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AbilityEffectSpecificAura? Aura { get; set; }
        [JsonPropertyName("MovingIntervalCast")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AbilityEffectSpecificMovingIntervalCast? MovingIntervalCast { get; set; }
        /// <summary>
        ///  If you think something interesting got hidden by Other report it
        /// </summary>
        [JsonPropertyName("Other")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AbilityEffectSpecificOther? Other { get; set; }

        public AbilityEffectSpecific Get()
        {
            if (DamageArea != null) { return DamageArea; }
            if (DamageOverTime != null) { return DamageOverTime; }
            if (LinkedFire != null) { return LinkedFire; }
            if (SpellOnEntityNearby != null) { return SpellOnEntityNearby; }
            if (TimedSpell != null) { return TimedSpell; }
            if (Collector != null) { return Collector; }
            if (Aura != null) { return Aura; }
            if (MovingIntervalCast != null) { return MovingIntervalCast; }
            if (Other != null) { return Other; }
            else { throw new InvalidOperationException("Impossible, because all cases was handled above"); }
        }
        public AbilityEffectSpecificHolder() { }
        public AbilityEffectSpecificHolder(AbilityEffectSpecificDamageArea v) { DamageArea = v; }
        public AbilityEffectSpecificHolder(AbilityEffectSpecificDamageOverTime v) { DamageOverTime = v; }
        public AbilityEffectSpecificHolder(AbilityEffectSpecificLinkedFire v) { LinkedFire = v; }
        public AbilityEffectSpecificHolder(AbilityEffectSpecificSpellOnEntityNearby v) { SpellOnEntityNearby = v; }
        public AbilityEffectSpecificHolder(AbilityEffectSpecificTimedSpell v) { TimedSpell = v; }
        public AbilityEffectSpecificHolder(AbilityEffectSpecificCollector v) { Collector = v; }
        public AbilityEffectSpecificHolder(AbilityEffectSpecificAura v) { Aura = v; }
        public AbilityEffectSpecificHolder(AbilityEffectSpecificMovingIntervalCast v) { MovingIntervalCast = v; }
        public AbilityEffectSpecificHolder(AbilityEffectSpecificOther v) { Other = v; }
        public AbilityEffectSpecificHolder(AbilityEffectSpecific v)
        {
            switch (v)
            {
                case AbilityEffectSpecificDamageArea s:
                    DamageArea = s; break;
                case AbilityEffectSpecificDamageOverTime s:
                    DamageOverTime = s; break;
                case AbilityEffectSpecificLinkedFire s:
                    LinkedFire = s; break;
                case AbilityEffectSpecificSpellOnEntityNearby s:
                    SpellOnEntityNearby = s; break;
                case AbilityEffectSpecificTimedSpell s:
                    TimedSpell = s; break;
                case AbilityEffectSpecificCollector s:
                    Collector = s; break;
                case AbilityEffectSpecificAura s:
                    Aura = s; break;
                case AbilityEffectSpecificMovingIntervalCast s:
                    MovingIntervalCast = s; break;
                case AbilityEffectSpecificOther s:
                    Other = s; break;
            }
        }
    }
    [JsonDerivedType(typeof(AbilityEffectSpecificDamageArea))]
    [JsonDerivedType(typeof(AbilityEffectSpecificDamageOverTime))]
    [JsonDerivedType(typeof(AbilityEffectSpecificLinkedFire))]
    [JsonDerivedType(typeof(AbilityEffectSpecificSpellOnEntityNearby))]
    [JsonDerivedType(typeof(AbilityEffectSpecificTimedSpell))]
    [JsonDerivedType(typeof(AbilityEffectSpecificCollector))]
    [JsonDerivedType(typeof(AbilityEffectSpecificAura))]
    [JsonDerivedType(typeof(AbilityEffectSpecificMovingIntervalCast))]
    [JsonDerivedType(typeof(AbilityEffectSpecificOther))]
    public /*abstract*/ class AbilityEffectSpecific { }

    public sealed class AbilityEffectSpecificDamageArea : AbilityEffectSpecific
    {
        [JsonPropertyName("progress_current")]
        public required float ProgressCurrent { get; set; }
        [JsonPropertyName("progress_delta")]
        public required float ProgressDelta { get; set; }
        [JsonPropertyName("damage_remaining")]
        public required float DamageRemaining { get; set; }
        [JsonPropertyName("shape")]
        public required AreaShapeHolder Shape { get; set; }
    }

    public sealed class AbilityEffectSpecificDamageOverTime : AbilityEffectSpecific
    {
        [JsonPropertyName("tick_wait_duration")]
        public required TickCount TickWaitDuration { get; set; }
        [JsonPropertyName("ticks_left")]
        public required TickCount TicksLeft { get; set; }
        [JsonPropertyName("tick_damage")]
        public required float TickDamage { get; set; }
    }

    public sealed class AbilityEffectSpecificLinkedFire : AbilityEffectSpecific
    {
        [JsonPropertyName("linked")]
        public required bool Linked { get; set; }
        [JsonPropertyName("fighting")]
        public required bool Fighting { get; set; }
        [JsonPropertyName("fast_cast")]
        public required UInt32 FastCast { get; set; }
        [JsonPropertyName("support_cap")]
        public required UInt16 SupportCap { get; set; }
        [JsonPropertyName("support_production")]
        public required byte SupportProduction { get; set; }
    }

    public sealed class AbilityEffectSpecificSpellOnEntityNearby : AbilityEffectSpecific
    {
        [JsonPropertyName("spell_on_owner")]
        public required SpellId[] SpellOnOwner { get; set; }
        [JsonPropertyName("spell_on_source")]
        public required SpellId[] SpellOnSource { get; set; }
        [JsonPropertyName("radius")]
        public required float Radius { get; set; }
        [JsonPropertyName("remaining_targets")]
        public required UInt32 RemainingTargets { get; set; }
    }

    public sealed class AbilityEffectSpecificTimedSpell : AbilityEffectSpecific
    {
        [JsonPropertyName("spells_to_cast")]
        public required SpellId[] SpellsToCast { get; set; }
    }

    public sealed class AbilityEffectSpecificCollector : AbilityEffectSpecific
    {
        [JsonPropertyName("spell_to_cast")]
        public required SpellId SpellToCast { get; set; }
        [JsonPropertyName("radius")]
        public required float Radius { get; set; }
    }

    public sealed class AbilityEffectSpecificAura : AbilityEffectSpecific
    {
        [JsonPropertyName("spells_to_apply")]
        public required SpellId[] SpellsToApply { get; set; }
        [JsonPropertyName("abilities_to_apply")]
        public required AbilityId[] AbilitiesToApply { get; set; }
        [JsonPropertyName("radius")]
        public required float Radius { get; set; }
    }

    public sealed class AbilityEffectSpecificMovingIntervalCast : AbilityEffectSpecific
    {
        [JsonPropertyName("spell_to_cast")]
        public required SpellId[] SpellToCast { get; set; }
        [JsonPropertyName("direction_step")]
        public required Position2D DirectionStep { get; set; }
        [JsonPropertyName("cast_every_nth_tick")]
        public required TickCount CastEveryNthTick { get; set; }
    }

    /// <summary>
    ///  If you think something interesting got hidden by Other report it
    /// </summary>
    public sealed class AbilityEffectSpecificOther : AbilityEffectSpecific
    {
    }


    public class AbilityEffect
    {
        [JsonPropertyName("id")]
        public required AbilityId Id { get; set; }
        [JsonPropertyName("line")]
        public required AbilityLine Line { get; set; }
        [JsonPropertyName("source")]
        public required EntityId Source { get; set; }
        [JsonPropertyName("source_team")]
        public required byte SourceTeam { get; set; }
        [JsonPropertyName("start_tick")]
        public Tick? StartTick { get; set; }
        [JsonPropertyName("end_tick")]
        public Tick? EndTick { get; set; }
        [JsonPropertyName("specific")]
        public required AbilityEffectSpecificHolder Specific { get; set; }
    }

    public class AspectHolder
    {
        /// <summary>
        ///  Used by *mostly* power wells
        /// </summary>
        [JsonPropertyName("PowerProduction")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectPowerProduction? PowerProduction { get; set; }
        /// <summary>
        ///  Health of an entity.
        /// </summary>
        [JsonPropertyName("Health")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectHealth? Health { get; set; }
        [JsonPropertyName("Combat")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectCombat? Combat { get; set; }
        [JsonPropertyName("ModeChange")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectModeChange? ModeChange { get; set; }
        [JsonPropertyName("Ammunition")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectAmmunition? Ammunition { get; set; }
        [JsonPropertyName("SuperWeaponShadow")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectSuperWeaponShadow? SuperWeaponShadow { get; set; }
        [JsonPropertyName("WormMovement")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectWormMovement? WormMovement { get; set; }
        [JsonPropertyName("NPCTag")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectNPCTag? NPCTag { get; set; }
        [JsonPropertyName("PlayerKit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectPlayerKit? PlayerKit { get; set; }
        [JsonPropertyName("Loot")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectLoot? Loot { get; set; }
        [JsonPropertyName("Immunity")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectImmunity? Immunity { get; set; }
        [JsonPropertyName("Turret")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectTurret? Turret { get; set; }
        [JsonPropertyName("Tunnel")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectTunnel? Tunnel { get; set; }
        [JsonPropertyName("MountBarrier")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectMountBarrier? MountBarrier { get; set; }
        [JsonPropertyName("SpellMemory")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectSpellMemory? SpellMemory { get; set; }
        [JsonPropertyName("Portal")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectPortal? Portal { get; set; }
        [JsonPropertyName("Hate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectHate? Hate { get; set; }
        [JsonPropertyName("BarrierGate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectBarrierGate? BarrierGate { get; set; }
        [JsonPropertyName("Attackable")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectAttackable? Attackable { get; set; }
        [JsonPropertyName("SquadRefill")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectSquadRefill? SquadRefill { get; set; }
        [JsonPropertyName("PortalExit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectPortalExit? PortalExit { get; set; }
        /// <summary>
        ///  When building / barrier is under construction it has this aspect.
        /// </summary>
        [JsonPropertyName("ConstructionData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectConstructionData? ConstructionData { get; set; }
        [JsonPropertyName("SuperWeaponShadowBomb")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectSuperWeaponShadowBomb? SuperWeaponShadowBomb { get; set; }
        [JsonPropertyName("RepairBarrierSet")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectRepairBarrierSet? RepairBarrierSet { get; set; }
        [JsonPropertyName("ConstructionRepair")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectConstructionRepair? ConstructionRepair { get; set; }
        [JsonPropertyName("Follower")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectFollower? Follower { get; set; }
        [JsonPropertyName("CollisionBase")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectCollisionBase? CollisionBase { get; set; }
        [JsonPropertyName("EditorUniqueID")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectEditorUniqueID? EditorUniqueID { get; set; }
        [JsonPropertyName("Roam")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AspectRoam? Roam { get; set; }

        public Aspect Get()
        {
            if (PowerProduction != null) { return PowerProduction; }
            if (Health != null) { return Health; }
            if (Combat != null) { return Combat; }
            if (ModeChange != null) { return ModeChange; }
            if (Ammunition != null) { return Ammunition; }
            if (SuperWeaponShadow != null) { return SuperWeaponShadow; }
            if (WormMovement != null) { return WormMovement; }
            if (NPCTag != null) { return NPCTag; }
            if (PlayerKit != null) { return PlayerKit; }
            if (Loot != null) { return Loot; }
            if (Immunity != null) { return Immunity; }
            if (Turret != null) { return Turret; }
            if (Tunnel != null) { return Tunnel; }
            if (MountBarrier != null) { return MountBarrier; }
            if (SpellMemory != null) { return SpellMemory; }
            if (Portal != null) { return Portal; }
            if (Hate != null) { return Hate; }
            if (BarrierGate != null) { return BarrierGate; }
            if (Attackable != null) { return Attackable; }
            if (SquadRefill != null) { return SquadRefill; }
            if (PortalExit != null) { return PortalExit; }
            if (ConstructionData != null) { return ConstructionData; }
            if (SuperWeaponShadowBomb != null) { return SuperWeaponShadowBomb; }
            if (RepairBarrierSet != null) { return RepairBarrierSet; }
            if (ConstructionRepair != null) { return ConstructionRepair; }
            if (Follower != null) { return Follower; }
            if (CollisionBase != null) { return CollisionBase; }
            if (EditorUniqueID != null) { return EditorUniqueID; }
            if (Roam != null) { return Roam; }
            else { throw new InvalidOperationException("Impossible, because all cases was handled above"); }
        }
        public AspectHolder() { }
        public AspectHolder(AspectPowerProduction v) { PowerProduction = v; }
        public AspectHolder(AspectHealth v) { Health = v; }
        public AspectHolder(AspectCombat v) { Combat = v; }
        public AspectHolder(AspectModeChange v) { ModeChange = v; }
        public AspectHolder(AspectAmmunition v) { Ammunition = v; }
        public AspectHolder(AspectSuperWeaponShadow v) { SuperWeaponShadow = v; }
        public AspectHolder(AspectWormMovement v) { WormMovement = v; }
        public AspectHolder(AspectNPCTag v) { NPCTag = v; }
        public AspectHolder(AspectPlayerKit v) { PlayerKit = v; }
        public AspectHolder(AspectLoot v) { Loot = v; }
        public AspectHolder(AspectImmunity v) { Immunity = v; }
        public AspectHolder(AspectTurret v) { Turret = v; }
        public AspectHolder(AspectTunnel v) { Tunnel = v; }
        public AspectHolder(AspectMountBarrier v) { MountBarrier = v; }
        public AspectHolder(AspectSpellMemory v) { SpellMemory = v; }
        public AspectHolder(AspectPortal v) { Portal = v; }
        public AspectHolder(AspectHate v) { Hate = v; }
        public AspectHolder(AspectBarrierGate v) { BarrierGate = v; }
        public AspectHolder(AspectAttackable v) { Attackable = v; }
        public AspectHolder(AspectSquadRefill v) { SquadRefill = v; }
        public AspectHolder(AspectPortalExit v) { PortalExit = v; }
        public AspectHolder(AspectConstructionData v) { ConstructionData = v; }
        public AspectHolder(AspectSuperWeaponShadowBomb v) { SuperWeaponShadowBomb = v; }
        public AspectHolder(AspectRepairBarrierSet v) { RepairBarrierSet = v; }
        public AspectHolder(AspectConstructionRepair v) { ConstructionRepair = v; }
        public AspectHolder(AspectFollower v) { Follower = v; }
        public AspectHolder(AspectCollisionBase v) { CollisionBase = v; }
        public AspectHolder(AspectEditorUniqueID v) { EditorUniqueID = v; }
        public AspectHolder(AspectRoam v) { Roam = v; }
        public AspectHolder(Aspect v)
        {
            switch (v)
            {
                case AspectPowerProduction s:
                    PowerProduction = s; break;
                case AspectHealth s:
                    Health = s; break;
                case AspectCombat s:
                    Combat = s; break;
                case AspectModeChange s:
                    ModeChange = s; break;
                case AspectAmmunition s:
                    Ammunition = s; break;
                case AspectSuperWeaponShadow s:
                    SuperWeaponShadow = s; break;
                case AspectWormMovement s:
                    WormMovement = s; break;
                case AspectNPCTag s:
                    NPCTag = s; break;
                case AspectPlayerKit s:
                    PlayerKit = s; break;
                case AspectLoot s:
                    Loot = s; break;
                case AspectImmunity s:
                    Immunity = s; break;
                case AspectTurret s:
                    Turret = s; break;
                case AspectTunnel s:
                    Tunnel = s; break;
                case AspectMountBarrier s:
                    MountBarrier = s; break;
                case AspectSpellMemory s:
                    SpellMemory = s; break;
                case AspectPortal s:
                    Portal = s; break;
                case AspectHate s:
                    Hate = s; break;
                case AspectBarrierGate s:
                    BarrierGate = s; break;
                case AspectAttackable s:
                    Attackable = s; break;
                case AspectSquadRefill s:
                    SquadRefill = s; break;
                case AspectPortalExit s:
                    PortalExit = s; break;
                case AspectConstructionData s:
                    ConstructionData = s; break;
                case AspectSuperWeaponShadowBomb s:
                    SuperWeaponShadowBomb = s; break;
                case AspectRepairBarrierSet s:
                    RepairBarrierSet = s; break;
                case AspectConstructionRepair s:
                    ConstructionRepair = s; break;
                case AspectFollower s:
                    Follower = s; break;
                case AspectCollisionBase s:
                    CollisionBase = s; break;
                case AspectEditorUniqueID s:
                    EditorUniqueID = s; break;
                case AspectRoam s:
                    Roam = s; break;
            }
        }
    }
    [JsonDerivedType(typeof(AspectPowerProduction))]
    [JsonDerivedType(typeof(AspectHealth))]
    [JsonDerivedType(typeof(AspectCombat))]
    [JsonDerivedType(typeof(AspectModeChange))]
    [JsonDerivedType(typeof(AspectAmmunition))]
    [JsonDerivedType(typeof(AspectSuperWeaponShadow))]
    [JsonDerivedType(typeof(AspectWormMovement))]
    [JsonDerivedType(typeof(AspectNPCTag))]
    [JsonDerivedType(typeof(AspectPlayerKit))]
    [JsonDerivedType(typeof(AspectLoot))]
    [JsonDerivedType(typeof(AspectImmunity))]
    [JsonDerivedType(typeof(AspectTurret))]
    [JsonDerivedType(typeof(AspectTunnel))]
    [JsonDerivedType(typeof(AspectMountBarrier))]
    [JsonDerivedType(typeof(AspectSpellMemory))]
    [JsonDerivedType(typeof(AspectPortal))]
    [JsonDerivedType(typeof(AspectHate))]
    [JsonDerivedType(typeof(AspectBarrierGate))]
    [JsonDerivedType(typeof(AspectAttackable))]
    [JsonDerivedType(typeof(AspectSquadRefill))]
    [JsonDerivedType(typeof(AspectPortalExit))]
    [JsonDerivedType(typeof(AspectConstructionData))]
    [JsonDerivedType(typeof(AspectSuperWeaponShadowBomb))]
    [JsonDerivedType(typeof(AspectRepairBarrierSet))]
    [JsonDerivedType(typeof(AspectConstructionRepair))]
    [JsonDerivedType(typeof(AspectFollower))]
    [JsonDerivedType(typeof(AspectCollisionBase))]
    [JsonDerivedType(typeof(AspectEditorUniqueID))]
    [JsonDerivedType(typeof(AspectRoam))]
    public /*abstract*/ class Aspect { }

    /// <summary>
    ///  Used by *mostly* power wells
    /// </summary>
    public sealed class AspectPowerProduction : Aspect
    {
        /// <summary>
        ///  How much more power it will produce
        /// </summary>
        [JsonPropertyName("current_power")]
        public required float CurrentPower { get; set; }
        /// <summary>
        ///  Same as `current_power`, before it is build for the first time.
        /// </summary>
        [JsonPropertyName("power_capacity")]
        public required float PowerCapacity { get; set; }
    }

    /// <summary>
    ///  Health of an entity.
    /// </summary>
    public sealed class AspectHealth : Aspect
    {
        /// <summary>
        ///  Actual HP that it can lose before dying.
        /// </summary>
        [JsonPropertyName("current_hp")]
        public required float CurrentHp { get; set; }
        /// <summary>
        ///  Current maximum including bufs and debufs.
        /// </summary>
        [JsonPropertyName("cap_current_max")]
        public required float CapCurrentMax { get; set; }
    }

    public sealed class AspectCombat : Aspect
    {
    }

    public sealed class AspectModeChange : Aspect
    {
        [JsonPropertyName("current_mode")]
        public required ModeId CurrentMode { get; set; }
        [JsonPropertyName("all_modes")]
        public required ModeId[] AllModes { get; set; }
    }

    public sealed class AspectAmmunition : Aspect
    {
    }

    public sealed class AspectSuperWeaponShadow : Aspect
    {
    }

    public sealed class AspectWormMovement : Aspect
    {
    }

    public sealed class AspectNPCTag : Aspect
    {
    }

    public sealed class AspectPlayerKit : Aspect
    {
    }

    public sealed class AspectLoot : Aspect
    {
    }

    public sealed class AspectImmunity : Aspect
    {
    }

    public sealed class AspectTurret : Aspect
    {
    }

    public sealed class AspectTunnel : Aspect
    {
    }

    public sealed class AspectMountBarrier : Aspect
    {
    }

    public sealed class AspectSpellMemory : Aspect
    {
    }

    public sealed class AspectPortal : Aspect
    {
    }

    public sealed class AspectHate : Aspect
    {
    }

    public sealed class AspectBarrierGate : Aspect
    {
        [JsonPropertyName("open")]
        public required bool Open { get; set; }
    }

    public sealed class AspectAttackable : Aspect
    {
    }

    public sealed class AspectSquadRefill : Aspect
    {
    }

    public sealed class AspectPortalExit : Aspect
    {
    }

    /// <summary>
    ///  When building / barrier is under construction it has this aspect.
    /// </summary>
    public sealed class AspectConstructionData : Aspect
    {
        /// <summary>
        ///  Build ticks until finished.
        /// </summary>
        [JsonPropertyName("refresh_count_remaining")]
        public required TickCount RefreshCountRemaining { get; set; }
        /// <summary>
        ///  Build ticks needed from start of construction to finish it.
        /// </summary>
        [JsonPropertyName("refresh_count_total")]
        public required TickCount RefreshCountTotal { get; set; }
        /// <summary>
        ///  How much health is added on build tick.
        /// </summary>
        [JsonPropertyName("health_per_build_update_trigger")]
        public required float HealthPerBuildUpdateTrigger { get; set; }
        /// <summary>
        ///  How much health is still missing.
        /// </summary>
        [JsonPropertyName("remaining_health_to_add")]
        public required float RemainingHealthToAdd { get; set; }
    }

    public sealed class AspectSuperWeaponShadowBomb : Aspect
    {
    }

    public sealed class AspectRepairBarrierSet : Aspect
    {
    }

    public sealed class AspectConstructionRepair : Aspect
    {
    }

    public sealed class AspectFollower : Aspect
    {
    }

    public sealed class AspectCollisionBase : Aspect
    {
    }

    public sealed class AspectEditorUniqueID : Aspect
    {
    }

    public sealed class AspectRoam : Aspect
    {
    }


    /// <summary>
    ///  Simplified version of how many monuments of each color player have
    /// </summary>
    public class Orbs
    {
        [JsonPropertyName("shadow")]
        public required byte Shadow { get; set; }
        [JsonPropertyName("nature")]
        public required byte Nature { get; set; }
        [JsonPropertyName("frost")]
        public required byte Frost { get; set; }
        [JsonPropertyName("fire")]
        public required byte Fire { get; set; }
        /// <summary>
        ///  Can be used instead of any color, and then changes to color of first token on the used card.
        /// </summary>
        [JsonPropertyName("starting")]
        public required byte Starting { get; set; }
        /// <summary>
        ///  Can be used only for colorless tokens on the card. (Curse Orb changes colored orb to white one)
        /// </summary>
        [JsonPropertyName("white")]
        public required byte White { get; set; }
        /// <summary>
        ///  Can be used as any color. Only provided by map scripts.
        /// </summary>
        [JsonPropertyName("all")]
        public required byte All { get; set; }
    }

    /// <summary>
    ///  Technically it is specific case of `Entity`, but we decided to move players out,
    ///  and move few fields up like position and owning player id
    /// </summary>
    public class PlayerEntity
    {
        /// <summary>
        ///  Unique id of the entity
        /// </summary>
        [JsonPropertyName("id")]
        public required EntityId Id { get; set; }
        /// <summary>
        ///  List of effects the entity have.
        /// </summary>
        [JsonPropertyName("effects")]
        public required AbilityEffect[] Effects { get; set; }
        /// <summary>
        ///  List of aspects entity have.
        /// </summary>
        [JsonPropertyName("aspects")]
        public required AspectHolder[] Aspects { get; set; }
        [JsonPropertyName("team")]
        public required byte Team { get; set; }
        [JsonPropertyName("power")]
        public required float Power { get; set; }
        [JsonPropertyName("void_power")]
        public required float VoidPower { get; set; }
        [JsonPropertyName("population_count")]
        public required UInt16 PopulationCount { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("orbs")]
        public required Orbs Orbs { get; set; }
    }

    public class MatchPlayer
    {
        /// <summary>
        ///  Name of player.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        /// <summary>
        ///  Deck used by that player.
        ///  TODO Due to technical difficulties might be empty.
        /// </summary>
        [JsonPropertyName("deck")]
        public required Deck Deck { get; set; }
        /// <summary>
        ///  entity controlled by this player
        /// </summary>
        [JsonPropertyName("entity")]
        public required PlayerEntity Entity { get; set; }
    }

    /// <summary>
    ///  With the way the game works, I would not be surprised, if this will cause more issues.
    ///  If the game crashes send the log to `Kubik` it probably mean some field in
    ///  one of the `Job`s needs to be `Option`.
    /// </summary>
    public class JobHolder
    {
        [JsonPropertyName("NoJob")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobNoJob? NoJob { get; set; }
        [JsonPropertyName("Idle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobIdle? Idle { get; set; }
        [JsonPropertyName("Goto")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobGoto? Goto { get; set; }
        [JsonPropertyName("AttackMelee")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobAttackMelee? AttackMelee { get; set; }
        [JsonPropertyName("CastSpell")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobCastSpell? CastSpell { get; set; }
        [JsonPropertyName("Die")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobDie? Die { get; set; }
        [JsonPropertyName("Talk")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobTalk? Talk { get; set; }
        [JsonPropertyName("ScriptTalk")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobScriptTalk? ScriptTalk { get; set; }
        [JsonPropertyName("Freeze")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobFreeze? Freeze { get; set; }
        [JsonPropertyName("Spawn")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobSpawn? Spawn { get; set; }
        [JsonPropertyName("Cheer")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobCheer? Cheer { get; set; }
        [JsonPropertyName("AttackSquad")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobAttackSquad? AttackSquad { get; set; }
        [JsonPropertyName("CastSpellSquad")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobCastSpellSquad? CastSpellSquad { get; set; }
        [JsonPropertyName("PushBack")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobPushBack? PushBack { get; set; }
        [JsonPropertyName("Stampede")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobStampede? Stampede { get; set; }
        [JsonPropertyName("BarrierCrush")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobBarrierCrush? BarrierCrush { get; set; }
        [JsonPropertyName("BarrierGateToggle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobBarrierGateToggle? BarrierGateToggle { get; set; }
        [JsonPropertyName("FlameThrower")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobFlameThrower? FlameThrower { get; set; }
        [JsonPropertyName("Construct")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobConstruct? Construct { get; set; }
        [JsonPropertyName("Crush")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobCrush? Crush { get; set; }
        [JsonPropertyName("MountBarrierSquad")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobMountBarrierSquad? MountBarrierSquad { get; set; }
        [JsonPropertyName("MountBarrier")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobMountBarrier? MountBarrier { get; set; }
        [JsonPropertyName("ModeChangeSquad")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobModeChangeSquad? ModeChangeSquad { get; set; }
        [JsonPropertyName("ModeChange")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobModeChange? ModeChange { get; set; }
        [JsonPropertyName("SacrificeSquad")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobSacrificeSquad? SacrificeSquad { get; set; }
        [JsonPropertyName("UsePortalSquad")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobUsePortalSquad? UsePortalSquad { get; set; }
        [JsonPropertyName("Channel")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobChannel? Channel { get; set; }
        [JsonPropertyName("SpawnSquad")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobSpawnSquad? SpawnSquad { get; set; }
        [JsonPropertyName("LootTargetSquad")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobLootTargetSquad? LootTargetSquad { get; set; }
        [JsonPropertyName("Morph")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobMorph? Morph { get; set; }
        /// <summary>
        ///  if you see this it means we did not account for some EA's case, so please report it
        /// </summary>
        [JsonPropertyName("Unknown")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JobUnknown? Unknown { get; set; }

        public Job Get()
        {
            if (NoJob != null) { return NoJob; }
            if (Idle != null) { return Idle; }
            if (Goto != null) { return Goto; }
            if (AttackMelee != null) { return AttackMelee; }
            if (CastSpell != null) { return CastSpell; }
            if (Die != null) { return Die; }
            if (Talk != null) { return Talk; }
            if (ScriptTalk != null) { return ScriptTalk; }
            if (Freeze != null) { return Freeze; }
            if (Spawn != null) { return Spawn; }
            if (Cheer != null) { return Cheer; }
            if (AttackSquad != null) { return AttackSquad; }
            if (CastSpellSquad != null) { return CastSpellSquad; }
            if (PushBack != null) { return PushBack; }
            if (Stampede != null) { return Stampede; }
            if (BarrierCrush != null) { return BarrierCrush; }
            if (BarrierGateToggle != null) { return BarrierGateToggle; }
            if (FlameThrower != null) { return FlameThrower; }
            if (Construct != null) { return Construct; }
            if (Crush != null) { return Crush; }
            if (MountBarrierSquad != null) { return MountBarrierSquad; }
            if (MountBarrier != null) { return MountBarrier; }
            if (ModeChangeSquad != null) { return ModeChangeSquad; }
            if (ModeChange != null) { return ModeChange; }
            if (SacrificeSquad != null) { return SacrificeSquad; }
            if (UsePortalSquad != null) { return UsePortalSquad; }
            if (Channel != null) { return Channel; }
            if (SpawnSquad != null) { return SpawnSquad; }
            if (LootTargetSquad != null) { return LootTargetSquad; }
            if (Morph != null) { return Morph; }
            if (Unknown != null) { return Unknown; }
            else { throw new InvalidOperationException("Impossible, because all cases was handled above"); }
        }
        public JobHolder() { }
        public JobHolder(JobNoJob v) { NoJob = v; }
        public JobHolder(JobIdle v) { Idle = v; }
        public JobHolder(JobGoto v) { Goto = v; }
        public JobHolder(JobAttackMelee v) { AttackMelee = v; }
        public JobHolder(JobCastSpell v) { CastSpell = v; }
        public JobHolder(JobDie v) { Die = v; }
        public JobHolder(JobTalk v) { Talk = v; }
        public JobHolder(JobScriptTalk v) { ScriptTalk = v; }
        public JobHolder(JobFreeze v) { Freeze = v; }
        public JobHolder(JobSpawn v) { Spawn = v; }
        public JobHolder(JobCheer v) { Cheer = v; }
        public JobHolder(JobAttackSquad v) { AttackSquad = v; }
        public JobHolder(JobCastSpellSquad v) { CastSpellSquad = v; }
        public JobHolder(JobPushBack v) { PushBack = v; }
        public JobHolder(JobStampede v) { Stampede = v; }
        public JobHolder(JobBarrierCrush v) { BarrierCrush = v; }
        public JobHolder(JobBarrierGateToggle v) { BarrierGateToggle = v; }
        public JobHolder(JobFlameThrower v) { FlameThrower = v; }
        public JobHolder(JobConstruct v) { Construct = v; }
        public JobHolder(JobCrush v) { Crush = v; }
        public JobHolder(JobMountBarrierSquad v) { MountBarrierSquad = v; }
        public JobHolder(JobMountBarrier v) { MountBarrier = v; }
        public JobHolder(JobModeChangeSquad v) { ModeChangeSquad = v; }
        public JobHolder(JobModeChange v) { ModeChange = v; }
        public JobHolder(JobSacrificeSquad v) { SacrificeSquad = v; }
        public JobHolder(JobUsePortalSquad v) { UsePortalSquad = v; }
        public JobHolder(JobChannel v) { Channel = v; }
        public JobHolder(JobSpawnSquad v) { SpawnSquad = v; }
        public JobHolder(JobLootTargetSquad v) { LootTargetSquad = v; }
        public JobHolder(JobMorph v) { Morph = v; }
        public JobHolder(JobUnknown v) { Unknown = v; }
        public JobHolder(Job v)
        {
            switch (v)
            {
                case JobNoJob s:
                    NoJob = s; break;
                case JobIdle s:
                    Idle = s; break;
                case JobGoto s:
                    Goto = s; break;
                case JobAttackMelee s:
                    AttackMelee = s; break;
                case JobCastSpell s:
                    CastSpell = s; break;
                case JobDie s:
                    Die = s; break;
                case JobTalk s:
                    Talk = s; break;
                case JobScriptTalk s:
                    ScriptTalk = s; break;
                case JobFreeze s:
                    Freeze = s; break;
                case JobSpawn s:
                    Spawn = s; break;
                case JobCheer s:
                    Cheer = s; break;
                case JobAttackSquad s:
                    AttackSquad = s; break;
                case JobCastSpellSquad s:
                    CastSpellSquad = s; break;
                case JobPushBack s:
                    PushBack = s; break;
                case JobStampede s:
                    Stampede = s; break;
                case JobBarrierCrush s:
                    BarrierCrush = s; break;
                case JobBarrierGateToggle s:
                    BarrierGateToggle = s; break;
                case JobFlameThrower s:
                    FlameThrower = s; break;
                case JobConstruct s:
                    Construct = s; break;
                case JobCrush s:
                    Crush = s; break;
                case JobMountBarrierSquad s:
                    MountBarrierSquad = s; break;
                case JobMountBarrier s:
                    MountBarrier = s; break;
                case JobModeChangeSquad s:
                    ModeChangeSquad = s; break;
                case JobModeChange s:
                    ModeChange = s; break;
                case JobSacrificeSquad s:
                    SacrificeSquad = s; break;
                case JobUsePortalSquad s:
                    UsePortalSquad = s; break;
                case JobChannel s:
                    Channel = s; break;
                case JobSpawnSquad s:
                    SpawnSquad = s; break;
                case JobLootTargetSquad s:
                    LootTargetSquad = s; break;
                case JobMorph s:
                    Morph = s; break;
                case JobUnknown s:
                    Unknown = s; break;
            }
        }
    }
    [JsonDerivedType(typeof(JobNoJob))]
    [JsonDerivedType(typeof(JobIdle))]
    [JsonDerivedType(typeof(JobGoto))]
    [JsonDerivedType(typeof(JobAttackMelee))]
    [JsonDerivedType(typeof(JobCastSpell))]
    [JsonDerivedType(typeof(JobDie))]
    [JsonDerivedType(typeof(JobTalk))]
    [JsonDerivedType(typeof(JobScriptTalk))]
    [JsonDerivedType(typeof(JobFreeze))]
    [JsonDerivedType(typeof(JobSpawn))]
    [JsonDerivedType(typeof(JobCheer))]
    [JsonDerivedType(typeof(JobAttackSquad))]
    [JsonDerivedType(typeof(JobCastSpellSquad))]
    [JsonDerivedType(typeof(JobPushBack))]
    [JsonDerivedType(typeof(JobStampede))]
    [JsonDerivedType(typeof(JobBarrierCrush))]
    [JsonDerivedType(typeof(JobBarrierGateToggle))]
    [JsonDerivedType(typeof(JobFlameThrower))]
    [JsonDerivedType(typeof(JobConstruct))]
    [JsonDerivedType(typeof(JobCrush))]
    [JsonDerivedType(typeof(JobMountBarrierSquad))]
    [JsonDerivedType(typeof(JobMountBarrier))]
    [JsonDerivedType(typeof(JobModeChangeSquad))]
    [JsonDerivedType(typeof(JobModeChange))]
    [JsonDerivedType(typeof(JobSacrificeSquad))]
    [JsonDerivedType(typeof(JobUsePortalSquad))]
    [JsonDerivedType(typeof(JobChannel))]
    [JsonDerivedType(typeof(JobSpawnSquad))]
    [JsonDerivedType(typeof(JobLootTargetSquad))]
    [JsonDerivedType(typeof(JobMorph))]
    [JsonDerivedType(typeof(JobUnknown))]
    public /*abstract*/ class Job { }

    public sealed class JobNoJob : Job
    {
    }

    public sealed class JobIdle : Job
    {
    }

    public sealed class JobGoto : Job
    {
        [JsonPropertyName("waypoints")]
        public required Position2DWithOrientation[] Waypoints { get; set; }
        [JsonPropertyName("target_entity_id")]
        public EntityId? TargetEntityId { get; set; }
        [JsonPropertyName("walk_mode")]
        public required WalkMode WalkMode { get; set; }
    }

    public sealed class JobAttackMelee : Job
    {
        [JsonPropertyName("target")]
        public required TargetHolder Target { get; set; }
        [JsonPropertyName("use_force_goto")]
        public required bool UseForceGoto { get; set; }
        [JsonPropertyName("no_move")]
        public required bool NoMove { get; set; }
        [JsonPropertyName("too_close_range")]
        public required float TooCloseRange { get; set; }
    }

    public sealed class JobCastSpell : Job
    {
        [JsonPropertyName("target")]
        public required TargetHolder Target { get; set; }
        [JsonPropertyName("spell_id")]
        public required SpellId SpellId { get; set; }
        [JsonPropertyName("use_force_goto")]
        public required bool UseForceGoto { get; set; }
        [JsonPropertyName("no_move")]
        public required bool NoMove { get; set; }
    }

    public sealed class JobDie : Job
    {
    }

    public sealed class JobTalk : Job
    {
        [JsonPropertyName("target")]
        public required EntityId Target { get; set; }
        [JsonPropertyName("walk_to_target")]
        public required bool WalkToTarget { get; set; }
    }

    public sealed class JobScriptTalk : Job
    {
        [JsonPropertyName("hide_weapon")]
        public required bool HideWeapon { get; set; }
    }

    public sealed class JobFreeze : Job
    {
        [JsonPropertyName("end_step")]
        public required Tick EndStep { get; set; }
        [JsonPropertyName("source")]
        public required EntityId Source { get; set; }
        [JsonPropertyName("spell_id")]
        public required SpellId SpellId { get; set; }
        [JsonPropertyName("duration")]
        public required TickCount Duration { get; set; }
        [JsonPropertyName("delay_ability")]
        public required TickCount DelayAbility { get; set; }
        [JsonPropertyName("ability_id_while_frozen")]
        public required AbilityId[] AbilityIdWhileFrozen { get; set; }
        [JsonPropertyName("ability_id_delayed")]
        public required AbilityId[] AbilityIdDelayed { get; set; }
        [JsonPropertyName("ability_line_id_cancel_on_start")]
        public required AbilityLine AbilityLineIdCancelOnStart { get; set; }
        [JsonPropertyName("pushback_immunity")]
        public required bool PushbackImmunity { get; set; }
        [JsonPropertyName("mode")]
        public required UInt32 Mode { get; set; }
    }

    public sealed class JobSpawn : Job
    {
        [JsonPropertyName("duration")]
        public required TickCount Duration { get; set; }
        [JsonPropertyName("end_step")]
        public required Tick EndStep { get; set; }
    }

    public sealed class JobCheer : Job
    {
    }

    public sealed class JobAttackSquad : Job
    {
        [JsonPropertyName("target")]
        public required TargetHolder Target { get; set; }
        [JsonPropertyName("weapon_type")]
        public required byte WeaponType { get; set; }
        [JsonPropertyName("damage")]
        public required float Damage { get; set; }
        [JsonPropertyName("range_min")]
        public required float RangeMin { get; set; }
        [JsonPropertyName("range_max")]
        public required float RangeMax { get; set; }
        [JsonPropertyName("attack_spell")]
        public SpellId? AttackSpell { get; set; }
        [JsonPropertyName("use_force_goto")]
        public required bool UseForceGoto { get; set; }
        [JsonPropertyName("operation_range")]
        public required float OperationRange { get; set; }
        [JsonPropertyName("no_move")]
        public required bool NoMove { get; set; }
        [JsonPropertyName("was_in_attack")]
        public required bool WasInAttack { get; set; }
        [JsonPropertyName("melee_attack")]
        public required bool MeleeAttack { get; set; }
    }

    public sealed class JobCastSpellSquad : Job
    {
        [JsonPropertyName("target")]
        public required TargetHolder Target { get; set; }
        [JsonPropertyName("spell_id")]
        public required SpellId SpellId { get; set; }
        [JsonPropertyName("use_force_goto")]
        public required bool UseForceGoto { get; set; }
        [JsonPropertyName("spell_fired")]
        public required bool SpellFired { get; set; }
        [JsonPropertyName("spell_per_source_entity")]
        public required bool SpellPerSourceEntity { get; set; }
        [JsonPropertyName("was_in_attack")]
        public required bool WasInAttack { get; set; }
    }

    public sealed class JobPushBack : Job
    {
        [JsonPropertyName("start_coord")]
        public required Position2D StartCoord { get; set; }
        [JsonPropertyName("target_coord")]
        public required Position2D TargetCoord { get; set; }
        [JsonPropertyName("speed")]
        public required float Speed { get; set; }
        [JsonPropertyName("rotation_speed")]
        public required float RotationSpeed { get; set; }
        [JsonPropertyName("damage")]
        public required float Damage { get; set; }
        [JsonPropertyName("source")]
        public EntityId? Source { get; set; }
    }

    public sealed class JobStampede : Job
    {
        [JsonPropertyName("spell")]
        public required SpellId Spell { get; set; }
        [JsonPropertyName("target")]
        public required TargetHolder Target { get; set; }
        [JsonPropertyName("start_coord")]
        public required Position2D StartCoord { get; set; }
    }

    public sealed class JobBarrierCrush : Job
    {
    }

    public sealed class JobBarrierGateToggle : Job
    {
    }

    public sealed class JobFlameThrower : Job
    {
        [JsonPropertyName("target")]
        public required TargetHolder Target { get; set; }
        [JsonPropertyName("spell_id")]
        public required SpellId SpellId { get; set; }
        [JsonPropertyName("duration_step_init")]
        public required TickCount DurationStepInit { get; set; }
        [JsonPropertyName("duration_step_shut_down")]
        public required TickCount DurationStepShutDown { get; set; }
    }

    public sealed class JobConstruct : Job
    {
        [JsonPropertyName("construction_update_steps")]
        public required TickCount ConstructionUpdateSteps { get; set; }
        [JsonPropertyName("construction_update_count_remaining")]
        public required TickCount ConstructionUpdateCountRemaining { get; set; }
    }

    public sealed class JobCrush : Job
    {
        [JsonPropertyName("crush_steps")]
        public required TickCount CrushSteps { get; set; }
        [JsonPropertyName("entity_update_steps")]
        public required TickCount EntityUpdateSteps { get; set; }
        [JsonPropertyName("remaining_crush_steps")]
        public required TickCount RemainingCrushSteps { get; set; }
    }

    public sealed class JobMountBarrierSquad : Job
    {
        [JsonPropertyName("barrier_module")]
        public required EntityId BarrierModule { get; set; }
    }

    public sealed class JobMountBarrier : Job
    {
        [JsonPropertyName("current_barrier_module")]
        public EntityId? CurrentBarrierModule { get; set; }
        [JsonPropertyName("goal_barrier_module")]
        public EntityId? GoalBarrierModule { get; set; }
    }

    public sealed class JobModeChangeSquad : Job
    {
        [JsonPropertyName("new_mode")]
        public required ModeId NewMode { get; set; }
        [JsonPropertyName("mode_change_done")]
        public required bool ModeChangeDone { get; set; }
    }

    public sealed class JobModeChange : Job
    {
        [JsonPropertyName("new_mode")]
        public required ModeId NewMode { get; set; }
    }

    public sealed class JobSacrificeSquad : Job
    {
        [JsonPropertyName("target_entity")]
        public required EntityId TargetEntity { get; set; }
    }

    public sealed class JobUsePortalSquad : Job
    {
        [JsonPropertyName("target_entity_id")]
        public required EntityId TargetEntityId { get; set; }
    }

    public sealed class JobChannel : Job
    {
        [JsonPropertyName("target_squad_id")]
        public EntityId? TargetSquadId { get; set; }
        [JsonPropertyName("mode_target_world")]
        public required bool ModeTargetWorld { get; set; }
        [JsonPropertyName("entity_id")]
        public EntityId? EntityId { get; set; }
        [JsonPropertyName("spell_id")]
        public required SpellId SpellId { get; set; }
        [JsonPropertyName("spell_id_on_target_on_finish")]
        public SpellId? SpellIdOnTargetOnFinish { get; set; }
        [JsonPropertyName("spell_id_on_target_on_start")]
        public SpellId? SpellIdOnTargetOnStart { get; set; }
        [JsonPropertyName("step_duration_until_finish")]
        public required TickCount StepDurationUntilFinish { get; set; }
        [JsonPropertyName("timing_channel_start")]
        public required UInt32 TimingChannelStart { get; set; }
        [JsonPropertyName("timing_channel_loop")]
        public required UInt32 TimingChannelLoop { get; set; }
        [JsonPropertyName("timing_channel_end")]
        public required UInt32 TimingChannelEnd { get; set; }
        [JsonPropertyName("abort_on_out_of_range_squared")]
        public required float AbortOnOutOfRangeSquared { get; set; }
        [JsonPropertyName("abort_check_failed")]
        public required bool AbortCheckFailed { get; set; }
        [JsonPropertyName("orientate_to_target")]
        public required bool OrientateToTarget { get; set; }
        [JsonPropertyName("orientate_to_target_max_step")]
        public required TickCount OrientateToTargetMaxStep { get; set; }
        [JsonPropertyName("abort_on_owner_get_damaged")]
        public required bool AbortOnOwnerGetDamaged { get; set; }
        [JsonPropertyName("abort_on_mode_change")]
        public required bool AbortOnModeChange { get; set; }
    }

    public sealed class JobSpawnSquad : Job
    {
    }

    public sealed class JobLootTargetSquad : Job
    {
        [JsonPropertyName("target_entity_id")]
        public required EntityId TargetEntityId { get; set; }
    }

    public sealed class JobMorph : Job
    {
        [JsonPropertyName("target")]
        public required TargetHolder Target { get; set; }
        [JsonPropertyName("spell")]
        public required SpellId Spell { get; set; }
    }

    /// <summary>
    ///  if you see this it means we did not account for some EA's case, so please report it
    /// </summary>
    public sealed class JobUnknown : Job
    {
        [JsonPropertyName("id")]
        public required UInt32 Id { get; set; }
    }


    public enum Ping
    {
        Attention = 0,
        Attack = 1,
        Defend = 2,
        NeedHelp = 4,
        Meet = 5,
    }

    /// <summary>
    ///  Entity on the map
    /// </summary>
    public class Entity
    {
        /// <summary>
        ///  Unique id of the entity
        /// </summary>
        [JsonPropertyName("id")]
        public required EntityId Id { get; set; }
        /// <summary>
        ///  List of effects the entity have.
        /// </summary>
        [JsonPropertyName("effects")]
        public required AbilityEffect[] Effects { get; set; }
        /// <summary>
        ///  List of aspects entity have.
        /// </summary>
        [JsonPropertyName("aspects")]
        public required AspectHolder[] Aspects { get; set; }
        /// <summary>
        ///  What is the entity doing right now
        /// </summary>
        [JsonPropertyName("job")]
        public required JobHolder Job { get; set; }
        /// <summary>
        ///  position on the map
        /// </summary>
        [JsonPropertyName("position")]
        public required Position Position { get; set; }
        /// <summary>
        ///  id of player that owns this entity
        /// </summary>
        [JsonPropertyName("player_entity_id")]
        public EntityId? PlayerEntityId { get; set; }
    }

    public class Projectile
    {
        /// <summary>
        ///  Unique id of the entity
        /// </summary>
        [JsonPropertyName("id")]
        public required EntityId Id { get; set; }
        /// <summary>
        ///  position on the map
        /// </summary>
        [JsonPropertyName("position")]
        public required Position Position { get; set; }
    }

    public class PowerSlot
    {
        [JsonPropertyName("entity")]
        public required Entity Entity { get; set; }
        [JsonPropertyName("res_id")]
        public required UInt32 ResId { get; set; }
        [JsonPropertyName("state")]
        public required UInt32 State { get; set; }
        [JsonPropertyName("team")]
        public required byte Team { get; set; }
    }

    public class TokenSlot
    {
        [JsonPropertyName("entity")]
        public required Entity Entity { get; set; }
        [JsonPropertyName("color")]
        public required OrbColor Color { get; set; }
    }

    public class AbilityWorldObject
    {
        [JsonPropertyName("entity")]
        public required Entity Entity { get; set; }
    }

    public class Squad
    {
        [JsonPropertyName("entity")]
        public required Entity Entity { get; set; }
        [JsonPropertyName("card_id")]
        public required CardId CardId { get; set; }
        [JsonPropertyName("res_squad_id")]
        public required SquadId ResSquadId { get; set; }
        [JsonPropertyName("bound_power")]
        public required float BoundPower { get; set; }
        [JsonPropertyName("squad_size")]
        public required byte SquadSize { get; set; }
        /// <summary>
        ///  IDs of the figures in the squad
        /// </summary>
        [JsonPropertyName("figures")]
        public required EntityId[] Figures { get; set; }
    }

    public class Figure
    {
        [JsonPropertyName("entity")]
        public required Entity Entity { get; set; }
        [JsonPropertyName("squad_id")]
        public required EntityId SquadId { get; set; }
        [JsonPropertyName("current_speed")]
        public required float CurrentSpeed { get; set; }
        [JsonPropertyName("rotation_speed")]
        public required float RotationSpeed { get; set; }
        [JsonPropertyName("unit_size")]
        public required byte UnitSize { get; set; }
        [JsonPropertyName("move_mode")]
        public required byte MoveMode { get; set; }
    }

    public class Building
    {
        [JsonPropertyName("entity")]
        public required Entity Entity { get; set; }
        [JsonPropertyName("building_id")]
        public required BuildingId BuildingId { get; set; }
        [JsonPropertyName("card_id")]
        public required CardId CardId { get; set; }
        [JsonPropertyName("power_cost")]
        public required float PowerCost { get; set; }
    }

    public class BarrierSet
    {
        [JsonPropertyName("entity")]
        public required Entity Entity { get; set; }
    }

    public class BarrierModule
    {
        [JsonPropertyName("entity")]
        public required Entity Entity { get; set; }
        [JsonPropertyName("team")]
        public required byte Team { get; set; }
        [JsonPropertyName("set")]
        public required EntityId Set { get; set; }
        [JsonPropertyName("state")]
        public required UInt32 State { get; set; }
        [JsonPropertyName("slots")]
        public required byte Slots { get; set; }
        [JsonPropertyName("free_slots")]
        public required byte FreeSlots { get; set; }
        [JsonPropertyName("walkable")]
        public required bool Walkable { get; set; }
    }

    /// <summary>
    ///  All the different command bot can issue.
    ///  For spectating bots all commands except Ping and WhisperToMaster are ignored
    /// </summary>
    public class CommandHolder
    {
        /// <summary>
        ///  Play card of building type.
        /// </summary>
        [JsonPropertyName("BuildHouse")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandBuildHouse? BuildHouse { get; set; }
        /// <summary>
        ///  Play card of Spell type. (single target)
        /// </summary>
        [JsonPropertyName("CastSpellGod")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandCastSpellGod? CastSpellGod { get; set; }
        /// <summary>
        ///  Play card of Spell type. (line target)
        /// </summary>
        [JsonPropertyName("CastSpellGodMulti")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandCastSpellGodMulti? CastSpellGodMulti { get; set; }
        /// <summary>
        ///  Play card of squad type (on ground)
        /// </summary>
        [JsonPropertyName("ProduceSquad")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandProduceSquad? ProduceSquad { get; set; }
        /// <summary>
        ///  Play card of squad type (on barrier)
        /// </summary>
        [JsonPropertyName("ProduceSquadOnBarrier")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandProduceSquadOnBarrier? ProduceSquadOnBarrier { get; set; }
        /// <summary>
        ///  Activates spell or ability on entity.
        /// </summary>
        [JsonPropertyName("CastSpellEntity")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandCastSpellEntity? CastSpellEntity { get; set; }
        /// <summary>
        ///  Opens or closes gate.
        /// </summary>
        [JsonPropertyName("BarrierGateToggle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandBarrierGateToggle? BarrierGateToggle { get; set; }
        /// <summary>
        ///  Build barrier. (same as BarrierRepair if not inverted)
        /// </summary>
        [JsonPropertyName("BarrierBuild")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandBarrierBuild? BarrierBuild { get; set; }
        /// <summary>
        ///  Repair barrier.
        /// </summary>
        [JsonPropertyName("BarrierRepair")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandBarrierRepair? BarrierRepair { get; set; }
        [JsonPropertyName("BarrierCancelRepair")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandBarrierCancelRepair? BarrierCancelRepair { get; set; }
        [JsonPropertyName("RepairBuilding")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRepairBuilding? RepairBuilding { get; set; }
        [JsonPropertyName("CancelRepairBuilding")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandCancelRepairBuilding? CancelRepairBuilding { get; set; }
        [JsonPropertyName("GroupAttack")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandGroupAttack? GroupAttack { get; set; }
        [JsonPropertyName("GroupEnterWall")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandGroupEnterWall? GroupEnterWall { get; set; }
        [JsonPropertyName("GroupExitWall")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandGroupExitWall? GroupExitWall { get; set; }
        [JsonPropertyName("GroupGoto")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandGroupGoto? GroupGoto { get; set; }
        [JsonPropertyName("GroupHoldPosition")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandGroupHoldPosition? GroupHoldPosition { get; set; }
        [JsonPropertyName("GroupStopJob")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandGroupStopJob? GroupStopJob { get; set; }
        [JsonPropertyName("ModeChange")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandModeChange? ModeChange { get; set; }
        [JsonPropertyName("PowerSlotBuild")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandPowerSlotBuild? PowerSlotBuild { get; set; }
        [JsonPropertyName("TokenSlotBuild")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandTokenSlotBuild? TokenSlotBuild { get; set; }
        [JsonPropertyName("Ping")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandPing? Ping { get; set; }
        [JsonPropertyName("Surrender")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandSurrender? Surrender { get; set; }
        [JsonPropertyName("WhisperToMaster")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandWhisperToMaster? WhisperToMaster { get; set; }

        public Command Get()
        {
            if (BuildHouse != null) { return BuildHouse; }
            if (CastSpellGod != null) { return CastSpellGod; }
            if (CastSpellGodMulti != null) { return CastSpellGodMulti; }
            if (ProduceSquad != null) { return ProduceSquad; }
            if (ProduceSquadOnBarrier != null) { return ProduceSquadOnBarrier; }
            if (CastSpellEntity != null) { return CastSpellEntity; }
            if (BarrierGateToggle != null) { return BarrierGateToggle; }
            if (BarrierBuild != null) { return BarrierBuild; }
            if (BarrierRepair != null) { return BarrierRepair; }
            if (BarrierCancelRepair != null) { return BarrierCancelRepair; }
            if (RepairBuilding != null) { return RepairBuilding; }
            if (CancelRepairBuilding != null) { return CancelRepairBuilding; }
            if (GroupAttack != null) { return GroupAttack; }
            if (GroupEnterWall != null) { return GroupEnterWall; }
            if (GroupExitWall != null) { return GroupExitWall; }
            if (GroupGoto != null) { return GroupGoto; }
            if (GroupHoldPosition != null) { return GroupHoldPosition; }
            if (GroupStopJob != null) { return GroupStopJob; }
            if (ModeChange != null) { return ModeChange; }
            if (PowerSlotBuild != null) { return PowerSlotBuild; }
            if (TokenSlotBuild != null) { return TokenSlotBuild; }
            if (Ping != null) { return Ping; }
            if (Surrender != null) { return Surrender; }
            if (WhisperToMaster != null) { return WhisperToMaster; }
            else { throw new InvalidOperationException("Impossible, because all cases was handled above"); }
        }
        public CommandHolder() { }
        public CommandHolder(CommandBuildHouse v) { BuildHouse = v; }
        public CommandHolder(CommandCastSpellGod v) { CastSpellGod = v; }
        public CommandHolder(CommandCastSpellGodMulti v) { CastSpellGodMulti = v; }
        public CommandHolder(CommandProduceSquad v) { ProduceSquad = v; }
        public CommandHolder(CommandProduceSquadOnBarrier v) { ProduceSquadOnBarrier = v; }
        public CommandHolder(CommandCastSpellEntity v) { CastSpellEntity = v; }
        public CommandHolder(CommandBarrierGateToggle v) { BarrierGateToggle = v; }
        public CommandHolder(CommandBarrierBuild v) { BarrierBuild = v; }
        public CommandHolder(CommandBarrierRepair v) { BarrierRepair = v; }
        public CommandHolder(CommandBarrierCancelRepair v) { BarrierCancelRepair = v; }
        public CommandHolder(CommandRepairBuilding v) { RepairBuilding = v; }
        public CommandHolder(CommandCancelRepairBuilding v) { CancelRepairBuilding = v; }
        public CommandHolder(CommandGroupAttack v) { GroupAttack = v; }
        public CommandHolder(CommandGroupEnterWall v) { GroupEnterWall = v; }
        public CommandHolder(CommandGroupExitWall v) { GroupExitWall = v; }
        public CommandHolder(CommandGroupGoto v) { GroupGoto = v; }
        public CommandHolder(CommandGroupHoldPosition v) { GroupHoldPosition = v; }
        public CommandHolder(CommandGroupStopJob v) { GroupStopJob = v; }
        public CommandHolder(CommandModeChange v) { ModeChange = v; }
        public CommandHolder(CommandPowerSlotBuild v) { PowerSlotBuild = v; }
        public CommandHolder(CommandTokenSlotBuild v) { TokenSlotBuild = v; }
        public CommandHolder(CommandPing v) { Ping = v; }
        public CommandHolder(CommandSurrender v) { Surrender = v; }
        public CommandHolder(CommandWhisperToMaster v) { WhisperToMaster = v; }
        public CommandHolder(Command v)
        {
            switch (v)
            {
                case CommandBuildHouse s:
                    BuildHouse = s; break;
                case CommandCastSpellGod s:
                    CastSpellGod = s; break;
                case CommandCastSpellGodMulti s:
                    CastSpellGodMulti = s; break;
                case CommandProduceSquad s:
                    ProduceSquad = s; break;
                case CommandProduceSquadOnBarrier s:
                    ProduceSquadOnBarrier = s; break;
                case CommandCastSpellEntity s:
                    CastSpellEntity = s; break;
                case CommandBarrierGateToggle s:
                    BarrierGateToggle = s; break;
                case CommandBarrierBuild s:
                    BarrierBuild = s; break;
                case CommandBarrierRepair s:
                    BarrierRepair = s; break;
                case CommandBarrierCancelRepair s:
                    BarrierCancelRepair = s; break;
                case CommandRepairBuilding s:
                    RepairBuilding = s; break;
                case CommandCancelRepairBuilding s:
                    CancelRepairBuilding = s; break;
                case CommandGroupAttack s:
                    GroupAttack = s; break;
                case CommandGroupEnterWall s:
                    GroupEnterWall = s; break;
                case CommandGroupExitWall s:
                    GroupExitWall = s; break;
                case CommandGroupGoto s:
                    GroupGoto = s; break;
                case CommandGroupHoldPosition s:
                    GroupHoldPosition = s; break;
                case CommandGroupStopJob s:
                    GroupStopJob = s; break;
                case CommandModeChange s:
                    ModeChange = s; break;
                case CommandPowerSlotBuild s:
                    PowerSlotBuild = s; break;
                case CommandTokenSlotBuild s:
                    TokenSlotBuild = s; break;
                case CommandPing s:
                    Ping = s; break;
                case CommandSurrender s:
                    Surrender = s; break;
                case CommandWhisperToMaster s:
                    WhisperToMaster = s; break;
            }
        }
    }
    [JsonDerivedType(typeof(CommandBuildHouse))]
    [JsonDerivedType(typeof(CommandCastSpellGod))]
    [JsonDerivedType(typeof(CommandCastSpellGodMulti))]
    [JsonDerivedType(typeof(CommandProduceSquad))]
    [JsonDerivedType(typeof(CommandProduceSquadOnBarrier))]
    [JsonDerivedType(typeof(CommandCastSpellEntity))]
    [JsonDerivedType(typeof(CommandBarrierGateToggle))]
    [JsonDerivedType(typeof(CommandBarrierBuild))]
    [JsonDerivedType(typeof(CommandBarrierRepair))]
    [JsonDerivedType(typeof(CommandBarrierCancelRepair))]
    [JsonDerivedType(typeof(CommandRepairBuilding))]
    [JsonDerivedType(typeof(CommandCancelRepairBuilding))]
    [JsonDerivedType(typeof(CommandGroupAttack))]
    [JsonDerivedType(typeof(CommandGroupEnterWall))]
    [JsonDerivedType(typeof(CommandGroupExitWall))]
    [JsonDerivedType(typeof(CommandGroupGoto))]
    [JsonDerivedType(typeof(CommandGroupHoldPosition))]
    [JsonDerivedType(typeof(CommandGroupStopJob))]
    [JsonDerivedType(typeof(CommandModeChange))]
    [JsonDerivedType(typeof(CommandPowerSlotBuild))]
    [JsonDerivedType(typeof(CommandTokenSlotBuild))]
    [JsonDerivedType(typeof(CommandPing))]
    [JsonDerivedType(typeof(CommandSurrender))]
    [JsonDerivedType(typeof(CommandWhisperToMaster))]
    public /*abstract*/ class Command { }

    /// <summary>
    ///  Play card of building type.
    /// </summary>
    public sealed class CommandBuildHouse : Command
    {
        /// <summary>
        ///  TODO will be 0 when received as command by another player
        /// </summary>
        [JsonPropertyName("card_position")]
        public required byte CardPosition { get; set; }
        [JsonPropertyName("xy")]
        public required Position2D Xy { get; set; }
        [JsonPropertyName("angle")]
        public required float Angle { get; set; }
    }

    /// <summary>
    ///  Play card of Spell type. (single target)
    /// </summary>
    public sealed class CommandCastSpellGod : Command
    {
        [JsonPropertyName("card_position")]
        public required byte CardPosition { get; set; }
        [JsonPropertyName("target")]
        public required SingleTargetHolder Target { get; set; }
    }

    /// <summary>
    ///  Play card of Spell type. (line target)
    /// </summary>
    public sealed class CommandCastSpellGodMulti : Command
    {
        [JsonPropertyName("card_position")]
        public required byte CardPosition { get; set; }
        [JsonPropertyName("xy1")]
        public required Position2D Xy1 { get; set; }
        [JsonPropertyName("xy2")]
        public required Position2D Xy2 { get; set; }
    }

    /// <summary>
    ///  Play card of squad type (on ground)
    /// </summary>
    public sealed class CommandProduceSquad : Command
    {
        [JsonPropertyName("card_position")]
        public required byte CardPosition { get; set; }
        [JsonPropertyName("xy")]
        public required Position2D Xy { get; set; }
    }

    /// <summary>
    ///  Play card of squad type (on barrier)
    /// </summary>
    public sealed class CommandProduceSquadOnBarrier : Command
    {
        [JsonPropertyName("card_position")]
        public required byte CardPosition { get; set; }
        /// <summary>
        ///  Squad will spawn based on this position and go to the barrier.
        /// </summary>
        [JsonPropertyName("xy")]
        public required Position2D Xy { get; set; }
        /// <summary>
        ///  Squad will go to this barrier, after spawning.
        /// </summary>
        [JsonPropertyName("barrier_to_mount")]
        public required EntityId BarrierToMount { get; set; }
    }

    /// <summary>
    ///  Activates spell or ability on entity.
    /// </summary>
    public sealed class CommandCastSpellEntity : Command
    {
        [JsonPropertyName("entity")]
        public required EntityId Entity { get; set; }
        [JsonPropertyName("spell")]
        public required SpellId Spell { get; set; }
        [JsonPropertyName("target")]
        public required SingleTargetHolder Target { get; set; }
    }

    /// <summary>
    ///  Opens or closes gate.
    /// </summary>
    public sealed class CommandBarrierGateToggle : Command
    {
        [JsonPropertyName("barrier_id")]
        public required EntityId BarrierId { get; set; }
    }

    /// <summary>
    ///  Build barrier. (same as BarrierRepair if not inverted)
    /// </summary>
    public sealed class CommandBarrierBuild : Command
    {
        [JsonPropertyName("barrier_id")]
        public required EntityId BarrierId { get; set; }
        [JsonPropertyName("inverted_direction")]
        public required bool InvertedDirection { get; set; }
    }

    /// <summary>
    ///  Repair barrier.
    /// </summary>
    public sealed class CommandBarrierRepair : Command
    {
        [JsonPropertyName("barrier_id")]
        public required EntityId BarrierId { get; set; }
    }

    public sealed class CommandBarrierCancelRepair : Command
    {
        [JsonPropertyName("barrier_id")]
        public required EntityId BarrierId { get; set; }
    }

    public sealed class CommandRepairBuilding : Command
    {
        [JsonPropertyName("building_id")]
        public required EntityId BuildingId { get; set; }
    }

    public sealed class CommandCancelRepairBuilding : Command
    {
        [JsonPropertyName("building_id")]
        public required EntityId BuildingId { get; set; }
    }

    public sealed class CommandGroupAttack : Command
    {
        [JsonPropertyName("squads")]
        public required EntityId[] Squads { get; set; }
        [JsonPropertyName("target_entity_id")]
        public required EntityId TargetEntityId { get; set; }
        [JsonPropertyName("force_attack")]
        public required bool ForceAttack { get; set; }
    }

    public sealed class CommandGroupEnterWall : Command
    {
        [JsonPropertyName("squads")]
        public required EntityId[] Squads { get; set; }
        [JsonPropertyName("barrier_id")]
        public required EntityId BarrierId { get; set; }
    }

    public sealed class CommandGroupExitWall : Command
    {
        [JsonPropertyName("squads")]
        public required EntityId[] Squads { get; set; }
        [JsonPropertyName("barrier_id")]
        public required EntityId BarrierId { get; set; }
    }

    public sealed class CommandGroupGoto : Command
    {
        [JsonPropertyName("squads")]
        public required EntityId[] Squads { get; set; }
        [JsonPropertyName("positions")]
        public required Position2D[] Positions { get; set; }
        [JsonPropertyName("walk_mode")]
        public required WalkMode WalkMode { get; set; }
        [JsonPropertyName("orientation")]
        public required float Orientation { get; set; }
    }

    public sealed class CommandGroupHoldPosition : Command
    {
        [JsonPropertyName("squads")]
        public required EntityId[] Squads { get; set; }
    }

    public sealed class CommandGroupStopJob : Command
    {
        [JsonPropertyName("squads")]
        public required EntityId[] Squads { get; set; }
    }

    public sealed class CommandModeChange : Command
    {
        [JsonPropertyName("entity_id")]
        public required EntityId EntityId { get; set; }
        [JsonPropertyName("new_mode_id")]
        public required ModeId NewModeId { get; set; }
    }

    public sealed class CommandPowerSlotBuild : Command
    {
        [JsonPropertyName("slot_id")]
        public required EntityId SlotId { get; set; }
    }

    public sealed class CommandTokenSlotBuild : Command
    {
        [JsonPropertyName("slot_id")]
        public required EntityId SlotId { get; set; }
        [JsonPropertyName("color")]
        public required CreateOrbColor Color { get; set; }
    }

    public sealed class CommandPing : Command
    {
        [JsonPropertyName("xy")]
        public required Position2D Xy { get; set; }
        [JsonPropertyName("ping")]
        public required Ping Ping { get; set; }
    }

    public sealed class CommandSurrender : Command
    {
    }

    public sealed class CommandWhisperToMaster : Command
    {
        [JsonPropertyName("text")]
        public required string Text { get; set; }
    }


    /// <summary>
    ///  Command that happen.
    /// </summary>
    public class PlayerCommand
    {
        [JsonPropertyName("player")]
        public required EntityId Player { get; set; }
        [JsonPropertyName("command")]
        public required CommandHolder Command { get; set; }
    }

    public enum WhyCanNotPlayCardThere
    {
        DoesNotHaveEnoughPower = 0x10,
        /// <summary>
        ///  too close to (0,y), or (x,0)
        /// </summary>
        InvalidPosition = 0x20,
        CardCondition = 0x80,
        ConditionPreventCardPlay = 0x100,
        DoesNotHaveThatCard = 0x200,
        DoesNotHaveEnoughOrbs = 0x400,
        CastingTooOften = 0x10000,
    }

    /// <summary>
    ///  Reason why command was rejected
    /// </summary>
    public class CommandRejectionReasonHolder
    {
        /// <summary>
        ///  Rejection reason for `BuildHouse`, `ProduceSquad`, and `ProduceSquadOnBarrier`
        /// </summary>
        [JsonPropertyName("CardRejected")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRejectionReasonCardRejected? CardRejected { get; set; }
        /// <summary>
        ///  Player did not have enough power to play the card or activate the ability
        /// </summary>
        [JsonPropertyName("NotEnoughPower")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRejectionReasonNotEnoughPower? NotEnoughPower { get; set; }
        /// <summary>
        ///  Spell with given ID does not exist
        /// </summary>
        [JsonPropertyName("SpellDoesNotExist")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRejectionReasonSpellDoesNotExist? SpellDoesNotExist { get; set; }
        /// <summary>
        ///  The entity is not on the map
        /// </summary>
        [JsonPropertyName("EntityDoesNotExist")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRejectionReasonEntityDoesNotExist? EntityDoesNotExist { get; set; }
        /// <summary>
        ///  Entity exist, but type is not correct
        /// </summary>
        [JsonPropertyName("InvalidEntityType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRejectionReasonInvalidEntityType? InvalidEntityType { get; set; }
        /// <summary>
        ///  Rejection reason for `CastSpellEntity`
        /// </summary>
        [JsonPropertyName("CanNotCast")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRejectionReasonCanNotCast? CanNotCast { get; set; }
        /// <summary>
        ///  Bot issued command for entity that is not owned by anyone
        /// </summary>
        [JsonPropertyName("EntityNotOwned")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRejectionReasonEntityNotOwned? EntityNotOwned { get; set; }
        /// <summary>
        ///  Bot issued command for entity owned by someone else
        /// </summary>
        [JsonPropertyName("EntityOwnedBySomeoneElse")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRejectionReasonEntityOwnedBySomeoneElse? EntityOwnedBySomeoneElse { get; set; }
        /// <summary>
        ///  Bot issued command for entity to change mode, but the entity does not have `ModeChange` aspect.
        /// </summary>
        [JsonPropertyName("NoModeChange")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRejectionReasonNoModeChange? NoModeChange { get; set; }
        /// <summary>
        ///  Trying to change to mode, in which the entity already is.
        /// </summary>
        [JsonPropertyName("EntityAlreadyInThisMode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRejectionReasonEntityAlreadyInThisMode? EntityAlreadyInThisMode { get; set; }
        /// <summary>
        ///  Trying to change to moe, that the entity does not have.
        /// </summary>
        [JsonPropertyName("ModeNotExist")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRejectionReasonModeNotExist? ModeNotExist { get; set; }
        /// <summary>
        ///  Card index must be 0-19
        /// </summary>
        [JsonPropertyName("InvalidCardIndex")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRejectionReasonInvalidCardIndex? InvalidCardIndex { get; set; }
        /// <summary>
        ///  Card on the given index is invalid
        /// </summary>
        [JsonPropertyName("InvalidCard")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CommandRejectionReasonInvalidCard? InvalidCard { get; set; }

        public CommandRejectionReason Get()
        {
            if (CardRejected != null) { return CardRejected; }
            if (NotEnoughPower != null) { return NotEnoughPower; }
            if (SpellDoesNotExist != null) { return SpellDoesNotExist; }
            if (EntityDoesNotExist != null) { return EntityDoesNotExist; }
            if (InvalidEntityType != null) { return InvalidEntityType; }
            if (CanNotCast != null) { return CanNotCast; }
            if (EntityNotOwned != null) { return EntityNotOwned; }
            if (EntityOwnedBySomeoneElse != null) { return EntityOwnedBySomeoneElse; }
            if (NoModeChange != null) { return NoModeChange; }
            if (EntityAlreadyInThisMode != null) { return EntityAlreadyInThisMode; }
            if (ModeNotExist != null) { return ModeNotExist; }
            if (InvalidCardIndex != null) { return InvalidCardIndex; }
            if (InvalidCard != null) { return InvalidCard; }
            else { throw new InvalidOperationException("Impossible, because all cases was handled above"); }
        }
        public CommandRejectionReasonHolder() { }
        public CommandRejectionReasonHolder(CommandRejectionReasonCardRejected v) { CardRejected = v; }
        public CommandRejectionReasonHolder(CommandRejectionReasonNotEnoughPower v) { NotEnoughPower = v; }
        public CommandRejectionReasonHolder(CommandRejectionReasonSpellDoesNotExist v) { SpellDoesNotExist = v; }
        public CommandRejectionReasonHolder(CommandRejectionReasonEntityDoesNotExist v) { EntityDoesNotExist = v; }
        public CommandRejectionReasonHolder(CommandRejectionReasonInvalidEntityType v) { InvalidEntityType = v; }
        public CommandRejectionReasonHolder(CommandRejectionReasonCanNotCast v) { CanNotCast = v; }
        public CommandRejectionReasonHolder(CommandRejectionReasonEntityNotOwned v) { EntityNotOwned = v; }
        public CommandRejectionReasonHolder(CommandRejectionReasonEntityOwnedBySomeoneElse v) { EntityOwnedBySomeoneElse = v; }
        public CommandRejectionReasonHolder(CommandRejectionReasonNoModeChange v) { NoModeChange = v; }
        public CommandRejectionReasonHolder(CommandRejectionReasonEntityAlreadyInThisMode v) { EntityAlreadyInThisMode = v; }
        public CommandRejectionReasonHolder(CommandRejectionReasonModeNotExist v) { ModeNotExist = v; }
        public CommandRejectionReasonHolder(CommandRejectionReasonInvalidCardIndex v) { InvalidCardIndex = v; }
        public CommandRejectionReasonHolder(CommandRejectionReasonInvalidCard v) { InvalidCard = v; }
        public CommandRejectionReasonHolder(CommandRejectionReason v)
        {
            switch (v)
            {
                case CommandRejectionReasonCardRejected s:
                    CardRejected = s; break;
                case CommandRejectionReasonNotEnoughPower s:
                    NotEnoughPower = s; break;
                case CommandRejectionReasonSpellDoesNotExist s:
                    SpellDoesNotExist = s; break;
                case CommandRejectionReasonEntityDoesNotExist s:
                    EntityDoesNotExist = s; break;
                case CommandRejectionReasonInvalidEntityType s:
                    InvalidEntityType = s; break;
                case CommandRejectionReasonCanNotCast s:
                    CanNotCast = s; break;
                case CommandRejectionReasonEntityNotOwned s:
                    EntityNotOwned = s; break;
                case CommandRejectionReasonEntityOwnedBySomeoneElse s:
                    EntityOwnedBySomeoneElse = s; break;
                case CommandRejectionReasonNoModeChange s:
                    NoModeChange = s; break;
                case CommandRejectionReasonEntityAlreadyInThisMode s:
                    EntityAlreadyInThisMode = s; break;
                case CommandRejectionReasonModeNotExist s:
                    ModeNotExist = s; break;
                case CommandRejectionReasonInvalidCardIndex s:
                    InvalidCardIndex = s; break;
                case CommandRejectionReasonInvalidCard s:
                    InvalidCard = s; break;
            }
        }
    }
    [JsonDerivedType(typeof(CommandRejectionReasonCardRejected))]
    [JsonDerivedType(typeof(CommandRejectionReasonNotEnoughPower))]
    [JsonDerivedType(typeof(CommandRejectionReasonSpellDoesNotExist))]
    [JsonDerivedType(typeof(CommandRejectionReasonEntityDoesNotExist))]
    [JsonDerivedType(typeof(CommandRejectionReasonInvalidEntityType))]
    [JsonDerivedType(typeof(CommandRejectionReasonCanNotCast))]
    [JsonDerivedType(typeof(CommandRejectionReasonEntityNotOwned))]
    [JsonDerivedType(typeof(CommandRejectionReasonEntityOwnedBySomeoneElse))]
    [JsonDerivedType(typeof(CommandRejectionReasonNoModeChange))]
    [JsonDerivedType(typeof(CommandRejectionReasonEntityAlreadyInThisMode))]
    [JsonDerivedType(typeof(CommandRejectionReasonModeNotExist))]
    [JsonDerivedType(typeof(CommandRejectionReasonInvalidCardIndex))]
    [JsonDerivedType(typeof(CommandRejectionReasonInvalidCard))]
    public /*abstract*/ class CommandRejectionReason { }

    /// <summary>
    ///  Rejection reason for `BuildHouse`, `ProduceSquad`, and `ProduceSquadOnBarrier`
    /// </summary>
    public sealed class CommandRejectionReasonCardRejected : CommandRejectionReason
    {
        [JsonPropertyName("reason")]
        public required WhyCanNotPlayCardThere Reason { get; set; }
        [JsonPropertyName("failed_card_conditions")]
        public required UInt32[] FailedCardConditions { get; set; }
    }

    /// <summary>
    ///  Player did not have enough power to play the card or activate the ability
    /// </summary>
    public sealed class CommandRejectionReasonNotEnoughPower : CommandRejectionReason
    {
        [JsonPropertyName("player_power")]
        public required float PlayerPower { get; set; }
        [JsonPropertyName("required")]
        public required UInt16 Required { get; set; }
    }

    /// <summary>
    ///  Spell with given ID does not exist
    /// </summary>
    public sealed class CommandRejectionReasonSpellDoesNotExist : CommandRejectionReason
    {
    }

    /// <summary>
    ///  The entity is not on the map
    /// </summary>
    public sealed class CommandRejectionReasonEntityDoesNotExist : CommandRejectionReason
    {
    }

    /// <summary>
    ///  Entity exist, but type is not correct
    /// </summary>
    public sealed class CommandRejectionReasonInvalidEntityType : CommandRejectionReason
    {
        [JsonPropertyName("entity_type")]
        public required UInt32 EntityType { get; set; }
    }

    /// <summary>
    ///  Rejection reason for `CastSpellEntity`
    /// </summary>
    public sealed class CommandRejectionReasonCanNotCast : CommandRejectionReason
    {
        [JsonPropertyName("failed_spell_conditions")]
        public required UInt32[] FailedSpellConditions { get; set; }
    }

    /// <summary>
    ///  Bot issued command for entity that is not owned by anyone
    /// </summary>
    public sealed class CommandRejectionReasonEntityNotOwned : CommandRejectionReason
    {
    }

    /// <summary>
    ///  Bot issued command for entity owned by someone else
    /// </summary>
    public sealed class CommandRejectionReasonEntityOwnedBySomeoneElse : CommandRejectionReason
    {
    }

    /// <summary>
    ///  Bot issued command for entity to change mode, but the entity does not have `ModeChange` aspect.
    /// </summary>
    public sealed class CommandRejectionReasonNoModeChange : CommandRejectionReason
    {
    }

    /// <summary>
    ///  Trying to change to mode, in which the entity already is.
    /// </summary>
    public sealed class CommandRejectionReasonEntityAlreadyInThisMode : CommandRejectionReason
    {
    }

    /// <summary>
    ///  Trying to change to moe, that the entity does not have.
    /// </summary>
    public sealed class CommandRejectionReasonModeNotExist : CommandRejectionReason
    {
    }

    /// <summary>
    ///  Card index must be 0-19
    /// </summary>
    public sealed class CommandRejectionReasonInvalidCardIndex : CommandRejectionReason
    {
    }

    /// <summary>
    ///  Card on the given index is invalid
    /// </summary>
    public sealed class CommandRejectionReasonInvalidCard : CommandRejectionReason
    {
    }


    /// <summary>
    ///  Command that was rejected.
    /// </summary>
    public class RejectedCommand
    {
        [JsonPropertyName("player")]
        public required EntityId Player { get; set; }
        [JsonPropertyName("reason")]
        public required CommandRejectionReasonHolder Reason { get; set; }
        [JsonPropertyName("command")]
        public required CommandHolder Command { get; set; }
    }

    /// <summary>
    ///  Response on the `/hello` endpoint.
    /// </summary>
    public class AiForMap
    {
        /// <summary>
        ///  The unique name of the bot.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        /// <summary>
        ///  List of decks this bot can use on the map.
        ///  Empty to signalize, that bot can not play on given map.
        /// </summary>
        [JsonPropertyName("decks")]
        public required Deck[] Decks { get; set; }
    }

    public class MapEntities
    {
        [JsonPropertyName("projectiles")]
        public required Projectile[] Projectiles { get; set; }
        [JsonPropertyName("power_slots")]
        public required PowerSlot[] PowerSlots { get; set; } // Wells
        [JsonPropertyName("token_slots")]
        public required TokenSlot[] TokenSlots { get; set; } // Orbs
        [JsonPropertyName("ability_world_objects")]
        public required AbilityWorldObject[] AbilityWorldObjects { get; set; }
        [JsonPropertyName("squads")]
        public required Squad[] Squads { get; set; } // Groups of units
        [JsonPropertyName("figures")]
        public required Figure[] Figures { get; set; } // Units
        [JsonPropertyName("buildings")]
        public required Building[] Buildings { get; set; }
        [JsonPropertyName("barrier_sets")]
        public required BarrierSet[] BarrierSets { get; set; }
        [JsonPropertyName("barrier_modules")]
        public required BarrierModule[] BarrierModules { get; set; }
    }

    /// <summary>
    ///  Used in `/start` endpoint.
    /// </summary>
    public class GameStartState
    {
        /// <summary>
        ///  Tells the bot which player it is supposed to control.
        ///  If bot is only spectating, this is the ID of player that it is spectating for
        /// </summary>
        [JsonPropertyName("your_player_id")]
        public required EntityId YourPlayerId { get; set; }
        /// <summary>
        ///  Players in the match.
        /// </summary>
        [JsonPropertyName("players")]
        public required MatchPlayer[] Players { get; set; }
        [JsonPropertyName("entities")]
        public required MapEntities Entities { get; set; }
    }

    /// <summary>
    ///  Used in `/tick` endpoint, on every tick from 2 forward.
    /// </summary>
    public class GameState
    {
        /// <summary>
        ///  Time since start of the match measured in ticks.
        ///  One tick is 0.1 second = 100 milliseconds = (10 ticks per second)
        ///  Each tick is 100 ms. 1 second is 10 ticks. 1 minute is 600 ticks.
        /// </summary>
        [JsonPropertyName("current_tick")]
        public required Tick CurrentTick { get; set; }
        /// <summary>
        ///  Commands that will be executed this tick.
        /// </summary>
        [JsonPropertyName("commands")]
        public required PlayerCommand[] Commands { get; set; }
        /// <summary>
        ///  Commands that was rejected.
        /// </summary>
        [JsonPropertyName("rejected_commands")]
        public required RejectedCommand[] RejectedCommands { get; set; }
        /// <summary>
        ///  player entities in the match
        /// </summary>
        [JsonPropertyName("players")]
        public required PlayerEntity[] Players { get; set; }
        [JsonPropertyName("entities")]
        public required MapEntities Entities { get; set; }
    }

    /// <summary>
    ///  Used in `/prepare` endpoint
    /// </summary>
    public class Prepare
    {
        /// <summary>
        ///  Name of deck, selected from `AiForMap` returned by `/hello` endpoint.
        /// </summary>
        [JsonPropertyName("deck")]
        public required string Deck { get; set; }
        /// <summary>
        ///  Repeating `map_info` in case bot want to prepare differently based on map.
        /// </summary>
        [JsonPropertyName("map_info")]
        public required MapInfo MapInfo { get; set; }
    }

    /// <summary>
    ///  Used in `/hello` endpoint
    /// </summary>
    public class ApiHello
    {
        /// <summary>
        ///  Must match the version in this file, to guarantee structures matching.
        /// </summary>
        [JsonPropertyName("version")]
        public required UInt64 Version { get; set; }
        /// <summary>
        ///  Map about which is the game asking.
        /// </summary>
        [JsonPropertyName("map")]
        public required MapInfo Map { get; set; }
    }

}
