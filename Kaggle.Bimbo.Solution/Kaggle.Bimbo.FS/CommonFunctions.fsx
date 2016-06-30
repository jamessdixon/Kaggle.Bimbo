
open System

type ExperimentResult = {Simulated:float; Observed:int}

let RMSLE (results:ExperimentResult[]) =
    results
    |> Seq.averageBy(fun r -> Math.Pow((log(r.Simulated) - log(float r.Observed)), 2.0))
    |> sqrt

