namespace Streamer 

module Binance = 
    open Websocket.Client
    open System
    
    open Common.Logging
    open Common.Models
    open Naive.Trader


    // How to use websocket to do something similar from
    // https://book.elixircryptobot.com/stream-live-cryptocurrency-prices-from-the-binance-wss.html#create-a-supervised-application-inside-an-umbrella
    let endpoint = "wss://stream.binance.us:9443/ws/"

    let tradeUrl (symbol: string) = 
        endpoint + symbol.ToLower() + "@trade" 



    let processMessage (msg: ResponseMessage) (trader: Trader) = 
        // It received something like:
        //{"e":"trade","E":1678281305803,"s":"ETHUSD","t":44172879,"p":"1549.38000000","q":"0.05300000","b":1478766161,"a":1479008752,"T":1678281305802,"m":true,"M":true}
        //printfn $"Received message: {msg}"
        match msg.ToString() with 
        | GetTradeEvent event -> 
            log 3 $"Trade event received: {event.symbol}@{event.price}"
            trader.ProcessTradeEvent event |> Async.RunSynchronously
        | msg -> 
            log 0 $"Unknown event: {msg}"


    let processReconnection (msg: Models.ReconnectionInfo) = 
        log 1 $"Reconnection happened, type: {msg.Type}"

    let processDisconnection (msg: DisconnectionInfo) = 
        log 1 $"disconnected: {msg}"

    let rec receiveMessage (ws: WebsocketClient) =       
        async {
            do! receiveMessage ws
        }

    let runClient (ws: WebsocketClient) = 
        async {
            
            do! ws.Start() |> Async.AwaitTask
            do! [
                    receiveMessage ws 
                ] |> Async.Parallel |> Async.Ignore
        }



    let startStreaming (symbol: string) = 
        let trader = Trader(symbol, 0.05M)

        let ws = new WebsocketClient(new Uri(tradeUrl symbol))

        ws.ReconnectTimeout <- TimeSpan.FromSeconds(30)
        ws.DisconnectionHappened.Subscribe(processDisconnection) |> ignore
        ws.ReconnectionHappened.Subscribe(processReconnection) |> ignore
        ws.MessageReceived.Subscribe(fun x -> processMessage x trader) |> ignore

        runClient ws |> Async.RunSynchronously

        
        

