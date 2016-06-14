
#r "../packages/Accord/lib/net40/Accord.dll"
#r "../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "../packages/Accord.Math/lib/net40/Accord.Math.dll"
#r "../packages/Accord.Statistics/lib/net40/Accord.Statistics.dll"

open Accord
open System.IO
open FSharp.Data
open Accord.Math
open Accord.Statistics.Links
open Accord.Statistics.Models.Regression
open Accord.Statistics.Models.Regression.Fitting

type TownState = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/town_state.csv">
type ClienteTabla = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/cliente_tabla.csv">
type ProductoTabla = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/producto_tabla.csv">
type Train = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/train.csv",InferRows=100>
type Test = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/test.csv",InferRows=100>

let townState = TownState.GetSample().Rows
let clienteTabla = ClienteTabla.GetSample().Rows
let productoTabla = ProductoTabla.GetSample().Rows
let train = Train.GetSample().Rows |> Seq.take(20000)
let test = Test.GetSample().Rows

let input = train |> Seq.map(fun i -> [|(float)i.Semana; (float)i.Agencia_ID; (float)i.Canal_ID; (float)i.Ruta_SAK; (float)i.Cliente_ID; (float)i.Producto_ID|]) |> Seq.toArray
let output = train |> Seq.map(fun i -> (float)i.Demanda_uni_equil) |> Seq.toArray

let regression = new GeneralizedLinearRegression(new ProbitLinkFunction(), 6)
let teacher = new IterativeReweightedLeastSquares(regression)
let mutable delta = 0.0
while (delta > 0.001) do
    delta <- teacher.Run(input,output)

let input' = test |> Seq.map(fun i -> [|(float)i.Semana; (float)i.Agencia_ID; (float)i.Canal_ID; (float)i.Ruta_SAK; (float)i.Cliente_ID; (float)i.Producto_ID|]) |> Seq.toArray
let predicted = input' |> Seq.map(fun i -> regression.Compute(i)) 

let submission =
    test 
    |> Seq.map(fun i -> i.Id)
    |> Seq.zip predicted
    |> Seq.map(fun (v,id) -> sprintf "%A,%A" id v)
    |> Seq.toArray    


let outputPath = __SOURCE_DIRECTORY__ + @"/benchmark_submission_FSharp.csv"
File.WriteAllLines(outputPath, submission)


