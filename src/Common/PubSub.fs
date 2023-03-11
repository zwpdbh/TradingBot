namespace Common

open System

module PubSub = 
    module Example01 = 

        open System
        open Models

        type TradeEventObservable() =
            let observers = ResizeArray()
    
            interface IObservable<TradeEvent> with
                member this.Subscribe(observer : IObserver<TradeEvent>) =
                    observers.Add(observer)
    
                    { new System.IDisposable with
                        member this.Dispose() =
                            observers.Remove(observer) |> ignore 
                    }
    
            member this.Publish (tradeEvent : TradeEvent) =
                for (observer: IObserver<TradeEvent>) in observers do
                    observer.OnNext(tradeEvent)

        let demo () = 
            0



    
