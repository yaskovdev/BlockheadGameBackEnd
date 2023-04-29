module BlockheadGameBackEnd.Game

open System

type Move = Field.Cell * char

type Path = seq<Field.Cell>

type WordPath = string * Path

let private contains x = Seq.exists ((=) x)

let appendCell (field: Field.Field) ((word, path): WordPath) (x, y) : WordPath =
    (word + Char.ToString(field[x][y]), Seq.append [ (x, y) ] path)

let alphabet = [ 'А' .. 'Е' ] @ [ 'Ё' ] @ [ 'Ж' .. 'Я' ]

let mkUniq (seq: seq<Path * string * (Field.Cell * char)>) =
    let words = Set.ofSeq (Seq.map (fun (_, word, _) -> word) seq)
    Seq.filter (fun (_, word, _) -> not (Set.contains word words)) seq

let private reachableCells (field: Field.Field) (cell: Field.Cell) (visited: Set<Field.Cell>) : seq<Field.Cell> =
    let isNotVisitedCellWithLetter c =
        not (Set.contains c visited) && Field.isNotEmpty field c

    Seq.filter isNotVisitedCellWithLetter (Field.neighboursOf field cell)

let availableMoves (field: Field.Field) : seq<Move> =
    Seq.collect (fun cell -> (Seq.map (fun letter -> (cell, letter)) alphabet)) (Field.availableCells field)

let rec paths2 (prefixDictionary: Set<string>) (field: Field.Field) (current: Field.Cell) (visited: Set<Field.Cell>) ((word, path): WordPath): seq<WordPath> =
    if Set.contains word prefixDictionary then Seq.append [(word, path)] (Seq.collect (fun cell -> paths2 prefixDictionary field cell (Set.add cell visited) (appendCell field (word, path) cell)) (reachableCells field current visited))
    else []

let paths (prefixDictionary: Set<string>) (field: Field.Field) ((x, y): Field.Cell) : seq<WordPath> =
    paths2 prefixDictionary field (x, y) (Set.ofSeq [ (x, y) ]) (Char.ToString(field[x][y]), [ (x, y) ])

let availableWords4 (prefixDictionary: Set<string>) (field: Field.Field) (updatedCell: Field.Cell) =
    Seq.filter (fun (_, path) -> contains updatedCell path) (Seq.collect (paths prefixDictionary field) (Field.notEmptyCellsOf field))

let availableWords3 (prefixDictionary: Set<string>) (field: Field.Field) (cell: Field.Cell, letter: char) =
    let fieldAfterMove = Field.replaceLetter field cell letter
    Seq.map (fun (word, path) -> (path, word, (cell, letter))) (availableWords4 prefixDictionary fieldAfterMove cell)

let availableWords2 (prefixDictionary: Set<string>) (field: Field.Field) (moves: seq<Move>) =
    Seq.distinctBy (fun (_, word, _) -> word) (Seq.collect (availableWords3 prefixDictionary field) moves)

let availableWords (prefixDictionary: Set<string>) (field: Field.Field) =
    availableWords2 prefixDictionary field (availableMoves field)

let makeMove prefixDictionary dictionary difficulty usedWords field : bool * Field.Field * Path * string * ((int * int) * char) =
    let availableWords = availableWords prefixDictionary field
    let foundWords = Seq.filter (fun (_, w, _) -> Set.contains w dictionary && not (Seq.contains w usedWords)) availableWords
    if Seq.isEmpty foundWords then (false, field, [], "", ((0, 0), ' '))
    else
        let longestWordsFirst = Seq.toList (Seq.sortWith (fun (_, a, _) (_, b, _) -> (Seq.length b).CompareTo(Seq.length a)) foundWords)
        let random = Random()
        let wordIndex = random.Next(Math.Min(difficulty + 1, Seq.length longestWordsFirst))
        let oneOfLongestWord = longestWordsFirst[wordIndex]
        let path, word, (cell, letter) = oneOfLongestWord
        let updatedField = Field.replaceLetter field cell letter
        (true, updatedField, path, word, (cell, letter))
