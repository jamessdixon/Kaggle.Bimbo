#r "../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "../packages/FSharp.Collections.ParallelSeq/lib/net40/FSharp.Collections.ParallelSeq.dll"
#r "../packages/RProvider/lib/net40/RProvider.dll"

#load "../packages/FSharp.Charting/FSharp.Charting.fsx"

open System
open FSharp.Data
open FSharp.Charting
open System.Text.RegularExpressions
open RProvider

let basePath = @"../../Data/"

type TownState = CsvProvider<"C:/Users/Edouard/Documents/27.Kaggle/Kaggle.Bimbo/Data/town_state.csv">
type ClienteTabla = CsvProvider<"C:/Users/Edouard/Documents/27.Kaggle/Kaggle.Bimbo/Data/cliente_tabla.csv">
type ProductoTabla = CsvProvider<"C:/Users/Edouard/Documents/27.Kaggle/Kaggle.Bimbo/Data/producto_tabla.csv">
type Train = CsvProvider<"C:/Users/Edouard/Documents/27.Kaggle/Kaggle.Bimbo/Data/train.csv",InferRows=100,CacheRows=false>
type Test = CsvProvider<"C:/Users/Edouard/Documents/27.Kaggle/Kaggle.Bimbo/Data/test.csv",InferRows=100>

let clientes = ClienteTabla.GetSample().Rows
let productos = ProductoTabla.GetSample().Rows
let trainItems = Train.GetSample().Rows |> Seq.take(100000)
let testItems = Test.GetSample().Rows


let town_States = TownState.Load(basePath + "town_state.csv")
///List of States:
let states =
    town_States.Rows
    |> Seq.map(fun ts -> ts.State)
    |> Seq.sort
    |> Set.ofSeq

//Number of states:
// on wikipedia: https://en.wikipedia.org/wiki/Administrative_divisions_of_Mexico
// 31 + 1 Federal district for Mexico city
states |> Set.count // => 33
states |> Set.map (printfn "%s") // Queretaro in duplicated

///Number of agencia / city
let cities =
    town_States.Rows
    |> Seq.take(100)
    |> List.ofSeq

//Get products
let products = ProductoTabla.Load(basePath + "producto_tabla.csv")

//name + number of pieces + weignt + BRAND + ID
products.Rows
|> Seq.take(20)
|> List.ofSeq

//Weigth of Products:
let weightRegEx = new Regex("(\d+)(Kg|g)")

let weights =
  products.Rows
  |> Seq.choose(fun p ->
      let w = weightRegEx.Match(p.NombreProducto)
      match w.Groups.Count with
        | 3 ->
              let amount = (float)(w.Groups.[1]).Value
              let unit = w.Groups.[2].Value
              if unit = "Kg" then Some (amount * 100.0) else Some amount
        | _ -> None
    )
  |> Array.ofSeq

//Stats
R.summary( weights ) |> R.print

//Histogram of Weights
Chart.Histogram (weights) //Few products with high weight

let brandRegEx = new Regex("^.+\s(\D+) \d+$")
let brands =
    products.Rows
    |> Seq.map(fun product ->
          let brand = brandRegEx.Match product.NombreProducto
          match brand.Groups.Count with
            | 2 -> brand.Groups.[1].Value
            | _ -> "")
    |> Set.ofSeq
    |> Set.map (printfn "%s")


// EXPLORE CUSTOMERS:
let clients = ClienteTabla.Load(basePath + "cliente_tabla.csv")
//Several duplicate IDs due to mistypping: 4862 !
clients.Rows
|> Seq.groupBy ( fun cl -> cl.Cliente_ID)
|> Seq.filter(fun (id, grp) -> grp |> Seq.length > 1)
|> Seq.length

let multipleSpaceRegex = new Regex(" {2,}")
let removeMultipleSpaces input = multipleSpaceRegex.Replace(input, " ")

//Check if mistypping is corrected: OK
clients.Rows
|> Seq.map(fun cl -> cl.Cliente_ID , removeMultipleSpaces cl.NombreCliente)
|> Seq.distinctBy snd
|> Seq.groupBy fst
|> Seq.filter(fun (id, grp) -> grp |> Seq.length > 1)
|> Seq.length // 0 as expected


//Examine Clients with multiple ID
// => Restaurant Chains > 100
// => 1: Non identified : unique person ?
// => 1 - 10: Family business
let client_Names =
    clients.Rows
    |> Seq.map(fun cl -> cl.Cliente_ID , removeMultipleSpaces cl.NombreCliente)
    |> Seq.countBy snd
    |> Seq.filter(fun (name, cnt) -> cnt > 100)
    |> Seq.sortByDescending snd
    |> Seq.tail
    |> Array.ofSeq

Chart.Histogram (client_Names |> Array.map snd)

//Find Categories of customers by examining data:
clients.Rows
|> Seq.skip(100)
|> Seq.take(100)
|> Seq.map(fun cl -> cl.NombreCliente)
|> List.ofSeq
// Names - Family business
// FMAXXXXX - Vending Machine?
// ESCUELA - School
// Contains : ALIM / MINI / SUPER / BODEGA- Groceries
// Contains: Cafeteria - Vending machine
