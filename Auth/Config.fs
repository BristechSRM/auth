module Config

open Serilog
open System
open System.Configuration

let getString (key : string) =
    let value = ConfigurationManager.AppSettings.Get(key)
    if String.IsNullOrEmpty value then
        let msg = sprintf "AppSetting '%s' is null or empty" key
        Log.Error(msg)
        failwith msg
    else
        value

