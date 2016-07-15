
#r "../packages/alglibnet2/lib/alglibnet2.dll"

#load "Common.fs"
#load "PrepareData.fs"
#load "RandomForest.fs"

open Kaggle.Bimbo

let trainItems = 
    PrepareData.getTrainItems (PrepareData.Random {Percent=0.02;SeedValue=42})   
    
let holdOutItems =
    PrepareData.getTrainItems (PrepareData.Random {Percent=0.01;SeedValue=5})
    
let predicted = RandomForest.run trainItems holdOutItems
    
let rmsle = 
    Seq.zip holdOutItems predicted
    |> Seq.map(fun (ho,p) -> {Common.Simulated=p; Common.Observed=ho.AdjustedDemand})
    |> Seq.toArray
    |> Common.RMSLE


//2~1 42 seed, 50 trees, .1 model = .66 rsmle
//2~1 42/5 seeds, 50 trees, .1 model = .72 rsmle

