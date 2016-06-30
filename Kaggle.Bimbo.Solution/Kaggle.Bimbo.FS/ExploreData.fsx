
#load "PrepareData.fsx"

#time
let all = PrepareData.All
let townState = PrepareData.getTownStates all
let products = PrepareData.getProducts all
let clients = PrepareData.getClients all
let trainItems = PrepareData.getTrainItems (PrepareData.Random 0.05)
let testItems = PrepareData.getTestItems (PrepareData.Random 0.05)

trainItems
|> Seq.distinctBy(fun ti -> ti.WeekNumber)
|> Seq.map(fun ti -> ti.WeekNumber)
|> Seq.iter(fun w -> printfn "%i" w)
// 3,4,5,6,7,8,9

#r "../packages/FSharp.Charting/lib/net40/FSharp.Charting.dll"
open FSharp.Charting

let adjustedDemand =
    trainItems
    |> Seq.map(fun ti -> ti.AdjustedDemand)
    |> Seq.map(fun ad -> float ad)
    |> Seq.toArray

adjustedDemand |> Chart.Point

adjustedDemand
|> Array.average
//7.234130686

let sampleCount =
    adjustedDemand
    |> Array.length
    //3709817

let midpoint = sampleCount / 2
//1854908

adjustedDemand
|> Array.sort
|> Array.skip(midpoint)
|> Array.head
//3.0

let maxValue =
    adjustedDemand
    |> Array.sortDescending
    |> Array.head
// 3960

//mean is 7.24, median is 3.0 -> skewed left
//min is 0, max is 3,960

let adjustedDemandGrouping =
    trainItems
    |> Seq.groupBy(fun ti -> ti.AdjustedDemand)
    |> Seq.map(fun (f,s) -> f, s |> Seq.length)
    |> Seq.sortBy(fun (f,s) -> f)
    |> Seq.toArray

adjustedDemandGrouping
|> Chart.FastLine
