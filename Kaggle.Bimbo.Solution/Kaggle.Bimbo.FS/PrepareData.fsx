#r "../packages/FSharp.Data/lib/net40/FSharp.Data.dll"

open FSharp.Data

type Town_State = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/town_state.csv">
type ProductoTabla = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/producto_tabla.csv">
type ClienteTabla = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/cliente_tabla.csv">
type Train = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/train.csv",InferRows=100>
type Test = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/test.csv",InferRows=100>

let town_States = Town_State.GetSample().Rows
let productos = ProductoTabla.GetSample().Rows
let clientes = ClienteTabla.GetSample().Rows
let trainItems = Train.GetSample().Rows |> Seq.take(100000)
let testItems = Test.GetSample().Rows

type TownState = {SalesDepotId:int; TownId:int; TownDesc: string; StateDesc:string}

let TownStates = 
    town_States 
    |> Seq.map(fun i -> {SalesDepotId=i.Agencia_ID; TownId=System.Int32.Parse(i.Town.Substring(0,4)); TownDesc=i.Town.Substring(5,i.Town.Length-5); StateDesc=i.State})
    |> Set.ofSeq

type Product = {ProductId:int; Brand:string; ShortName:string; Weight:option<float>; Quantity: option<float>}

open System.Text.RegularExpressions
let createProduct (producto_ID, producto) =
    let shortNameRegEx = new Regex("^(\D*)")
    let shortName = shortNameRegEx.Match(producto).Value
    let brandRegEx = new Regex("^.+\s(\D+) \d+$")
    let brand = brandRegEx.Match(producto)
    let brand' =
        match brand.Groups.Count with
        | 2 -> brand.Groups.[1].Value
        | _ -> ""
    let weightRegEx = new Regex("(\d+)(Kg|g)")
    let weight = weightRegEx.Match(producto)
    let weight' = 
        match weight.Groups.Count with
        | 3 -> let amount = (float)(weight.Groups.[1]).Value
               let unit = weight.Groups.[2].Value
               if unit = "Kg" then Some (amount * 100.0) else Some amount 
        | _ -> None
    let quantityRegEx = new Regex("(\d+)p ")
    let quantity = quantityRegEx.Match(producto)
    let quantity' = 
        match quantity.Groups.Count with
        | 2 -> Some ((float)(quantity.Groups.[1]).Value)
        | _ -> None
    {ProductId = producto_ID; Brand = brand'; ShortName = shortName; Weight = weight'; Quantity = quantity'}

let products =
    productos
    |> Seq.map(fun p -> createProduct(p.Producto_ID, p.NombreProducto))
    |> Set.ofSeq

type Client = {ClientId:int; ClientDesc:string}

let clients = 
    clientes 
    |> Seq.map(fun r -> {ClientId = r.Cliente_ID; ClientDesc = r.NombreCliente})
    |> Set.ofSeq

type Transaction = {WeekNumber:int; SalesDepotId:int; SalesChannelId: int; SalesRouteId: int; ClientId: int; ProductId: int; 
                    SalesThisWeek:float; ReturnsNextWeek:float; SalesUnitThisWeek:int; ReturnsUnitNextWeek:int; AdjustedDemand:int}

let trainTransactions =
    trainItems
    |> Seq.take(5000)
    |> Seq.map(fun i -> {WeekNumber=i.Semana; SalesDepotId = i.Agencia_ID; SalesChannelId = i.Canal_ID; SalesRouteId = i.Ruta_SAK; ClientId = i.Cliente_ID;
                         ProductId = i.Producto_ID; SalesThisWeek = (float)i.Venta_hoy; ReturnsNextWeek = (float)i.Dev_proxima; SalesUnitThisWeek = i.Venta_uni_hoy;
                         ReturnsUnitNextWeek = i.Dev_uni_proxima; AdjustedDemand = i.Demanda_uni_equil})
    |> Set.ofSeq
