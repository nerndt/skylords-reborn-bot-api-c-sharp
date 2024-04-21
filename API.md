# SR Bot API description:
Each bot needs to provide 4 endpoint accessible trough http.
For full description check out: [Rust API.md](https://gitlab.com/skylords-reborn/skylords-reborn-bot-api-rust/-/blob/main/API.md?ref_type=heads)

## AspWrapper
This **C#** wrapper implements the boilerplate for the API and simplifies it to implementation of ``IAspWrapperImpl`` interface.
Users that decide to use this wrapper to not need to implement any part of HTTP communication themselves.


#### POST ``hello`` endpoint
- ``DecksForMap`` function call with ``MapInfo`` as parameter, and ``DeckAPI []`` as return type while also ensuring the name to be an ``null`` when not relevant.
- it also sources the ``Name`` from the implementation's property.

#### POST ``prepare`` endpoint
- ``DecksForMap`` function call with ``MapInfo`` as parameter as with the ``hello`` endpoint.
- It finds the requested deck, and calls ``PrepareForBattle`` with map, and deck information.

#### POST ``start`` endpoint
- ``MatchStart`` function call with the state.

#### POST ``tick`` endpoint
- ``Tick`` function call with the state.
- Later it might provide some local validation on commands, so you can get better understanding, instead of game just ignoring it. 		
    I plan to have this validation kind customizable (optional) to ease up development, while not costing any performance during real matches.

#### GET ``end`` endpoint
- does nothing
