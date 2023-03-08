namespace Streamer 

module Binance = 
    open Websocket.Client
    open System
    open Thoth.Json.Net
    open Tools.Logging
    type TradeEvent =
        {
            eventType: string 
            eventTime: DateTime
            symbol: string 
            tradeId: int 
            price: decimal
            quantity: decimal 
            buyerOrderId: int 
            sellerOrderId: int 
            tradeTime: DateTime 
            buyerMarketMarker: bool
            ignore: bool
        }


    
    //{
    //  "e": "trade",       // Event type
    //  "E": 1672515782136, // Event time
    //  "s": "BNBBTC",      // Symbol
    //  "t": 12345,         // Trade ID
    //  "p": "0.001",       // Price
    //  "q": "100",         // Quantity
    //  "b": 88,            // Buyer order ID
    //  "a": 50,            // Seller order ID
    //  "T": 1672515782136, // Trade time
    //  "m": true,          // Is the buyer the market maker?
    //  "M": true           // Ignore
    //}


    let dateDecoder: Decoder<DateTime> = 
        Decode.int64
        |> Decode.andThen (fun unixTimestamp ->
            Decode.succeed (DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp).DateTime)
        )

    let (|GetTradeEvent|_|) str: option<TradeEvent> = 
        let tradeEventDecoder: Decoder<TradeEvent> = 

            Decode.object (
                fun get -> 
                    {
                        TradeEvent.eventType = get.Required.Field "e" (Decode.string)
                        TradeEvent.eventTime = get.Required.Field "E" dateDecoder
                        TradeEvent.symbol = get.Required.Field "s" (Decode.string)
                        TradeEvent.tradeId = get.Required.Field "t" (Decode.int)
                        TradeEvent.price = get.Required.Field "p" (Decode.decimal)
                        TradeEvent.quantity = get.Required.Field "q" (Decode.decimal)
                        TradeEvent.buyerOrderId = get.Required.Field "b" (Decode.int)
                        TradeEvent.sellerOrderId = get.Required.Field "a" (Decode.int)
                        TradeEvent.tradeTime = get.Required.Field "T" dateDecoder
                        TradeEvent.buyerMarketMarker = get.Required.Field "m" (Decode.bool)
                        TradeEvent.ignore = get.Required.Field "M" (Decode.bool)
                    }
            )

        match str |> Decode.fromString tradeEventDecoder with 
        | Ok data -> Some data 
        | Error err -> 
            log 0 $"decode err: {err}"
            None


    // How to use websocket to do something similar from
    // https://book.elixircryptobot.com/stream-live-cryptocurrency-prices-from-the-binance-wss.html#create-a-supervised-application-inside-an-umbrella
    let endpoint = "wss://stream.binance.us:9443/ws/"

    let tradeUrl (symbol: string) = 
        endpoint + symbol.ToLower() + "@trade" 


    let processMessage (msg: ResponseMessage) = 
        // It received something like:
        //{"e":"trade","E":1678281305803,"s":"ETHUSD","t":44172879,"p":"1549.38000000","q":"0.05300000","b":1478766161,"a":1479008752,"T":1678281305802,"m":true,"M":true}
        //printfn $"Received message: {msg}"
        match msg.ToString() with 
        | GetTradeEvent event -> 
            log 3 $"Trade event received: {event.symbol}@{event.price}"
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
        let ws = new WebsocketClient(new Uri(tradeUrl symbol))

        ws.ReconnectTimeout <- TimeSpan.FromSeconds(30)
        ws.DisconnectionHappened.Subscribe(processDisconnection) |> ignore
        ws.ReconnectionHappened.Subscribe(processReconnection) |> ignore
        ws.MessageReceived.Subscribe(processMessage) |> ignore

        runClient ws |> Async.RunSynchronously

        
        

