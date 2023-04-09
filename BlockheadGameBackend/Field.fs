module BlockheadGameBackend.Field

let replaceRow field index newRow =
    List.take index field @ [ newRow ] @ List.skip (index + 1) field

let createEmptyField size =
    [ for _ in 1..size -> [ for _ in 1..size -> '.' ] ]

let createField (initialWord: char list) : char list list =
    replaceRow (createEmptyField initialWord.Length) (initialWord.Length / 2) initialWord

let neighboursOf (field: char list list) (x: int, y: int) =
    let neighbours = [ (x - 1, y); (x + 1, y); (x, y - 1); (x, y + 1) ]
    List.filter (fun (x: int, y: int) -> 0 <= x && x < field.Length && 0 <= y && y < field[x].Length) neighbours
