module BlockheadGameBackEnd.Game

open System

type Difficulty =
    | Easy
    | Medium
    | Hard

type private Move = Field.Cell * char

type private Path = seq<Field.Cell>

let private contains x = Seq.exists ((=) x)

let private wordInPath (field: Field.Field) (path: Path) =
    String(Seq.toArray (Seq.map (fun (x, y) -> if field[x][y] = '.' then raise (Exception("GOVNO")) else field[x][y]) path))

let private wordPickRange (difficulty: Difficulty) : int =
    match difficulty with
    | Easy -> 30
    | Medium -> 13
    | Hard -> 0

let private alphabet = [ 'А' .. 'Е' ] @ [ 'Ё' ] @ [ 'Ж' .. 'Я' ]

let private reachableCells (field: Field.Field) (cell: Field.Cell) (visited: Set<Field.Cell>) : seq<Field.Cell> =
    let isNotVisitedCellWithLetter c =
        not (Set.contains c visited) && Field.isNotEmpty field c

    Seq.filter isNotVisitedCellWithLetter (Field.neighboursOf field cell)

let private availableMoves (field: Field.Field) : seq<Move> =
    Seq.collect (fun cell -> (Seq.map (fun letter -> (cell, letter)) alphabet)) (Field.availableCells field)

let rec private paths2 (prefixDictionary: Set<string>) (field: Field.Field) (current: Field.Cell) (visited: Set<Field.Cell>) (path: Path): seq<Path> =
    let word = (wordInPath field path)
    if Set.contains word prefixDictionary then Seq.append [path] (Seq.collect (fun cell -> paths2 prefixDictionary field cell (Set.add cell visited) (Seq.append path [ cell ])) (reachableCells field current visited))
    else []

let private paths (prefixDictionary: Set<string>) (field: Field.Field) (start: Field.Cell) : seq<Path> =
    paths2 prefixDictionary field start (Set.ofSeq [ start ]) [ start ]

let private availableWords4 (prefixDictionary: Set<string>) (field: Field.Field) (updatedCell: Field.Cell): seq<Path> =
    Seq.filter (contains updatedCell) (Seq.collect (paths prefixDictionary field) (Field.notEmptyCellsOf field))

let private availableWords3 (prefixDictionary: Set<string>) (field: Field.Field) (cell: Field.Cell, letter: char): seq<Path * (Field.Cell * char)> =
    let fieldAfterMove = Field.replaceLetter field cell letter
    Seq.map (fun path -> (path, (cell, letter))) (availableWords4 prefixDictionary fieldAfterMove cell)

let private availableWords2 (prefixDictionary: Set<string>) (field: Field.Field) (moves: seq<Move>) =
    Seq.distinctBy (fun (path, (cell, letter)) -> wordInPath (Field.replaceLetter field cell letter) path) (Seq.collect (availableWords3 prefixDictionary field) moves)

let private availableWords (prefixDictionary: Set<string>) (field: Field.Field) =
    availableWords2 prefixDictionary field (availableMoves field)

let makeMove prefixDictionary dictionary difficulty usedWords field : bool * Field.Field * string * Path * ((int * int) * char) =
    let availableWords = availableWords prefixDictionary field
    let longWords = Seq.filter (fun (path, _) -> Seq.length path > 3) availableWords
    let longestWordsFirst = Seq.toList (Seq.sortWith (fun (a, _) (b, _) -> (Seq.length b).CompareTo(Seq.length a)) availableWords)
    let foundWords = Seq.filter (fun (path, (cell, letter)) -> Set.contains (wordInPath (Field.replaceLetter field cell letter) path) dictionary && not (Seq.contains (wordInPath (Field.replaceLetter field cell letter) path) usedWords)) availableWords
    if Seq.isEmpty foundWords then (false, field, "", [], ((0, 0), ' '))
    else
        let longestWordsFirst = Seq.toList (Seq.sortWith (fun (a, _) (b, _) -> (Seq.length b).CompareTo(Seq.length a)) foundWords)
        let random = Random()
        let wordIndex = random.Next(Math.Min((wordPickRange difficulty) + 1, Seq.length longestWordsFirst))
        let oneOfLongestWord = longestWordsFirst[wordIndex]
        let path, (cell, letter) = oneOfLongestWord
        let updatedField = Field.replaceLetter field cell letter
        (true, updatedField, (wordInPath updatedField path), path, (cell, letter))
