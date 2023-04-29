namespace BlockheadGameBackEnd

type MoveRequest =
    { Field: seq<string>
      UsedWords: seq<string>
      Difficulty: int }
