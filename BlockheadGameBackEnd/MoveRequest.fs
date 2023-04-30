namespace BlockheadGameBackEnd

open System.Text.Json.Serialization

type Difficulty =
    | Easy
    | Medium
    | Hard

type DifficultyTypeConverter() =
    inherit JsonConverter<Difficulty>()

    override this.Read(reader, _, _) =
        (Util.fromString<Difficulty> (reader.GetString())).Value

    override this.Write(writer, value, _) =
        writer.WriteStringValue(Util.toString value)

type MoveRequest =
    { Field: seq<string>
      UsedWords: seq<string>
      [<JsonConverter(typeof<DifficultyTypeConverter>)>]
      Difficulty: Difficulty }
