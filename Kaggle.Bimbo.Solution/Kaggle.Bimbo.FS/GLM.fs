
namespace Kaggle.Bimbo

module GLM = 
    open Accord
    open Accord.Math
    open Common
    open Accord.Statistics.Links
    open Accord.Statistics.Models.Regression
    open Accord.Statistics.Models.Regression.Fitting

    let run (trainItems:List<PrepareData.TrainItem>) (holdOutItems:List<PrepareData.TrainItem>) =
        let input = 
            trainItems 
            |> Seq.filter(fun i -> i.AdjustedDemand < 21)
            |> Seq.map(fun i -> [|i.WeekNumber;i.SalesDepotId; i.SalesChannelId; i.SalesRouteId; i.ClientId; i.ProductId |] |> Array.map float)
            |> Seq.toArray

        let output = 
            trainItems
            |> Seq.filter(fun i -> i.AdjustedDemand < 21)
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

