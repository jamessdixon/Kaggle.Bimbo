
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

let runGLM (trainItems:List<PrepareData.TrainItem>) (holdOutItems:List<PrepareData.TrainItem>) =
    let input = 
        trainItems 
        |> Seq.map(fun i -> [|i.WeekNumber;i.SalesDepotId; i.SalesChannelId; i.SalesRouteId; i.ClientId; i.ProductId |] |> Array.map float)
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

    //new LogitLinkFunction()
    let regression = new GeneralizedLinearRegression(new IdentityLinkFunction(), numberOfClasses)
    let teacher = new IterativeReweightedLeastSquares(regression)
    let mutable delta = 1.0
    while (delta > 0.001) do
        delta <- teacher.Run(input,output)

    let testInput = 
        holdOutItems
        |> Seq.map(fun i -> [|i.WeekNumber;i.SalesDepotId; i.SalesChannelId; i.SalesRouteId; i.ClientId; i.ProductId |] |> Array.map float) 
        |> Seq.toArray

    let predicted = 
        testInput 
        |> Seq.map(fun i -> regression.Compute(i)) 
        |> Seq.toArray

    let testOutput =
        holdOutItems
        |> Seq.map(fun i ->i.AdjustedDemand) 
        |> Seq.toArray

    let rmsle =
        Array.zip predicted testOutput
        |> Array.map(fun (f,s) -> {Simulated=f;Observed=s})
        |> RMSLE
    rmsle
       

let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random 0.02)   

let testItems =
    PrepareData.getTrainItems (PrepareData.Random 0.01)

runGLM trainItems testItems

//val rmsle : float = 1.234622652