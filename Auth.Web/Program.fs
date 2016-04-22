module Program

open Microsoft.Owin.Hosting
open System.Threading
open Startup
open Logging
open Serilog

[<EntryPoint>]
let main _ =
    setupLogging()

    let baseAddress = "http://*:9003"
    use server = WebApp.Start<Startup>(baseAddress)
    Log.Information("Listening on {Address}", baseAddress)
    
    let waitIndefinitelyWithToken = 
        let cancelSource = new CancellationTokenSource()
        cancelSource.Token.WaitHandle.WaitOne() |> ignore
    0
