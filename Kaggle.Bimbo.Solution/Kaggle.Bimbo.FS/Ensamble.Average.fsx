
#load "PrepareData.fsx"
#load "CommonFunctions.fsx"
#load "RandomForest.fsx"
#load "GLM.fsx"
#load "NaiveBayes.fsx"
#load "NeuralNetwork.fsx"

open RandomForest
open GLM
open NaiveBayes
open NeuralNetwork

#time
let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random 0.02)   

let holdOutItems =
    PrepareData.getTrainItems (PrepareData.Random 0.01)

let randomForestScore = runRandomForest trainItems holdOutItems
let glmScore = runNeuralNetwork trainItems holdOutItems
let naiveBayesScore = runNaiveBayes trainItems holdOutItems
let neuralNetworkScore = runNeuralNetwork trainItems holdOutItems






