
#load "PrepareData.fsx"

#time
let all = PrepareData.All
let townState = PrepareData.getTownStates all
let products = PrepareData.getProducts all
let clients = PrepareData.getClients all
let trainItems = PrepareData.getTrainItems (PrepareData.Random 0.05)
let testItems = PrepareData.getTestItems (PrepareData.Random 0.05)

trainItems |> Seq.length

trainItems
|> Seq.distinctBy(fun ti -> ti.WeekNumber)
|> Seq.map(fun ti -> ti.WeekNumber)
|> Seq.iter(fun w -> printfn "%i" w)
// 3,4,5,6,7,8,9

#r "../packages/RProvider/lib/net40/RProvider.dll"
#r "../packages/RProvider/lib/net40/RProvider.Runtime.dll"
#r "../packages/R.NET.Community/lib/net40/RDotNet.dll"
#r "../packages/R.NET.Community.FSharp/lib/net40/RDotNet.FSharp.dll"

open RProvider
open RProvider.graphics

let adjustedDemand =
    trainItems
    |> Seq.map(fun ti -> (float) ti.AdjustedDemand)
    |> Seq.toArray

R.summary( adjustedDemand ) |> R.print
R.plot (adjustedDemand)

let demandGroup =
    trainItems
    |> Seq.countBy(fun ti -> ti.AdjustedDemand )
    |> Seq.sortBy(fun (x,y) -> x)
    |> Seq.toArray

let x = demandGroup |> Seq.map(fun (x,y) -> x) |> Seq.toArray
let y = demandGroup |> Seq.map(fun (x,y) -> log((float)y)) |> Seq.toArray

//let dataFrame = 
//  [ "demand", x;
//    "count", y ]
//
//namedParams dataFrame
//|> R.data_frame
//|> R.plot

let outliers = 
    demandGroup
    |> Seq.filter(fun (x,y) -> x >= 10)
    |> Seq.sumBy(fun (x,y) -> y)
//652902

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

//number of Sales Depots -- 552
trainItems
|> Seq.distinctBy(fun ti -> ti.SalesDepotId)
|> Seq.length

//number of Sales Channels -- 9
trainItems
|> Seq.distinctBy(fun ti -> ti.SalesChannelId)
|> Seq.length

trainItems
|> Seq.distinctBy(fun ti -> ti.SalesChannelId)
|> Seq.map(fun ti -> ti.SalesChannelId)
|> Seq.sort
|> Seq.iter(fun i -> printfn "%i" i)

//number of routes --2578
trainItems
|> Seq.distinctBy(fun ti -> ti.SalesRouteId)
|> Seq.length

//number of clients - 694,716
trainItems
|> Seq.distinctBy(fun ti -> ti.ClientId)
|> Seq.length

//number of products - 1484
trainItems
|> Seq.distinctBy(fun ti -> ti.ProductId)
|> Seq.length


//crosstab channel with demand
//trainItems
//|> Seq.countBy(fun ti -> (float) ti.AdjustedDemand)
//|> Seq.sortBy(fun (x,y) -> x)
//
//let x = adjustedDemand' |> Seq.map(fun (x,y) -> x) |> Seq.toArray
//let y = adjustedDemand' |> Seq.map(fun (x,y) -> log((float)y)) |> Seq.toArray
//
//
//let dataFrame = 
//  [ "demand", x;
//    "count", y ]
//
//namedParams dataFrame
//|> R.data_frame
//|> R.plot

let salesChannel = trainItems |> Seq.map(fun ti -> (float) ti.SalesChannelId)
let weekNumber = trainItems |> Seq.map(fun ti -> (float) ti.WeekNumber)
let adjustedDemand'' = trainItems |> Seq.map(fun ti -> (float) ti.AdjustedDemand)

let df = ["salesChannel",salesChannel;
          "weekNumber",weekNumber;
          "demand",adjustedDemand'']

namedParams df
|> R.data_frame
|> R.plot

let trainItems' = PrepareData.getTrainItems (PrepareData.Top 100)
trainItems'.[0]
trainItems'.[1]

trainItems 
|> Seq.filter(fun ti -> ti.ReturnsUnitNextWeek > 0)

//Hypothesis
//direct delivery sales employees have different biases
//a way to figure out that employee made the estimation
//Sales Route?

//Certain types of products are harder to estimate
//If not seen before, take average?




