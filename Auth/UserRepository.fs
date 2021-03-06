﻿module UserRepository

open System
open System.Threading
open Serilog
open Amazon
open Amazon.DynamoDBv2
open Amazon.DynamoDBv2.DataModel
open Amazon.DynamoDBv2.DocumentModel
open Entities

let client = new AmazonDynamoDBClient(DynamoDbConfig.config)
let context = new DynamoDBContext(client)

let getUsers() = 
    Log.Debug("getUsers() - Accessing DynamoDb for AuthUsers")
    context.Scan<AuthUserEntity>()

let getUserAsync (userId : Guid) = 
    async { 
        Log.Debug("getUserAsync(id: {id}) - Accessing DynamoDb for authUser", userId)
        let cancelSource = new CancellationTokenSource()
        let! user = context.LoadAsync<AuthUserEntity>(userId, cancelSource.Token) |> Async.AwaitTask
        return if isNull <| box user then None
               else Some user
    }

let getUserByEmailAsync (email : string) = 
    Log.Debug("getUserByEmailAsync(email: {email}) - Accessing DynamoDb for authUser", email)
    let scanResults = context.Scan<AuthUserEntity>(ScanCondition("Email",ScanOperator.Equal,email)) |> Seq.toList
    match scanResults with
    | [user] -> Some user
    | [] -> None
    | _ -> 
        let message = sprintf "Multiple authentication users were found in database with email: %s. Email should be unique!" email
        Log.Error(message)
        raise <| ArgumentException message