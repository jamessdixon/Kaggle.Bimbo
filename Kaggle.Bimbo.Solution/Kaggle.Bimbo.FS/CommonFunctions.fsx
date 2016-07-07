
open System

type ExperimentResult = {Simulated:float; Observed:int}

let RMSE (results:ExperimentResult[])=
    results
    |> Seq.averageBy (fun r ->
        let delta = r.Simulated - ((float)r.Observed)
        delta * delta)
    |> sqrt

let RMSLE (results:ExperimentResult[]) =
    results
    |> Seq.averageBy(fun r -> Math.Pow((log((r.Simulated + 1.0)) - log(float (r.Observed + 1))), 2.0))
    |> sqrt

//[0.0 .. 100.0]
//|> Seq.iter(fun i -> printfn "%A/1.0: %A: %A" i (RMSE [|{Simulated=i;Observed=1}|]) (RMSLE [|{Simulated=i;Observed=1}|]))

let createArray length index=
    let (x:int array) = Array.zeroCreate (length + 1)
    if index > length  then
        x.[length] <- 1
    else
        x.[index] <- 1
    x

//createArray 10 0
//createArray 10 10
//createArray 10 20

let getDistributions (a:int array) =
    a
    |> Array.countBy(fun x -> x)
    |> Array.map(fun (x,y) -> float y/float (a |> Array.length))

//[|1;1;1;1;1;1|] |> getDistributions
//[|1;1;1;2;2;2|] |> getDistributions

