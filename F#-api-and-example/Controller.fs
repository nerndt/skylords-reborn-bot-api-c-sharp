namespace F__api_and_example.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open F__api_and_example.Types

type BotImpl = {
    Name: string
    DecksForMap: MapInfo -> Deck[]
    Prepare: Prepare -> unit
    Start: GameStartState -> unit
    Tick: GameState -> Command[]
}


[<ApiController>]
[<Route("/hello")>]
type Hello (logger : ILogger<Hello>, bot_impl: BotImpl) =
    inherit ControllerBase()


    [<HttpPost>]
    member _.Post(hello: ApiHello) : AiForMap =
        logger.LogInformation($"hello: %A{hello}")
        let response ={
                Name = bot_impl.Name
                Decks = bot_impl.DecksForMap hello.Map
            }
        logger.LogInformation($"hello: %A{response}")
        response


[<ApiController>]
[<Route("/prepare")>]
type PrepareControler (logger : ILogger<Hello>, bot_impl: BotImpl) =
    inherit ControllerBase()


    [<HttpPost>]
    member _.Post(prepare: Prepare) =
        logger.LogInformation($"prepare: %A{prepare}")
        bot_impl.Prepare prepare


[<ApiController>]
[<Route("/start")>]
type Start (logger : ILogger<Hello>, bot_impl: BotImpl) =
    inherit ControllerBase()


    [<HttpPost>]
    member _.Post(start: GameStartState) =
        //logger.LogInformation($"start: %A{start}")
        bot_impl.Start start


[<ApiController>]
[<Route("/tick")>]
type Tick (logger : ILogger<Hello>, bot_impl: BotImpl) =
    inherit ControllerBase()


    [<HttpPost>]
    member _.Post(tick: GameState) =
        //logger.LogInformation($"tick: %A{tick}")
        bot_impl.Tick tick

