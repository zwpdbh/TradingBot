namespace Naive

module Trader = 
    open Common.Models
    open Common.Logging  
    open Common.PubSub

    type BuyOrder = {
        price: decimal 
        orderId: int 
        origQty: decimal
    }

    type SellOrder = {
        orderId: int 
        origQty: decimal
    }

    type TradeState = 
        | NewTrader 
        | BuyPlaced of BuyOrder
        | SellPlaced of SellOrder

    type OrderResponse = 
        {
            id: string 
        }

    let fetchTickSize (symbol: string) = 
        // use binance API to get the symbol's tick size
        10

    let calculateSellPrice (buyPrice: decimal) (profitInterval: decimal) (tickSize: decimal) = 
        let fee = 0.01M
        let originalPrice: decimal = buyPrice * fee 
        let netTargetPrice: decimal = 
            originalPrice * (1.0M + profitInterval)

        let grossTargetPrice: decimal = 
            netTargetPrice * fee 

        (grossTargetPrice / tickSize) * tickSize


    // Our trader will be receiving trade events sequentially and take decisions based on its own state and trade event’s contents.
    type Trader(id: string, symbol: string, profitInterval: decimal) =
        let id = id 
        let symbol = symbol 
        let profitInterval = profitInterval

        // Need to subscribe for certain symbol to receive tradeEvent and process received tradeEvent.
        // This is done by implement ITradeObserver interface

        let agent = 
            MailboxProcessor<TradeEvent>.Start(fun inbox ->
                let rec loop(state: TradeState) = 
                    async {
                        let! msg = inbox.Receive() 
                        match state, msg with 
                        | NewTrader, _ ->
                            // TODO: call binance API to get buyOrder
                            log 2 $"-> New Trade: ${msg}"
                            let buyOrder = {
                                price = 100M
                                orderId = 4567123
                                origQty = 100M
                            }
                            return! loop(BuyPlaced buyOrder)

                        | BuyPlaced buyOrder, msg when buyOrder.orderId = msg.buyerOrderId -> 
                        //| BuyPlaced buyOrder, msg -> 
                            // TODO:: calculate sell price 
                            // call binance api to get sellOrder 
                            log 2 $"-> BuyPlaced: ${msg}"

                            let sellOrder = {
                                orderId = 4524123
                                origQty = 100M
                            }
                            return! loop(SellPlaced sellOrder)
                        | SellPlaced sellOrder, msg when sellOrder.orderId = msg.sellerOrderId -> 
                        //| SellPlaced sellOrder, msg  -> 
                            log 2 $"-> SellPlaced: ${msg}"
                            log 2 $"==End cycle=="
                            //() Do this will make trader exit. 
                            return! loop(NewTrader)
                        | _, _ ->
                            return! loop(state)
                    }

                loop (NewTrader)
            )
        member x.ProcessTradeEvent tradeEvent = 
            async {
                log 3 $"{id} event received: {tradeEvent.symbol}@{tradeEvent.price}"
                agent.Post tradeEvent
            }

        interface ITradeObserver with 
            member x.processTradeEvent event = 
                x.ProcessTradeEvent event 
            member x.id = id

    //let 
    //let sendEvent (tradeEvent: TradeEvent) (profitInterval: decimal) = 
    //    trader = new Trader(tradeEvent.symbol, 0.05M)
    //let start(symbol: string) = 
        
        