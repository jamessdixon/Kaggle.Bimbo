
#r "../packages/alglibnet2/lib/alglibnet2.dll"
#r "../packages/Accord/lib/net40/Accord.dll" 
#r "../packages/Accord.Math/lib/net40/Accord.Math.dll" 
#r "../packages/Accord.Statistics/lib/net40/Accord.Statistics.dll" 
#r "../packages/Accord.MachineLearning/lib/net40/Accord.MachineLearning.dll" 

#load "Common.fs"
#load "PrepareData.fs"
#load "RandomForest.fs"
#load "GLM.fs"

open Kaggle.Bimbo

#time
let randomForestTrainItems = 
    PrepareData.getTrainItems (PrepareData.Random {Percent=0.02;SeedValue=86})   

let glmTrainItems = 
    PrepareData.getTrainItems (PrepareData.Random {Percent=0.02;SeedValue=75})   

let holdOutItems =
    PrepareData.getTrainItems (PrepareData.Random {Percent=0.01;SeedValue=30})

let randomForest = RandomForest.run randomForestTrainItems holdOutItems
let glm = GLM.run glmTrainItems holdOutItems

let ensambles =
    holdOutItems
    |> Seq.mapi(fun i hoi -> 
            {Common.LimitedEnsamble.Observed=hoi.AdjustedDemand;
             Common.LimitedEnsamble.RandomForest=randomForest.[i];
             Common.LimitedEnsamble.GLM=glm.[i]})
    |> Seq.toArray

let totalCount = ensambles |> Seq.length //742114

let foo = 
    ensambles
    |> Seq.groupBy(fun e -> abs((int e.RandomForest) - e.Observed))
    |> Seq.map(fun (v,a) -> v, a |> Seq.length)
    |> Seq.sortBy(fun (k,v) -> k)
    |> Seq.take(20)

let bar = 
    ensambles
    |> Seq.groupBy(fun e -> abs((int e.GLM) - e.Observed))
    |> Seq.map(fun (v,a) -> v, a |> Seq.length)
    |> Seq.sortBy(fun (k,v) -> k)
    |> Seq.take(20)

Seq.zip foo bar
|> Seq.iter(fun (f,s) -> printfn "%A , %A , %A" (fst f) (snd f) (snd s))

Seq.zip foo bar
|> Seq.iter(fun (f,s) -> printfn "%A" (fst f))

Seq.zip foo bar
|> Seq.iter(fun (f,s) -> printfn "%A" (snd f))

Seq.zip foo bar
|> Seq.iter(fun (f,s) -> printfn "%A" (snd s))

//let blendedRMSLE =
//    ensambles
//    |> Seq.map(fun e -> {Common.ExperimentResult.Observed = e.Observed; Common.ExperimentResult.Simulated= ((e.RandomForest + e.GLM)/2.0)})
//    |> Seq.toArray
//    |> Common.RMSLE
//
//RF (.66) and GLM (.82) = .67

