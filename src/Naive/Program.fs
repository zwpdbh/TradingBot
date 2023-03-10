namespace Navie 


module NavieMain = 
    open Naive

    [<EntryPoint>]
    let main argv = 
        Trader.start() |> ignore
        0