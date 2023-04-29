namespace BlockheadGameBackEnd.Controllers

open System
open System.Runtime.InteropServices
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open BlockheadGameBackEnd
open BlockheadGameBackEnd.MoveResponse


[<ApiController>]
type BlockheadGameController(logger: ILogger<BlockheadGameController>) =
    inherit ControllerBase()

    let fromRequestField (field: seq<string>) = Seq.toList (Seq.map Seq.toList field)

    let toRequestField field =
        Seq.map (fun row -> String(List.toArray row)) field

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
        let success, updatedField, path, word, (cell, letter) =
            Game.makeMove prefixDictionary dictionary request.Difficulty request.UsedWords (fromRequestField request.Field)

        MoveResponse([ fst cell; snd cell ], Char.ToString(letter), Seq.map (fun (x, y) -> [ x; y ]) path, success, toRequestField updatedField, word)
