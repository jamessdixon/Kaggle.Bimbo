
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


