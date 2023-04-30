module BlockheadGameBackEnd.Game

open System

type Difficulty =
    | Easy
    | Medium
    | Hard

type private Move = Field.Cell * char

type private Path = seq<Field.Cell>

type private WordPath = string * Path

let private contains x = Seq.exists ((=) x)

let private wordPickRange (difficulty: Difficulty) : int =
    match difficulty with
    | Easy -> 30
    | Medium -> 13
    | Hard -> 0

let private appendCell (field: Field.Field) ((word, path): WordPath) (x, y) : WordPath =
    (word + Char.ToString(field[x][y]), Seq.append [ (x, y) ] path)

let private alphabet = [ 'А' .. 'Е' ] @ [ 'Ё' ] @ [ 'Ж' .. 'Я' ]

let private reachableCells (field: Field.Field) (cell: Field.Cell) (visited: Set<Field.Cell>) : seq<Field.Cell> =
    let isNotVisitedCellWithLetter c =
        not (Set.contains c visited) && Field.isNotEmpty field c

    Seq.filter isNotVisitedCellWithLetter (Field.neighboursOf field cell)

let private availableMoves (field: Field.Field) : seq<Move> =
    Seq.collect (fun cell -> (Seq.map (fun letter -> (cell, letter)) alphabet)) (Field.availableCells field)

let rec private paths2 (prefixDictionary: Set<string>) (field: Field.Field) (current: Field.Cell) (visited: Set<Field.Cell>) ((word, path): WordPath): seq<WordPath> =
    if Set.contains word prefixDictionary then Seq.append [(word, path)] (Seq.collect (fun cell -> paths2 prefixDictionary field cell (Set.add cell visited) (appendCell field (word, path) cell)) (reachableCells field current visited))
    else []

let private paths (prefixDictionary: Set<string>) (field: Field.Field) (start: Field.Cell) : seq<WordPath> =
    paths2 prefixDictionary field start (Set.ofSeq [ start ]) (Char.ToString(field[fst start][snd start]), [ start ])

let private availableWords4 (prefixDictionary: Set<string>) (field: Field.Field) (updatedCell: Field.Cell): seq<WordPath> =
    Seq.filter (fun (_, path) -> contains updatedCell path) (Seq.collect (paths prefixDictionary field) (Field.notEmptyCellsOf field))

let private availableWords3 (prefixDictionary: Set<string>) (field: Field.Field) (cell: Field.Cell, letter: char): seq<string * Path * (Field.Cell * char)> =
    let fieldAfterMove = Field.replaceLetter field cell letter
    Seq.map (fun (word, path) -> (word, path, (cell, letter))) (availableWords4 prefixDictionary fieldAfterMove cell)

let availableWords2 (prefixDictionary: Set<string>) (field: Field.Field) (moves: seq<Move>) =
    Seq.distinctBy (fun (_, word, _) -> word) (Seq.collect (availableWords3 prefixDictionary field) moves)

let private availableWords (prefixDictionary: Set<string>) (field: Field.Field) =
    availableWords2 prefixDictionary field (availableMoves field)

let makeMove prefixDictionary dictionary difficulty usedWords field : bool * Field.Field * string * Path * ((int * int) * char) =
    let availableWords = availableWords prefixDictionary field
    let foundWords = Seq.filter (fun (w, _, _) -> Set.contains w dictionary && not (Seq.contains w usedWords)) availableWords
    if Seq.isEmpty foundWords then (false, field, "", [], ((0, 0), ' '))
    else
        let longestWordsFirst = Seq.toList (Seq.sortWith (fun (_, a, _) (_, b, _) -> (Seq.length b).CompareTo(Seq.length a)) foundWords)
        let random = Random()
        let wordIndex = random.Next(Math.Min((wordPickRange difficulty) + 1, Seq.length longestWordsFirst))
        let oneOfLongestWord = longestWordsFirst[wordIndex]
        let word, path, (cell, letter) = oneOfLongestWord
        let updatedField = Field.replaceLetter field cell letter
        (true, updatedField, word, path, (cell, letter))
