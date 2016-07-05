

#load "PrepareData.fsx"
#load "CommonFunctions.fsx"

#r "../packages/Accord/lib/net40/Accord.dll"
#r "../packages/Accord.Math/lib/net40/Accord.Math.dll"
#r "../packages/Accord.Statistics/lib/net40/Accord.Statistics.dll"

open Accord
open Accord.Math
open CommonFunctions
open Accord.Statistics.Links
open Accord.Statistics.Models.Regression
open Accord.Statistics.Models.Regression.Fitting

#time
let trainItems = PrepareData.getTrainItems (PrepareData.Random 0.01)

let startWeek = 
    trainItems 
    |>Seq.sortBy(fun ti -> ti.WeekNumber)
    |>Seq.head
    |>fun ti -> ti.WeekNumber

let endWeek = 
    trainItems 
    |>Seq.sortByDescending(fun ti -> ti.WeekNumber)
    |>Seq.head
    |>fun ti -> ti.WeekNumber

let arrayLength = endWeek - startWeek

let createArray index =
    let (x:int array) = Array.zeroCreate (arrayLength + 1)
    x.[index - startWeek] <- 1
    x
   
let weekNumbers = 
    trainItems 
    |> Seq.map(fun i -> createArray i.WeekNumber) 
    |> Seq.toArray

let input = 
    trainItems 
    |> Seq.map(fun i -> createArray i.WeekNumber |> Array.append [|i.SalesDepotId|])
    |> Seq.map(fun a -> a |> Array.map float) 
    |> Seq.toArray

let output = 
    trainItems
    |> Seq.map(fun i -> i.AdjustedDemand) 
    |> Seq.toArray
    |> Array.map float

let numberOfClasses = 
    input 
    |> Array.head 
    |> Array.length

let regression = new GeneralizedLinearRegression(new ProbitLinkFunction(), numberOfClasses)
let teacher = new IterativeReweightedLeastSquares(regression)
let mutable delta = 1.0
while (delta > 0.01) do
    delta <- teacher.Run(input,output)

let testItems = PrepareData.getTrainItems (PrepareData.Random 0.01)

let testInput = 
    testItems 
    |> Seq.map(fun i -> createArray i.WeekNumber |> Array.append [|i.SalesDepotId|])
    |> Seq.map(fun a -> a |> Array.map float) 
    |> Seq.toArray

let predicted = 
    testInput 
    |> Seq.map(fun i -> regression.Compute(i)) 
    |> Seq.toArray

let testOutput =
    testItems
    |> Seq.map(fun i ->i.AdjustedDemand) 
    |> Seq.toArray

let rmsle =
    Array.zip predicted testOutput
    |> Array.map(fun (f,s) -> {Simula ted=f;Observed=s})
    |> RMSLE
   
//val rmsle : float = 1.23417018