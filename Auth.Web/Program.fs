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
    use server =
        try
             WebApp.Start<Startup>(baseAddress)
        with 
            | :? System.Exception as ex -> raise ex
    Log.Information("Listening on {Address}", baseAddress)
    
    let waitIndefinitelyWithToken = 
        let cancelSource = new CancellationTokenSource()
        cancelSource.Token.WaitHandle.WaitOne() |> ignore
    0
