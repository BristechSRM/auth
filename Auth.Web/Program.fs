module Program

open Microsoft.Owin.Hosting
open System
open System.Configuration
open System.Threading
open Logging
open Serilog
open Startup

(*
    Do not run Visual Studio as Administrator!

    Open a command prompt as Administrator and run the following command, replacing username with your username
    netsh http add urlacl url=http://*:8080/ user=username
*)
[<EntryPoint>]
let main _ =
    setupLogging()

    try
        let baseUrl = ConfigurationManager.AppSettings.Get("BaseUrl")
        if String.IsNullOrEmpty baseUrl then
            failwith "Missing configuration value: 'BaseUrl'"

        use server = WebApp.Start<Startup>(baseUrl)
        Log.Information("Listening on {Address}", baseUrl)

        let waitIndefinitelyWithToken = 
            let cancelSource = new CancellationTokenSource()
            cancelSource.Token.WaitHandle.WaitOne() |> ignore
        0

    with
    | ex ->
        Log.Fatal("Exception: {0}", ex)
        -1
