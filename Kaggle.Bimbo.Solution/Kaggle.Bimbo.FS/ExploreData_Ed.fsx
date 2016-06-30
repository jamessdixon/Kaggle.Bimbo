#r "../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "../packages/FSharp.Collections.ParallelSeq/lib/net40/FSharp.Collections.ParallelSeq.dll"

#r "../packages/RProvider/lib/net40/RProvider.dll"
#r "../packages/RProvider/lib/net40/RProvider.Runtime.dll"
#r "../packages/R.NET.Community/lib/net40/RDotNet.dll"


#load "../packages/FSharp.Charting/FSharp.Charting.fsx"


open System
open FSharp.Data
open FSharp.Charting
open System.Text.RegularExpressions
open RProvider

#load "PrepareData.fsx"

#time
let all = PrepareData.All
let townState = PrepareData.getTownStates all
let products = PrepareData.getProducts all
let clients = PrepareData.getClients all
let trainItems = PrepareData.getTrainItems (PrepareData.Random 0.05)
let testItems = PrepareData.getTestItems (PrepareData.Random 0.05)

///List of States:
let states =
    townState
    |> Seq.map(fun ts -> ts.StateDesc)
    |> Seq.sort
    |> Set.ofSeq

//Number of states:
// on wikipedia: https://en.wikipedia.org/wiki/Administrative_divisions_of_Mexico
// 31 + 1 Federal district for Mexico city
states |> Set.count // => 33
states |> Set.map (printfn "%s") // Queretaro in duplicated

///Number of agencia / city 
// => Try to find criterion of importance ?
let cities =
    townState
    |> Seq.take(100)
    |> List.ofSeq


//EXPLORE PRODUCTS:
//name + number of pieces + weight + BRAND + ID
products
|> Seq.take(20)
|> List.ofSeq

let weights = 
    products
    |> Seq.map(fun p -> p.Weight)
    |> Seq.choose id
    |> Array.ofSeq

//Stats
R.summary( weights ) |> R.print

//Histogram of Weights
Chart.Histogram (weights) //Few products with high weight

//Look at log of weight => Normal Distribution
let logweights = Array.map log weights
Chart.Histogram (logweights) //OK close to normal distribution

//Done by Jamie
let brandRegEx = new Regex("^.+\s(\D+) \d+$")
let brands =
    products
    |> Seq.map(fun p -> p.Brand)
    |> Set.ofSeq
    |> Set.map (printfn "%s")




// EXPLORE CUSTOMERS:

//Several duplicate IDs due to mistypping: 4862 !
clients
|> Seq.groupBy ( fun cl -> cl.ClientId)
|> Seq.filter(fun (id, grp) -> grp |> Seq.length > 1)
|> Seq.length

let multipleSpaceRegex = new Regex(" {2,}")
let removeMultipleSpaces input = multipleSpaceRegex.Replace(input, " ")

//Check if mistypping is corrected: OK
clients
|> Seq.map(fun cl -> cl.ClientId, removeMultipleSpaces cl.ClientDesc)
|> Seq.distinctBy snd
|> Seq.groupBy fst
|> Seq.filter(fun (id, grp) -> grp |> Seq.length > 1)
|> Seq.length // 0 as expected


//Examine Clients with multiple ID
// => Restaurant Chains > 100
// => 1: Non identified : unique person ?
// => 1 - 10: Family business or ?
// => 10 - 100: ?
let client_Names =
    clients
    |> Seq.map(fun cl -> cl.ClientId , removeMultipleSpaces cl.ClientDesc)
    |> Seq.countBy snd
    //Skip small clients
    |> Seq.filter(fun (name, cnt) -> cnt > 100)
    |> Seq.sortByDescending snd
    |> Seq.tail
    |> Array.ofSeq

Chart.Histogram (client_Names |> Array.map snd)

//Find Categories of customers by examining data:
clients
|> Seq.skip(100)
|> Seq.take(100)
|> Seq.map(fun cl -> cl.ClientDesc)
|> List.ofSeq
// Names - Family business
// FMAXXXXX - Vending Machine?
// ESCUELA - School
// Contains : ALIM / MINI / SUPER / BODEGA- Groceries
// Contains: Cafeteria - Vending machine
