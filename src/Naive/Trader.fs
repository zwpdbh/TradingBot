namespace Naive

module Trader = 
    open Common.Models
    open Common.Logging  


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
    type Trader(symbol: string, profitInterval: decimal) = 
        let symbol = symbol 
        let profitInterval = profitInterval

        let agent = 
            MailboxProcessor<TradeEvent>.Start(fun inbox ->
                let rec loop(state: TradeState) = 
                    async {
                        let! msg = inbox.Receive() 
                        match state, msg with 
                        | NewTrader, _ ->
                            // TODO: call binance API to get buy_order 
                            let buyOrder = {
                                price = 100M
                                orderId = 123
                                origQty = 100M
                            }
                            return! loop(BuyPlaced buyOrder)

                        | BuyPlaced buyOrder, msg when buyOrder.orderId = msg.buyerOrderId -> 
                            // calculate sell price
                            // call binance api
                            let sellOrder = {
                                orderId = 124
                                origQty = 100M
                            }
                            return! loop(SellPlaced sellOrder)

                        | SellPlaced sellOrder, msg when sellOrder.orderId = msg.sellerOrderId -> 
                            ()
                        | _, _ ->
                            return! loop(state)
                    }

                loop (NewTrader)
            )

        member x.ProcessTradeEvent tradeEvent = 
            async {
                agent.Post tradeEvent
            }

    //let 
    //let sendEvent (tradeEvent: TradeEvent) (profitInterval: decimal) = 
    //    trader = new Trader(tradeEvent.symbol, 0.05M)
    //let start(symbol: string) = 
        
        