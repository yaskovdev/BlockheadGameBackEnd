module BlockheadGameBackEnd.Field

open System

type Field = char list list

let private replaceRow field index newRow =
    List.take index field @ [ newRow ] @ List.skip (index + 1) field

let private emptyCell = '.'

let private createEmptyField size =
    [ for _ in 1..size -> [ for _ in 1..size -> emptyCell ] ]

let createField (initialWord: string) : Field =
    replaceRow (createEmptyField initialWord.Length) (initialWord.Length / 2) (Seq.toList initialWord)

let createNewField dictionary size =
    let candidates = Dictionary.wordsOfLength size dictionary
    let random = Random()
    createField candidates[random.Next(candidates.Length)]

let private deltas = [ (-1, 0); (1, 0); (0, -1); (0, 1) ]

let private isEmpty (field: Field) (i, j) = field[i][j] = emptyCell

let private isNotEmpty (field: Field) cell = not (isEmpty field cell)

let private isOn (field: Field) (x, y) =
    0 <= x && x < field.Length && 0 <= y && y < field[x].Length

let neighboursOf (field: Field) (x: int, y: int) =
    List.filter (isOn field) (List.map (fun (dx, dy) -> (x + dx, y + dy)) deltas)

let private cellsOf (field: Field) =
    Seq.collect (fun i -> Seq.map (fun j -> (i, j)) [ 0 .. field[i].Length - 1 ]) [ 0 .. field.Length ]

let private hasNeighboursWithLetter (field: Field) cell =
    Seq.contains true (Seq.map (isNotEmpty field) (neighboursOf field cell))

let private isCellAvailable (field: Field) cell =
    isEmpty field cell && hasNeighboursWithLetter field cell

let getAvailableCells field =
    Seq.filter (isCellAvailable field) (cellsOf field)
