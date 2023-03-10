namespace Common
module Models = 
    open Thoth.Json.Net
    open System
    open Logging

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

