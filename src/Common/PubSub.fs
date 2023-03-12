namespace Common

open System
open Common.Models

module PubSub = 
    type ITradeObserver =
        abstract member processTradeEvent: TradeEvent -> Async<unit> 
        abstract member id: string with get

    
    type Topic = string

    type PubSubMsg = 
        | Subscribe of topic: Topic * observer: ITradeObserver * chnl: AsyncReplyChannel<Result<bool, string>>
        | Broadcast of topic: Topic * event: TradeEvent


    type PubSub() = 
        let subscribers: Map<Topic, Map<string, ITradeObserver>> = 
            Map.empty


        let agent =
            MailboxProcessor<PubSubMsg>.Start(fun inbox ->
                let rec loop(subscribers: Map<Topic, Map<string, ITradeObserver>>) =
                    async {
                        let! msg = inbox.Receive()
                        match msg with 
                        | Subscribe (topic, tradeObserver, chnl) -> 
                            let subscribers' = 
                                match Map.tryFind topic subscribers with 
                                | None -> 
                                    Map.empty
                                    |> Map.add tradeObserver.id tradeObserver
                                    |> fun observers' -> subscribers.Add(topic, observers')
                                | Some observers -> 
                                    observers
                                    |> Map.add tradeObserver.id tradeObserver
                                    |> fun observers' -> subscribers.Add(topic, observers')

                            chnl.Reply (Ok true)
                            return! loop(subscribers')

                        | Broadcast (topic, event) ->
                            match Map.tryFind topic subscribers with 
                            | None -> ()
                            | Some observers -> 
                                observers
                                |> Map.iter (fun _key (observer: ITradeObserver) -> 
                                    observer.processTradeEvent event |> Async.RunSynchronously
                                )
                            return! loop(subscribers)
                    }
                loop (subscribers)
            )

        member x.Subscribe topic observer = 
            async {
                return! agent.PostAndAsyncReply(fun chnl -> Subscribe (topic, observer, chnl))
            }

        member x.Broadcast topic event = 
            agent.Post (Broadcast (topic, event))


    module PubSubService = 
        let pubSub = new PubSub() 
        
        let broadcast topic event = 
            pubSub.Broadcast topic event

        let subscribe topic observer = 
            pubSub.Subscribe topic observer

    
