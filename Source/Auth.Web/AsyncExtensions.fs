[<AutoOpen>]
module Async
    open System.Threading.Tasks
    let inline StartAsUnitTask (work : Async<unit>) = Task.Factory.StartNew(fun () -> work |> Async.RunSynchronously)
    

