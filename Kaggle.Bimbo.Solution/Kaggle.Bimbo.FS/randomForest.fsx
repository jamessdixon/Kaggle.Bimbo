
#load "PrepareData.fsx"
#r "../packages/alglibnet2/lib/alglibnet2.dll"

#time
let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random 0.02)
    
// NOTE: ALGLIB wants prediction variable at end of input array
let xy = 
    trainItems 
    |> Seq.map(fun ti -> [|ti.WeekNumber; ti.SalesDepotId; ti.ClientId; ti.ProductId; ti.SalesRouteId; ti.AdjustedDemand|] |> Array.map float)   
    |> array2D
    
let numberOfPoints = trainItems |> Seq.length
let numberOfVariables = 5
let numberOfClasses = 1;
let numberOfTrees = 50
let r = 0.1
let mutable info = 0
let forest = alglib.dforest.decisionforest()
let report = alglib.dforest.dfreport()

alglib.dforest.dfbuildrandomdecisionforest(xy, numberOfPoints, numberOfVariables, numberOfClasses,numberOfTrees,r,&info,forest,report)

let testItems =
    PrepareData.getTrainItems (PrepareData.Random 0.01)

let makePrediction (item:PrepareData.TrainItem) =
    let x = item |> fun ti -> [|ti.WeekNumber; ti.SalesDepotId; ti.ClientId; ti.ProductId; ti.SalesRouteId |] |> Array.map float
    let mutable predictions : float[] = [||]
    alglib.dforest.dfprocess(forest,x,&predictions)
    predictions.[0]

testItems
|> Seq.map(fun ti -> float ti.AdjustedDemand, makePrediction ti)
|> Seq.averageBy(fun (a,p) -> a-p)

