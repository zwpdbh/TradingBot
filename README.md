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


Things to do:
1) Change `Binance` module to publish trade events. Currently, it has direct reference:
```F#
let startStreaming (symbol: string) = 
    let trader = Trader(symbol, 0.05M)
    ...
```
In Elixir, it does it by 
```Elixir
# Naive.send_event(trade_event)
Phoenix.PubSub.broadcast(
    Streamer.PubSub,
    "TRADE_EVENTS:#{trade_event.symbol}",
    trade_event
)
```

2) Create PubSub module to receive events from Streamer

3) Change `Trader` to subscribe certain events. In Elixir example, it dooes it by 
```Elixir
Phoenix.PubSub.subscribe(
    Streamer.PubSub,
    "TRADE_EVENTS:#{symbol}"
)
```