
namespace Kaggle.Bimbo

module RandomForest = 
    open Common
    open PrepareData

    let run (trainItems:TrainItem list) (holdOutItems:TrainItem list) =
        let xy =
            trainItems 
            |> Seq.map(fun ti -> [|ti.WeekNumber;ti.SalesDepotId; ti.ClientId; ti.ProductId; ti.SalesRouteId; ti.AdjustedDemand|] |> Array.map float)   
            |> array2D
   
        let numberOfPoints = trainItems |> Seq.length
        let numberOfVariables = 5
        let numberOfClasses = 1;
        let numberOfTrees = 50
        let r = 0.1
        let mutable info = 0
        let forest = alglib.dforest.decisionforest()
        let report = alglib.dforest.dfreport()

        alglib.dforest.dfbuildrandomdecisionforest(xy, numberOfPoints, numberOfVariables, numberOfClasses,numberOfTrees,r,&info,forest,report)

        let makePrediction (item:TrainItem) =
            let x = item |> fun ti -> [|ti.WeekNumber;ti.SalesDepotId; ti.ClientId; ti.ProductId; ti.SalesRouteId |] |> Array.map float
            let mutable predictions : float[] = [||]
            alglib.dforest.dfprocess(forest,x,&predictions)
            predictions.[0]

        holdOutItems
        |> Seq.map(fun ti -> makePrediction ti)
        |> Seq.toArray




