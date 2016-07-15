
#r "../packages/alglibnet2/lib/alglibnet2.dll"
#r "../packages/Accord/lib/net40/Accord.dll" 
#r "../packages/Accord.Math/lib/net40/Accord.Math.dll" 
#r "../packages/Accord.Statistics/lib/net40/Accord.Statistics.dll" 
#r "../packages/Accord.MachineLearning/lib/net40/Accord.MachineLearning.dll" 
#r "../packages/Accord.Neuro/lib/net40/Accord.Neuro.dll" 

#load "Common.fs"
#load "PrepareData.fs"
#load "RandomForest.fs"
#load "GLM.fs"
#load "NaiveBayes.fs"
#load "NeuralNetwork.fs"

open Kaggle.Bimbo

#time
let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random {Percent=0.10;SeedValue=5150})   

let holdOutItems =
    PrepareData.getTrainItems (PrepareData.Random {Percent=0.10;SeedValue=99})

let trainItemsCount = trainItems |> Seq.length |> float
let attachmentPoint = (trainItemsCount / 5.0) |> int

let slices =
    [1..5]
    |> Seq.map(fun i -> trainItems.GetSlice((Some ((i-1)*attachmentPoint)), (Some (i*attachmentPoint))))
    |> Seq.toArray

let randomForestTrain = 
    [0..3]
    |> Seq.map(fun i -> slices.[i])
    |> Seq.concat
    |> Seq.toList

let randomForestTest =
    slices.[4] 
    |> Seq.toList

let randomForest = RandomForest.run randomForestTrain randomForestTest

let glmTrain = 
    [1..4]
    |> Seq.map(fun i -> slices.[i])
    |> Seq.concat
    |> Seq.toList

let glmTest =
    slices.[0]
    |> Seq.toList

let glm = GLM.run glmTrain glmTest



