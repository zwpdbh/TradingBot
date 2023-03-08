

namespace Streamer 

module OStreamerMain = 
    [<EntryPoint>]
    let main argv = 

        Binance.startStreaming "ethusd"
        0