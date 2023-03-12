module TradingBotMain = 
    open Naive
    open Trader
    open Common.PubSub

    [<EntryPoint>]
    let main argv = 

        let topic01 = "ethusd".ToUpper()
        let topic02 = "OtherSymbol".ToUpper()

        let trader01 = Trader("trader01", topic01, 0.05M)
        let trader02 = Trader("trader02", topic01, 0.05M)
        let trader03 = Trader("trader03", topic01, 0.05M)
        let trader04 = Trader("trader04", topic02, 0.05M)

        PubSubService.subscribe topic01 trader01 |> Async.RunSynchronously |> ignore
        PubSubService.subscribe topic01 trader02 |> Async.RunSynchronously |> ignore
        PubSubService.subscribe topic01 trader03 |> Async.RunSynchronously |> ignore
        PubSubService.subscribe topic02 trader04 |> Async.RunSynchronously |> ignore

        Streamer.Binance.startStreaming topic01

        0