
open System

type ExperimentResult = {Simulated:float; Observed:int}

let RMSLE (results:ExperimentResult[]) =
    results
    |> Seq.averageBy(fun r -> Math.Pow((log((r.Simulated + 1.0)) - log(float (r.Observed + 1))), 2.0))
    |> sqrt

//let test = [|{Simulated = 18.66; Observed = 0;}|]
//RMSLE test

