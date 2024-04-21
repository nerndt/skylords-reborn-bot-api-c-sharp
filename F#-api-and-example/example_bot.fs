module F__api_and_example.example_bot

open F__api_and_example.Controllers
open Types
open Helpers

type CardTemplate = Api.CardTemplate

// /AI: add F#_Example Tutorial 4

type State = {
    MyId: EntityId
    MyTeam: uint8
    Opponents: EntityId list
    MyStart: Position2D
}

let mutable Me : State voption = ValueNone

let spawnUnit (pos: Position2D) (my_power: single) =
    if my_power >= 50.f then
        ValueSome (ProduceSquad {| CardPosition = uint8(0); Xy = pos |})
    else
        ValueNone

let tick (state: GameState) : Command[] =
    let me = ValueOption.get Me
    let my_power = (state.Players |> Array.find(fun p -> p.Id = me.MyId)).Power
    let my_army =
        state.Entities.Squads
        |> Array.filter(fun s -> s.Entity.PlayerEntityId = ValueSome me.MyId)
        |> Array.map(fun s -> s.Entity.Id)
    let target =
        state.Entities.TokenSlots
        |> Array.tryFind(fun s -> 
            match s.Entity.PlayerEntityId with
            | ValueNone -> false
            | ValueSome id -> me.Opponents |> List.contains id )
            
    printfn $"%A{state.CurrentTick} target: %A{target.IsSome} my power: %A{my_power} my army: %A{my_army.Length}"
    let (spawn, attack) = 
        match my_army.Length, target with
        | 0, _ -> (spawnUnit me.MyStart my_power), ValueNone
        | _, None ->
            // I will win soon, right?
            ValueNone, ValueNone
        | _, Some target ->
            let attack =
                GroupAttack {|
                    Squads = my_army
                    TargetEntityId = target.Entity.Id
                    ForceAttack = false
                |}
            (spawnUnit me.MyStart my_power), ValueSome attack
            
    printfn $"%A{state.CurrentTick} spawn: %A{spawn.IsSome} attack: %A{attack.IsSome}"
    match spawn, attack with
        | ValueNone, ValueNone -> [||]
        | ValueSome(spawn), ValueNone -> [| spawn |]
        | ValueNone, ValueSome(attack) -> [| attack |]
        | ValueSome(spawn), ValueSome(attack) -> [| spawn; attack |]

let start (state: GameStartState) =
    let myTeam = (state.Players |> Array.find(fun p -> p.Entity.Id = state.YourPlayerId)).Entity.Team
    
    printfn $"my player ID is: %A{state.YourPlayerId}, and I own:"
    for ps in state.Entities.PowerSlots do
        if ps.Entity.PlayerEntityId = ValueSome state.YourPlayerId
        then printfn $"power slot: %A{ps.Entity.Id} at %A{ps.Entity.Position}"
    for ts in state.Entities.TokenSlots do
        if ts.Entity.PlayerEntityId = ValueSome state.YourPlayerId
        then printfn $"token slot: %A{ts.Entity.Id} at %A{ts.Entity.Position}"

    Me <- ValueSome {
        MyId = state.YourPlayerId
        MyTeam = myTeam
        Opponents =
            state.Players
            |> Seq.filter(fun p -> p.Entity.Team <> myTeam)
            |> Seq.map(fun p -> p.Entity.Id)
            |> Seq.toList
        MyStart =
            state.Entities.TokenSlots
            |> Seq.find(fun ts -> ts.Entity.PlayerEntityId = ValueSome state.YourPlayerId)
            |> fun e -> Position2D.from3D e.Entity.Position
    }

let deck : Deck = {
    Name = "Tutorial"
    CoverCardIndex = uint8(3)
    Cards = [|
        CardId.New CardTemplate.MasterArchers Upgrade.U3
        CardId.New CardTemplate.Northguards Upgrade.U3
        CardId.New CardTemplate.Eruption Upgrade.U3
        CardId.New CardTemplate.CannonTower Upgrade.U3
        CardId.New CardTemplate.FireStalker Upgrade.U3
        CardId.New CardTemplate.MagmaHurler Upgrade.U3
        CardId.New CardTemplate.Tremor Upgrade.U3
        CardId.None
        CardId.None
        CardId.None
        CardId.None
        CardId.None
        CardId.None
        CardId.None
        CardId.None
        CardId.None
        CardId.None
        CardId.None
        CardId.None
        CardId.None
    |]
}

let decks_for_map (map: MapInfo) =
    if map.CommunityMapDetails.IsSome
    then [||]
    else [| deck |]


let Impl = {
    Name = "F#_Example"
    DecksForMap = decks_for_map
    Prepare = fun _ -> ()
    Start = start
    Tick = tick
}