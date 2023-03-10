namespace Naive

module Trader = 
    open Streamer

    let start () = 
        Binance.startStreaming "ethusd"
        0
