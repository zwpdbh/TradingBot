# Initialize project
```
dotnet tool restore
paket install
```

# About Paket
## Install Paket
```
# Install it globally
dotnet tool install paket -g

# Or
dotnet new tool-manifest
dotnet tool install paket
dotnet tool restore
```

## Initialize Paket from solution
```
dotnet paket init
```

## Useful commands
```
# Convert projects from NuGet to Paket in the solution root.
paket convert-from-nuget --force

# Install dependencies
paket install

# Check outdate and update
paket outdated
paket update
```

# Note for Chapter03
In this chapter we need to create PubSub module to de-couple the streamer and trader.\
Currently, they have direct project reference. 

## Publish-Subscribe(PubSub) vs Obserable Pattern
PubSub pattern and Obserable pattern are basically the same except a litte differences: \
Observers are aware of the Subject, but Publish-Subscribe does not require Subjects and Observers to know about each other \
because they communicate through a central broker. \
In our case, it is the PubSub module.

## Things to do:
1) Create PubSub module to receive events from Streamer. \
Actually, we need to create `PubSub` modules( as broker) in which we define: 
- The interface which Publisher(Observable) and Subscriber(Observer) are both awared.
- We implement the `PubSub`(the broker) as a MailboxProcessor to avoid concurrence issue.
- Wrap the broker inside `PubSubService` to provide user APIs for other module to use. 

2) Change `Binance` module to publish trade events. Currently, it has direct reference:

In Elixir, it does it by 
```Elixir
# Naive.send_event(trade_event)
Phoenix.PubSub.broadcast(
    Streamer.PubSub,
    "TRADE_EVENTS:#{trade_event.symbol}",
    trade_event
)
```

In F#, we do it by
```F#
match msg.ToString() with 
| GetTradeEvent event -> 
    //Before, we do this.
    //trader.ProcessTradeEvent event |> Async.RunSynchronously

    // Now, we just broadcast event into some topic.
    PubSubService.broadcast $"{event.symbol.ToUpper()}" event
```


3) Change `Trader` to subscribe certain events. \
In Elixir example, it dooes it by 
```Elixir
Phoenix.PubSub.subscribe(Streamer.PubSub, "TRADE_EVENTS:#{symbol}")
```
In F#, we do it by
```F#
let topic01 = "ethusd".ToUpper()
let topic02 = "OtherSymbol".ToUpper()

// Create traders
let trader01 = Trader("trader01", topic01, 0.05M)
let trader02 = Trader("trader02", topic01, 0.05M)
let trader03 = Trader("trader03", topic01, 0.05M)
let trader04 = Trader("trader04", topic02, 0.05M)

// Use PubSubService's subscribe API to make a trader to subscribe a certain topic.
PubSubService.subscribe topic01 trader01 |> Async.RunSynchronously |> ignore
PubSubService.subscribe topic01 trader02 |> Async.RunSynchronously |> ignore
PubSubService.subscribe topic01 trader03 |> Async.RunSynchronously |> ignore
PubSubService.subscribe topic02 trader04 |> Async.RunSynchronously |> ignore

// Start the streamer to keep waiting events from WebSocket.
Streamer.Binance.startStreaming topic01
```


