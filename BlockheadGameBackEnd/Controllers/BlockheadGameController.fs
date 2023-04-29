namespace BlockheadGameBackEnd.Controllers

open System.Runtime.InteropServices
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open BlockheadGameBackEnd


[<ApiController>]
type BlockheadGameController(logger: ILogger<BlockheadGameController>) =
    inherit ControllerBase()

    let mapField (field: seq<string>) =
        Seq.toList (Seq.map (fun row -> Seq.toList row) field)

    let dictionary = Dictionary.readDictionary

    let prefixDictionary = Dictionary.toPrefixDictionary dictionary

    [<HttpGet("/api/field")>]
    member _.Field([<Optional; DefaultParameterValue(5)>] size: int) = Field.createNewField dictionary size

    [<HttpGet("/api/words")>]
    member _.Words() = Dictionary.wordsOfLength 5 dictionary

    [<HttpGet("/api/prefixes")>]
    member _.Prefixes() = prefixDictionary

    [<HttpGet("/api/alphabet")>]
    member _.Alphabet() = Game.alphabet

    [<HttpPost("/api/move-requests")>]
    member _.MakeMove(request: MoveRequest) =
        Game.makeMove prefixDictionary dictionary request.Difficulty request.UsedWords (mapField request.Field)
