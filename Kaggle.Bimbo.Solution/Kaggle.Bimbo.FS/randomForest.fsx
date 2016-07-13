﻿
#load "PrepareData.fsx"
#load "CommonFunctions.fsx"

#r "../packages/alglibnet2/lib/alglibnet2.dll"
open CommonFunctions

// NOTE: ALGLIB wants prediction variable at end of input array

let runRandomForest (trainItems:List<PrepareData.TrainItem>) (holdOutItems:List<PrepareData.TrainItem>) =
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

    let makePrediction (item:PrepareData.TrainItem) =
        let x = item |> fun ti -> [|ti.WeekNumber;ti.SalesDepotId; ti.ClientId; ti.ProductId; ti.SalesRouteId |] |> Array.map float
        let mutable predictions : float[] = [||]
        alglib.dforest.dfprocess(forest,x,&predictions)
        predictions.[0]

    holdOutItems
    |> Seq.map(fun ti -> makePrediction ti)
    |> Seq.toArray


let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random 0.02)   

let holdOutItems =
    PrepareData.getTrainItems (PrepareData.Random 0.01)

let predicted = runRandomForest trainItems holdOutItems

let rmsle = 
    Seq.zip holdOutItems predicted
    |> Seq.map(fun (ho,p) -> {Simulated=p; Observed=ho.AdjustedDemand})
    |> Seq.toArray
    |> RMSLE
rmsle


//printfn "rmsError %A, oobError %A rmsle %A" report.rmserror report.oobrmserror rmsle
//2% train/1% test/050 trees/r=.1 = rmsError 16.90219728, oobError 18.82415846 rmsle 0.7196989171   03.42 to train model
//2% train/1% test/100 trees/r=.1 = rmsError 16.88868484, oobError 18.68523038 rmsle 0.7195092518   09:17 to train model
//2% train/1% test/150 trees/r=.1 = rmsError 16.79528265, oobError 18.64584115 rmsle 0.7160595963   14:22 to train model
//2% train/1% test/050 trees/r=.1/Filter demand < 100 = rmsError 6.622706999, oobError 7.356243501 rmsle 0.690260845   03:53 to train model



