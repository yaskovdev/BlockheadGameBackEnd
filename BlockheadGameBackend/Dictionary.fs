module BlockheadGameBackend.Dictionary

open System.IO
open System.Reflection

let executableLocation =
    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

let readDictionary: Set<string> =
    Set.ofSeq (Seq.cast (File.ReadLines(Path.Combine(executableLocation, "dictionary.txt"))))

let wordsOfLength (n: int) (dictionary: Set<string>) =
    Set.filter (fun (word: string) -> word.Length = n) dictionary
