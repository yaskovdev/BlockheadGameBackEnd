module BlockheadGameBackEnd.Field

open System

let private replaceRow field index newRow =
    List.take index field @ [ newRow ] @ List.skip (index + 1) field

let private createEmptyField size =
    [ for _ in 1..size -> [ for _ in 1..size -> '.' ] ]

let createField (initialWord: string) : char list list =
    replaceRow (createEmptyField initialWord.Length) (initialWord.Length / 2) (Seq.toList initialWord)

let createNewField dictionary size =
    let candidates = Dictionary.wordsOfLength size dictionary
    let random = Random()
    createField candidates[random.Next(candidates.Length)]

let private deltas = [ (-1, 0); (1, 0); (0, -1); (0, 1) ]

let private isOn (field: char list list) (x, y) =
    0 <= x && x < field.Length && 0 <= y && y < field[x].Length

let neighboursOf (field: char list list) (x: int, y: int) =
    List.filter (isOn field) (List.map (fun (dx, dy) -> (x + dx, y + dy)) deltas)
