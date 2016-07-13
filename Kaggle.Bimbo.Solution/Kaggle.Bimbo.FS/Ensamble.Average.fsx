
#load "CommonFunctions.fsx"
#load "PrepareData.fsx"
#load "RandomForest.fsx"
#load "GLM.fsx"
#load "NaiveBayes.fsx"
#load "NeuralNetwork.fsx"

open CommonFunctions
open PrepareData
open RandomForest
open GLM
open NaiveBayes
open NeuralNetwork

#time
let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random 0.02)   

let holdOutItems =
    PrepareData.getTrainItems (PrepareData.Random 0.01)

let randomForest = RandomForest.runRandomForest trainItems holdOutItems
let glm = GLM.runGLM trainItems holdOutItems
let naiveBayes = NaiveBayes.runNaiveBayes trainItems holdOutItems
let neuralNetwork = NeuralNetwork.runNeuralNetwork trainItems holdOutItems






