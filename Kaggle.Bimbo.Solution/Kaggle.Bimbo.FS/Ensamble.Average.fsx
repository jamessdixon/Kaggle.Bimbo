
#load "PrepareData.fsx"
#load "CommonFunctions.fsx"
#load "RandomForest.fsx"
#load "NaiveBayes.fsx"
#load "NeuralNetwork.fsx"

#time
let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random 0.02)   

let testItems =
    PrepareData.getTrainItems (PrepareData.Random 0.01)



