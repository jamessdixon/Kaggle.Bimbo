
#load "PrepareData.fsx"
#r "../packages/alglibnet2/lib/alglibnet2.dll"
open System

type ExperimentResult = {Id:int; Simulated:float; Observed:int}

let RMSLE (results:ExperimentResult[]) =
    results
    |> Seq.averageBy(fun r -> Math.Pow((log(r.Simulated) - log(float r.Observed)), 2.0))
    |> sqrt

#time
let trainItems = PrepareData.getTrainItems (PrepareData.Random 0.02)
let testItems = PrepareData.getTrainItems (PrepareData.Random 0.01)

// NOTE: ALGLIB wants prediction variable at end of input array
let xy = 
    trainItems 
    |> Seq.map(fun ti -> [|float ti.WeekNumber; float ti.SalesDepotId; float ti.AdjustedDemand|])
    |> array2D
    
let numberOfPoints = xy.Length
let numberOfVariables = 2
let numberOfClasses = 1;
let numberOfTrees = 50
let r = 0.1

let info, forest, report = 
    alglib.dfbuildrandomdecisionforest(xy, numberOfPoints, numberOfVariables,
                                        numberOfClasses,numberOfTrees,r)



