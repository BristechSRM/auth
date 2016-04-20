module Program

open Microsoft.Owin.Hosting
open System.Threading
open Logging
open Serilog

(*
    Note: When running this app from Visual studio / On Windows / Possibly with mono develop (Not checked)
    Because of its use of the network interfaces, you'll need to run Visual studio as administrator.
    However the better solution is to do the following:

    Open a command prompt as administrator and run the following command replacing username with your username
    netsh http ad urlacl url=http://*:9000/ user=username

    After running this command, you won't need to run visual studio as administrator again.

    Reference : http://stackoverflow.com/questions/27842979/owin-webapp-start-gives-a-first-chance-exception-of-type-system-reflection-targ 
*)
[<EntryPoint>]
let main _ =
    setupLogging()

    let baseAddress = "http://*:9003"
    use server = WebApp.Start<Bristech.Srm.HttpConfig.Startup>(baseAddress)
    Log.Information("Listening on {Address}", baseAddress)

    (*
        Because of the way the self hosted server works, it is waiting asynchronously for requests. 
        It starts running then returns to our code, meaning our program will exit. 
        This code will wait indefinitely for a signal so that the overall project will continue to run.
    *)
    
    let waitIndefinitelyWithToken = 
        let cancelSource = new CancellationTokenSource()
        cancelSource.Token.WaitHandle.WaitOne() |> ignore
    0
