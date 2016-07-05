
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

let input = 
    trainItems 
    |> Seq.map(fun i -> [|float i.SalesDepotId; float i.WeekNumber|]) 
    |> Seq.toArray

let output = 
    trainItems
    |> Seq.map(fun i -> (float)i.AdjustedDemand) 
    |> Seq.toArray

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
    |> Seq.map(fun i -> [|float i.SalesDepotId; float i.WeekNumber|]) 
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
    |> Array.map(fun (f,s) -> {Simulated=f;Observed=s})
    |> RMSLE
   

//val rmsle : float = 1.234622652