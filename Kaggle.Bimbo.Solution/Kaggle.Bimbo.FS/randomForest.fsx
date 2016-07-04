
#load "PrepareData.fsx"
#r "../packages/alglibnet2/lib/alglibnet2.dll"

#time
let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random 0.02)

// NOTE: ALGLIB wants prediction variable at end of input array
let xy = 
    trainItems 
    |> Seq.map(fun ti -> [|ti.WeekNumber; ti.SalesDepotId; ti.AdjustedDemand|] |> Array.map float)   
    |> array2D
    
let numberOfPoints = trainItems |> Seq.length
let numberOfVariables = 2
let numberOfClasses = 1;
let numberOfTrees = 50
let r = 0.1

let info, forest, report = 
    alglib.dfbuildrandomdecisionforest(xy, numberOfPoints, numberOfVariables, numberOfClasses,numberOfTrees,r)

report

