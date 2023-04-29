namespace BlockheadGameBackEnd

open System

type MoveRequest =
    { Field: seq<string>
      UsedWords: seq<string>
      Difficulty: int }
