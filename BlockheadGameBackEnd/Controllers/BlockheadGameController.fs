namespace BlockheadGameBackEnd.Controllers

open System.Runtime.InteropServices
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open BlockheadGameBackEnd

[<ApiController>]
[<Route("[controller]")>]
type BlockheadGameController(logger: ILogger<BlockheadGameController>) =
    inherit ControllerBase()

    let dictionary = Dictionary.readDictionary

    [<HttpGet("/field")>]
    member _.Field([<Optional; DefaultParameterValue(5)>] size: int) = Field.createNewField dictionary size

    [<HttpGet("/words")>]
    member _.Words() = Dictionary.wordsOfLength 5 dictionary

    [<HttpGet("/prefixes")>]
    member _.Prefixes() = Dictionary.toPrefixDictionary Dictionary.readDictionary
