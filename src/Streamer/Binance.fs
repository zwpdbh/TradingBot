namespace Streamer 



module Binance = 
    // How to use websocket to do something similar from
    // https://book.elixircryptobot.com/stream-live-cryptocurrency-prices-from-the-binance-wss.html#create-a-supervised-application-inside-an-umbrella

    open Websocket.Client
    //open System.Net.WebSockets
    open System

    let endpoint = "wss://stream.binance.com:9443/ws/"

    let tradeUrl (symbol: string) = 
        $"{endpoint}{symbol.ToLower()}@trade"

    let example01 symbol = 
        // See: http://www.fssnip.net/88W/title/Websocket-request-blocking
        async {
            let client = new WebsocketClient(new Uri(tradeUrl symbol))
            client.MessageReceived.Add (fun x -> 
                printfn "%A" x
            )
            do! client.Start() |> Async.AwaitTask
        } |> Async.RunSynchronously

        

