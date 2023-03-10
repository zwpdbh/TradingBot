namespace Naive

module Trader = 
    open Streamer
    open Tools.Logging  

    type Order = 
        {
            price: decimal
            orderId: int 
            orderQty: int 
        }

    type TradeOrder = 
        | Buy of Order
        | Sell of Order

    type TradeState = 
        {
            symbol: string 
            profitInterval: int 
            tickSize: int 
            order: TradeOrder option
        }

    let fetchTickSize (symbol: string) = 
        // TODO use real API
        10

    //let init (symbol: string) (profileInterval: int) =
    //    let symbol = symbol.ToUpper()
    //    log 3 $"Initializing new trader for ${symbol}"

    //    let tick_size = fetchTickSize(symbol)



    let rec tradeLoop (tradeEvent: Binance.TradeEvent) (state: TradeState) = 
        async {


            return! tradeLoop(tradeEvent, resul)
        }

    let start () = 
        Binance.startStreaming "ethusd"
        0
