
#r "../packages/FSharp.Data/lib/net40/FSharp.Data.dll"

open FSharp.Data

type Town_State = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/town_state.csv">
type ClienteTabla = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/cliente_tabla.csv">
type ProductoTabla = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/producto_tabla.csv">
type Train = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/train.csv",InferRows=100>
type Test = CsvProvider<"C:/Git/Kaggle.Bimbo/Data/test.csv",InferRows=100>

let town_States = Town_State.GetSample().Rows
let clientes = ClienteTabla.GetSample().Rows
let productos = ProductoTabla.GetSample().Rows
let trainItems = Train.GetSample().Rows |> Seq.take(100000)
let testItems = Test.GetSample().Rows

//parse townState
//townState 5,21,37
town_States |> Seq.take(3)
//1110, "2008 AG. LAGO FILT", "MÉXICO, D.F."
//1111, "2002 AG. AZCAPOTZALCO", "MÉXICO, D.F."
let firstTownState = town_States |> Seq.head
//firstTownState.Agencia_ID
//firstTownState.State
//firstTownState.Town

let states = 
    town_States 
    |> Seq.groupBy(fun r -> r.State)
    |> Seq.map(fun (f,s) -> f, s|> Seq.length)
    |> Seq.sortByDescending(fun (f,s) -> s)
    |> Seq.iter(fun (f,s) -> printfn "%s %i" f s)

let towns = 
    town_States 
    |> Seq.groupBy(fun r -> r.Town)
    |> Seq.map(fun (f,s) -> f, s|> Seq.length)
    |> Seq.sortByDescending(fun (f,s) -> s)
    |> Seq.iter(fun (f,s) -> printfn "%s %i" f s)

town_States |> Seq.filter(fun r -> r.Town = "2013 AG. MEGA NAUCALPAN")

//parse clienteTabla 
//clienteTabla 10,72
clientes |> Seq.take(3)
//0, "SIN NOMBRE"
//1, "OXXO XINANTECATL"
//2, "SIN NOMBRE"
let firstClient = clientes |> Seq.head
//firstClient.Cliente_ID
//firstClient.NombreCliente

//parse productoTabla
productos |> Seq.take(3)
//0, "NO IDENTIFICADO 0
//9, "Capuccino Moka 750g NES 9
//41, "Bimbollos Ext sAjonjoli 6p 480g BIM 41
let firstProduct = productos |> Seq.head
//foo.NombreProducto
//foo.Producto_ID

trainItems |> Seq.take(3)
//train 4,2,7,4,7,4,5,4,1,7,4
//3, 1110, 7, 3301, 15766, 1212, 3, 25.14M, 0, 0.0M, 3
//3, 1110, 7, 3301, 15766, 1216, 4, 33.52M, 0, 0.0M, 4
//3, 1110, 7, 3301, 15766, 1238, 4, 39.32M, 0, 0.0M, 4
let firstTrain = trainItems |> Seq.head
//firstTrain.Agencia_ID
//firstTrain.Canal_ID
//firstTrain.Cliente_ID
//firstTrain.Demanda_uni_equil
//firstTrain.Dev_proxima
//firstTrain.Dev_uni_proxima
//firstTrain.Producto_ID
//firstTrain.Ruta_SAK
//firstTrain.Semana
//firstTrain.Venta_hoy
//firstTrain.Venta_uni_hoy


//Train & Test
//•Semana — Week number (From Thursday to Wednesday)
//•Agencia_ID — Sales Depot ID
//•Canal_ID — Sales Channel ID
//•Ruta_SAK — Route ID (Several routes = Sales Depot)
//•Cliente_ID — Client ID
//•NombreCliente — Client name
//•Producto_ID — Product ID
//•NombreProducto — Product Name
//•Venta_uni_hoy — Sales unit this week (integer)
//•Venta_hoy — Sales this week (unit: pesos)
//•Dev_uni_proxima — Returns unit next week (integer)
//•Dev_proxima — Returns next week (unit: pesos)
//•Demanda_uni_equil — Adjusted Demand (integer) (This is the target you will predict)



