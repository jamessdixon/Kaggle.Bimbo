
#load "PrepareData.fsx"
#r "../packages/alglibnet2/lib/alglibnet2.dll"

#time
let trainItems = PrepareData.getTrainItems (PrepareData.Random 0.02)

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

//OOM if numberOfTrees = 50
//OutOfIndex exception if numberOfTrees = 2

