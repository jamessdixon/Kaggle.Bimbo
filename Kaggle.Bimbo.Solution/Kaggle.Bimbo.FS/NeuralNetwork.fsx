
#load "PrepareData.fsx"
#load "CommonFunctions.fsx"

#r "../packages/Accord/lib/net40/Accord.dll"
#r "../packages/Accord.Math/lib/net40/Accord.Math.dll"
#r "../packages/Accord.Neuro/lib/net40/Accord.Neuro.dll"
#r "../packages/Accord.Statistics/lib/net40/Accord.Statistics.dll"
#r "../packages/Accord.MachineLearning/lib/net40/Accord.MachineLearning.dll"


open Accord
open Accord.Math
open AForge.Neuro
open Accord.Neuro
open CommonFunctions
open Accord.Neuro.Learning
open Accord.Statistics.Distributions
open Accord.Statistics.Distributions.Fitting
open Accord.Statistics.Distributions.Univariate

let runNeuralNetwork (trainItems:List<PrepareData.TrainItem>) (holdOutItems:List<PrepareData.TrainItem>) =

    let input = 
        trainItems 
        |> Seq.map(fun i -> [|i.WeekNumber; i.SalesDepotId; i.SalesChannelId; i.SalesRouteId; i.ClientId; i.ProductId |] |> Array.map float)
        |> Seq.toArray

    let output = 
        trainItems
        |> Seq.map(fun ti -> ti.AdjustedDemand)
        |> Seq.map(fun d -> if d > 20 then 21 else d)
        |> Seq.map(fun d -> if d < 0 then 0 else d)
        |> Seq.toArray

    let outputs = Accord.Statistics.Tools.Expand(output)
    let sigmoidFunction = new BipolarSigmoidFunction()
    let network = new ActivationNetwork(sigmoidFunction, 6, 5, 3)
    let widrow = new NguyenWidrow(network)
    widrow.Randomize()
    let teacher = new ParallelResilientBackpropagationLearning(network)

    //let mutable error = 1.0
    //while (error > 0.01) do
    //    error <- teacher.RunEpoch(input, outputs)
    [0..10]
    |> Seq.map(fun _ -> teacher.RunEpoch(input, outputs))
    |> ignore

    let makePrediction (item:PrepareData.TrainItem) =
        let x = item |> fun ti -> [|ti.WeekNumber; ti.SalesDepotId; ti.SalesChannelId; ti.SalesRouteId; ti.ClientId; ti.ProductId |] |> Array.map float
        network.Compute(x)
        |> Array.head

    holdOutItems
    |> Seq.map(fun hoi -> makePrediction hoi)
    |> Seq.toArray

let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random 0.02)   

let holdOutItems =
    PrepareData.getTrainItems (PrepareData.Random 0.01)

let predicted = runNeuralNetwork trainItems holdOutItems

let rmsle = 
    Seq.zip predicted holdOutItems
    |> Seq.map(fun (p,hoi) -> {Simulated=p; Observed=hoi.AdjustedDemand})
    |> Seq.toArray
    |> RMSLE
rmsle





