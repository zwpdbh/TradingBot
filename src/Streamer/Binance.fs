namespace Streamer 

module Binance = 
    // How to use websocket to do something similar from
    // https://book.elixircryptobot.com/stream-live-cryptocurrency-prices-from-the-binance-wss.html#create-a-supervised-application-inside-an-umbrella
    open Websocket.Client
    open System

    let endpoint = "wss://stream.binance.us:9443/ws/"

    let tradeUrl (symbol: string) = 
        endpoint + symbol.ToLower() + "@trade" 


    let processMessage (msg: ResponseMessage) = 
        // It received something like:
        //{"e":"trade","E":1678281305803,"s":"ETHUSD","t":44172879,"p":"1549.38000000","q":"0.05300000","b":1478766161,"a":1479008752,"T":1678281305802,"m":true,"M":true}
        printfn $"Received message: {msg}"

    let processReconnection (msg: Models.ReconnectionInfo) = 
        printfn $"Reconnection happened, type: {msg.Type}"

    let processDisconnection (msg: DisconnectionInfo) = 
        printfn $"disconnected: {msg}"

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
        let ws = new WebsocketClient(new Uri(tradeUrl symbol))

        ws.ReconnectTimeout <- TimeSpan.FromSeconds(30)
        ws.DisconnectionHappened.Subscribe(processDisconnection) |> ignore
        ws.ReconnectionHappened.Subscribe(processReconnection) |> ignore
        ws.MessageReceived.Subscribe(processMessage) |> ignore

        runClient ws |> Async.RunSynchronously

        


//module Binance = 
//    open System
//    open Websocket.Client

//    let endpoint = "wss://stream.binance.us:9443/ws/"
//    let url = endpoint + "ethusd" + "@trade"
    
//    let ws = new WebsocketClient(new Uri(url))

//    ws.DisconnectionHappened.Subscribe (fun msg -> 
//        printfn $"disconnected: {msg}"
//    ) |> ignore

//    ws.ReconnectTimeout <- TimeSpan.FromSeconds(300)

//    ws.ReconnectionHappened.Subscribe(fun msg ->
//        printfn $"Reconnection happened, type: {msg.Type}") |> ignore

//    ws.MessageReceived.Subscribe(fun msg ->
//        printfn $"Received message: {msg}"
//    ) |> ignore 

//    let rec receiveMessage (ws: WebsocketClient) = 
//        // It received something like:
//        //{"e":"trade","E":1678281305803,"s":"ETHUSD","t":44172879,"p":"1549.38000000","q":"0.05300000","b":1478766161,"a":1479008752,"T":1678281305802,"m":true,"M":true}
//        async {
//            do! receiveMessage ws
//        }

//    let startStreaming () = 
//        let runClient() = 
//            async {
                
//                do! ws.Start() |> Async.AwaitTask
//                do! [
//                        receiveMessage ws 
//                    ] |> Async.Parallel |> Async.Ignore
//            }

//        runClient () |> Async.RunSynchronously
    
        

