module BlockheadGameBackEnd.MoveResponse

type Dummy(success: bool) =
    member _.Success = success

type MoveResponse(cell: seq<int>, letter: string, path: seq<seq<int>>, success: bool, updatedField: seq<string>, word: string) =
    member _.Cell = cell
    member _.Letter = letter
    member _.Path = path
    member _.Success = success
    member _.UpdatedField = updatedField
    member _.Word = word
