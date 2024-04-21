
module F__api_and_example.Types

type Maps = Api.Maps
type CardTemplate = Api.CardTemplate
open System.Text.Json.Serialization

let VERSION : uint64 = 19UL

type Upgrade =
    | U0 = 0
    | U1 = 1000000
    | U2 = 2000000
    | U3 = 3000000



/// <summary>
///  ID of the card resource
/// </summary>
type CardId = | CardId of uint32
type CardIdConverter() =
    inherit JsonConverter<CardId>()
    override _.Write(writer, (CardId value), _) =
        writer.WriteNumberValue(value)
    override _.Read(reader, _, _) =
        CardId(reader.GetUInt32())


/// <summary>
///  ID of squad resource
/// </summary>
type SquadId = | SquadId of uint32
type SquadIdConverter() =
    inherit JsonConverter<SquadId>()
    override _.Write(writer, (SquadId value), _) =
        writer.WriteNumberValue(value)
    override _.Read(reader, _, _) =
        SquadId(reader.GetUInt32())


/// <summary>
///  ID of building resource
/// </summary>
type BuildingId = | BuildingId of uint32
type BuildingIdConverter() =
    inherit JsonConverter<BuildingId>()
    override _.Write(writer, (BuildingId value), _) =
        writer.WriteNumberValue(value)
    override _.Read(reader, _, _) =
        BuildingId(reader.GetUInt32())


/// <summary>
///  ID of spell resource
/// </summary>
type SpellId = | SpellId of uint32
type SpellIdConverter() =
    inherit JsonConverter<SpellId>()
    override _.Write(writer, (SpellId value), _) =
        writer.WriteNumberValue(value)
    override _.Read(reader, _, _) =
        SpellId(reader.GetUInt32())


/// <summary>
///  ID of ability resource
/// </summary>
type AbilityId = | AbilityId of uint32
type AbilityIdConverter() =
    inherit JsonConverter<AbilityId>()
    override _.Write(writer, (AbilityId value), _) =
        writer.WriteNumberValue(value)
    override _.Read(reader, _, _) =
        AbilityId(reader.GetUInt32())


/// <summary>
///  ID of mode resource
/// </summary>
type ModeId = | ModeId of uint32
type ModeIdConverter() =
    inherit JsonConverter<ModeId>()
    override _.Write(writer, (ModeId value), _) =
        writer.WriteNumberValue(value)
    override _.Read(reader, _, _) =
        ModeId(reader.GetUInt32())


/// <summary>
///  ID of an entity present in the match unique to that match
///  First entity have ID 1, next 2, ...
///  Ids are never reused
/// </summary>
type EntityId = | EntityId of uint32
type EntityIdConverter() =
    inherit JsonConverter<EntityId>()
    override _.Write(writer, (EntityId value), _) =
        writer.WriteNumberValue(value)
    override _.Read(reader, _, _) =
        EntityId(reader.GetUInt32())


/// <summary>
///  Time information 1 tick = 0.1s = 100 ms
/// </summary>
type Tick = | Tick of uint32
type TickConverter() =
    inherit JsonConverter<Tick>()
    override _.Write(writer, (Tick value), _) =
        writer.WriteNumberValue(value)
    override _.Read(reader, _, _) =
        Tick(reader.GetUInt32())


/// <summary>
///  Difference between two `Tick` (points in times, remaining time, ...)
/// </summary>
type TickCount = | TickCount of uint32
type TickCountConverter() =
    inherit JsonConverter<TickCount>()
    override _.Write(writer, (TickCount value), _) =
        writer.WriteNumberValue(value)
    override _.Read(reader, _, _) =
        TickCount(reader.GetUInt32())


/// <summary>
///  `x` and `z` are coordinates on the 2D map.
/// </summary>
type Position = {
    [<JsonPropertyName("x")>]
        X : single
    /// <summary>
    ///  Also known as height.
    /// </summary>
    [<JsonPropertyName("y")>]
        Y : single
    [<JsonPropertyName("z")>]
        Z : single
}


type Position2D = {
    [<JsonPropertyName("x")>]
        X : single
    [<JsonPropertyName("y")>]
        Y : single
}


type Position2DWithOrientation = {
    [<JsonPropertyName("x")>]
        X : single
    [<JsonPropertyName("y")>]
        Y : single
    /// <summary>
    ///  in default camera orientation
    ///  0 = down, π/2 = right, π = up, π3/2 = left
    /// </summary>
    [<JsonPropertyName("orientation")>]
        Orientation : single
}


/// <summary>
///  Color of an orb.
/// </summary>
type OrbColor =
    | White = 0
    | Shadow = 1
    | Nature = 2
    | Frost = 3
    | Fire = 4
    | Starting = 5
    | All = 7



/// <summary>
///  Subset of `OrbColor`, because creating the other colors does not make sense.
/// </summary>
type CreateOrbColor =
    | Shadow = 1
    | Nature = 2
    | Frost = 3
    | Fire = 4



/// <summary>
///  When targeting you can target either entity, or ground coordinates.
/// </summary>
type SingleTarget =
    /// <summary>
    ///  Target entity
    /// </summary>
    | SingleEntity of {|
          Id : EntityId
        |}
    /// <summary>
    ///  Target location on the ground
    /// </summary>
    | Location of {|
          Xy : Position2D
        |}

type SingleTargetConverter() =
    inherit JsonConverter<SingleTarget>()
    override _.Write(writer, value, options) =
        match value with
        | SingleEntity f ->
            writer.WriteStartObject()
            writer.WriteStartObject("SingleEntity")
            writer.WritePropertyName("id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.Id, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Location f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Location")
            writer.WritePropertyName("xy")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.Xy, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
    override _.Read(reader, _, options) =
        if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
        then failwith "Expecting an object"
        reader.Read() |> ignore
        if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
        then failwith "Expecting an name"
        let str = reader.GetString()
        reader.Read() |> ignore
        match str with
        | "SingleEntity" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            SingleTarget.SingleEntity {| Id = f_id; |}
        | "Location" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("xy" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_xy = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            SingleTarget.Location {| Xy = f_xy; |}
        | _ -> failwith $"Unexpected type: %s{str}"

type Target =
    | Single of {|
          Single : SingleTarget
        |}
    | Multi of {|
          XyBegin : Position2D
          XyEnd : Position2D
        |}

type TargetConverter() =
    inherit JsonConverter<Target>()
    override _.Write(writer, value, options) =
        match value with
        | Single f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Single")
            writer.WritePropertyName("single")
            let c = options.GetConverter(typeof<SingleTarget>) :?> JsonConverter<SingleTarget>
            c.Write(writer, f.Single, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Multi f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Multi")
            writer.WritePropertyName("xy_begin")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.XyBegin, options)
            writer.WritePropertyName("xy_end")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.XyEnd, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
    override _.Read(reader, _, options) =
        if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
        then failwith "Expecting an object"
        reader.Read() |> ignore
        if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
        then failwith "Expecting an name"
        let str = reader.GetString()
        reader.Read() |> ignore
        match str with
        | "Single" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("single" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SingleTarget>) :?> JsonConverter<SingleTarget>
            let f_single = c.Read(&reader, typeof<SingleTarget>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Target.Single {| Single = f_single; |}
        | "Multi" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("xy_begin" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_xy_begin = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("xy_end" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_xy_end = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Target.Multi {| XyBegin = f_xy_begin;XyEnd = f_xy_end; |}
        | _ -> failwith $"Unexpected type: %s{str}"

type WalkMode =
    | PartialForce = 1
    | Force = 2
    /// <summary>
    ///  Also called by players "Attack move", or "Q move"
    /// </summary>
    | Normal = 4
    | Crusade = 5
    | Scout = 6
    | Patrol = 7



type CommunityMapInfo = {
    /// <summary>
    ///  Name of the map.
    /// </summary>
    [<JsonPropertyName("name")>]
        Name : string
    /// <summary>
    ///  Checksum of them map.
    /// </summary>
    [<JsonPropertyName("crc")>]
        Crc : uint64
}


/// <summary>
///  Official spectator maps are normal maps (have unique id) so only `map` field is needed.
/// </summary>
type MapInfo = {
    /// <summary>
    ///  Represents the map, unfortunately EA decided, it will be harder for community maps.
    /// </summary>
    [<JsonPropertyName("map")>]
        Map : Maps
    /// <summary>
    ///  Is relevant only for community maps.
    /// </summary>
    [<JsonPropertyName("community_map_details")>]
        CommunityMapDetails : CommunityMapInfo voption
}


type Deck = {
    /// <summary>
    ///  Name of the deck, must be unique across decks used by bot, but different bots can have same deck names.
    ///  Must not contain spaces, to be addable in game.
    /// </summary>
    [<JsonPropertyName("name")>]
        Name : string
    /// <summary>
    ///  Index of a card that will be deck icon 0 to 19 inclusive
    /// </summary>
    [<JsonPropertyName("cover_card_index")>]
        CoverCardIndex : uint8
    /// <summary>
    ///  List of 20 cards in deck.
    ///  Fill empty spaces with `NotACard`.
    /// </summary>
    [<JsonPropertyName("cards")>]
        Cards : CardId[(*size=20*)]
}


type AbilityLine =
    | _EAsBug_betterSafeThanSorry = 0
    | ModifyWalkSpeed = 1
    | UnControllable = 4
    | UnKillable = 5
    | UnAttackable = 9
    | HitMultiple = 10
    | _ACModifier = 14
    | DamageOverTime = 15
    | DamageBuff = 21
    | PowerOutputModifier = 23
    | MoveSpeedOverwrite = 24
    | HitMultipleRanged = 25
    | HPModifier = 26
    | Aura = 27
    | PreventCardPlay = 29
    | _SpreadFire = 31
    | _SquadSpawnZone = 32
    | HitMultipleProjectile = 33
    | _MarkedTargetDamageMultiplier = 36
    | _MarkedTargetDamage = 37
    | _AttackPauseDelay = 39
    | _MarkedForTeleport = 40
    | _RangeModifier = 41
    | ForceAttack = 42
    | OnEntityDie = 44
    | _FireLanceAbility = 47
    | Collector = 50
    | _ChangeTargetAggro = 51
    | _FireLanceBurstCollector = 53
    | RegenerationOld = 54
    | _TimedSpell = 57
    | Scatter = 58
    | _DamageOverTimeNoCombat = 59
    | LifeStealer = 60
    | BarrierGate = 61
    | GlobalRevive = 62
    | TrampleResistance = 64
    | TrampleOverwrite = 65
    | PushbackResistance = 66
    | MeleePushbackOverride = 67
    | _FanCollector = 69
    | MeleeFightSpeedModifier = 71
    | _SpellRangeModifierIncoming = 72
    | SpellRangeModifierOutgoing = 73
    | _FanCollectorBurst = 74
    | RangedFightSpeedModifier = 75
    | _SquadRestore = 76
    | DamagePowerTransfer = 79
    | TimedSpell = 80
    | TrampleRevengeDamage = 81
    | LinkedFire = 83
    | DamageBuffAgainst = 84
    | IncomingDamageModifier = 85
    | GeneratorPower = 86
    | IceShield = 87
    | DoTRefresh = 88
    | EnrageThreshold = 89
    | Immunity = 90
    | _UnitSpawnZone = 91
    | Rage = 92
    | _PassiveCharge = 93
    | MeleeHitSpell = 95
    | _FireDebuff = 97
    | FrostDebuff = 98
    | SpellBlocker = 100
    | ShadowDebuff = 102
    | SuicidalBomb = 103
    | GrantToken = 110
    | TurretCannon = 112
    | SpellOnSelfCast = 113
    | AbilityOnSelfResolve = 114
    | SuppressUserCommand = 118
    | LineCast = 120
    | NoCheer = 132
    | UnitShredderJobCondition = 133
    | DamageRadialArea = 134
    | _DamageConeArea = 137
    | DamageConeCutArea = 138
    | ConstructionRepairModifier = 139
    | Portal = 140
    | Tunnel = 141
    | ModeConditionDelay = 142
    | HealAreaRadial = 144
    | _145LeftoverDoesNotReallyExistButIsUsed = 145
    | _146LeftoverDoesNotReallyExistButIsUsed = 146
    | OverrideWeaponType = 151
    | DamageRadialAreaUsingCorpse = 153
    | HealAreaRadialInstantContinues = 154
    | ChargeableBombController = 155
    | ChargeAttack = 156
    | ChargeableBomb = 157
    | ModifyRotationSpeed = 159
    | ModifyAcceleration = 160
    | FormationOverwrite = 161
    | EffectHolder = 162
    | WhiteRangersHomeDefenseTrigger = 163
    | _167LeftoverDoesNotReallyExistButIsUsed = 167
    | _168LeftoverDoesNotReallyExistButIsUsed = 168
    | HealReservoirUsingCorpse = 170
    | ModeChangeBlocker = 171
    | BarrierModuleEnterBlock = 172
    | ProduceAmmoUsingCorpseInjurity = 173
    | IncomingDamageSpreadOnTargetAlignmentArea1 = 174
    | DamageSelfOnMeleeHit = 175
    | HealthCapCurrent = 176
    | ConstructionUnCrushable = 179
    | ProduceAmmoOverTime = 180
    | BarrierSetBuildDelay = 181
    | ChannelTimedSpell = 183
    | AuraOnEnter = 184
    | ParalyzeAbility = 185
    | IgnoreSummoningSickness = 186
    | BlockRepair = 187
    | Corruption = 188
    | UnHealable = 189
    | Immobile = 190
    | ModifyHealing = 191
    | IgnoreInCardCondition = 192
    | MovementMode = 193
    | ConsumeAmmoHealSelf = 195
    | ConsumeAmmoHealAreaRadial = 196
    | CorpseGather = 197
    | AbilityNearEntity = 198
    | ModifyIceShieldDecayRate = 200
    | ModifyDamageIncomingAuraContingentSelfDamage = 201
    | ModifyDamageIncomingAuraContingentSelfDamageTargetAbility = 202
    | ConvertCorpseToPower = 203
    | EraseOverTime = 204
    | FireStreamChannel = 205
    | DisableMeleeAttack = 206
    | AbilityOnPlayer = 207
    | GlobalAbilityOnEntity = 208
    | AuraModifyCardCost = 209
    | AuraModifyBuildTime = 210
    | GlobalRotTimeModifier = 211
    | _212LeftoverDoesNotReallyExistButIsUsed = 212
    | MindControl = 213
    | SpellOnEntityNearby = 214
    | AmmoConsumeModifyIncomingDamage = 216
    | AmmoConsumeModifyOutgoingDamage = 217
    | GlobalSuppressRefund = 219
    | DirectRefundOnDie = 220
    | OutgoingDamageDependendSpell = 221
    | DeathCounter = 222
    | DeathCounterController = 223
    | DamageRadialAreaUsingGraveyard = 224
    | MovingIntervalCast = 225
    | BarrierGateDelay = 226
    | EffectHolderAmmo = 227
    | FightDependentAbility = 228
    | GlobalIgnoreCardPlayConditions = 229
    | WormMovement = 230
    | DamageRectAreaAligned = 231
    | GlobalRefundOnEntityDie = 232
    | GlobalDamageAbsorption = 233
    | GlobalPowerRecovermentModifier = 234
    | GlobalDamageAbsorptionTargetAbility = 235
    | OverwriteVisRange = 236
    | DamageOverTimeCastDepending = 237
    | ModifyDamageIncomingAuraContingentSelfRadialAreaDamage = 238
    | SuperWeaponShadow = 239
    | NoMeleeAgainstAir = 240
    | _SuperWeaponShadowDamage = 242
    | NoCardPlay = 243
    | NoClaim = 244
    | DamageRadialAreaAmmo = 246
    | PathLayerOverride = 247
    | ChannelBlock = 248
    | Polymorph = 249
    | Delay = 250
    | ModifyDamageIncomingOnFigure = 251
    | ImmobileRoot = 252
    | GlobalModifyCorpseGather = 253
    | AbilityDependentAbility = 254
    | CorpseManager = 255
    | DisableToken = 256
    | Piercing = 258
    | ReceiveMeleeAttacks = 259
    | BuildBlock = 260
    | PreventCardPlayAuraBuilding = 262
    | GraveyardDependentRecast = 263
    | ClaimBlock = 264
    | AmmoStartup = 265
    | DamageDistribution = 266
    | SwapSquadNightGuard = 267
    | Revive = 268
    | Amok = 269
    | NoCombat = 270
    | SlowDownDisabled = 271
    | CrowdControlTimeModifier = 272
    | DamageOnMeleeHit = 273
    | IgnoreIncomingDamageModifier = 275
    | BlockRevive = 278
    | GlobalMorphState = 279
    | SpecialOnTarget = 280
    | FleshBenderBugSwitch = 281
    | TimedMorph = 282
    | GlobalBuildTimeModifier = 283
    | CardBlock = 285
    | IceShieldRegeneration = 286
    | HealOverTime = 287
    | IceShieldTimerOffset = 288
    | SpellOnVanish = 289
    | GlobalVoidAbsorption = 290
    | VoidContainer = 291
    | ConvertCorpseToHealing = 292
    | OnEntitySpawn = 293
    | OnMorph = 294
    | Sprint = 295



type AreaShape =
    | Circle of {|
          Center : Position2D
          Radius : single
        |}
    | Cone of {|
          Base : Position2D
          Radius : single
          Angle : single
        |}
    | ConeCut of {|
          Start : Position2D
          /// <summary>
          ///  or maybe direction (normalized to length 1), I did not quickly find example to check this on
          /// </summary>
          End : Position2D
          Radius : single
          WidthNear : single
          WidthFar : single
        |}
    | WideLine of {|
          Start : Position2D
          End : Position2D
          Width : single
        |}

type AreaShapeConverter() =
    inherit JsonConverter<AreaShape>()
    override _.Write(writer, value, options) =
        match value with
        | Circle f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Circle")
            writer.WritePropertyName("center")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.Center, options)
            writer.WritePropertyName("radius")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.Radius, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Cone f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Cone")
            writer.WritePropertyName("base")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.Base, options)
            writer.WritePropertyName("radius")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.Radius, options)
            writer.WritePropertyName("angle")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.Angle, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | ConeCut f ->
            writer.WriteStartObject()
            writer.WriteStartObject("ConeCut")
            writer.WritePropertyName("start")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.Start, options)
            writer.WritePropertyName("end")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.End, options)
            writer.WritePropertyName("radius")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.Radius, options)
            writer.WritePropertyName("width_near")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.WidthNear, options)
            writer.WritePropertyName("width_far")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.WidthFar, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | WideLine f ->
            writer.WriteStartObject()
            writer.WriteStartObject("WideLine")
            writer.WritePropertyName("start")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.Start, options)
            writer.WritePropertyName("end")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.End, options)
            writer.WritePropertyName("width")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.Width, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
    override _.Read(reader, _, options) =
        if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
        then failwith "Expecting an object"
        reader.Read() |> ignore
        if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
        then failwith "Expecting an name"
        let str = reader.GetString()
        reader.Read() |> ignore
        match str with
        | "Circle" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("center" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_center = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("radius" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_radius = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            AreaShape.Circle {| Center = f_center;Radius = f_radius; |}
        | "Cone" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("base" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_base = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("radius" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_radius = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("angle" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_angle = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            AreaShape.Cone {| Base = f_base;Radius = f_radius;Angle = f_angle; |}
        | "ConeCut" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("start" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_start = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("end" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_end = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("radius" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_radius = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("width_near" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_width_near = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("width_far" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_width_far = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            AreaShape.ConeCut {| Start = f_start;End = f_end;Radius = f_radius;WidthNear = f_width_near;WidthFar = f_width_far; |}
        | "WideLine" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("start" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_start = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("end" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_end = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("width" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_width = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            AreaShape.WideLine {| Start = f_start;End = f_end;Width = f_width; |}
        | _ -> failwith $"Unexpected type: %s{str}"

type AbilityEffectSpecific =
    | DamageArea of {|
          ProgressCurrent : single
          ProgressDelta : single
          DamageRemaining : single
          Shape : AreaShape
        |}
    | DamageOverTime of {|
          TickWaitDuration : TickCount
          TicksLeft : TickCount
          TickDamage : single
        |}
    | LinkedFire of {|
          Linked : bool
          Fighting : bool
          FastCast : uint32
          SupportCap : uint16
          SupportProduction : uint8
        |}
    | SpellOnEntityNearby of {|
          SpellOnOwner : SpellId[]
          SpellOnSource : SpellId[]
          Radius : single
          RemainingTargets : uint32
        |}
    | TimedSpell of {|
          SpellsToCast : SpellId[]
        |}
    | Collector of {|
          SpellToCast : SpellId
          Radius : single
        |}
    | Aura of {|
          SpellsToApply : SpellId[]
          AbilitiesToApply : AbilityId[]
          Radius : single
        |}
    | MovingIntervalCast of {|
          SpellToCast : SpellId[]
          DirectionStep : Position2D
          CastEveryNthTick : TickCount
        |}
    /// <summary>
    ///  If you think something interesting got hidden by Other report it
    /// </summary>
    | Other

type AbilityEffectSpecificConverter() =
    inherit JsonConverter<AbilityEffectSpecific>()
    override _.Write(writer, value, options) =
        match value with
        | DamageArea f ->
            writer.WriteStartObject()
            writer.WriteStartObject("DamageArea")
            writer.WritePropertyName("progress_current")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.ProgressCurrent, options)
            writer.WritePropertyName("progress_delta")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.ProgressDelta, options)
            writer.WritePropertyName("damage_remaining")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.DamageRemaining, options)
            writer.WritePropertyName("shape")
            let c = options.GetConverter(typeof<AreaShape>) :?> JsonConverter<AreaShape>
            c.Write(writer, f.Shape, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | DamageOverTime f ->
            writer.WriteStartObject()
            writer.WriteStartObject("DamageOverTime")
            writer.WritePropertyName("tick_wait_duration")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.TickWaitDuration, options)
            writer.WritePropertyName("ticks_left")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.TicksLeft, options)
            writer.WritePropertyName("tick_damage")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.TickDamage, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | LinkedFire f ->
            writer.WriteStartObject()
            writer.WriteStartObject("LinkedFire")
            writer.WritePropertyName("linked")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.Linked, options)
            writer.WritePropertyName("fighting")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.Fighting, options)
            writer.WritePropertyName("fast_cast")
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            c.Write(writer, f.FastCast, options)
            writer.WritePropertyName("support_cap")
            let c = options.GetConverter(typeof<uint16>) :?> JsonConverter<uint16>
            c.Write(writer, f.SupportCap, options)
            writer.WritePropertyName("support_production")
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            c.Write(writer, f.SupportProduction, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | SpellOnEntityNearby f ->
            writer.WriteStartObject()
            writer.WriteStartObject("SpellOnEntityNearby")
            writer.WritePropertyName("spell_on_owner")
            let c = options.GetConverter(typeof<SpellId[]>) :?> JsonConverter<SpellId[]>
            c.Write(writer, f.SpellOnOwner, options)
            writer.WritePropertyName("spell_on_source")
            let c = options.GetConverter(typeof<SpellId[]>) :?> JsonConverter<SpellId[]>
            c.Write(writer, f.SpellOnSource, options)
            writer.WritePropertyName("radius")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.Radius, options)
            writer.WritePropertyName("remaining_targets")
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            c.Write(writer, f.RemainingTargets, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | TimedSpell f ->
            writer.WriteStartObject()
            writer.WriteStartObject("TimedSpell")
            writer.WritePropertyName("spells_to_cast")
            let c = options.GetConverter(typeof<SpellId[]>) :?> JsonConverter<SpellId[]>
            c.Write(writer, f.SpellsToCast, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Collector f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Collector")
            writer.WritePropertyName("spell_to_cast")
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            c.Write(writer, f.SpellToCast, options)
            writer.WritePropertyName("radius")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.Radius, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Aura f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Aura")
            writer.WritePropertyName("spells_to_apply")
            let c = options.GetConverter(typeof<SpellId[]>) :?> JsonConverter<SpellId[]>
            c.Write(writer, f.SpellsToApply, options)
            writer.WritePropertyName("abilities_to_apply")
            let c = options.GetConverter(typeof<AbilityId[]>) :?> JsonConverter<AbilityId[]>
            c.Write(writer, f.AbilitiesToApply, options)
            writer.WritePropertyName("radius")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.Radius, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | MovingIntervalCast f ->
            writer.WriteStartObject()
            writer.WriteStartObject("MovingIntervalCast")
            writer.WritePropertyName("spell_to_cast")
            let c = options.GetConverter(typeof<SpellId[]>) :?> JsonConverter<SpellId[]>
            c.Write(writer, f.SpellToCast, options)
            writer.WritePropertyName("direction_step")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.DirectionStep, options)
            writer.WritePropertyName("cast_every_nth_tick")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.CastEveryNthTick, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Other ->
            writer.WriteStartObject()
            writer.WriteStartObject("Other")
            writer.WriteEndObject()
    override _.Read(reader, _, options) =
        if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
        then failwith "Expecting an object"
        reader.Read() |> ignore
        if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
        then failwith "Expecting an name"
        let str = reader.GetString()
        reader.Read() |> ignore
        match str with
        | "DamageArea" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("progress_current" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_progress_current = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("progress_delta" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_progress_delta = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("damage_remaining" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_damage_remaining = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("shape" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<AreaShape>) :?> JsonConverter<AreaShape>
            let f_shape = c.Read(&reader, typeof<AreaShape>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            AbilityEffectSpecific.DamageArea {| ProgressCurrent = f_progress_current;ProgressDelta = f_progress_delta;DamageRemaining = f_damage_remaining;Shape = f_shape; |}
        | "DamageOverTime" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("tick_wait_duration" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_tick_wait_duration = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("ticks_left" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_ticks_left = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("tick_damage" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_tick_damage = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            AbilityEffectSpecific.DamageOverTime {| TickWaitDuration = f_tick_wait_duration;TicksLeft = f_ticks_left;TickDamage = f_tick_damage; |}
        | "LinkedFire" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("linked" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_linked = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("fighting" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_fighting = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("fast_cast" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            let f_fast_cast = c.Read(&reader, typeof<uint32>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("support_cap" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint16>) :?> JsonConverter<uint16>
            let f_support_cap = c.Read(&reader, typeof<uint16>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("support_production" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            let f_support_production = c.Read(&reader, typeof<uint8>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            AbilityEffectSpecific.LinkedFire {| Linked = f_linked;Fighting = f_fighting;FastCast = f_fast_cast;SupportCap = f_support_cap;SupportProduction = f_support_production; |}
        | "SpellOnEntityNearby" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell_on_owner" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId[]>) :?> JsonConverter<SpellId[]>
            let f_spell_on_owner = c.Read(&reader, typeof<SpellId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell_on_source" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId[]>) :?> JsonConverter<SpellId[]>
            let f_spell_on_source = c.Read(&reader, typeof<SpellId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("radius" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_radius = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("remaining_targets" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            let f_remaining_targets = c.Read(&reader, typeof<uint32>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            AbilityEffectSpecific.SpellOnEntityNearby {| SpellOnOwner = f_spell_on_owner;SpellOnSource = f_spell_on_source;Radius = f_radius;RemainingTargets = f_remaining_targets; |}
        | "TimedSpell" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spells_to_cast" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId[]>) :?> JsonConverter<SpellId[]>
            let f_spells_to_cast = c.Read(&reader, typeof<SpellId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            AbilityEffectSpecific.TimedSpell {| SpellsToCast = f_spells_to_cast; |}
        | "Collector" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell_to_cast" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            let f_spell_to_cast = c.Read(&reader, typeof<SpellId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("radius" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_radius = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            AbilityEffectSpecific.Collector {| SpellToCast = f_spell_to_cast;Radius = f_radius; |}
        | "Aura" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spells_to_apply" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId[]>) :?> JsonConverter<SpellId[]>
            let f_spells_to_apply = c.Read(&reader, typeof<SpellId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("abilities_to_apply" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<AbilityId[]>) :?> JsonConverter<AbilityId[]>
            let f_abilities_to_apply = c.Read(&reader, typeof<AbilityId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("radius" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_radius = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            AbilityEffectSpecific.Aura {| SpellsToApply = f_spells_to_apply;AbilitiesToApply = f_abilities_to_apply;Radius = f_radius; |}
        | "MovingIntervalCast" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell_to_cast" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId[]>) :?> JsonConverter<SpellId[]>
            let f_spell_to_cast = c.Read(&reader, typeof<SpellId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("direction_step" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_direction_step = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("cast_every_nth_tick" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_cast_every_nth_tick = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            AbilityEffectSpecific.MovingIntervalCast {| SpellToCast = f_spell_to_cast;DirectionStep = f_direction_step;CastEveryNthTick = f_cast_every_nth_tick; |}
        | "Other" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            AbilityEffectSpecific.Other
        | _ -> failwith $"Unexpected type: %s{str}"

type AbilityEffect = {
    [<JsonPropertyName("id")>]
        Id : AbilityId
    [<JsonPropertyName("line")>]
        Line : AbilityLine
    [<JsonPropertyName("source")>]
        Source : EntityId
    [<JsonPropertyName("source_team")>]
        SourceTeam : uint8
    [<JsonPropertyName("start_tick")>]
        StartTick : Tick voption
    [<JsonPropertyName("end_tick")>]
        EndTick : Tick voption
    [<JsonPropertyName("specific")>]
        Specific : AbilityEffectSpecific
}


type Aspect =
    /// <summary>
    ///  Used by *mostly* power wells
    /// </summary>
    | PowerProduction of {|
          /// <summary>
          ///  How much more power it will produce
          /// </summary>
          CurrentPower : single
          /// <summary>
          ///  Same as `current_power`, before it is build for the first time.
          /// </summary>
          PowerCapacity : single
        |}
    /// <summary>
    ///  Health of an entity.
    /// </summary>
    | Health of {|
          /// <summary>
          ///  Actual HP that it can lose before dying.
          /// </summary>
          CurrentHp : single
          /// <summary>
          ///  Current maximum including bufs and debufs.
          /// </summary>
          CapCurrentMax : single
        |}
    | Combat
    | ModeChange of {|
          CurrentMode : ModeId
          AllModes : ModeId[]
        |}
    | Ammunition
    | SuperWeaponShadow
    | WormMovement
    | NPCTag
    | PlayerKit
    | Loot
    | Immunity
    | Turret
    | Tunnel
    | MountBarrier
    | SpellMemory
    | Portal
    | Hate
    | BarrierGate of {|
          Open : bool
        |}
    | Attackable
    | SquadRefill
    | PortalExit
    /// <summary>
    ///  When building / barrier is under construction it has this aspect.
    /// </summary>
    | ConstructionData of {|
          /// <summary>
          ///  Build ticks until finished.
          /// </summary>
          RefreshCountRemaining : TickCount
          /// <summary>
          ///  Build ticks needed from start of construction to finish it.
          /// </summary>
          RefreshCountTotal : TickCount
          /// <summary>
          ///  How much health is added on build tick.
          /// </summary>
          HealthPerBuildUpdateTrigger : single
          /// <summary>
          ///  How much health is still missing.
          /// </summary>
          RemainingHealthToAdd : single
        |}
    | SuperWeaponShadowBomb
    | RepairBarrierSet
    | ConstructionRepair
    | Follower
    | CollisionBase
    | EditorUniqueID
    | Roam

type AspectConverter() =
    inherit JsonConverter<Aspect>()
    override _.Write(writer, value, options) =
        match value with
        | PowerProduction f ->
            writer.WriteStartObject()
            writer.WriteStartObject("PowerProduction")
            writer.WritePropertyName("current_power")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.CurrentPower, options)
            writer.WritePropertyName("power_capacity")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.PowerCapacity, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Health f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Health")
            writer.WritePropertyName("current_hp")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.CurrentHp, options)
            writer.WritePropertyName("cap_current_max")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.CapCurrentMax, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Combat ->
            writer.WriteStartObject()
            writer.WriteStartObject("Combat")
            writer.WriteEndObject()
        | ModeChange f ->
            writer.WriteStartObject()
            writer.WriteStartObject("ModeChange")
            writer.WritePropertyName("current_mode")
            let c = options.GetConverter(typeof<ModeId>) :?> JsonConverter<ModeId>
            c.Write(writer, f.CurrentMode, options)
            writer.WritePropertyName("all_modes")
            let c = options.GetConverter(typeof<ModeId[]>) :?> JsonConverter<ModeId[]>
            c.Write(writer, f.AllModes, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Ammunition ->
            writer.WriteStartObject()
            writer.WriteStartObject("Ammunition")
            writer.WriteEndObject()
        | SuperWeaponShadow ->
            writer.WriteStartObject()
            writer.WriteStartObject("SuperWeaponShadow")
            writer.WriteEndObject()
        | WormMovement ->
            writer.WriteStartObject()
            writer.WriteStartObject("WormMovement")
            writer.WriteEndObject()
        | NPCTag ->
            writer.WriteStartObject()
            writer.WriteStartObject("NPCTag")
            writer.WriteEndObject()
        | PlayerKit ->
            writer.WriteStartObject()
            writer.WriteStartObject("PlayerKit")
            writer.WriteEndObject()
        | Loot ->
            writer.WriteStartObject()
            writer.WriteStartObject("Loot")
            writer.WriteEndObject()
        | Immunity ->
            writer.WriteStartObject()
            writer.WriteStartObject("Immunity")
            writer.WriteEndObject()
        | Turret ->
            writer.WriteStartObject()
            writer.WriteStartObject("Turret")
            writer.WriteEndObject()
        | Tunnel ->
            writer.WriteStartObject()
            writer.WriteStartObject("Tunnel")
            writer.WriteEndObject()
        | MountBarrier ->
            writer.WriteStartObject()
            writer.WriteStartObject("MountBarrier")
            writer.WriteEndObject()
        | SpellMemory ->
            writer.WriteStartObject()
            writer.WriteStartObject("SpellMemory")
            writer.WriteEndObject()
        | Portal ->
            writer.WriteStartObject()
            writer.WriteStartObject("Portal")
            writer.WriteEndObject()
        | Hate ->
            writer.WriteStartObject()
            writer.WriteStartObject("Hate")
            writer.WriteEndObject()
        | BarrierGate f ->
            writer.WriteStartObject()
            writer.WriteStartObject("BarrierGate")
            writer.WritePropertyName("open")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.Open, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Attackable ->
            writer.WriteStartObject()
            writer.WriteStartObject("Attackable")
            writer.WriteEndObject()
        | SquadRefill ->
            writer.WriteStartObject()
            writer.WriteStartObject("SquadRefill")
            writer.WriteEndObject()
        | PortalExit ->
            writer.WriteStartObject()
            writer.WriteStartObject("PortalExit")
            writer.WriteEndObject()
        | ConstructionData f ->
            writer.WriteStartObject()
            writer.WriteStartObject("ConstructionData")
            writer.WritePropertyName("refresh_count_remaining")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.RefreshCountRemaining, options)
            writer.WritePropertyName("refresh_count_total")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.RefreshCountTotal, options)
            writer.WritePropertyName("health_per_build_update_trigger")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.HealthPerBuildUpdateTrigger, options)
            writer.WritePropertyName("remaining_health_to_add")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.RemainingHealthToAdd, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | SuperWeaponShadowBomb ->
            writer.WriteStartObject()
            writer.WriteStartObject("SuperWeaponShadowBomb")
            writer.WriteEndObject()
        | RepairBarrierSet ->
            writer.WriteStartObject()
            writer.WriteStartObject("RepairBarrierSet")
            writer.WriteEndObject()
        | ConstructionRepair ->
            writer.WriteStartObject()
            writer.WriteStartObject("ConstructionRepair")
            writer.WriteEndObject()
        | Follower ->
            writer.WriteStartObject()
            writer.WriteStartObject("Follower")
            writer.WriteEndObject()
        | CollisionBase ->
            writer.WriteStartObject()
            writer.WriteStartObject("CollisionBase")
            writer.WriteEndObject()
        | EditorUniqueID ->
            writer.WriteStartObject()
            writer.WriteStartObject("EditorUniqueID")
            writer.WriteEndObject()
        | Roam ->
            writer.WriteStartObject()
            writer.WriteStartObject("Roam")
            writer.WriteEndObject()
    override _.Read(reader, _, options) =
        if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
        then failwith "Expecting an object"
        reader.Read() |> ignore
        if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
        then failwith "Expecting an name"
        let str = reader.GetString()
        reader.Read() |> ignore
        match str with
        | "PowerProduction" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("current_power" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_current_power = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("power_capacity" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_power_capacity = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.PowerProduction {| CurrentPower = f_current_power;PowerCapacity = f_power_capacity; |}
        | "Health" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("current_hp" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_current_hp = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("cap_current_max" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_cap_current_max = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.Health {| CurrentHp = f_current_hp;CapCurrentMax = f_cap_current_max; |}
        | "Combat" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.Combat
        | "ModeChange" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("current_mode" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<ModeId>) :?> JsonConverter<ModeId>
            let f_current_mode = c.Read(&reader, typeof<ModeId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("all_modes" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<ModeId[]>) :?> JsonConverter<ModeId[]>
            let f_all_modes = c.Read(&reader, typeof<ModeId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.ModeChange {| CurrentMode = f_current_mode;AllModes = f_all_modes; |}
        | "Ammunition" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.Ammunition
        | "SuperWeaponShadow" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.SuperWeaponShadow
        | "WormMovement" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.WormMovement
        | "NPCTag" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.NPCTag
        | "PlayerKit" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.PlayerKit
        | "Loot" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.Loot
        | "Immunity" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.Immunity
        | "Turret" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.Turret
        | "Tunnel" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.Tunnel
        | "MountBarrier" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.MountBarrier
        | "SpellMemory" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.SpellMemory
        | "Portal" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.Portal
        | "Hate" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.Hate
        | "BarrierGate" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("open" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_open = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.BarrierGate {| Open = f_open; |}
        | "Attackable" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.Attackable
        | "SquadRefill" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.SquadRefill
        | "PortalExit" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.PortalExit
        | "ConstructionData" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("refresh_count_remaining" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_refresh_count_remaining = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("refresh_count_total" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_refresh_count_total = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("health_per_build_update_trigger" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_health_per_build_update_trigger = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("remaining_health_to_add" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_remaining_health_to_add = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.ConstructionData {| RefreshCountRemaining = f_refresh_count_remaining;RefreshCountTotal = f_refresh_count_total;HealthPerBuildUpdateTrigger = f_health_per_build_update_trigger;RemainingHealthToAdd = f_remaining_health_to_add; |}
        | "SuperWeaponShadowBomb" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.SuperWeaponShadowBomb
        | "RepairBarrierSet" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.RepairBarrierSet
        | "ConstructionRepair" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.ConstructionRepair
        | "Follower" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.Follower
        | "CollisionBase" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.CollisionBase
        | "EditorUniqueID" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.EditorUniqueID
        | "Roam" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Aspect.Roam
        | _ -> failwith $"Unexpected type: %s{str}"

/// <summary>
///  Simplified version of how many monuments of each color player have
/// </summary>
type Orbs = {
    [<JsonPropertyName("shadow")>]
        Shadow : uint8
    [<JsonPropertyName("nature")>]
        Nature : uint8
    [<JsonPropertyName("frost")>]
        Frost : uint8
    [<JsonPropertyName("fire")>]
        Fire : uint8
    /// <summary>
    ///  Can be used instead of any color, and then changes to color of first token on the used card.
    /// </summary>
    [<JsonPropertyName("starting")>]
        Starting : uint8
    /// <summary>
    ///  Can be used only for colorless tokens on the card. (Curse Orb changes colored orb to white one)
    /// </summary>
    [<JsonPropertyName("white")>]
        White : uint8
    /// <summary>
    ///  Can be used as any color. Only provided by map scripts.
    /// </summary>
    [<JsonPropertyName("all")>]
        All : uint8
}


/// <summary>
///  Technically it is specific case of `Entity`, but we decided to move players out,
///  and move few fields up like position and owning player id
/// </summary>
type PlayerEntity = {
    /// <summary>
    ///  Unique id of the entity
    /// </summary>
    [<JsonPropertyName("id")>]
        Id : EntityId
    /// <summary>
    ///  List of effects the entity have.
    /// </summary>
    [<JsonPropertyName("effects")>]
        Effects : AbilityEffect[]
    /// <summary>
    ///  List of aspects entity have.
    /// </summary>
    [<JsonPropertyName("aspects")>]
        Aspects : Aspect[]
    [<JsonPropertyName("team")>]
        Team : uint8
    [<JsonPropertyName("power")>]
        Power : single
    [<JsonPropertyName("void_power")>]
        VoidPower : single
    [<JsonPropertyName("population_count")>]
        PopulationCount : uint16
    [<JsonPropertyName("name")>]
        Name : string
    [<JsonPropertyName("orbs")>]
        Orbs : Orbs
}


type MatchPlayer = {
    /// <summary>
    ///  Name of player.
    /// </summary>
    [<JsonPropertyName("name")>]
        Name : string
    /// <summary>
    ///  Deck used by that player.
    ///  TODO Due to technical difficulties might be empty.
    /// </summary>
    [<JsonPropertyName("deck")>]
        Deck : Deck
    /// <summary>
    ///  entity controlled by this player
    /// </summary>
    [<JsonPropertyName("entity")>]
        Entity : PlayerEntity
}


/// <summary>
///  With the way the game works, I would not be surprised, if this will cause more issues.
///  If the game crashes send the log to `Kubik` it probably mean some field in
///  one of the `Job`s needs to be `Option`.
/// </summary>
type Job =
    | NoJob
    | Idle
    | Goto of {|
          Waypoints : Position2DWithOrientation[]
          TargetEntityId : EntityId voption
          WalkMode : WalkMode
        |}
    | AttackMelee of {|
          Target : Target
          UseForceGoto : bool
          NoMove : bool
          TooCloseRange : single
        |}
    | CastSpell of {|
          Target : Target
          SpellId : SpellId
          UseForceGoto : bool
          NoMove : bool
        |}
    | Die
    | Talk of {|
          Target : EntityId
          WalkToTarget : bool
        |}
    | ScriptTalk of {|
          HideWeapon : bool
        |}
    | Freeze of {|
          EndStep : Tick
          Source : EntityId
          SpellId : SpellId
          Duration : TickCount
          DelayAbility : TickCount
          AbilityIdWhileFrozen : AbilityId[]
          AbilityIdDelayed : AbilityId[]
          AbilityLineIdCancelOnStart : AbilityLine
          PushbackImmunity : bool
          Mode : uint32
        |}
    | Spawn of {|
          Duration : TickCount
          EndStep : Tick
        |}
    | Cheer
    | AttackSquad of {|
          Target : Target
          WeaponType : uint8
          Damage : single
          RangeMin : single
          RangeMax : single
          AttackSpell : SpellId voption
          UseForceGoto : bool
          OperationRange : single
          NoMove : bool
          WasInAttack : bool
          MeleeAttack : bool
        |}
    | CastSpellSquad of {|
          Target : Target
          SpellId : SpellId
          UseForceGoto : bool
          SpellFired : bool
          SpellPerSourceEntity : bool
          WasInAttack : bool
        |}
    | PushBack of {|
          StartCoord : Position2D
          TargetCoord : Position2D
          Speed : single
          RotationSpeed : single
          Damage : single
          Source : EntityId voption
        |}
    | Stampede of {|
          Spell : SpellId
          Target : Target
          StartCoord : Position2D
        |}
    | BarrierCrush
    | BarrierGateToggle
    | FlameThrower of {|
          Target : Target
          SpellId : SpellId
          DurationStepInit : TickCount
          DurationStepShutDown : TickCount
        |}
    | Construct of {|
          ConstructionUpdateSteps : TickCount
          ConstructionUpdateCountRemaining : TickCount
        |}
    | Crush of {|
          CrushSteps : TickCount
          EntityUpdateSteps : TickCount
          RemainingCrushSteps : TickCount
        |}
    | MountBarrierSquad of {|
          BarrierModule : EntityId
        |}
    | MountBarrier of {|
          CurrentBarrierModule : EntityId voption
          GoalBarrierModule : EntityId voption
        |}
    | ModeChangeSquad of {|
          NewMode : ModeId
          ModeChangeDone : bool
        |}
    | ModeChange of {|
          NewMode : ModeId
        |}
    | SacrificeSquad of {|
          TargetEntity : EntityId
        |}
    | UsePortalSquad of {|
          TargetEntityId : EntityId
        |}
    | Channel of {|
          TargetSquadId : EntityId voption
          ModeTargetWorld : bool
          EntityId : EntityId voption
          SpellId : SpellId
          SpellIdOnTargetOnFinish : SpellId voption
          SpellIdOnTargetOnStart : SpellId voption
          StepDurationUntilFinish : TickCount
          TimingChannelStart : uint32
          TimingChannelLoop : uint32
          TimingChannelEnd : uint32
          AbortOnOutOfRangeSquared : single
          AbortCheckFailed : bool
          OrientateToTarget : bool
          OrientateToTargetMaxStep : TickCount
          AbortOnOwnerGetDamaged : bool
          AbortOnModeChange : bool
        |}
    | SpawnSquad
    | LootTargetSquad of {|
          TargetEntityId : EntityId
        |}
    | Morph of {|
          Target : Target
          Spell : SpellId
        |}
    /// <summary>
    ///  if you see this it means we did not account for some EA's case, so please report it
    /// </summary>
    | Unknown of {|
          Id : uint32
        |}

type JobConverter() =
    inherit JsonConverter<Job>()
    override _.Write(writer, value, options) =
        match value with
        | NoJob ->
            writer.WriteStartObject()
            writer.WriteStartObject("NoJob")
            writer.WriteEndObject()
        | Idle ->
            writer.WriteStartObject()
            writer.WriteStartObject("Idle")
            writer.WriteEndObject()
        | Goto f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Goto")
            writer.WritePropertyName("waypoints")
            let c = options.GetConverter(typeof<Position2DWithOrientation[]>) :?> JsonConverter<Position2DWithOrientation[]>
            c.Write(writer, f.Waypoints, options)
            writer.WritePropertyName("target_entity_id")
            let c = options.GetConverter(typeof<EntityId voption>) :?> JsonConverter<EntityId voption>
            c.Write(writer, f.TargetEntityId, options)
            writer.WritePropertyName("walk_mode")
            let c = options.GetConverter(typeof<WalkMode>) :?> JsonConverter<WalkMode>
            c.Write(writer, f.WalkMode, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | AttackMelee f ->
            writer.WriteStartObject()
            writer.WriteStartObject("AttackMelee")
            writer.WritePropertyName("target")
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            c.Write(writer, f.Target, options)
            writer.WritePropertyName("use_force_goto")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.UseForceGoto, options)
            writer.WritePropertyName("no_move")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.NoMove, options)
            writer.WritePropertyName("too_close_range")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.TooCloseRange, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | CastSpell f ->
            writer.WriteStartObject()
            writer.WriteStartObject("CastSpell")
            writer.WritePropertyName("target")
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            c.Write(writer, f.Target, options)
            writer.WritePropertyName("spell_id")
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            c.Write(writer, f.SpellId, options)
            writer.WritePropertyName("use_force_goto")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.UseForceGoto, options)
            writer.WritePropertyName("no_move")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.NoMove, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Die ->
            writer.WriteStartObject()
            writer.WriteStartObject("Die")
            writer.WriteEndObject()
        | Talk f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Talk")
            writer.WritePropertyName("target")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.Target, options)
            writer.WritePropertyName("walk_to_target")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.WalkToTarget, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | ScriptTalk f ->
            writer.WriteStartObject()
            writer.WriteStartObject("ScriptTalk")
            writer.WritePropertyName("hide_weapon")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.HideWeapon, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Freeze f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Freeze")
            writer.WritePropertyName("end_step")
            let c = options.GetConverter(typeof<Tick>) :?> JsonConverter<Tick>
            c.Write(writer, f.EndStep, options)
            writer.WritePropertyName("source")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.Source, options)
            writer.WritePropertyName("spell_id")
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            c.Write(writer, f.SpellId, options)
            writer.WritePropertyName("duration")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.Duration, options)
            writer.WritePropertyName("delay_ability")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.DelayAbility, options)
            writer.WritePropertyName("ability_id_while_frozen")
            let c = options.GetConverter(typeof<AbilityId[]>) :?> JsonConverter<AbilityId[]>
            c.Write(writer, f.AbilityIdWhileFrozen, options)
            writer.WritePropertyName("ability_id_delayed")
            let c = options.GetConverter(typeof<AbilityId[]>) :?> JsonConverter<AbilityId[]>
            c.Write(writer, f.AbilityIdDelayed, options)
            writer.WritePropertyName("ability_line_id_cancel_on_start")
            let c = options.GetConverter(typeof<AbilityLine>) :?> JsonConverter<AbilityLine>
            c.Write(writer, f.AbilityLineIdCancelOnStart, options)
            writer.WritePropertyName("pushback_immunity")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.PushbackImmunity, options)
            writer.WritePropertyName("mode")
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            c.Write(writer, f.Mode, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Spawn f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Spawn")
            writer.WritePropertyName("duration")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.Duration, options)
            writer.WritePropertyName("end_step")
            let c = options.GetConverter(typeof<Tick>) :?> JsonConverter<Tick>
            c.Write(writer, f.EndStep, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Cheer ->
            writer.WriteStartObject()
            writer.WriteStartObject("Cheer")
            writer.WriteEndObject()
        | AttackSquad f ->
            writer.WriteStartObject()
            writer.WriteStartObject("AttackSquad")
            writer.WritePropertyName("target")
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            c.Write(writer, f.Target, options)
            writer.WritePropertyName("weapon_type")
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            c.Write(writer, f.WeaponType, options)
            writer.WritePropertyName("damage")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.Damage, options)
            writer.WritePropertyName("range_min")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.RangeMin, options)
            writer.WritePropertyName("range_max")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.RangeMax, options)
            writer.WritePropertyName("attack_spell")
            let c = options.GetConverter(typeof<SpellId voption>) :?> JsonConverter<SpellId voption>
            c.Write(writer, f.AttackSpell, options)
            writer.WritePropertyName("use_force_goto")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.UseForceGoto, options)
            writer.WritePropertyName("operation_range")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.OperationRange, options)
            writer.WritePropertyName("no_move")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.NoMove, options)
            writer.WritePropertyName("was_in_attack")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.WasInAttack, options)
            writer.WritePropertyName("melee_attack")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.MeleeAttack, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | CastSpellSquad f ->
            writer.WriteStartObject()
            writer.WriteStartObject("CastSpellSquad")
            writer.WritePropertyName("target")
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            c.Write(writer, f.Target, options)
            writer.WritePropertyName("spell_id")
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            c.Write(writer, f.SpellId, options)
            writer.WritePropertyName("use_force_goto")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.UseForceGoto, options)
            writer.WritePropertyName("spell_fired")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.SpellFired, options)
            writer.WritePropertyName("spell_per_source_entity")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.SpellPerSourceEntity, options)
            writer.WritePropertyName("was_in_attack")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.WasInAttack, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | PushBack f ->
            writer.WriteStartObject()
            writer.WriteStartObject("PushBack")
            writer.WritePropertyName("start_coord")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.StartCoord, options)
            writer.WritePropertyName("target_coord")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.TargetCoord, options)
            writer.WritePropertyName("speed")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.Speed, options)
            writer.WritePropertyName("rotation_speed")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.RotationSpeed, options)
            writer.WritePropertyName("damage")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.Damage, options)
            writer.WritePropertyName("source")
            let c = options.GetConverter(typeof<EntityId voption>) :?> JsonConverter<EntityId voption>
            c.Write(writer, f.Source, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Stampede f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Stampede")
            writer.WritePropertyName("spell")
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            c.Write(writer, f.Spell, options)
            writer.WritePropertyName("target")
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            c.Write(writer, f.Target, options)
            writer.WritePropertyName("start_coord")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.StartCoord, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | BarrierCrush ->
            writer.WriteStartObject()
            writer.WriteStartObject("BarrierCrush")
            writer.WriteEndObject()
        | BarrierGateToggle ->
            writer.WriteStartObject()
            writer.WriteStartObject("BarrierGateToggle")
            writer.WriteEndObject()
        | FlameThrower f ->
            writer.WriteStartObject()
            writer.WriteStartObject("FlameThrower")
            writer.WritePropertyName("target")
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            c.Write(writer, f.Target, options)
            writer.WritePropertyName("spell_id")
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            c.Write(writer, f.SpellId, options)
            writer.WritePropertyName("duration_step_init")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.DurationStepInit, options)
            writer.WritePropertyName("duration_step_shut_down")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.DurationStepShutDown, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Construct f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Construct")
            writer.WritePropertyName("construction_update_steps")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.ConstructionUpdateSteps, options)
            writer.WritePropertyName("construction_update_count_remaining")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.ConstructionUpdateCountRemaining, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Crush f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Crush")
            writer.WritePropertyName("crush_steps")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.CrushSteps, options)
            writer.WritePropertyName("entity_update_steps")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.EntityUpdateSteps, options)
            writer.WritePropertyName("remaining_crush_steps")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.RemainingCrushSteps, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | MountBarrierSquad f ->
            writer.WriteStartObject()
            writer.WriteStartObject("MountBarrierSquad")
            writer.WritePropertyName("barrier_module")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.BarrierModule, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | MountBarrier f ->
            writer.WriteStartObject()
            writer.WriteStartObject("MountBarrier")
            writer.WritePropertyName("current_barrier_module")
            let c = options.GetConverter(typeof<EntityId voption>) :?> JsonConverter<EntityId voption>
            c.Write(writer, f.CurrentBarrierModule, options)
            writer.WritePropertyName("goal_barrier_module")
            let c = options.GetConverter(typeof<EntityId voption>) :?> JsonConverter<EntityId voption>
            c.Write(writer, f.GoalBarrierModule, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | ModeChangeSquad f ->
            writer.WriteStartObject()
            writer.WriteStartObject("ModeChangeSquad")
            writer.WritePropertyName("new_mode")
            let c = options.GetConverter(typeof<ModeId>) :?> JsonConverter<ModeId>
            c.Write(writer, f.NewMode, options)
            writer.WritePropertyName("mode_change_done")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.ModeChangeDone, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | ModeChange f ->
            writer.WriteStartObject()
            writer.WriteStartObject("ModeChange")
            writer.WritePropertyName("new_mode")
            let c = options.GetConverter(typeof<ModeId>) :?> JsonConverter<ModeId>
            c.Write(writer, f.NewMode, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | SacrificeSquad f ->
            writer.WriteStartObject()
            writer.WriteStartObject("SacrificeSquad")
            writer.WritePropertyName("target_entity")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.TargetEntity, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | UsePortalSquad f ->
            writer.WriteStartObject()
            writer.WriteStartObject("UsePortalSquad")
            writer.WritePropertyName("target_entity_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.TargetEntityId, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Channel f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Channel")
            writer.WritePropertyName("target_squad_id")
            let c = options.GetConverter(typeof<EntityId voption>) :?> JsonConverter<EntityId voption>
            c.Write(writer, f.TargetSquadId, options)
            writer.WritePropertyName("mode_target_world")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.ModeTargetWorld, options)
            writer.WritePropertyName("entity_id")
            let c = options.GetConverter(typeof<EntityId voption>) :?> JsonConverter<EntityId voption>
            c.Write(writer, f.EntityId, options)
            writer.WritePropertyName("spell_id")
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            c.Write(writer, f.SpellId, options)
            writer.WritePropertyName("spell_id_on_target_on_finish")
            let c = options.GetConverter(typeof<SpellId voption>) :?> JsonConverter<SpellId voption>
            c.Write(writer, f.SpellIdOnTargetOnFinish, options)
            writer.WritePropertyName("spell_id_on_target_on_start")
            let c = options.GetConverter(typeof<SpellId voption>) :?> JsonConverter<SpellId voption>
            c.Write(writer, f.SpellIdOnTargetOnStart, options)
            writer.WritePropertyName("step_duration_until_finish")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.StepDurationUntilFinish, options)
            writer.WritePropertyName("timing_channel_start")
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            c.Write(writer, f.TimingChannelStart, options)
            writer.WritePropertyName("timing_channel_loop")
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            c.Write(writer, f.TimingChannelLoop, options)
            writer.WritePropertyName("timing_channel_end")
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            c.Write(writer, f.TimingChannelEnd, options)
            writer.WritePropertyName("abort_on_out_of_range_squared")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.AbortOnOutOfRangeSquared, options)
            writer.WritePropertyName("abort_check_failed")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.AbortCheckFailed, options)
            writer.WritePropertyName("orientate_to_target")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.OrientateToTarget, options)
            writer.WritePropertyName("orientate_to_target_max_step")
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            c.Write(writer, f.OrientateToTargetMaxStep, options)
            writer.WritePropertyName("abort_on_owner_get_damaged")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.AbortOnOwnerGetDamaged, options)
            writer.WritePropertyName("abort_on_mode_change")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.AbortOnModeChange, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | SpawnSquad ->
            writer.WriteStartObject()
            writer.WriteStartObject("SpawnSquad")
            writer.WriteEndObject()
        | LootTargetSquad f ->
            writer.WriteStartObject()
            writer.WriteStartObject("LootTargetSquad")
            writer.WritePropertyName("target_entity_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.TargetEntityId, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Morph f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Morph")
            writer.WritePropertyName("target")
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            c.Write(writer, f.Target, options)
            writer.WritePropertyName("spell")
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            c.Write(writer, f.Spell, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Unknown f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Unknown")
            writer.WritePropertyName("id")
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            c.Write(writer, f.Id, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
    override _.Read(reader, _, options) =
        if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
        then failwith "Expecting an object"
        reader.Read() |> ignore
        if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
        then failwith "Expecting an name"
        let str = reader.GetString()
        reader.Read() |> ignore
        match str with
        | "NoJob" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.NoJob
        | "Idle" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.Idle
        | "Goto" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("waypoints" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2DWithOrientation[]>) :?> JsonConverter<Position2DWithOrientation[]>
            let f_waypoints = c.Read(&reader, typeof<Position2DWithOrientation[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target_entity_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId voption>) :?> JsonConverter<EntityId voption>
            let f_target_entity_id = c.Read(&reader, typeof<EntityId voption>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("walk_mode" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<WalkMode>) :?> JsonConverter<WalkMode>
            let f_walk_mode = c.Read(&reader, typeof<WalkMode>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.Goto {| Waypoints = f_waypoints;TargetEntityId = f_target_entity_id;WalkMode = f_walk_mode; |}
        | "AttackMelee" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            let f_target = c.Read(&reader, typeof<Target>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("use_force_goto" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_use_force_goto = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("no_move" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_no_move = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("too_close_range" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_too_close_range = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.AttackMelee {| Target = f_target;UseForceGoto = f_use_force_goto;NoMove = f_no_move;TooCloseRange = f_too_close_range; |}
        | "CastSpell" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            let f_target = c.Read(&reader, typeof<Target>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            let f_spell_id = c.Read(&reader, typeof<SpellId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("use_force_goto" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_use_force_goto = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("no_move" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_no_move = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.CastSpell {| Target = f_target;SpellId = f_spell_id;UseForceGoto = f_use_force_goto;NoMove = f_no_move; |}
        | "Die" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.Die
        | "Talk" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_target = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("walk_to_target" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_walk_to_target = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.Talk {| Target = f_target;WalkToTarget = f_walk_to_target; |}
        | "ScriptTalk" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("hide_weapon" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_hide_weapon = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.ScriptTalk {| HideWeapon = f_hide_weapon; |}
        | "Freeze" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("end_step" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Tick>) :?> JsonConverter<Tick>
            let f_end_step = c.Read(&reader, typeof<Tick>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("source" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_source = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            let f_spell_id = c.Read(&reader, typeof<SpellId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("duration" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_duration = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("delay_ability" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_delay_ability = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("ability_id_while_frozen" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<AbilityId[]>) :?> JsonConverter<AbilityId[]>
            let f_ability_id_while_frozen = c.Read(&reader, typeof<AbilityId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("ability_id_delayed" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<AbilityId[]>) :?> JsonConverter<AbilityId[]>
            let f_ability_id_delayed = c.Read(&reader, typeof<AbilityId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("ability_line_id_cancel_on_start" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<AbilityLine>) :?> JsonConverter<AbilityLine>
            let f_ability_line_id_cancel_on_start = c.Read(&reader, typeof<AbilityLine>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("pushback_immunity" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_pushback_immunity = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("mode" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            let f_mode = c.Read(&reader, typeof<uint32>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.Freeze {| EndStep = f_end_step;Source = f_source;SpellId = f_spell_id;Duration = f_duration;DelayAbility = f_delay_ability;AbilityIdWhileFrozen = f_ability_id_while_frozen;AbilityIdDelayed = f_ability_id_delayed;AbilityLineIdCancelOnStart = f_ability_line_id_cancel_on_start;PushbackImmunity = f_pushback_immunity;Mode = f_mode; |}
        | "Spawn" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("duration" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_duration = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("end_step" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Tick>) :?> JsonConverter<Tick>
            let f_end_step = c.Read(&reader, typeof<Tick>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.Spawn {| Duration = f_duration;EndStep = f_end_step; |}
        | "Cheer" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.Cheer
        | "AttackSquad" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            let f_target = c.Read(&reader, typeof<Target>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("weapon_type" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            let f_weapon_type = c.Read(&reader, typeof<uint8>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("damage" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_damage = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("range_min" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_range_min = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("range_max" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_range_max = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("attack_spell" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId voption>) :?> JsonConverter<SpellId voption>
            let f_attack_spell = c.Read(&reader, typeof<SpellId voption>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("use_force_goto" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_use_force_goto = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("operation_range" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_operation_range = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("no_move" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_no_move = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("was_in_attack" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_was_in_attack = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("melee_attack" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_melee_attack = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.AttackSquad {| Target = f_target;WeaponType = f_weapon_type;Damage = f_damage;RangeMin = f_range_min;RangeMax = f_range_max;AttackSpell = f_attack_spell;UseForceGoto = f_use_force_goto;OperationRange = f_operation_range;NoMove = f_no_move;WasInAttack = f_was_in_attack;MeleeAttack = f_melee_attack; |}
        | "CastSpellSquad" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            let f_target = c.Read(&reader, typeof<Target>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            let f_spell_id = c.Read(&reader, typeof<SpellId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("use_force_goto" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_use_force_goto = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell_fired" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_spell_fired = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell_per_source_entity" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_spell_per_source_entity = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("was_in_attack" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_was_in_attack = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.CastSpellSquad {| Target = f_target;SpellId = f_spell_id;UseForceGoto = f_use_force_goto;SpellFired = f_spell_fired;SpellPerSourceEntity = f_spell_per_source_entity;WasInAttack = f_was_in_attack; |}
        | "PushBack" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("start_coord" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_start_coord = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target_coord" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_target_coord = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("speed" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_speed = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("rotation_speed" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_rotation_speed = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("damage" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_damage = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("source" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId voption>) :?> JsonConverter<EntityId voption>
            let f_source = c.Read(&reader, typeof<EntityId voption>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.PushBack {| StartCoord = f_start_coord;TargetCoord = f_target_coord;Speed = f_speed;RotationSpeed = f_rotation_speed;Damage = f_damage;Source = f_source; |}
        | "Stampede" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            let f_spell = c.Read(&reader, typeof<SpellId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            let f_target = c.Read(&reader, typeof<Target>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("start_coord" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_start_coord = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.Stampede {| Spell = f_spell;Target = f_target;StartCoord = f_start_coord; |}
        | "BarrierCrush" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.BarrierCrush
        | "BarrierGateToggle" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.BarrierGateToggle
        | "FlameThrower" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            let f_target = c.Read(&reader, typeof<Target>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            let f_spell_id = c.Read(&reader, typeof<SpellId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("duration_step_init" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_duration_step_init = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("duration_step_shut_down" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_duration_step_shut_down = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.FlameThrower {| Target = f_target;SpellId = f_spell_id;DurationStepInit = f_duration_step_init;DurationStepShutDown = f_duration_step_shut_down; |}
        | "Construct" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("construction_update_steps" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_construction_update_steps = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("construction_update_count_remaining" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_construction_update_count_remaining = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.Construct {| ConstructionUpdateSteps = f_construction_update_steps;ConstructionUpdateCountRemaining = f_construction_update_count_remaining; |}
        | "Crush" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("crush_steps" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_crush_steps = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("entity_update_steps" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_entity_update_steps = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("remaining_crush_steps" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_remaining_crush_steps = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.Crush {| CrushSteps = f_crush_steps;EntityUpdateSteps = f_entity_update_steps;RemainingCrushSteps = f_remaining_crush_steps; |}
        | "MountBarrierSquad" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("barrier_module" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_barrier_module = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.MountBarrierSquad {| BarrierModule = f_barrier_module; |}
        | "MountBarrier" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("current_barrier_module" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId voption>) :?> JsonConverter<EntityId voption>
            let f_current_barrier_module = c.Read(&reader, typeof<EntityId voption>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("goal_barrier_module" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId voption>) :?> JsonConverter<EntityId voption>
            let f_goal_barrier_module = c.Read(&reader, typeof<EntityId voption>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.MountBarrier {| CurrentBarrierModule = f_current_barrier_module;GoalBarrierModule = f_goal_barrier_module; |}
        | "ModeChangeSquad" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("new_mode" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<ModeId>) :?> JsonConverter<ModeId>
            let f_new_mode = c.Read(&reader, typeof<ModeId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("mode_change_done" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_mode_change_done = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.ModeChangeSquad {| NewMode = f_new_mode;ModeChangeDone = f_mode_change_done; |}
        | "ModeChange" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("new_mode" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<ModeId>) :?> JsonConverter<ModeId>
            let f_new_mode = c.Read(&reader, typeof<ModeId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.ModeChange {| NewMode = f_new_mode; |}
        | "SacrificeSquad" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target_entity" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_target_entity = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.SacrificeSquad {| TargetEntity = f_target_entity; |}
        | "UsePortalSquad" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target_entity_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_target_entity_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.UsePortalSquad {| TargetEntityId = f_target_entity_id; |}
        | "Channel" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target_squad_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId voption>) :?> JsonConverter<EntityId voption>
            let f_target_squad_id = c.Read(&reader, typeof<EntityId voption>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("mode_target_world" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_mode_target_world = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("entity_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId voption>) :?> JsonConverter<EntityId voption>
            let f_entity_id = c.Read(&reader, typeof<EntityId voption>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            let f_spell_id = c.Read(&reader, typeof<SpellId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell_id_on_target_on_finish" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId voption>) :?> JsonConverter<SpellId voption>
            let f_spell_id_on_target_on_finish = c.Read(&reader, typeof<SpellId voption>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell_id_on_target_on_start" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId voption>) :?> JsonConverter<SpellId voption>
            let f_spell_id_on_target_on_start = c.Read(&reader, typeof<SpellId voption>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("step_duration_until_finish" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_step_duration_until_finish = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("timing_channel_start" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            let f_timing_channel_start = c.Read(&reader, typeof<uint32>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("timing_channel_loop" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            let f_timing_channel_loop = c.Read(&reader, typeof<uint32>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("timing_channel_end" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            let f_timing_channel_end = c.Read(&reader, typeof<uint32>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("abort_on_out_of_range_squared" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_abort_on_out_of_range_squared = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("abort_check_failed" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_abort_check_failed = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("orientate_to_target" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_orientate_to_target = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("orientate_to_target_max_step" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<TickCount>) :?> JsonConverter<TickCount>
            let f_orientate_to_target_max_step = c.Read(&reader, typeof<TickCount>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("abort_on_owner_get_damaged" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_abort_on_owner_get_damaged = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("abort_on_mode_change" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_abort_on_mode_change = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.Channel {| TargetSquadId = f_target_squad_id;ModeTargetWorld = f_mode_target_world;EntityId = f_entity_id;SpellId = f_spell_id;SpellIdOnTargetOnFinish = f_spell_id_on_target_on_finish;SpellIdOnTargetOnStart = f_spell_id_on_target_on_start;StepDurationUntilFinish = f_step_duration_until_finish;TimingChannelStart = f_timing_channel_start;TimingChannelLoop = f_timing_channel_loop;TimingChannelEnd = f_timing_channel_end;AbortOnOutOfRangeSquared = f_abort_on_out_of_range_squared;AbortCheckFailed = f_abort_check_failed;OrientateToTarget = f_orientate_to_target;OrientateToTargetMaxStep = f_orientate_to_target_max_step;AbortOnOwnerGetDamaged = f_abort_on_owner_get_damaged;AbortOnModeChange = f_abort_on_mode_change; |}
        | "SpawnSquad" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.SpawnSquad
        | "LootTargetSquad" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target_entity_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_target_entity_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.LootTargetSquad {| TargetEntityId = f_target_entity_id; |}
        | "Morph" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Target>) :?> JsonConverter<Target>
            let f_target = c.Read(&reader, typeof<Target>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            let f_spell = c.Read(&reader, typeof<SpellId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.Morph {| Target = f_target;Spell = f_spell; |}
        | "Unknown" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            let f_id = c.Read(&reader, typeof<uint32>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Job.Unknown {| Id = f_id; |}
        | _ -> failwith $"Unexpected type: %s{str}"

type Ping =
    | Attention = 0
    | Attack = 1
    | Defend = 2
    | NeedHelp = 4
    | Meet = 5



/// <summary>
///  Entity on the map
/// </summary>
type Entity = {
    /// <summary>
    ///  Unique id of the entity
    /// </summary>
    [<JsonPropertyName("id")>]
        Id : EntityId
    /// <summary>
    ///  List of effects the entity have.
    /// </summary>
    [<JsonPropertyName("effects")>]
        Effects : AbilityEffect[]
    /// <summary>
    ///  List of aspects entity have.
    /// </summary>
    [<JsonPropertyName("aspects")>]
        Aspects : Aspect[]
    /// <summary>
    ///  What is the entity doing right now
    /// </summary>
    [<JsonPropertyName("job")>]
        Job : Job
    /// <summary>
    ///  position on the map
    /// </summary>
    [<JsonPropertyName("position")>]
        Position : Position
    /// <summary>
    ///  id of player that owns this entity
    /// </summary>
    [<JsonPropertyName("player_entity_id")>]
        PlayerEntityId : EntityId voption
}


type Projectile = {
    /// <summary>
    ///  Unique id of the entity
    /// </summary>
    [<JsonPropertyName("id")>]
        Id : EntityId
    /// <summary>
    ///  position on the map
    /// </summary>
    [<JsonPropertyName("position")>]
        Position : Position
}


type PowerSlot = {
    [<JsonPropertyName("entity")>]
        Entity : Entity
    [<JsonPropertyName("res_id")>]
        ResId : uint32
    [<JsonPropertyName("state")>]
        State : uint32
    [<JsonPropertyName("team")>]
        Team : uint8
}


type TokenSlot = {
    [<JsonPropertyName("entity")>]
        Entity : Entity
    [<JsonPropertyName("color")>]
        Color : OrbColor
}


type AbilityWorldObject = {
    [<JsonPropertyName("entity")>]
        Entity : Entity
}


type Squad = {
    [<JsonPropertyName("entity")>]
        Entity : Entity
    [<JsonPropertyName("card_id")>]
        CardId : CardId
    [<JsonPropertyName("res_squad_id")>]
        ResSquadId : SquadId
    [<JsonPropertyName("bound_power")>]
        BoundPower : single
    [<JsonPropertyName("squad_size")>]
        SquadSize : uint8
    /// <summary>
    ///  IDs of the figures in the squad
    /// </summary>
    [<JsonPropertyName("figures")>]
        Figures : EntityId[]
}


type Figure = {
    [<JsonPropertyName("entity")>]
        Entity : Entity
    [<JsonPropertyName("squad_id")>]
        SquadId : EntityId
    [<JsonPropertyName("current_speed")>]
        CurrentSpeed : single
    [<JsonPropertyName("rotation_speed")>]
        RotationSpeed : single
    [<JsonPropertyName("unit_size")>]
        UnitSize : uint8
    [<JsonPropertyName("move_mode")>]
        MoveMode : uint8
}


type Building = {
    [<JsonPropertyName("entity")>]
        Entity : Entity
    [<JsonPropertyName("building_id")>]
        BuildingId : BuildingId
    [<JsonPropertyName("card_id")>]
        CardId : CardId
    [<JsonPropertyName("power_cost")>]
        PowerCost : single
}


type BarrierSet = {
    [<JsonPropertyName("entity")>]
        Entity : Entity
}


type BarrierModule = {
    [<JsonPropertyName("entity")>]
        Entity : Entity
    [<JsonPropertyName("team")>]
        Team : uint8
    [<JsonPropertyName("set")>]
        Set : EntityId
    [<JsonPropertyName("state")>]
        State : uint32
    [<JsonPropertyName("slots")>]
        Slots : uint8
    [<JsonPropertyName("free_slots")>]
        FreeSlots : uint8
    [<JsonPropertyName("walkable")>]
        Walkable : bool
}


/// <summary>
///  All the different command bot can issue.
///  For spectating bots all commands except Ping and WhisperToMaster are ignored
/// </summary>
type Command =
    /// <summary>
    ///  Play card of building type.
    /// </summary>
    | BuildHouse of {|
          /// <summary>
          ///  TODO will be 0 when received as command by another player
          /// </summary>
          CardPosition : uint8
          Xy : Position2D
          Angle : single
        |}
    /// <summary>
    ///  Play card of Spell type. (single target)
    /// </summary>
    | CastSpellGod of {|
          CardPosition : uint8
          Target : SingleTarget
        |}
    /// <summary>
    ///  Play card of Spell type. (line target)
    /// </summary>
    | CastSpellGodMulti of {|
          CardPosition : uint8
          Xy1 : Position2D
          Xy2 : Position2D
        |}
    /// <summary>
    ///  Play card of squad type (on ground)
    /// </summary>
    | ProduceSquad of {|
          CardPosition : uint8
          Xy : Position2D
        |}
    /// <summary>
    ///  Play card of squad type (on barrier)
    /// </summary>
    | ProduceSquadOnBarrier of {|
          CardPosition : uint8
          /// <summary>
          ///  Squad will spawn based on this position and go to the barrier.
          /// </summary>
          Xy : Position2D
          /// <summary>
          ///  Squad will go to this barrier, after spawning.
          /// </summary>
          BarrierToMount : EntityId
        |}
    /// <summary>
    ///  Activates spell or ability on entity.
    /// </summary>
    | CastSpellEntity of {|
          Entity : EntityId
          Spell : SpellId
          Target : SingleTarget
        |}
    /// <summary>
    ///  Opens or closes gate.
    /// </summary>
    | BarrierGateToggle of {|
          BarrierId : EntityId
        |}
    /// <summary>
    ///  Build barrier. (same as BarrierRepair if not inverted)
    /// </summary>
    | BarrierBuild of {|
          BarrierId : EntityId
          InvertedDirection : bool
        |}
    /// <summary>
    ///  Repair barrier.
    /// </summary>
    | BarrierRepair of {|
          BarrierId : EntityId
        |}
    | BarrierCancelRepair of {|
          BarrierId : EntityId
        |}
    | RepairBuilding of {|
          BuildingId : EntityId
        |}
    | CancelRepairBuilding of {|
          BuildingId : EntityId
        |}
    | GroupAttack of {|
          Squads : EntityId[]
          TargetEntityId : EntityId
          ForceAttack : bool
        |}
    | GroupEnterWall of {|
          Squads : EntityId[]
          BarrierId : EntityId
        |}
    | GroupExitWall of {|
          Squads : EntityId[]
          BarrierId : EntityId
        |}
    | GroupGoto of {|
          Squads : EntityId[]
          Positions : Position2D[]
          WalkMode : WalkMode
          Orientation : single
        |}
    | GroupHoldPosition of {|
          Squads : EntityId[]
        |}
    | GroupStopJob of {|
          Squads : EntityId[]
        |}
    | ModeChange of {|
          EntityId : EntityId
          NewModeId : ModeId
        |}
    | PowerSlotBuild of {|
          SlotId : EntityId
        |}
    | TokenSlotBuild of {|
          SlotId : EntityId
          Color : CreateOrbColor
        |}
    | Ping of {|
          Xy : Position2D
          Ping : Ping
        |}
    | Surrender
    | WhisperToMaster of {|
          Text : string
        |}

type CommandConverter() =
    inherit JsonConverter<Command>()
    override _.Write(writer, value, options) =
        match value with
        | BuildHouse f ->
            writer.WriteStartObject()
            writer.WriteStartObject("BuildHouse")
            writer.WritePropertyName("card_position")
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            c.Write(writer, f.CardPosition, options)
            writer.WritePropertyName("xy")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.Xy, options)
            writer.WritePropertyName("angle")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.Angle, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | CastSpellGod f ->
            writer.WriteStartObject()
            writer.WriteStartObject("CastSpellGod")
            writer.WritePropertyName("card_position")
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            c.Write(writer, f.CardPosition, options)
            writer.WritePropertyName("target")
            let c = options.GetConverter(typeof<SingleTarget>) :?> JsonConverter<SingleTarget>
            c.Write(writer, f.Target, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | CastSpellGodMulti f ->
            writer.WriteStartObject()
            writer.WriteStartObject("CastSpellGodMulti")
            writer.WritePropertyName("card_position")
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            c.Write(writer, f.CardPosition, options)
            writer.WritePropertyName("xy1")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.Xy1, options)
            writer.WritePropertyName("xy2")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.Xy2, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | ProduceSquad f ->
            writer.WriteStartObject()
            writer.WriteStartObject("ProduceSquad")
            writer.WritePropertyName("card_position")
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            c.Write(writer, f.CardPosition, options)
            writer.WritePropertyName("xy")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.Xy, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | ProduceSquadOnBarrier f ->
            writer.WriteStartObject()
            writer.WriteStartObject("ProduceSquadOnBarrier")
            writer.WritePropertyName("card_position")
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            c.Write(writer, f.CardPosition, options)
            writer.WritePropertyName("xy")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.Xy, options)
            writer.WritePropertyName("barrier_to_mount")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.BarrierToMount, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | CastSpellEntity f ->
            writer.WriteStartObject()
            writer.WriteStartObject("CastSpellEntity")
            writer.WritePropertyName("entity")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.Entity, options)
            writer.WritePropertyName("spell")
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            c.Write(writer, f.Spell, options)
            writer.WritePropertyName("target")
            let c = options.GetConverter(typeof<SingleTarget>) :?> JsonConverter<SingleTarget>
            c.Write(writer, f.Target, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | BarrierGateToggle f ->
            writer.WriteStartObject()
            writer.WriteStartObject("BarrierGateToggle")
            writer.WritePropertyName("barrier_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.BarrierId, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | BarrierBuild f ->
            writer.WriteStartObject()
            writer.WriteStartObject("BarrierBuild")
            writer.WritePropertyName("barrier_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.BarrierId, options)
            writer.WritePropertyName("inverted_direction")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.InvertedDirection, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | BarrierRepair f ->
            writer.WriteStartObject()
            writer.WriteStartObject("BarrierRepair")
            writer.WritePropertyName("barrier_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.BarrierId, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | BarrierCancelRepair f ->
            writer.WriteStartObject()
            writer.WriteStartObject("BarrierCancelRepair")
            writer.WritePropertyName("barrier_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.BarrierId, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | RepairBuilding f ->
            writer.WriteStartObject()
            writer.WriteStartObject("RepairBuilding")
            writer.WritePropertyName("building_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.BuildingId, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | CancelRepairBuilding f ->
            writer.WriteStartObject()
            writer.WriteStartObject("CancelRepairBuilding")
            writer.WritePropertyName("building_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.BuildingId, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | GroupAttack f ->
            writer.WriteStartObject()
            writer.WriteStartObject("GroupAttack")
            writer.WritePropertyName("squads")
            let c = options.GetConverter(typeof<EntityId[]>) :?> JsonConverter<EntityId[]>
            c.Write(writer, f.Squads, options)
            writer.WritePropertyName("target_entity_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.TargetEntityId, options)
            writer.WritePropertyName("force_attack")
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            c.Write(writer, f.ForceAttack, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | GroupEnterWall f ->
            writer.WriteStartObject()
            writer.WriteStartObject("GroupEnterWall")
            writer.WritePropertyName("squads")
            let c = options.GetConverter(typeof<EntityId[]>) :?> JsonConverter<EntityId[]>
            c.Write(writer, f.Squads, options)
            writer.WritePropertyName("barrier_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.BarrierId, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | GroupExitWall f ->
            writer.WriteStartObject()
            writer.WriteStartObject("GroupExitWall")
            writer.WritePropertyName("squads")
            let c = options.GetConverter(typeof<EntityId[]>) :?> JsonConverter<EntityId[]>
            c.Write(writer, f.Squads, options)
            writer.WritePropertyName("barrier_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.BarrierId, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | GroupGoto f ->
            writer.WriteStartObject()
            writer.WriteStartObject("GroupGoto")
            writer.WritePropertyName("squads")
            let c = options.GetConverter(typeof<EntityId[]>) :?> JsonConverter<EntityId[]>
            c.Write(writer, f.Squads, options)
            writer.WritePropertyName("positions")
            let c = options.GetConverter(typeof<Position2D[]>) :?> JsonConverter<Position2D[]>
            c.Write(writer, f.Positions, options)
            writer.WritePropertyName("walk_mode")
            let c = options.GetConverter(typeof<WalkMode>) :?> JsonConverter<WalkMode>
            c.Write(writer, f.WalkMode, options)
            writer.WritePropertyName("orientation")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.Orientation, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | GroupHoldPosition f ->
            writer.WriteStartObject()
            writer.WriteStartObject("GroupHoldPosition")
            writer.WritePropertyName("squads")
            let c = options.GetConverter(typeof<EntityId[]>) :?> JsonConverter<EntityId[]>
            c.Write(writer, f.Squads, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | GroupStopJob f ->
            writer.WriteStartObject()
            writer.WriteStartObject("GroupStopJob")
            writer.WritePropertyName("squads")
            let c = options.GetConverter(typeof<EntityId[]>) :?> JsonConverter<EntityId[]>
            c.Write(writer, f.Squads, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | ModeChange f ->
            writer.WriteStartObject()
            writer.WriteStartObject("ModeChange")
            writer.WritePropertyName("entity_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.EntityId, options)
            writer.WritePropertyName("new_mode_id")
            let c = options.GetConverter(typeof<ModeId>) :?> JsonConverter<ModeId>
            c.Write(writer, f.NewModeId, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | PowerSlotBuild f ->
            writer.WriteStartObject()
            writer.WriteStartObject("PowerSlotBuild")
            writer.WritePropertyName("slot_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.SlotId, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | TokenSlotBuild f ->
            writer.WriteStartObject()
            writer.WriteStartObject("TokenSlotBuild")
            writer.WritePropertyName("slot_id")
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            c.Write(writer, f.SlotId, options)
            writer.WritePropertyName("color")
            let c = options.GetConverter(typeof<CreateOrbColor>) :?> JsonConverter<CreateOrbColor>
            c.Write(writer, f.Color, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Ping f ->
            writer.WriteStartObject()
            writer.WriteStartObject("Ping")
            writer.WritePropertyName("xy")
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            c.Write(writer, f.Xy, options)
            writer.WritePropertyName("ping")
            let c = options.GetConverter(typeof<Ping>) :?> JsonConverter<Ping>
            c.Write(writer, f.Ping, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | Surrender ->
            writer.WriteStartObject()
            writer.WriteStartObject("Surrender")
            writer.WriteEndObject()
        | WhisperToMaster f ->
            writer.WriteStartObject()
            writer.WriteStartObject("WhisperToMaster")
            writer.WritePropertyName("text")
            let c = options.GetConverter(typeof<string>) :?> JsonConverter<string>
            c.Write(writer, f.Text, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
    override _.Read(reader, _, options) =
        if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
        then failwith "Expecting an object"
        reader.Read() |> ignore
        if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
        then failwith "Expecting an name"
        let str = reader.GetString()
        reader.Read() |> ignore
        match str with
        | "BuildHouse" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("card_position" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            let f_card_position = c.Read(&reader, typeof<uint8>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("xy" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_xy = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("angle" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_angle = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.BuildHouse {| CardPosition = f_card_position;Xy = f_xy;Angle = f_angle; |}
        | "CastSpellGod" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("card_position" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            let f_card_position = c.Read(&reader, typeof<uint8>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SingleTarget>) :?> JsonConverter<SingleTarget>
            let f_target = c.Read(&reader, typeof<SingleTarget>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.CastSpellGod {| CardPosition = f_card_position;Target = f_target; |}
        | "CastSpellGodMulti" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("card_position" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            let f_card_position = c.Read(&reader, typeof<uint8>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("xy1" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_xy1 = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("xy2" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_xy2 = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.CastSpellGodMulti {| CardPosition = f_card_position;Xy1 = f_xy1;Xy2 = f_xy2; |}
        | "ProduceSquad" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("card_position" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            let f_card_position = c.Read(&reader, typeof<uint8>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("xy" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_xy = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.ProduceSquad {| CardPosition = f_card_position;Xy = f_xy; |}
        | "ProduceSquadOnBarrier" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("card_position" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint8>) :?> JsonConverter<uint8>
            let f_card_position = c.Read(&reader, typeof<uint8>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("xy" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_xy = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("barrier_to_mount" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_barrier_to_mount = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.ProduceSquadOnBarrier {| CardPosition = f_card_position;Xy = f_xy;BarrierToMount = f_barrier_to_mount; |}
        | "CastSpellEntity" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("entity" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_entity = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("spell" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SpellId>) :?> JsonConverter<SpellId>
            let f_spell = c.Read(&reader, typeof<SpellId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<SingleTarget>) :?> JsonConverter<SingleTarget>
            let f_target = c.Read(&reader, typeof<SingleTarget>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.CastSpellEntity {| Entity = f_entity;Spell = f_spell;Target = f_target; |}
        | "BarrierGateToggle" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("barrier_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_barrier_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.BarrierGateToggle {| BarrierId = f_barrier_id; |}
        | "BarrierBuild" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("barrier_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_barrier_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("inverted_direction" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_inverted_direction = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.BarrierBuild {| BarrierId = f_barrier_id;InvertedDirection = f_inverted_direction; |}
        | "BarrierRepair" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("barrier_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_barrier_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.BarrierRepair {| BarrierId = f_barrier_id; |}
        | "BarrierCancelRepair" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("barrier_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_barrier_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.BarrierCancelRepair {| BarrierId = f_barrier_id; |}
        | "RepairBuilding" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("building_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_building_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.RepairBuilding {| BuildingId = f_building_id; |}
        | "CancelRepairBuilding" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("building_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_building_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.CancelRepairBuilding {| BuildingId = f_building_id; |}
        | "GroupAttack" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("squads" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId[]>) :?> JsonConverter<EntityId[]>
            let f_squads = c.Read(&reader, typeof<EntityId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("target_entity_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_target_entity_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("force_attack" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<bool>) :?> JsonConverter<bool>
            let f_force_attack = c.Read(&reader, typeof<bool>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.GroupAttack {| Squads = f_squads;TargetEntityId = f_target_entity_id;ForceAttack = f_force_attack; |}
        | "GroupEnterWall" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("squads" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId[]>) :?> JsonConverter<EntityId[]>
            let f_squads = c.Read(&reader, typeof<EntityId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("barrier_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_barrier_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.GroupEnterWall {| Squads = f_squads;BarrierId = f_barrier_id; |}
        | "GroupExitWall" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("squads" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId[]>) :?> JsonConverter<EntityId[]>
            let f_squads = c.Read(&reader, typeof<EntityId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("barrier_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_barrier_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.GroupExitWall {| Squads = f_squads;BarrierId = f_barrier_id; |}
        | "GroupGoto" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("squads" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId[]>) :?> JsonConverter<EntityId[]>
            let f_squads = c.Read(&reader, typeof<EntityId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("positions" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D[]>) :?> JsonConverter<Position2D[]>
            let f_positions = c.Read(&reader, typeof<Position2D[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("walk_mode" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<WalkMode>) :?> JsonConverter<WalkMode>
            let f_walk_mode = c.Read(&reader, typeof<WalkMode>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("orientation" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_orientation = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.GroupGoto {| Squads = f_squads;Positions = f_positions;WalkMode = f_walk_mode;Orientation = f_orientation; |}
        | "GroupHoldPosition" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("squads" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId[]>) :?> JsonConverter<EntityId[]>
            let f_squads = c.Read(&reader, typeof<EntityId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.GroupHoldPosition {| Squads = f_squads; |}
        | "GroupStopJob" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("squads" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId[]>) :?> JsonConverter<EntityId[]>
            let f_squads = c.Read(&reader, typeof<EntityId[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.GroupStopJob {| Squads = f_squads; |}
        | "ModeChange" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("entity_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_entity_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("new_mode_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<ModeId>) :?> JsonConverter<ModeId>
            let f_new_mode_id = c.Read(&reader, typeof<ModeId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.ModeChange {| EntityId = f_entity_id;NewModeId = f_new_mode_id; |}
        | "PowerSlotBuild" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("slot_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_slot_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.PowerSlotBuild {| SlotId = f_slot_id; |}
        | "TokenSlotBuild" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("slot_id" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<EntityId>) :?> JsonConverter<EntityId>
            let f_slot_id = c.Read(&reader, typeof<EntityId>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("color" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<CreateOrbColor>) :?> JsonConverter<CreateOrbColor>
            let f_color = c.Read(&reader, typeof<CreateOrbColor>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.TokenSlotBuild {| SlotId = f_slot_id;Color = f_color; |}
        | "Ping" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("xy" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Position2D>) :?> JsonConverter<Position2D>
            let f_xy = c.Read(&reader, typeof<Position2D>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("ping" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<Ping>) :?> JsonConverter<Ping>
            let f_ping = c.Read(&reader, typeof<Ping>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.Ping {| Xy = f_xy;Ping = f_ping; |}
        | "Surrender" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.Surrender
        | "WhisperToMaster" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("text" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<string>) :?> JsonConverter<string>
            let f_text = c.Read(&reader, typeof<string>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            Command.WhisperToMaster {| Text = f_text; |}
        | _ -> failwith $"Unexpected type: %s{str}"

/// <summary>
///  Command that happen.
/// </summary>
type PlayerCommand = {
    [<JsonPropertyName("player")>]
        Player : EntityId
    [<JsonPropertyName("command")>]
        Command : Command
}


type WhyCanNotPlayCardThere =
    | DoesNotHaveEnoughPower = 0x10
    /// <summary>
    ///  too close to (0,y), or (x,0)
    /// </summary>
    | InvalidPosition = 0x20
    | CardCondition = 0x80
    | ConditionPreventCardPlay = 0x100
    | DoesNotHaveThatCard = 0x200
    | DoesNotHaveEnoughOrbs = 0x400
    | CastingTooOften = 0x10000



/// <summary>
///  Reason why command was rejected
/// </summary>
type CommandRejectionReason =
    /// <summary>
    ///  Rejection reason for `BuildHouse`, `ProduceSquad`, and `ProduceSquadOnBarrier`
    /// </summary>
    | CardRejected of {|
          Reason : WhyCanNotPlayCardThere
          FailedCardConditions : uint32[]
        |}
    /// <summary>
    ///  Player did not have enough power to play the card or activate the ability
    /// </summary>
    | NotEnoughPower of {|
          PlayerPower : single
          Required : uint16
        |}
    /// <summary>
    ///  Spell with given ID does not exist
    /// </summary>
    | SpellDoesNotExist
    /// <summary>
    ///  The entity is not on the map
    /// </summary>
    | EntityDoesNotExist
    /// <summary>
    ///  Entity exist, but type is not correct
    /// </summary>
    | InvalidEntityType of {|
          EntityType : uint32
        |}
    /// <summary>
    ///  Rejection reason for `CastSpellEntity`
    /// </summary>
    | CanNotCast of {|
          FailedSpellConditions : uint32[]
        |}
    /// <summary>
    ///  Bot issued command for entity that is not owned by anyone
    /// </summary>
    | EntityNotOwned
    /// <summary>
    ///  Bot issued command for entity owned by someone else
    /// </summary>
    | EntityOwnedBySomeoneElse
    /// <summary>
    ///  Bot issued command for entity to change mode, but the entity does not have `ModeChange` aspect.
    /// </summary>
    | NoModeChange
    /// <summary>
    ///  Trying to change to mode, in which the entity already is.
    /// </summary>
    | EntityAlreadyInThisMode
    /// <summary>
    ///  Trying to change to moe, that the entity does not have.
    /// </summary>
    | ModeNotExist
    /// <summary>
    ///  Card index must be 0-19
    /// </summary>
    | InvalidCardIndex
    /// <summary>
    ///  Card on the given index is invalid
    /// </summary>
    | InvalidCard

type CommandRejectionReasonConverter() =
    inherit JsonConverter<CommandRejectionReason>()
    override _.Write(writer, value, options) =
        match value with
        | CardRejected f ->
            writer.WriteStartObject()
            writer.WriteStartObject("CardRejected")
            writer.WritePropertyName("reason")
            let c = options.GetConverter(typeof<WhyCanNotPlayCardThere>) :?> JsonConverter<WhyCanNotPlayCardThere>
            c.Write(writer, f.Reason, options)
            writer.WritePropertyName("failed_card_conditions")
            let c = options.GetConverter(typeof<uint32[]>) :?> JsonConverter<uint32[]>
            c.Write(writer, f.FailedCardConditions, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | NotEnoughPower f ->
            writer.WriteStartObject()
            writer.WriteStartObject("NotEnoughPower")
            writer.WritePropertyName("player_power")
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            c.Write(writer, f.PlayerPower, options)
            writer.WritePropertyName("required")
            let c = options.GetConverter(typeof<uint16>) :?> JsonConverter<uint16>
            c.Write(writer, f.Required, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | SpellDoesNotExist ->
            writer.WriteStartObject()
            writer.WriteStartObject("SpellDoesNotExist")
            writer.WriteEndObject()
        | EntityDoesNotExist ->
            writer.WriteStartObject()
            writer.WriteStartObject("EntityDoesNotExist")
            writer.WriteEndObject()
        | InvalidEntityType f ->
            writer.WriteStartObject()
            writer.WriteStartObject("InvalidEntityType")
            writer.WritePropertyName("entity_type")
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            c.Write(writer, f.EntityType, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | CanNotCast f ->
            writer.WriteStartObject()
            writer.WriteStartObject("CanNotCast")
            writer.WritePropertyName("failed_spell_conditions")
            let c = options.GetConverter(typeof<uint32[]>) :?> JsonConverter<uint32[]>
            c.Write(writer, f.FailedSpellConditions, options)
            writer.WriteEndObject()
            writer.WriteEndObject()
        | EntityNotOwned ->
            writer.WriteStartObject()
            writer.WriteStartObject("EntityNotOwned")
            writer.WriteEndObject()
        | EntityOwnedBySomeoneElse ->
            writer.WriteStartObject()
            writer.WriteStartObject("EntityOwnedBySomeoneElse")
            writer.WriteEndObject()
        | NoModeChange ->
            writer.WriteStartObject()
            writer.WriteStartObject("NoModeChange")
            writer.WriteEndObject()
        | EntityAlreadyInThisMode ->
            writer.WriteStartObject()
            writer.WriteStartObject("EntityAlreadyInThisMode")
            writer.WriteEndObject()
        | ModeNotExist ->
            writer.WriteStartObject()
            writer.WriteStartObject("ModeNotExist")
            writer.WriteEndObject()
        | InvalidCardIndex ->
            writer.WriteStartObject()
            writer.WriteStartObject("InvalidCardIndex")
            writer.WriteEndObject()
        | InvalidCard ->
            writer.WriteStartObject()
            writer.WriteStartObject("InvalidCard")
            writer.WriteEndObject()
    override _.Read(reader, _, options) =
        if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
        then failwith "Expecting an object"
        reader.Read() |> ignore
        if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
        then failwith "Expecting an name"
        let str = reader.GetString()
        reader.Read() |> ignore
        match str with
        | "CardRejected" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("reason" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<WhyCanNotPlayCardThere>) :?> JsonConverter<WhyCanNotPlayCardThere>
            let f_reason = c.Read(&reader, typeof<WhyCanNotPlayCardThere>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("failed_card_conditions" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint32[]>) :?> JsonConverter<uint32[]>
            let f_failed_card_conditions = c.Read(&reader, typeof<uint32[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            CommandRejectionReason.CardRejected {| Reason = f_reason;FailedCardConditions = f_failed_card_conditions; |}
        | "NotEnoughPower" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("player_power" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<single>) :?> JsonConverter<single>
            let f_player_power = c.Read(&reader, typeof<single>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("required" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint16>) :?> JsonConverter<uint16>
            let f_required = c.Read(&reader, typeof<uint16>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            CommandRejectionReason.NotEnoughPower {| PlayerPower = f_player_power;Required = f_required; |}
        | "SpellDoesNotExist" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            CommandRejectionReason.SpellDoesNotExist
        | "EntityDoesNotExist" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            CommandRejectionReason.EntityDoesNotExist
        | "InvalidEntityType" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("entity_type" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint32>) :?> JsonConverter<uint32>
            let f_entity_type = c.Read(&reader, typeof<uint32>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            CommandRejectionReason.InvalidEntityType {| EntityType = f_entity_type; |}
        | "CanNotCast" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.PropertyName
            then failwith "Expecting an name"
            let name = reader.GetString()
            assert("failed_spell_conditions" = name)
            reader.Read() |> ignore
            let c = options.GetConverter(typeof<uint32[]>) :?> JsonConverter<uint32[]>
            let f_failed_spell_conditions = c.Read(&reader, typeof<uint32[]>, options)
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            CommandRejectionReason.CanNotCast {| FailedSpellConditions = f_failed_spell_conditions; |}
        | "EntityNotOwned" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            CommandRejectionReason.EntityNotOwned
        | "EntityOwnedBySomeoneElse" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            CommandRejectionReason.EntityOwnedBySomeoneElse
        | "NoModeChange" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            CommandRejectionReason.NoModeChange
        | "EntityAlreadyInThisMode" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            CommandRejectionReason.EntityAlreadyInThisMode
        | "ModeNotExist" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            CommandRejectionReason.ModeNotExist
        | "InvalidCardIndex" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            CommandRejectionReason.InvalidCardIndex
        | "InvalidCard" ->
            if reader.TokenType <> System.Text.Json.JsonTokenType.StartObject
            then failwith "Expecting an object"
            reader.Read() |> ignore
            if reader.TokenType <> System.Text.Json.JsonTokenType.EndObject
            then failwith "Expecting an name"
            reader.Read() |> ignore
            CommandRejectionReason.InvalidCard
        | _ -> failwith $"Unexpected type: %s{str}"

/// <summary>
///  Command that was rejected.
/// </summary>
type RejectedCommand = {
    [<JsonPropertyName("player")>]
        Player : EntityId
    [<JsonPropertyName("reason")>]
        Reason : CommandRejectionReason
    [<JsonPropertyName("command")>]
        Command : Command
}


/// <summary>
///  Response on the `/hello` endpoint.
/// </summary>
type AiForMap = {
    /// <summary>
    ///  The unique name of the bot.
    /// </summary>
    [<JsonPropertyName("name")>]
        Name : string
    /// <summary>
    ///  List of decks this bot can use on the map.
    ///  Empty to signalize, that bot can not play on given map.
    /// </summary>
    [<JsonPropertyName("decks")>]
        Decks : Deck[]
}


type MapEntities = {
    [<JsonPropertyName("projectiles")>]
        Projectiles : Projectile[]
    [<JsonPropertyName("power_slots")>]
        PowerSlots : PowerSlot[]
    [<JsonPropertyName("token_slots")>]
        TokenSlots : TokenSlot[]
    [<JsonPropertyName("ability_world_objects")>]
        AbilityWorldObjects : AbilityWorldObject[]
    [<JsonPropertyName("squads")>]
        Squads : Squad[]
    [<JsonPropertyName("figures")>]
        Figures : Figure[]
    [<JsonPropertyName("buildings")>]
        Buildings : Building[]
    [<JsonPropertyName("barrier_sets")>]
        BarrierSets : BarrierSet[]
    [<JsonPropertyName("barrier_modules")>]
        BarrierModules : BarrierModule[]
}


/// <summary>
///  Used in `/start` endpoint.
/// </summary>
type GameStartState = {
    /// <summary>
    ///  Tells the bot which player it is supposed to control.
    ///  If bot is only spectating, this is the ID of player that it is spectating for
    /// </summary>
    [<JsonPropertyName("your_player_id")>]
        YourPlayerId : EntityId
    /// <summary>
    ///  Players in the match.
    /// </summary>
    [<JsonPropertyName("players")>]
        Players : MatchPlayer[]
    [<JsonPropertyName("entities")>]
        Entities : MapEntities
}


/// <summary>
///  Used in `/tick` endpoint, on every tick from 2 forward.
/// </summary>
type GameState = {
    /// <summary>
    ///  Time since start of the match measured in ticks.
    ///  One tick is 0.1 second = 100 milliseconds = (10 ticks per second)
    ///  Each tick is 100 ms. 1 second is 10 ticks. 1 minute is 600 ticks.
    /// </summary>
    [<JsonPropertyName("current_tick")>]
        CurrentTick : Tick
    /// <summary>
    ///  Commands that will be executed this tick.
    /// </summary>
    [<JsonPropertyName("commands")>]
        Commands : PlayerCommand[]
    /// <summary>
    ///  Commands that was rejected.
    /// </summary>
    [<JsonPropertyName("rejected_commands")>]
        RejectedCommands : RejectedCommand[]
    /// <summary>
    ///  player entities in the match
    /// </summary>
    [<JsonPropertyName("players")>]
        Players : PlayerEntity[]
    [<JsonPropertyName("entities")>]
        Entities : MapEntities
}


/// <summary>
///  Used in `/prepare` endpoint
/// </summary>
type Prepare = {
    /// <summary>
    ///  Name of deck, selected from `AiForMap` returned by `/hello` endpoint.
    /// </summary>
    [<JsonPropertyName("deck")>]
        Deck : string
    /// <summary>
    ///  Repeating `map_info` in case bot want to prepare differently based on map.
    /// </summary>
    [<JsonPropertyName("map_info")>]
        MapInfo : MapInfo
}


/// <summary>
///  Used in `/hello` endpoint
/// </summary>
type ApiHello = {
    /// <summary>
    ///  Must match the version in this file, to guarantee structures matching.
    /// </summary>
    [<JsonPropertyName("version")>]
        Version : uint64
    /// <summary>
    ///  Map about which is the game asking.
    /// </summary>
    [<JsonPropertyName("map")>]
        Map : MapInfo
}


