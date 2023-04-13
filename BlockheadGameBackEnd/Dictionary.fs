module BlockheadGameBackEnd.Dictionary

open System.IO
open System.Reflection

let private hasLength length (word: string) = word.Length = length

let private executableLocation =
    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

let readDictionary: Set<string> =
    Set.ofSeq (Seq.cast (File.ReadLines(Path.Combine(executableLocation, "dictionary.txt"))))

let private prefixes (word: string) =
    Seq.map (fun length -> word[0..length]) [ 0 .. word.Length - 1 ]

let toPrefixDictionary (dictionary: Set<string>) =
    Set.ofSeq (Seq.collect prefixes dictionary)

let wordsOfLength (n: int) (dictionary: Set<string>) : string list =
    Set.toList (Set.filter (hasLength n) dictionary)
