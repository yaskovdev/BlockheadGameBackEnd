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
    String(Seq.toArray (Seq.map (fun (x, y) -> field[x][y]) path))

let private wordPickRange =
    function
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

let rec private paths (prefixDictionary: Set<string>) (field: Field.Field) (current: Field.Cell) (visited: Set<Field.Cell>) (path: Path): seq<Path> =
    if Set.contains (wordInPath field path) prefixDictionary then Seq.append [path] (Seq.collect (fun cell -> paths prefixDictionary field cell (Set.add cell visited) (Seq.append path [ cell ])) (reachableCells field current visited))
    else []

let private availableWords (prefixDictionary: Set<string>) (field: Field.Field) (cell: Field.Cell, letter: char): seq<Path * Move> =
    let fieldAfterMove = Field.replaceLetter field cell letter
    Seq.map (fun path -> (path, (cell, letter))) (Seq.filter (contains cell) (Seq.collect (fun start -> paths prefixDictionary fieldAfterMove start (Set.singleton start) (Seq.singleton start)) (Field.notEmptyCellsOf fieldAfterMove)))

let private isUnusedAvailableWord dictionary usedWords field cell letter path =
    let updatedField = Field.replaceLetter field cell letter
    let word = wordInPath updatedField path
    Set.contains word dictionary && not (Seq.contains word usedWords)

let private realUnusedAvailableWords (prefixDictionary: Set<string>) (dictionary: Set<string>) (usedWords: seq<string>) (field: Field.Field) =
    let availableWords = Seq.distinctBy fst (Seq.collect (availableWords prefixDictionary field) (availableMoves field))
    Seq.filter (fun (path, (cell, letter)) -> isUnusedAvailableWord dictionary usedWords field cell letter path) availableWords

let makeMove prefixDictionary dictionary difficulty usedWords field : bool * Field.Field * string * Path * ((int * int) * char) =
    let foundWords = realUnusedAvailableWords prefixDictionary dictionary usedWords field
    if Seq.isEmpty foundWords then (false, field, "", [], ((0, 0), ' '))
    else
        let longestWordsFirst = Seq.toList (Seq.sortWith (fun (a, _) (b, _) -> (Seq.length b).CompareTo(Seq.length a)) foundWords)
        let random = Random()
        let wordIndex = random.Next(Math.Min((wordPickRange difficulty) + 1, Seq.length longestWordsFirst))
        let oneOfLongestWord = longestWordsFirst[wordIndex]
        let path, (cell, letter) = oneOfLongestWord
        let updatedField = Field.replaceLetter field cell letter
        (true, updatedField, (wordInPath updatedField path), path, (cell, letter))
