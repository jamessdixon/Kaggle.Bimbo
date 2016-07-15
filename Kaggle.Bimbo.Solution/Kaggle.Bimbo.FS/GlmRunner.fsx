
#r "../packages/Accord/lib/net40/Accord.dll" 
#r "../packages/Accord.Math/lib/net40/Accord.Math.dll" 
#r "../packages/Accord.Statistics/lib/net40/Accord.Statistics.dll" 
#r "../packages/Accord.MachineLearning/lib/net40/Accord.MachineLearning.dll" 

#load "Common.fs"
#load "PrepareData.fs"
#load "GLM.fs"

open Kaggle.Bimbo

let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random {Percent=0.02;SeedValue=86})   

let holdOutItems =
    PrepareData.getTrainItems (PrepareData.Random {Percent=0.01;SeedValue=79})

let predicted = GLM.run trainItems holdOutItems

let testOutput =
    holdOutItems
    |> Seq.map(fun i ->i.AdjustedDemand) 
    |> Seq.toArray

let rmsle =
    Array.zip predicted testOutput
    |> Array.map(fun (f,s) -> {Common.Simulated=f;Common.Observed=s})
    |> Common.RMSLE


//base GLM .94
//filter < 21 .82
