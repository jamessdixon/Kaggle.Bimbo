
namespace Kaggle.Bimbo

module GLM = 
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
            input.[0] 
            |> Array.length

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

        predicted       

//let trainItems = 
//    PrepareData.getTrainItems (PrepareData.Random 0.02)   
//
//let holdOutItems =
//    PrepareData.getTrainItems (PrepareData.Random 0.01)
//
//let predicted = runGLM trainItems holdOutItems
//
//let testOutput =
//    holdOutItems
//    |> Seq.map(fun i ->i.AdjustedDemand) 
//    |> Seq.toArray
//
//let rmsle =
//    Array.zip predicted testOutput
//    |> Array.map(fun (f,s) -> {Simulated=f;Observed=s})
//    |> RMSLE
//rmsle
//
//
//val rmsle : float = 1.234622652