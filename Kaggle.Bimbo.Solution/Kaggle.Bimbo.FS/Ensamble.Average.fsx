
#r "../packages/alglibnet2/lib/alglibnet2.dll"
#r "../packages/Accord/lib/net40/Accord.dll" 
#r "../packages/Accord.Math/lib/net40/Accord.Math.dll" 
#r "../packages/Accord.Statistics/lib/net40/Accord.Statistics.dll" 
#r "../packages/Accord.MachineLearning/lib/net40/Accord.MachineLearning.dll" 
#r "../packages/Accord.Neuro/lib/net40/Accord.Neuro.dll" 

#load "CommonFunctions.fs"
#load "PrepareData.fs"
#load "RandomForest.fs"
#load "GLM.fs"
#load "NaiveBayes.fs"
#load "NeuralNetwork.fs"

open Kaggle.Bimbo

#time
let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random 0.02)   

let holdOutItems =
    PrepareData.getTrainItems (PrepareData.Random 0.01)

let randomForest = RandomForest.runRandomForest trainItems holdOutItems
let glm = GLM.runGLM trainItems holdOutItems
let naiveBayes = NaiveBayes.runNaiveBayes trainItems holdOutItems
let neuralNetwork = NeuralNetwork.runNeuralNetwork trainItems holdOutItems

type Ensamble = {Observed:int; RandomForest:float;GLM:float; NaiveBayes:float; NeuralNetwork:float}

let ensambles =
    holdOutItems
    |> Seq.mapi(fun i hoi -> 
            {Observed=hoi.AdjustedDemand;
             RandomForest=randomForest.[i];
             GLM=glm.[i];
             NaiveBayes=naiveBayes.[i];
             NeuralNetwork=neuralNetwork.[i]})
    |> Seq.toArray

let averageRMSLE =
    ensambles
    |> Seq.map(fun e -> {CommonFunctions.ExperimentResult.Observed = e.Observed; CommonFunctions.ExperimentResult.Simulated= ((e.RandomForest + e.GLM)/2.0)})
    |> Seq.toArray
    |> CommonFunctions.RMSLE



