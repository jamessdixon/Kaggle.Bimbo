
#load "PrepareData.fsx"

#time
let all = PrepareData.All
let townState = PrepareData.getTownStates all
let products = PrepareData.getProducts all
let clients = PrepareData.getClients all
let trainItems = PrepareData.getTrainItems all
let testItems = PrepareData.getTestItems all

//Real: 00:06:23.808, CPU: 00:05:53.156, GC gen0: 10991, gen1: 2940, gen2: 38
//Real: 00:01:42.469, CPU: 00:01:38.843, GC gen0: 786, gen1: 226, gen2: 10

trainItems 
|> Seq.groupBy(fun ti -> ti.WeekNumber)
|> Seq.map(fun (f,s) -> f, s |> Seq.length)




