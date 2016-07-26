
#r "../packages/alglibnet2/lib/alglibnet2.dll"
#r "../packages/Accord/lib/net40/Accord.dll" 
#r "../packages/Accord.Math/lib/net40/Accord.Math.dll" 
#r "../packages/Accord.Statistics/lib/net40/Accord.Statistics.dll" 
#r "../packages/Accord.MachineLearning/lib/net40/Accord.MachineLearning.dll" 

#load "Common.fs"
#load "PrepareData.fs"
#load "RandomForest.fs"
#load "GLM.fs"

open System.IO
open Kaggle.Bimbo

#time
let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random {Percent=0.02;SeedValue=86})   

let testItems =
    PrepareData.getTestItems (PrepareData.All)
    |> List.map(fun ti -> PrepareData.convertTestItemIntoTrainItem ti)

let randomForest = RandomForest.run trainItems testItems
let glm = GLM.run trainItems testItems

let ensambles =
    testItems
    |> Seq.mapi(fun i hoi -> 
            {Common.LimitedEnsamble.Observed=hoi.AdjustedDemand;
             Common.LimitedEnsamble.RandomForest=randomForest.[i];
             Common.LimitedEnsamble.GLM= glm.[i]})
    |> Seq.map(fun o -> if o.GLM < 0.0 then o.RandomForest else ((o.RandomForest + o.GLM)/2.0))
    |> Seq.map(fun v -> if v < 0.0 then 0.0 else v)
    |> Seq.toArray


let submission = 
    Seq.zip testItems ensambles
    |> Seq.map (fun (r,v) -> sprintf "%A,%A" r.Id v)
    |> Seq.toList

let outputPath = __SOURCE_DIRECTORY__ + @"../submission.csv"
File.WriteAllLines(outputPath, "id,Demanda_uni_equil" :: submission)


