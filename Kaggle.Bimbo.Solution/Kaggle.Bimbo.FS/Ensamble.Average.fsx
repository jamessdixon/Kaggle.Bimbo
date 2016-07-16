
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
let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random {Percent=0.02;SeedValue=86})   

let holdOutItems =
    PrepareData.getTrainItems (PrepareData.Random {Percent=0.01;SeedValue=75})

let randomForest = RandomForest.run trainItems holdOutItems
let glm = GLM.run trainItems holdOutItems

let ensambles =
    holdOutItems
    |> Seq.mapi(fun i hoi -> 
            {Common.LimitedEnsamble.Observed=hoi.AdjustedDemand;
             Common.LimitedEnsamble.RandomForest=randomForest.[i];
             Common.LimitedEnsamble.GLM= glm.[i]})
    |> Seq.toArray

//ensambles
//|> Seq.map(fun e -> e.GLM, abs(float e.Observed-e.GLM))
//|> Seq.filter(fun (v,d) -> d = 2.0 || d = 3.0)
//|> Seq.groupBy(fun (v,d) -> v)
//|> Seq.map(fun (v, da) -> v, da |> Seq.length)
//|> Seq.sortBy(fun (v,a) -> v)
//|> Seq.iter(fun (v,a) -> printfn "%A, %A" v a)

let averageExperimentResult =
    ensambles
    |> Seq.map(fun e -> {Common.ExperimentResult.Observed = e.Observed; 
                         Common.ExperimentResult.Simulated= ((float e.RandomForest + float e.GLM)/2.0)})
    |> Seq.toArray

let averageRMSLE =
    averageExperimentResult
    |> Common.RMSLE

//RF (.72) and GLM (.82) = .71

let adjustedAverageExperimentResult =
    ensambles
    |> Seq.map(fun e -> {Common.ExperimentResult.Observed = e.Observed; 
                         Common.ExperimentResult.Simulated=
                         if e.RandomForest < 5.0 then
                            ((float e.RandomForest + float e.GLM)/2.0)
                         else
                            e.RandomForest
                         })
    |> Seq.toArray

let adjustedAverageRMSLE =
    averageExperimentResult
    |> Common.RMSLE

