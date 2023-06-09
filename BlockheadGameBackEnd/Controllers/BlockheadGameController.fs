﻿namespace BlockheadGameBackEnd.Controllers

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

    let toRequestField = Seq.map (fun row -> String(List.toArray row))

    let dictionary = Dictionary.readDictionary

    let prefixDictionary = Dictionary.toPrefixDictionary dictionary

    [<HttpGet("/api/field")>]
    member _.Field([<Optional; DefaultParameterValue(5)>] size: int) =
        toRequestField (Field.createNewField dictionary size)

    [<HttpPost("/api/move-requests")>]
    member _.MakeMove(request: MoveRequest) =
        logger.LogInformation("Difficulty is " + request.Difficulty.ToString())
        let success, updatedField, word, path, (cell, letter) =
            Game.makeMove prefixDictionary dictionary request.Difficulty request.UsedWords (fromRequestField request.Field)

        MoveResponse([ fst cell; snd cell ], Char.ToString(letter), Seq.map (fun (x, y) -> [ x; y ]) path, success, toRequestField updatedField, word)
