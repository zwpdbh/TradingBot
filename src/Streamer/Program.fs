//Streamer.Binance.startStreaming "ethusd"

namespace Streamer 

module Main = 
    [<EntryPoint>]
    let main argv = 

        Binance.startStreaming "ethusd"
        0