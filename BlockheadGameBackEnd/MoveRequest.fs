namespace BlockheadGameBackEnd

open System.ComponentModel

type DifficultyTypeConverter() =
    inherit TypeConverter()

    override this.CanConvertFrom(context, sourceType) = true
    override this.CanConvertTo(context, destinationType) = true

    override this.ConvertFrom(context, culture, value) =
        match value with
        | :? string as res -> Util.fromString res
        | _ -> base.ConvertFrom(context, culture, value)

    override this.ConvertTo(context, culture, value, destinationType) = Util.toString value

type [<TypeConverter(typeof<DifficultyTypeConverter>)>] Difficulty =
    | Easy
    | Medium
    | Hard

type MoveRequest =
    { Field: seq<string>
      UsedWords: seq<string>
      Difficulty: Difficulty }
