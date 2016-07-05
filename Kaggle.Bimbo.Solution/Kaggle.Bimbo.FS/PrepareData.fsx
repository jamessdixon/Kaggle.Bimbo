
//let basePath = __SOURCE_DIRECTORY__ + @"../../../Data/"
let basePath =  @"F:\Git\Kaggle.Bimbo\Data\"

type TownState = {SalesDepotId:int; TownId:int; TownDesc:string; StateDesc:string}
type Product = {ProductId:int; Brand:string; ShortName:string; Weight:option<float>; Quantity: option<float>}
type Client = {ClientId:int; ClientDesc:string}
type TrainItem = {WeekNumber:int; SalesDepotId:int; SalesChannelId: int; SalesRouteId: int; ClientId: int; ProductId: int;
                    SalesThisWeek:float; ReturnsNextWeek:float; SalesUnitThisWeek:int; ReturnsUnitNextWeek:int;
                    AdjustedDemand:int}
type TestItem = {Id: int; WeekNumber:int; SalesDepotId:int; SalesChannelId: int; SalesRouteId: int;
                    ClientId: int; ProductId: int;}

type RecordAmount =
| All
| Top of int
| Random of float

let getTownStates recordAmount =
    let list = new System.Collections.Generic.List<TownState>()
    let addRow (row:string) =
        let r = row.Split(',')
        let salesDepotId = (int)r.[0]
        let townInfo = (string)r.[1]
        let townId = townInfo.Substring(0,4)
        let townDesc = townInfo.Substring(5, townInfo.Length-5)
        let stateInfo = (string)r.[2]
        let townState = {TownState.SalesDepotId = salesDepotId;
                        TownId = (int)townId;
                        TownDesc = townDesc;
                        StateDesc = stateInfo;}
        list.Add(townState)

    let path = basePath + "town_state.csv"
    let reader = new System.IO.StreamReader(path)
    let mutable row = reader.ReadLine()
    match recordAmount with
    | All ->
        while not(System.String.IsNullOrEmpty(row)) do
            row <- reader.ReadLine()
            if row <> null then addRow row
    | Top value ->
        for i = 0 to value - 1 do
            row <- reader.ReadLine()
            if row <> null then addRow row
    | Random value ->
        let random = new System.Random()
        while not(System.String.IsNullOrEmpty(row)) do
            row <- reader.ReadLine()
            if row <> null && (random.NextDouble() < value) then addRow row
    list

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
    {Product.ProductId = producto_ID; Brand = brand'; ShortName = shortName; Weight = weight'; Quantity = quantity'}

let getProducts recordAmount =
    let list = new System.Collections.Generic.List<Product>()
    let addRow (row:string) =
        let r = row.Split(',')
        let productId = (int)r.[0]
        let productInfo = (string)r.[1]
        let product = createProduct(productId, productInfo)
        list.Add(product)

    let path = basePath + "producto_tabla.csv"
    let reader = new System.IO.StreamReader(path)
    let mutable row = reader.ReadLine()
    match recordAmount with
    | All ->
        while not(System.String.IsNullOrEmpty(row)) do
            row <- reader.ReadLine()
            if row <> null then addRow row
    | Top value ->
        for i = 0 to value - 1 do
            row <- reader.ReadLine()
            if row <> null then addRow row
    | Random value ->
        let random = new System.Random()
        while not(System.String.IsNullOrEmpty(row)) do
            row <- reader.ReadLine()
            if row <> null && (random.NextDouble() < value) then addRow row
    list

let getClients recordAmount =
    let list = new System.Collections.Generic.List<Client>()
    let addRow (row:string) =
        let r = row.Split(',')
        let client = {Client.ClientId=(int)r.[0]; ClientDesc= (string)r.[1]}
        list.Add(client)

    let path = basePath + "cliente_tabla.csv"
    let reader = new System.IO.StreamReader(path)
    let mutable row = reader.ReadLine()
    match recordAmount with
    | All ->
        while not(System.String.IsNullOrEmpty(row)) do
            row <- reader.ReadLine()
            if row <> null then addRow row
    | Top value ->
        for i = 0 to value - 1 do
            row <- reader.ReadLine()
            if row <> null then addRow row
    | Random value ->
        let random = new System.Random()
        while not(System.String.IsNullOrEmpty(row)) do
            row <- reader.ReadLine()
            if row <> null && (random.NextDouble() < value) then addRow row
    list

let getTrainItems recordAmount =
    let list = new System.Collections.Generic.List<TrainItem>()
    let addRow (row:string) =
        let r = row.Split(',')
        let trainItem =
            {TrainItem.WeekNumber=(int)r.[0];
            SalesDepotId= (int)r.[1];
            SalesChannelId= (int)r.[2];
            SalesRouteId= (int)r.[3];
            ClientId= (int)r.[4];
            ProductId= (int)r.[5];
            SalesUnitThisWeek= (int)r.[6];
            SalesThisWeek= (float)r.[7];
            ReturnsUnitNextWeek= (int)r.[8];
            ReturnsNextWeek= (float)r.[9];
            AdjustedDemand= (int)r.[10]}
        list.Add(trainItem)

    let path = basePath + "train.csv"
    let reader = new System.IO.StreamReader(path)
    let mutable row = reader.ReadLine()
    match recordAmount with
    | All ->
        while not(System.String.IsNullOrEmpty(row)) do
            row <- reader.ReadLine()
            if row <> null then addRow row
    | Top value ->
        for i = 0 to value - 1 do
            row <- reader.ReadLine()
            if row <> null then addRow row
    | Random value ->
        let random = new System.Random()
        while not(System.String.IsNullOrEmpty(row)) do
            row <- reader.ReadLine()
            if row <> null && (random.NextDouble() < value) then addRow row
    list

let getTestItems recordAmount =
    let list = new System.Collections.Generic.List<TestItem>()
    let addRow (row:string) =
        let r = row.Split(',')
        let testItem =
            {TestItem.Id = (int)r.[0];
            WeekNumber= (int)r.[1];
            SalesDepotId= (int)r.[2];
            SalesChannelId= (int)r.[3];
            SalesRouteId= (int)r.[4];
            ClientId= (int)r.[5];
            ProductId= (int)r.[6];}
        list.Add(testItem)

    let path = basePath + "test.csv"
    let reader = new System.IO.StreamReader(path)
    let mutable row = reader.ReadLine()
    match recordAmount with
    | All ->
        while not(System.String.IsNullOrEmpty(row)) do
            row <- reader.ReadLine()
            if row <> null then addRow row
    | Top value ->
        for i = 0 to value - 1 do
            row <- reader.ReadLine()
            if row <> null then addRow row
    | Random value ->
        let random = new System.Random()
        while not(System.String.IsNullOrEmpty(row)) do
            row <- reader.ReadLine()
            if row <> null && (random.NextDouble() < value) then addRow row
    list
