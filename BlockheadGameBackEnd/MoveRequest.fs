namespace BlockheadGameBackEnd

open System.Text.Json
open BlockheadGameBackEnd
open System.Text.Json.Serialization

type DifficultyTypeConverter() =
    inherit JsonConverter<Game.Difficulty>()

    override this.Read(reader, _, _) =
        match Util.fromString<Game.Difficulty> (reader.GetString()) with
        | Some value -> value
        | None -> raise (JsonException())

    override this.Write(writer, value, _) =
        writer.WriteStringValue(Util.toString value)

type MoveRequest =
    { Field: seq<string>
      UsedWords: seq<string>
      [<JsonConverter(typeof<DifficultyTypeConverter>)>]
      Difficulty: Game.Difficulty }
