module F__api_and_example.Helpers

open Types

type CardTemplate = Api.CardTemplate

module Position2D =
    let Zero : Position2D = { X = 0.f; Y = 0.f }
    let from3D (position: Position) =
        { X = position.X; Y = position.Z }

module CardId =
    let New (ct: CardTemplate) (u: Upgrade) =
        CardId(uint32(ct) + uint32(u))
    let None  =
        CardId(0u)