
#r "../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
open FSharp.Data

[<Literal>]
let WineDataPath = "http://archive.ics.uci.edu/ml/machine-learning-databases/wine-quality/winequality-white.csv"
type WineDataCsv = CsvProvider<WineDataPath, ";">

let loadTrainingData () =
    WineDataCsv.GetSample().Rows
    |> Seq.map (fun r ->
        [| r.Alcohol
           r.Chlorides
           r.``Citric acid``
           r.Density
           r.``Fixed acidity``
           r.``Free sulfur dioxide``
           r.PH
           r.``Residual sugar``
           r.Sulphates
           r.``Total sulfur dioxide``
           r.``Volatile acidity`` |]
           |> Array.map float,
        r.Quality |> float)
    |> Array.ofSeq

#r "../packages/alglibnet2/lib/alglibnet2.dll"

let trainForest (features: float[][]) (outputs: float[]) trees subsample =
    let inputArray =
        Seq.zip features outputs
        |> Seq.map (fun (d,o) -> Array.append d [|o|])
        |> array2D
    let featureCount = inputArray.GetLength(1) - 1
    let mutable info = 0
    let forest = alglib.dforest.decisionforest()
    let report = alglib.dforest.dfreport()
    alglib.dforest.dfbuildrandomdecisionforest(
        inputArray,
        npoints = features.Length,
        nvars = featureCount,
        nclasses = 1, // 1 for regression, otherwise would be number of output classes
        ntrees = trees,
        r = subsample,
        info = &info,
        df = forest,
        rep = report)
    match info with
    | 1  ->
        let predictor features =
            let mutable predictions : float[] = [||]
            alglib.dforest.dfprocess(forest, features, &predictions)
            predictions
        predictor, report.oobrmserror
    | -1 -> failwith "Incorrect parameters"
    | -2 -> failwith "Invalid class data"
    | _  -> failwith "Unknown error"


let rnd = System.Random(1234) // static seed for reproducable runs
let partitionData ratio data =
    data |> Array.partition (fun _ -> rnd.NextDouble() < ratio)

let allData = loadTrainingData ()
let trainSet, testSet = allData |> partitionData 0.9
let trainFeatures, trainOutputs = trainSet |> Array.unzip
let testFeatures, testOutputs = testSet |> Array.unzip

